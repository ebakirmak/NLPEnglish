using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSParser
{
    public class Veritabani
    {
        //Baglanti adında bir connection oluşturdum
        public MySqlConnection Baglanti;
        private MySqlDataReader Oku;
        public bool BaglantiKontrol()
        {
            try
            {
                Baglanti = new MySqlConnection("Server=localhost;Database=dbnlp;Uid=root;Pwd='12991453Emre.';");
                Baglanti.Open();

                return true;
                //Veritabanına bağlanırsa baglanti_kontrol fonksiyonu "true" değeri gönderecek
            }

            catch (Exception)
            {
                return false;
                //Veritabanına bağlanamazsa "false" değeri dönecek
            }
        }

        public void VeritabaniSil()
        {
            if (BaglantiKontrol())
            {
                string komutWords = "Delete from words";
                MySqlCommand kmtWords = new MySqlCommand(komutWords, Baglanti);
                kmtWords.ExecuteNonQuery();

                string komutClass = "Delete from class";
                MySqlCommand kmtClass = new MySqlCommand(komutClass, Baglanti);
                kmtClass.ExecuteNonQuery();

                string komutAttribute = "Delete from attribute";
                MySqlCommand kmtAttribute = new MySqlCommand(komutAttribute, Baglanti);
                kmtAttribute.ExecuteNonQuery();

                string komutMethod = "Delete from method";
                MySqlCommand kmtMethod = new MySqlCommand(komutMethod, Baglanti);
                kmtMethod.ExecuteNonQuery();

                string komutRelationship = "Delete from relationship";
                MySqlCommand kmtRelationship = new MySqlCommand(komutRelationship, Baglanti);
                kmtRelationship.ExecuteNonQuery();

                Baglanti.Close();
            }
            else
            {
                throw new Exception("Veritabanı açılmadı...");
            }

        }

        //Veritabanına Insert işlemi yapılıyor.
        public bool Ekle(string adi, string tur, string sahibi)
        {
            try
            {
                if (BaglantiKontrol() == true)
                {
                    string komut = KomutEkle(adi, tur, sahibi);
                    MySqlCommand kmt = new MySqlCommand(komut, Baglanti);
                    if (kmt.CommandText == "")
                        return false;
                    kmt.ExecuteNonQuery();
                    Baglanti.Close();
                    return true;
                }
                else
                {
                    Baglanti.Close();
                    return false;
                }

            }
            catch (Exception ex)
            {

                Baglanti.Close();
                return false;
            }

        }

        //Veritabanına Update işlemi yapılıyor.
        public bool Guncelle(string ID)
        {
            try
            {
                if (BaglantiKontrol() == true)
                {
                    string komut = "UPDATE class SET ClassValues =ClassValues+1 where ID='" + ID + "' ";
                    MySqlCommand kmt = new MySqlCommand(komut, Baglanti);
                    kmt.ExecuteNonQuery();
                    Baglanti.Close();
                    return true;
                }
                else
                {
                    Baglanti.Close();
                    return false;
                }

            }
            catch (Exception ex)
            {

                Baglanti.Close();
                return false;
            }

        }

        //Kelimenin türüne göre sql komutu oluşturuluyor.
        private string KomutEkle(string adi, string tur, string sahibi)
        {
                string[] relationships = tur.Split('/');
                string komut = "";
                if (tur == "VB" || tur == "VBD" || tur == "VBG" || tur == "VBN" || tur == "VBP" || tur == "VBZ")
                {
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "words", "Word");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    if (returnValue == "false")
                        return komut = "insert into words (Word,VB) values('" + adi + "','" + tur + "')";
                    else
                        return komut = "UPDATE words SET wCount =wCount+1 where ID='" + returnValue + "' ";
                }
                //|| parse == "NNP" || parse == "NNPS" özel tekil ve özel çoğul isimler veritabanına eklenmiyor.
                else if (tur == "NN" || tur == "NNS")
                {

                    string[] eliminate = { "system", "data", "method" };
                    for (int i = 0; i < eliminate.Length; i++)
                    {
                        if (eliminate[i].ToString() == adi)
                        {
                            return "";
                        }
                    }
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "class", "CandidateClass");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    if (returnValue == "false")
                        return komut = "insert into class (`CandidateClass`,`ClassValues`) values('" + adi + "','" + 1 + "')";
                    else
                        return komut = "UPDATE class SET ClassValues = ClassValues+1 where ID='" + returnValue + "'";
                }
                else if (tur == "JJ" || tur == "JJR" || tur == "JJS")
                {
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "words", "Word");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    if (returnValue == "false")
                        return komut = "insert into words (Word,ADJ) values('" + adi + "','" + tur + "')";
                    else
                        return komut = "UPDATE words SET wCount =wCount+1 where ID='" + returnValue + "' ";
                }
                else if (tur == "PRP" || tur == "PRP$" || tur == "WP" || tur == "WP$")
                {
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "words", "Word");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    if (returnValue == "false")
                        return komut = "insert into words (Word,PRONOUN) values('" + adi + "','" + tur + "')";
                    else
                        return komut = "UPDATE words SET wCount =wCount+1 where ID='" + returnValue + "' ";
                }
                else if (tur == "RB" || tur == "RBR" || tur == "RBS")
                {
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "words", "Word");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    if (returnValue == "false")
                        return komut = "insert into words (Word,ADV) values('" + adi + "','" + tur + "')";
                    else
                        return komut = "UPDATE words SET wCount =wCount+1 where ID='" + returnValue + "' ";
                }
                else if (tur == "ATTRIBUTE")
                {
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "attribute", "CandidateAttribute");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    //Attribute eklediğimizde sahibi olan sınıfın values'ini 1.5 ile çarpıyoruz. Sınıf olma olasılığı yükselsin.
                    if (returnValue == "false")
                        return komut = "insert into attribute (CandidateAttribute,AttributeValues,AttributeOwner) values('" + adi + "','" + 1 + "','" + sahibi + "');" + "Update class set ClassValues=ClassValues+1 where CandidateClass='" + sahibi + "';";
                    else
                        return komut = "UPDATE attribute SET AttributeValues =AttributeValues+1 where ID='" + returnValue + "';" + "Update class set ClassValues=ClassValues + 1 where CandidateClass='" + sahibi + "';";
                }
                else if (tur == "METHOD")
                {
                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "method", "CandidateMethod");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    //Method eklediğimizde methodun sahibi olan sınıfın values'ini 1.5 ile çarpıyoruz. Sınıf olma olasılığı yükselsin.
                    if (returnValue == "false")
                        return komut = "insert into method (CandidateMethod,MethodValues,MethodOwner) values('" + adi + "','" + 1 + "','" + sahibi + "');" + "Update class set ClassValues=ClassValues + 1 where CandidateClass='" + sahibi + "';";
                    else
                        return komut = "UPDATE method SET MethodValues =MethodValues+1 where ID='" + returnValue + "';" + "Update class set ClassValues=ClassValues + 1 where CandidateClass='" + sahibi + "';";
                }
                else if (relationships[1] == "Relationships")
                {
                    return komut = "insert into relationship (FirstClass,RelationshipVerb,SecondClass) values('" + adi + "','" + relationships[0] + "','" + sahibi + "');" + "Update class set ClassValues=ClassValues +1 where CandidateClass='" + adi + "';" + "Update class set ClassValues=ClassValues +1 where CandidateClass='" + sahibi + "';";
                }
                else
                {



                    //Fonksiyondan sonra dönen değer 
                    string returnValue = VeriKontrol(adi, "words", "Word");
                    //Dönen değer false=> Database'de word kelimesi yoktur ekle
                    //Dönen değer true=> Database'de word kelimesi vardır count++
                    if (returnValue == "false")
                        return komut = "insert into words (Word,OTHER) values('" + adi + "','" + tur + "')";
                    else
                        return komut = "UPDATE words SET wCount =wCount+1 where ID='" + returnValue + "' ";
                }
         
          

          

        }


        //Verinin olup olmadığına bakıyoruz.
        private string VeriKontrol(string adi, string tabloAdi, string tabloSutun)
        {

            try
            {
                //Kolonda olan verinin ID bilgisi tutulacak.
                //Veri db varsa ID'sini yoksa false değeri tutuluyor.
                string sonuc = "false";
                //Yeni bir mysql komutu oluşturuluyor..
                MySqlCommand comm = Baglanti.CreateCommand();
                //Yapılacak işlem seçiliyor.
                comm.CommandText = "select ID from " + tabloAdi + " where " + tabloSutun + "= '" + adi + "'";
                Oku = comm.ExecuteReader();
                while (Oku.Read())
                {

                    sonuc = Oku["ID"].ToString();
                }
                Oku.Close();
                return sonuc;
            }
            catch (Exception ex)
            {

                Baglanti.Close();
                return ex.ToString();
            }

        }


        //Mysql Fulltext özelliğini kullanarak gelen kelimenin aday sınıf içinde olup olmadığına bakıyoruz.
        public List<string> IliskiVeriKontrol(string adi)
        {
            List<string> ClassList = new List<string>();
            try
            {
                //Kolonda olan verinin ID bilgisi tutulacak.
                //Veri db varsa true yoksa false değeri tutuluyor.
                if (BaglantiKontrol() != true)
                    return null;
                string sonuc = "false";
                //Yeni bir mysql komutu oluşturuluyor..
                MySqlCommand comm = Baglanti.CreateCommand();
                //Yapılacak işlem seçiliyor.
                comm.CommandText = "select * from class  where CandidateClass  like '%" + adi + "%'";
                Oku = comm.ExecuteReader();
                while (Oku.Read())
                {

                    sonuc = Oku["CandidateClass"].ToString() + "/NN";
                    ClassList.Add(sonuc);
                }
                Oku.Close();

                return ClassList;
            }
            catch (Exception ex)
            {
                Oku.Close();
                Baglanti.Close();
                ex.ToString();
                return null;
            }
        }
    }
}
