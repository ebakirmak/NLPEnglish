using java.util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSParser
{
    public class Nitelik
    {
        public int ID { get; set; }
        public string Adi { get; set; }
        public string Sahibi { get; set; }
        public float Deger { get; set; }



        string previous = "";
        //Attribute olacak kelimeler bulunacak.
        public string[] Bul(string adi, ArrayList Cumle, int sayac)
        {
            int atla = 0;
            string[] current = new string[3];

            //Tamlamalı attributeları bulmak için kontrol yapıyoruz. Örnek: computerized_banking_network, central_computer
            //next0 şuanki kelimedir.          
            //next1 item den bir sonraki eleman
            //next2 item den 2 sonraki eleman
            string next1 = "";
            string next2 = "";
            string next0 = adi.ToString();
            string[] currentClass;
            string[] currentClass0 = next0.Split('/');
            string[] currentClass1;
            string[] currentClass2;

            if (Cumle.size() >= sayac + 2)
                next1 = Cumle.get(sayac + 1).ToString();
            currentClass1 = next1.Split('/');

            if (Cumle.size() > sayac + 2)
                next2 = Cumle.get(sayac + 2).ToString();
            currentClass2 = next2.Split('/');

            Sinif sinif = new Sinif();
            currentClass = null;
            try
            {
                currentClass = sinif.Bul(currentClass2[0] + "/" + currentClass2[1], Cumle, sayac + 2);
            }
            catch (Exception)
            {

                Console.Write("Dizin dizi sınırları dışındaydı.");
            }
        
           
            if (currentClass != null)
            {
                currentClass2 = currentClass;
            }
           
            if (currentClass2.Count() < 2) return null;

            if (currentClass2[1] == "NN" || currentClass2[1] == "NNS")
            {
                atla = 2;
            }
            else
            {
                if (Cumle.size() > sayac + 3)
                    next2 = Cumle.get(sayac + 3).ToString();
                currentClass2 = next2.Split('/');
                atla = 3;
            }
            //Araların of veya 's takısı olan tamlamalar bulunuyor.
            
            if (currentClass1[0] == "of" )
            {


                if ((currentClass0[1] == "NN" && currentClass2[1] == "NN") || (currentClass0[1] == "NNS" && currentClass2[1] == "NNS") || (currentClass0[1] == "NN" && currentClass2[1] == "NNS") || (currentClass0[1] == "NNS" && currentClass2[1] == "NN"))
                {

                    next0 = currentClass0[0].ToString() + "/" + "ATTRIBUTE/" + atla.ToString() + "/" + currentClass2[0].ToString();
                    previous = next0;
                    return current = next0.Split('/');

                }
                else
                    return null;




            }
            else if (currentClass1[0].IndexOf("'s") != -1)
            {
                if ((currentClass0[1] == "NN" && currentClass2[1] == "NN") || (currentClass0[1] == "NNS" && currentClass2[1] == "NNS") || (currentClass0[1] == "NN" && currentClass2[1] == "NNS") || (currentClass0[1] == "NNS" && currentClass2[1] == "NN"))
                {

                    next0 = currentClass2[0].ToString() + "/" + "ATTRIBUTE/" + sayac.ToString() + "/" + currentClass0[0].ToString();
                    previous = next0;
                    return current = next0.Split('/');

                }
                else
                    return null;
            }
            //Belirtilen kuralda değil ise null dön.
            else
            {
                previous = next0;
                return null;
            }



        }

        //attribute tablosunda CandidateAttribute kolonunda olup AtttibuteOwner kolonunda olmayan kelimeleri bulduk. İstersek class tablosundan çıkarabiliriz.
        public List<Nitelik> HepsiniSiniftanSil()
        {
            List<Nitelik> liste = new List<Nitelik>();
            List<Nitelik> liste2 = new List<Nitelik>();
            Veritabani veritabani = new Veritabani();
            if (veritabani.BaglantiKontrol())
            {
                //Mysql tablosunda veri okuyacak nesnelerimizi oluşturduk. nextReader nesnemiz reader nesnesinden sonraki verileri okuyacak.
                MySqlDataReader reader;
                MySqlDataReader nextReader;
                //Yeni bir mysql komutu oluşturuluyor..
                MySqlCommand comm = veritabani.Baglanti.CreateCommand();
                MySqlCommand nextComm = veritabani.Baglanti.CreateCommand();
                //Tüm attribute tablosu reader nesnesine atanıyor.
                comm.CommandText = "SELECT * FROM attribute";
                reader = comm.ExecuteReader();
                //Attribute tablosundaki bütün veriler listeye eklendi.
                while (reader.Read())
                {
                    liste.Add(new Nitelik
                    {
                        ID = (int)reader["ID"],
                        Adi = reader["CandidateAttribute"].ToString(),
                        Sahibi = reader["AttributeOwner"].ToString(),
                        Deger = Convert.ToInt32(reader["AttributeValues"])

                    });

                }
                reader.Close();
                //Listedeki attribute ile tablodaki attributeowner'ları eşit olanları liste2'ye eklendi.
                foreach (var item in liste)
                {
                    nextComm.CommandText = "Select * from attribute";
                    nextReader = nextComm.ExecuteReader();
                    while (nextReader.Read())
                    {
                        if (nextReader["AttributeOwner"].ToString() == item.Adi.ToString())
                        {
                            liste2.Add(item);
                            break;
                        }
                    }
                    nextReader.Close();
                }

                // liste'den liste2'ye eklenenler çıkarıldı.
                for (int i = 0; i < liste2.Count; i++)
                {
                    liste.Remove(liste2.First());
                }

                veritabani.Baglanti.Close();
                return liste;
            }
            else
            {
                veritabani.Baglanti.Close();
                return null;
            }
        }


        //attribute tablosundaki nitelikleri getirelim.
        public List<Nitelik> Sec()
        {
            Veritabani veritabani = new Veritabani();
            if (veritabani.BaglantiKontrol())
            {
                List<Nitelik> liste = new List<Nitelik>();
                //Mysql tablosunda veri okuyacak nesnelerimizi oluşturduk. nextReader nesnemiz reader nesnesinden sonraki verileri okuyacak.
                MySqlDataReader reader;
                //Yeni bir mysql komutu oluşturuluyor..
                MySqlCommand comm = veritabani.Baglanti.CreateCommand();
                MySqlCommand nextComm = veritabani.Baglanti.CreateCommand();
                //Tüm attribute tablosu reader nesnesine atanıyor.
                comm.CommandText = "SELECT * FROM attribute";
                reader = comm.ExecuteReader();
                //Attribute tablosundaki bütün veriler listeye eklendi.
                while (reader.Read())
                {
                    liste.Add(new Nitelik
                    {
                        ID = (int)reader["ID"],
                        Adi = reader["CandidateAttribute"].ToString(),
                        Sahibi = reader["AttributeOwner"].ToString(),
                        Deger = Convert.ToInt32(reader["AttributeValues"])

                    });

                }
                reader.Close();
                return liste;
            }
            else
            {
                return null;
            }

        }
    }
}
