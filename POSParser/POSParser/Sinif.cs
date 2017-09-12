using edu.stanford.nlp.process;
using java.util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSParser
{
    public class Sinif
    {
        public int ID { get; set; }
        public string Adi { get; set; }
        public float Deger { get; set; }



        //bir önceki kelimeyi tutuyor.
        string previous = "";


        //Sıfatın nitelediği isimler Sıfat_İsim||Sıfat_İsim_İsim veya sıfatın nitelemediği isim_isim birleştirilerek class(sınıflar) bulundu. Örnek: computerized_banking_network, central_computer..
        public string[] Bul(string adi, ArrayList Cumle, int sayac)
        {
            string[] current;
            //Tamlamalı sınıf isimlerini bulmak için kontrol yapıyoruz. Örnek: computerized_banking_network, central_computer
            //next0 item dir.          
            //next1 item den bir sonraki eleman
            //next2 item den 2 sonraki eleman
            string next1 = "";
            string next2 = "";
            string next0 = adi.ToString();
            string[] currentClass0 = next0.Split('/');


            string[] currentClass1;
            string[] currentClass2;


            if (currentClass0[1] == "JJ"&&currentClass0[0]!="own")
            {
                if (Cumle.size() >= sayac + 2)
                    next1 = Cumle.get(sayac + 1).ToString();
                if (Cumle.size() > sayac + 2)
                    next2 = Cumle.get(sayac + 2).ToString();
                currentClass1 = next1.Split('/');
                currentClass2 = next2.Split('/');


                if (currentClass1[0].EndsWith("s"))
                {
                    currentClass1 = Stemming(currentClass1);
                }

                if (currentClass2[0].EndsWith("s"))
                {
                    currentClass2 = Stemming(currentClass2);
                }

                if (currentClass2[0] == "" || currentClass1[0] == "")
                    return null;

                if ((currentClass1[1] == "NN" && currentClass2[1] == "NN") || (currentClass1[1] == "NNS" && currentClass2[1] == "NNS") || (currentClass1[1] == "NN" && currentClass2[1] == "NNS") || (currentClass1[1] == "NNS" && currentClass2[1] == "NN"))
                {

                    next0 = currentClass0[0].ToString() + "_" + currentClass1[0].ToString() + "_" + currentClass2[0].ToString() + "/NN" + "/2";

                }
                else if (currentClass1[1] == "NN" || currentClass1[1] == "NNS" || currentClass1[1] == "NNP" || currentClass1[1] == "NNPS")
                {
                    next0 = currentClass0[0].ToString() + "_" + currentClass1[0].ToString() + "/NN" + "/1";
                }

                previous = next0;

                return current = next0.Split('/');
            }
            //SIFATLARIN NİTELEDİĞİ İSİM DEĞİL İSE Null dönsün.
            else
            {
                currentClass1 = null;
                if (Cumle.size() >= sayac + 2)
                {
                    next1 = Cumle.get(sayac + 1).ToString();
                    currentClass1 = next1.Split('/');
                }
                else
                    return null;
                string previous1 = "";
                string[] currentPrevious1 = new string[2];
                try
                {

                    if (Cumle.size() >= sayac - 1)
                        previous1 = Cumle.get(sayac - 1).ToString();
                    currentPrevious1 = previous1.Split('/');



                }
                catch (Exception)
                {
                    currentPrevious1[0] = "";
                }

                if (currentClass1[0].EndsWith("s"))
                {
                    currentClass1 = Stemming(currentClass1);
                }
                if ((currentClass0[1] == "NN" && currentClass1[1] == "NN") || (currentClass0[1] == "NN" && currentClass1[1] == "NNS") || (currentClass0[1] == "NNS" && currentClass1[1] == "NN") || (currentClass0[1] == "NNS" && currentClass1[1] == "NNS") && currentPrevious1[0].ToString() != "and")
                {
                    current = (currentClass0[0].ToString() + "_" + currentClass1[0].ToString() + "/NN" + "/1").Split('/');
                    return current;
                }
                return null;
            }






        }

        //Method Owner'ları bulmak için kullandığımız sınıf bul methodu...
        public string Bul(ArrayList Cumle, int sayac, string adi)
        {

            //Tamlamalı sınıf isimlerini bulmak için kontrol yapıyoruz. Örnek: computerized_banking_network, central_computer
            //next0 item dir.          
            //next1 item den bir sonraki eleman
            //next2 item den 2 sonraki eleman
            string next1 = "";
            string next2 = "";
            string next0 = adi.ToString();
            string[] currentClass0 = next0.Split('/');
            string[] currentClass1;
            string[] currentClass2;


            //SIFATLARIN NİTELEDİĞİ SINIFLAR BULUNUYOR.
            if (currentClass0[1] == "JJ")
            {
                if (Cumle.size() >= sayac + 2)
                    next1 = Cumle.get(sayac + 1).ToString();
                if (Cumle.size() > sayac + 2)
                    next2 = Cumle.get(sayac + 2).ToString();
                currentClass1 = next1.Split('/');
                currentClass2 = next2.Split('/');



                if (currentClass2[0] == "" || currentClass1[0] == "")
                    return null;

                if ((currentClass1[1] == "NN" && currentClass2[1] == "NN") || (currentClass1[1] == "NNS" && currentClass2[1] == "NNS") || (currentClass1[1] == "NN" && currentClass2[1] == "NNS") || (currentClass1[1] == "NNS" && currentClass2[1] == "NN"))
                {

                    next0 = currentClass0[0].ToString() + "_" + currentClass1[0].ToString() + "_" + currentClass2[0].ToString();

                }
                else if (currentClass1[1] == "NN" || currentClass1[1] == "NNS" || currentClass1[1] == "NNP" || currentClass1[1] == "NNPS")
                {
                    next0 = currentClass0[0].ToString() + "_" + currentClass1[0].ToString();
                }

                previous = next0;

                return next0;
            }
            //SIFATLARIN NİTELEDİĞİ İSİM DEĞİL İSE Null dönsün.
            else
            {
                if (currentClass0[1] == "NN" || currentClass0[1] == "NNS" || currentClass0[1] == "NNP" || currentClass0[1] == "NNPS")
                {
                    next0 = currentClass0[0].ToString();

                }

                return next0;
            }
        }

        //Aday sınıflar bulunuyor. class tablosuna ekleniyor ondan sonra liste geri dönüyor.
        public List<Sinif> Sec()
        {
            Veritabani veritabani = new Veritabani();
            MySqlDataReader reader;
            try
            {
                List<Sinif> liste = new List<Sinif>();
                if (veritabani.BaglantiKontrol())
                {
                    //Noun olan kelimelerin topluyoruz. Belirlenen yüzdesini alıyoruz.
                    MySqlCommand sumComm = veritabani.Baglanti.CreateCommand();
                    sumComm.CommandText = "SELECT count(ID) FROM class ";
                    float Count = (Convert.ToSingle(sumComm.ExecuteScalar())) * 15 / 100;



                    //Yeni bir mysql komutu oluşturuluyor..
                    MySqlCommand comm = veritabani.Baglanti.CreateCommand();
                    //Yapılacak işlem seçiliyor.
                    comm.CommandText = "SELECT * FROM class ";
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Count < Convert.ToSingle(reader["ClassValues"].ToString()))
                        {
                            liste.Add(new Sinif
                            {
                                ID = (int)reader["ID"],
                                Adi = reader["CandidateClass"].ToString(),
                                Deger = Convert.ToSingle(reader["ClassValues"].ToString()),
                            });

                        }

                    }
                    reader.Close();


                }

                //liste = Eliminate(liste);
                SinifTabloSil();
                Ekle(liste);
                return liste;
            }
            catch (Exception ex)
            {

                veritabani.Baglanti.Close();
                return null;
            }
        }

        //Class tablosunu temizleyelim.        
        public void SinifTabloSil()
        {
            Veritabani veritabani = new Veritabani();
            if (veritabani.BaglantiKontrol())
            {
                string komut = "Delete from class";
                MySqlCommand kmt = new MySqlCommand(komut, veritabani.Baglanti);
                kmt.ExecuteNonQuery();
                veritabani.Baglanti.Close();


            }
            else
            {
                throw new Exception("Veritabanı açılmadı...");
            }

        }

        //Aday sınıfları class tablosuna ekleyelim.
        public void Ekle(List<Sinif> Siniflar)
        {
            Veritabani veritabani = new Veritabani();
            if (veritabani.BaglantiKontrol())
            {
                foreach (var sinif in Siniflar)
                {
                    string komut = "insert into class (CandidateClass,ClassValues) values('" + sinif.Adi + "'," + Math.Ceiling(sinif.Deger) + ")";
                    MySqlCommand kmt = new MySqlCommand(komut, veritabani.Baglanti);
                    kmt.ExecuteNonQuery();
                }



                veritabani.Baglanti.Close();

            }
            else
            {
                veritabani.Baglanti.Close();
            }
        }

        private static string[] Stemming(string[] item)
        {
            string[] current = new string[2];
            Morphology stemming = new Morphology();
            current[0] = stemming.stem(item[0]);
            current[1] = item[1];
            return current;
        }
    }
}
