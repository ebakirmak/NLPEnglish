using edu.stanford.nlp.process;
using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSParser
{
    public class Iliskiler
    {
        private string IlkSinif { get; set; }

        private string IliskiFiili { get; set; }

        private string IkinciSinif { get; set; }

        //Cümlenin tamamına bakarak sinif tablosuda bulunan sinif adaylarıyla arasında ilişki olabilecek
        public void Bul(ArrayList Cumle)
        {
            Veritabani veritabani = new Veritabani();
            List<string> ClassList = new List<string>();
            List<string> RelationShips = new List<string>();
            string next1 = "";
            int sayac = 0;
            string[] word;
            int etiketListNext = 0;
            foreach (var eleman in Cumle)
            {

                //sıfat tamlamaları sınıf isimlerinde alınan kelimeyi geçmesi için döngüde atlarız. computerized_banking_network
                if (etiketListNext > 1)
                {
                    etiketListNext--;
                    sayac++;

                    continue;
                }
                word = eleman.ToString().Split('/');
                word[0] = Stemming(word[0]);
                //Gelen kelimenin türüne bakıp hangi işlemi yapacağını seçiyoruz.
                if (word[1] == "JJ" || word[1] == "NN" || word[1] == "NNS" || word[1] == "NNP" || word[1] == "NNPS")
                {

                    string[] current = { };

                    //Mysql Fulltext özelliğini kullanarak gönderilen kelimenin aday sınıf içinde olup olmadığına bakıyoruz. Varsa List ekliyoruz.
                    ClassList = veritabani.IliskiVeriKontrol(word[0]);


                    //Listedeki  gelen elemanlar için karşılaştırma yapıyoruz. örnek olarak bank gittiği zaman veritabanından 2 değer dönüyor, computerized_Banking_network ve bank sınıfı. Burada hangi kelimeyi alacağımızı belirliyoruz ve asıl listemize  RelationshipList atıyoruz.
                    foreach (var liste in ClassList)
                    {
                        if (Cumle.size() >= sayac + 2)
                            next1 = Cumle.get(sayac + 1).ToString();

                        current = next1.Split('/');
                        if (current[0].EndsWith("s"))
                        {
                            current[0] = Stemming(current[0].ToString());
                        }
                        if (liste.Contains(current[0]))
                        {

                            RelationShips.Add(liste);
                            current = liste.Split('_');
                        }
                        else if (word[0].ToString() == liste.Split('/')[0].ToString())
                        {
                            RelationShips.Add(liste);
                            current = liste.Split('_');
                        }

                    }

                    etiketListNext = current.Count();
                }
                else if (word[1] == "VB" || word[1] == "VBD" || word[1] == "VBG" || word[1] == "VBN" || word[1] == "VBP" || word[1] == "VBZ")
                {
                    word[0] = Stemming(word[0]);
                    RelationShips.Add(word[0] + "/VB");
                }
                else if (word[0] == "and")
                {
                    RelationShips.Add(word[0] + "/and");
                }

                sayac++;
            }

            //Class+RelationshipVerb+Class yapısına uygun kelimeleri vt. ekleyeceğiz.
            string[] kelime = { };

            bool first = true, insert = false;
            foreach (var eleman in RelationShips)
            {
                kelime = eleman.Split('/');
                if (this.IlkSinif != null && this.IliskiFiili != null && this.IkinciSinif != null && kelime[0] == "and")
                {
                    IkinciSinif = null;
                    insert = false;
                    first = false;
                }
                else if (insert == true)
                {
                    IlkSinif = null;
                    IkinciSinif = null;
                    IliskiFiili = null;
                }


                if (kelime[1] == "NN" && first == true)
                {
                    this.IlkSinif = kelime[0];
                    first = false;
                }
                else if (kelime[1] == "NN" && first == false)
                {
                    this.IkinciSinif = kelime[0];
                    first = true;
                }
                else if (kelime[1] == "VB" && IlkSinif != null)
                {
                    this.IliskiFiili = kelime[0];
                }

                if (this.IlkSinif != null && this.IliskiFiili != null && this.IkinciSinif != null)
                {

                    veritabani.Ekle(IlkSinif, IliskiFiili + "/Relationships", IkinciSinif);
                    insert = true;

                }


            }
            this.IlkSinif = null;
            this.IliskiFiili = null;
            this.IkinciSinif = null;
        }


        private static string Stemming(string item)
        {
            string[] current;
            current = item.ToString().Split('/');
            Morphology stemming = new Morphology();
            item = stemming.stem(current[0].ToString());
            return item;
        }
    }
}
