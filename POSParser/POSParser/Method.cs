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
    public class Method
    {
        public int ID { get; set; }
        public string Adi { get; set; }
        public string Sahibi { get; set; }
        public float Deger { get; set; }

        //bir önceki kelimeyi tutuyor.
        public string[] previous = { };
        //Method'ları bulacağız.
        public string[] Bul(string adi, ArrayList Cumle, int sayac)
        {
          
          
            string[] current;
            bool durum = false;
            //İlk kelime fiil ardından gelen kelimeler isim ise bunları fiil tamlaması diyip method olarak sınıflara ekleyeceğiz. enter_account_data, transaction_data...
            //next0 item dir.          
            //next1 item den bir sonraki eleman
            //next2 item den 2 sonraki eleman
            string next1 = "", next2 = "", next3 = "", next0 = adi.ToString();
            string[] currenMethod0 = next0.Split('/'), currenMethod1, currenMethod2, currenMethod3;

            try
            {
              
                if (((previous[1] == "VB" || previous[1] == "VBD" || previous[1] == "VBG" || previous[1] == "VBN" || previous[1] == "VBP" || previous[1] == "VBZ") && currenMethod0[0] == "and"))
                {
                    currenMethod0 = previous;
                    durum = true;
                    adi = currenMethod0[0] + "/VB";
                }
            }
            catch (Exception)
            {


            }

            if (currenMethod0[0]=="includes" || currenMethod0[0] == "include"|| currenMethod0[0] == "is"|| currenMethod0[0] == "am"|| currenMethod0[0] == "are")
            {
                return null;
            }

            //Fiil ile başlayan fiil tamlamasına bakılıyor.
            if (currenMethod0[1] == "VB" || currenMethod0[1] == "VBD" || currenMethod0[1] == "VBG" || currenMethod0[1] == "VBN" || currenMethod0[1] == "VBP" || currenMethod0[1] == "VBZ" || durum == true)
            {



                //enter account data and transaction data. Bu cümlemiz, listemizde POS'larına ayrılmış şekilde bulunmaktadır. {enter/VB,account/NN,data/NN,and/IN} 
                //Biz 39-47 kod satırlarında bu kelimeleri buluyoruz.
                if (Cumle.size() >= sayac + 2)
                    next1 = Cumle.get(sayac + 1).ToString();
                if (Cumle.size() > sayac + 2)
                    next2 = Cumle.get(sayac + 2).ToString();
                if (Cumle.size() > sayac + 3)
                    next3 = Cumle.get(sayac + 3).ToString();

                currenMethod1 = next1.Split('/');
                currenMethod2 = next2.Split('/');
                currenMethod3 = next3.Split('/');

                //eğer değişkenlerimiz boşsa return dön.
                if (currenMethod2[0] == "" || currenMethod1[0] == "")
                    return null;

                //eğer değişkenlerimiz arasında " 's " veya " of " varsa null dön.
                if (currenMethod1[1] == "s" || currenMethod2[1] == "of" || currenMethod2[1] == "POS")
                {
                    return null;
                }

                //eğer değişkenlerimiz istediğimiz fiil tamlamasını oluşturuyorsa next 0'a ata..
                else if ((currenMethod1[1] == "NN" && currenMethod2[1] == "NN") || (currenMethod1[1] == "NNS" && currenMethod2[1] == "NNS") || (currenMethod1[1] == "NN" && currenMethod2[1] == "NNS") || (currenMethod1[1] == "NNS" && currenMethod2[1] == "NN"))
                {
                    string owner = MethodOwner(Cumle, adi);
                    next0 = currenMethod0[0].ToString() + "_" + currenMethod1[0].ToString() + "_" + currenMethod2[0].ToString() + "/METHOD" + "/2/" + owner;


                }
                //eğer değişkenlerimiz istediğimiz fiil tamlamasını oluşturuyorsa next 0'a ata..
                else if ((currenMethod1[1] == "NN" || currenMethod1[1] == "NNS" || currenMethod1[1] == "NNP" || currenMethod1[1] == "NNPS") && currenMethod2[1] == "CC")
                {
                    string owner = MethodOwner(Cumle, adi);
                    next0 = currenMethod0[0].ToString() + "_" + currenMethod1[0].ToString() + "/METHOD" + "/1/" + owner;

                }
                //eğer değişkenlerimiz istediğimiz fiil tamlamasını oluşturuyorsa next 0'a ata..
                else if ((currenMethod1[1] == "NN" || currenMethod1[1] == "NNS" || currenMethod1[1] == "NNP" || currenMethod1[1] == "NNPS") && currenMethod2[1] == "IN")
                {
                    string owner = MethodOwner(Cumle, adi);
                    next0 = currenMethod0[0].ToString() + "_" + currenMethod1[0].ToString() + "/METHOD" + "/1/" + owner;

                }
                else
                {
                    return null;
                }

                previous = currenMethod0;

                return current = next0.Split('/');
            }
            //Null dönsün.
            else
            {
                return null;
            }



        }

        //Methodları Getir
        public List<Method> Sec()
        {
            Veritabani veritabani = new Veritabani();
            MySqlDataReader reader;
            try
            {
                List<Method> liste = new List<Method>();
                if (veritabani.BaglantiKontrol())
                {
                    ////Count
                    //MySqlCommand sumComm = db.baglanti.CreateCommand();
                    //sumComm.CommandText = "Select sum(wCount) from words where VB is not null;";
                    //int Count = (Convert.ToInt32(sumComm.ExecuteScalar())) * 5 / 100;


                    //Yeni bir mysql komutu oluşturuluyor..
                    MySqlCommand comm = veritabani.Baglanti.CreateCommand();
                    //Yapılacak işlem seçiliyor.
                    comm.CommandText = "SELECT * FROM method";
                    reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        liste.Add(new Method
                        {
                            ID = (int)reader["ID"],
                            Adi = reader["CandidateMethod"].ToString(),
                            Sahibi = reader["MethodOwner"].ToString(),
                            Deger = Convert.ToInt32(reader["MethodValues"].ToString())
                        });
                    }
                    reader.Close();

                }
                return liste;
            }
            catch (Exception ex)
            {

                veritabani.Baglanti.Close();
                return null;
            }
        }

        //Method Owner Bul
        string previousOwner = "";
        private string MethodOwner(ArrayList etiketList, string word)
        {

            string[] currentVerb = word.Split('/');

            if (currentVerb[1] != "VBP" && currentVerb[1] != "VBZ" && currentVerb[1] != "VB" && currentVerb[1] != "VBD" && currentVerb[1] != "VBG" && currentVerb[1] != "VBN")
                return null;

            Sinif sinif = new Sinif();
            string owner = "";
            string[] ownerMethod, etiketListe, etiketListe2, kelimeListe;
            int methodSayac = 0;
            foreach (var kelime in etiketList)
            {

                owner = sinif.Bul(etiketList, methodSayac, kelime.ToString());
                if (owner == "")
                    break;
                ownerMethod = owner.Split('_');
                etiketListe = etiketList.get(methodSayac + 1).ToString().Split('/');
                etiketListe2 = etiketList.get(methodSayac + 2).ToString().Split('/');
                kelimeListe = kelime.ToString().Split('/');

                if (ownerMethod.Count() >= 3)
                {
                    if (ownerMethod[0] == kelimeListe[0] && ownerMethod[1] == etiketListe[0] && ownerMethod[2] == etiketListe2[0])
                    {
                        if (ownerMethod[0].EndsWith("s") || ownerMethod[1].EndsWith("s") || ownerMethod[2].EndsWith("s"))
                        {
                            ownerMethod[0] = Stemming(ownerMethod[0].ToString());
                            ownerMethod[1] = Stemming(ownerMethod[1].ToString());
                            ownerMethod[2] = Stemming(ownerMethod[2].ToString());
                        }
                        owner = ownerMethod[0] + "_" + ownerMethod[1] + "_" + ownerMethod[2];
                        break;
                    }
                }
                else if (ownerMethod.Count() >= 2)
                {

                    if (ownerMethod[0] == kelimeListe[0] && ownerMethod[1] == etiketListe[0])
                    {
                        if (ownerMethod[0].EndsWith("s") || ownerMethod[1].EndsWith("s"))
                        {
                            ownerMethod[0] = Stemming(ownerMethod[0].ToString());
                            ownerMethod[1] = Stemming(ownerMethod[1].ToString());

                        }
                        owner = ownerMethod[0] + "_" + ownerMethod[1];
                        break;
                    }

                }
                else
                {
                    break;
                }

                methodSayac++;
            }
            previousOwner = currentVerb[0];
            return owner;

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
