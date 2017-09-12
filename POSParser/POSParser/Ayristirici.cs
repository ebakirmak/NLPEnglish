using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.parser;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.process;
using edu.stanford.nlp.trees;
using System.IO;
using System.Text.RegularExpressions;

namespace POSParser
{
    public class Ayristirici
    {

        public static void Baslat(string fileName)
        {
            string appPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            appPath = appPath + @"\englishPCFG.ser.gz";
            var lp = LexicalizedParser.loadModel(appPath);
            if (!String.IsNullOrEmpty(fileName))
                POS(lp, fileName);



        }

        //LexicalizedParser=Deyimleşmiş ayrıştırıcı
        public static void POS(LexicalizedParser ayristirici, string dosyaAdi)
        {
            Veritabani veritabani = new Veritabani();
            Sinif sinif = new Sinif();
            Nitelik nitelik = new Nitelik();
            Method method = new Method();
            Iliskiler iliski = new Iliskiler();
            //Veritabanındaki bütün kayıtlar siliniyor.
            veritabani.VeritabaniSil();
            
            // a file using DocumentPreprocessor
            var tlp = new PennTreebankLanguagePack();
            //grammaticalStructureFactory=gramer yapısı Fabrikası
            var gsf = tlp.grammaticalStructureFactory();

            // You could also create a tokenizer here (as below) and pass it
            // to DocumentPreprocessor

            foreach (java.util.List sentence in new DocumentPreprocessor(dosyaAdi))
            {
               
                string[] current = new string[3];
                //Deyimleyici ayrıştırıcıyı dokümana uyguladı ve parse değişkenine atadı.
                Tree parse = ayristirici.apply(sentence);
                //Parse değişkenindeki etiketleri arraylist'e atadık.                
                java.util.ArrayList Cumle = parse.taggedYield();
                int sayac = 0;
                string previousVerb = "";
                foreach (var kelime in Cumle)
                {
                    try
                    {
                        //Örneğin computerized banking network sınıfını ekledikten sonra network kelimesinden başlasın.
                        int eleman = Convert.ToInt32(current[2]);
                        if (eleman > 0)
                        {
                            current[2] = (eleman - 1).ToString();
                            sayac++;
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                    string word = kelime.ToString();

                    //Gelen karakter . ise onu vt eklemek ile uğraşmıyoruz.
                    if (word == "./.")
                    {

                        iliski.Bul(Cumle);
                        continue;
                    }

                    //Sınıflar bulunsun ve vt. eklensin. 
                    if ((current = sinif.Bul(word, Cumle, sayac)) != null)
                    {
                        if (veritabani.Ekle(current[0], current[1], null) == true)
                            System.Console.Write(current[0] + "=>" + current[1] + Environment.NewLine);
                        else
                            System.Console.Write("İşlem Başarısız\n");
                    }
                    //Attribute olacak kelimeler bulunsun ve vt. eklensin.
                    else if ((current = nitelik.Bul(word, Cumle, sayac)) != null)
                    {
                        //current = Stemming(current);
                        if (veritabani.Ekle(current[0], current[1], current[3]) == true)
                            System.Console.Write(current[0] + "=>" + current[1] + Environment.NewLine);
                        else
                            System.Console.Write("İşlem Başarısız\n");
                    }
                    //Method olacak fiil tamlamaları bulunacak.
                    else if ((current = method.Bul(word, Cumle, sayac)) != null || (previousVerb != "" && (word.IndexOf("NN") != -1 || word.IndexOf("NNS") != -1)))
                    {

                        if (veritabani.Ekle(current[0], current[1], current[3]) == true)
                            System.Console.Write(current[0] + "=>" + current[1] + Environment.NewLine);
                        else
                            System.Console.Write("İşlem Başarısız\n");
                    }
                    else
                    {
                        current = Stemming(word);
                        if (veritabani.Ekle(current[0], current[1], null) == true)
                            System.Console.Write(current[0] + "=>" + current[1] + Environment.NewLine);
                        else
                            System.Console.Write("İşlem Başarısız\n");
                    }
                    sayac++;
                }

            }
            method.previous = null;

        }

        //Girilen dokümanda gerekli olan temizleme işlemleri...
        public static void MetinTemizle(string yol)
        {
            string path = yol;
            string text = DosyadanOku(path);
            //Noktalama işaretlerini kaldırır. Regex kullanılmıştır.
            text = Regex.Replace(text, @"[^\w\^.\^']", " ");
            
            //Büyük harfleri küçüğe çevir.
            text = text.ToLower();
            //Gereksiz yapıları Kaldır. Örnek the,a,an
            string[] grammers = { "the", "a", "an" };



            foreach (var eleman in grammers)
            {
                if (eleman == "the")
                {
                    Regex the = new Regex(eleman + "\\s");
                    text = the.Replace(text, " ");
                }
                Regex regex = new Regex("\\s" + eleman + "\\s");
                text = regex.Replace(text, " ");

            }

            DosyayaYaz(yol, text);
        }

        //Dosyadan Okuma İşlemi
        private static string DosyadanOku(string yolAdi)
        {
            // Text dosyasından okuyan StreamReader sınıfına ait bir 
            // dosyaOku nesnesini oluşturuyoruz
            StreamReader dosyaOku;

            // dosyadan okuyacağımız yazıyı string olarak depolamak için
            // yazı nesnemizi oluşturuyoruz.
            string yazi;

            //Dosyamızı okumak için açıyoruz..
            dosyaOku = System.IO.File.OpenText(yolAdi);

            //Dosyamızı okumak için açıyoruz ve ilk satırını okuyoruz..
            yazi = dosyaOku.ReadLine();

            /* okuduğumuz satırı ekrana bastırıp bir sonraki satıra geçiyoruz
           * Eğer sonraki satırda da yazı varsa onu da okuyup ekrana bastırıyoruz. 
           * Bu işlemleri dosyanın sonuna kadar devam ettiriyoruz.. */

            while (dosyaOku.ReadLine() != null)
            {
                System.Console.WriteLine(yazi);
                yazi += dosyaOku.ReadLine();
            }

            // dosyamızı kapatıyoruz..
            dosyaOku.Close();
            return yazi;
        }
        //Dosyaya Yazma İşlemi
        private static void DosyayaYaz(string yolAdi, string metin)
        {
            //StreamWriter classından dosya isimli bir nesne oluşturalım
            StreamWriter dosya = new StreamWriter(yolAdi);

            //Dosyamıza birinci satırı yazalım
            dosya.WriteLine(metin);

            //Dosyamızın kapatılım..
            dosya.Close();

            //Yazma işlemini başarı ile tamamladığımızı kullanıcıya bildirelim..
            System.Console.WriteLine("Dosya yazımı Başarı ile tamamlandı...");

        }


        //Metindeki kelimenin kökünü buluyoruz.
        private static string[] Stemming(string[] kelime)
        {
            string[] current = new string[4];
            current[0] = kelime[0];
            current[1] = kelime[1];
            current[2] = kelime[2];
            current[3] = kelime[3];
            Morphology stemming = new Morphology();
            current[0] = stemming.stem(kelime[0]);
            return current;
        }

        public static string[] Stemming(object kelime)
        {
            string[] current;
            current = kelime.ToString().Split('/');
            Morphology stemming = new Morphology();
            current[0] = stemming.stem(current[0]);
            return current;
        }


    }
}
