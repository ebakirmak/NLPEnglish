
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using POSParser;
namespace POS
{
    public partial class FrmMain : Form
    {
        private ListBox[] listbox;
        public FrmMain()
        {
            InitializeComponent();
           
        }
        List<Sinif> ClassList = new List<Sinif>();
        private void btnRun_Click(object sender, EventArgs e)
        {
            //OpenFileDialog ile .txt dosyası alınıyor.
            string filePath = SelectDocument();
            if (filePath=="false")
            {
                MessageBox.Show("İşlem Gerçekleştirilmedi");
                return;
            }
            //Eğer Uml Diagramı varsa siliniyor.
            UmlRemove();
            //Alınan .txt dosyasındaki metine pos ve stemming işlemleri uygulanıyor.
            PosAndStemming(filePath);
            //Aday sınıf ve Methodlar listboxlarda gösteriliyor.
            DisplayCandidateClassAndMethod();
            //Aday sınıfların Uml Diagramları oluşturuluyor.
            UmlCreate();

        }

        private void DisplayCandidateClassAndMethod()
        {
            ListBoxClear();
            Sinif sinif = new Sinif();
            
            //ADAY SINIF
            List<Sinif> SinifListesi = new List<Sinif>();
            SinifListesi = sinif.Sec();
            if (SinifListesi != null)
            {
                foreach (var eleman in SinifListesi)
                {
                    //Listeler oluşturulması için eklendi.
                    ClassList.Add(eleman);
                    //lstClass.Items.Add("Word ID: " + eleman.ID + "  Word: " + eleman.Adi);
                }
            }
            else { }
                //lstClass.Items.Add("Aday Sınıf Yok.");

            //ADAY METHOD
            Method method = new Method();
            List<Method> ListCandidateMethod = new List<Method>();
            ListCandidateMethod = method.Sec();
            if (ListCandidateMethod != null)
            {
                foreach (var eleman in ListCandidateMethod)
                {
                    //lstMethod.Items.Add("Word ID: " + eleman.ID + "  Word: " + eleman.Adi);
                }
            }
            else { }
                //lstMethod.Items.Add("Aday Method Yok.");


        }
        private void PosAndStemming(string filePath)
        {
            //string appPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            //var dosyaYolu = appPath + @"\EnglishText.txt";
            Ayristirici.MetinTemizle(filePath);
            Ayristirici.Baslat(filePath);
        }
        private string SelectDocument()
        {
            string filePath = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Browse Text Files";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = (openFileDialog1.FileName);
            }
            else
            {
                return "false";
            }
            return filePath;
        }     
        private void UmlCreate()
        {
        
            int sayac = 1; 
            int height = 100, width = 250;
            int locHeight = 100, locWidth = 250; ;
            listbox=new ListBox[ClassList.Count];
          
            Nitelik nitelik = new Nitelik();
            List<Nitelik> NitelikList = new List<Nitelik>();

            Method method = new Method();
            List<Method> MethodList = new List<Method>();
            MethodList = method.Sec();
            NitelikList = nitelik.Sec();

            foreach (var elemanClass in ClassList)
            {
                
               
               

                if (elemanClass.Adi!="lst"+elemanClass.Adi)
                {
                    
                    ListBox lst = new ListBox();//Listbox nesnesi oluşturuldu.                
                    lst.Name = "lst" + elemanClass.Adi;
                    listbox[lstSayac++] = lst;
                    lst.Items.Add(elemanClass.Adi + " Sınıfı ");
                    lst.Items.Add("______________________");
                    foreach (var elemanAttribute in NitelikList)
                    {
                       

                        if (elemanClass.Adi==elemanAttribute.Sahibi)
                        {
                            lst.Items.Add("\n" + elemanAttribute.Adi);
                        }
                       
                    }
                    lst.Items.Add("______________________");
                    foreach (var elemanMethod in MethodList )
                    {
                        
                        if (elemanClass.Adi == elemanMethod.Sahibi)
                        {
                            lst.Items.Add("\n" + elemanMethod.Adi+"()");
                        }
                       
                    }

                    if (width * sayac + width < 1100)
                    {
                        lst.Size = new Size(width, height);
                        lst.Location = new Point((locWidth + 5) * sayac, locHeight);
                        panel.Controls.Add(lst);
                        sayac += 1;
                    }
                    else
                    {
                        locWidth = 250;
                        locHeight += height + 5;
                        lst.Size = new Size(width, height);
                        lst.Location = new Point((locWidth + 5), locHeight);
                        panel.Controls.Add(lst);
                        sayac = 2;

                    }
                }
              
                
            }
            ClassList.Clear();
        }
        int lstSayac = 0;
        private void UmlRemove()
        {
            //Eğer uml diagramı için kullanılmış liste varsa onları kaldırıyoruz.
            if (lstSayac > 0)
            {
                for (int i = 0; i < listbox.Count(); i++)
                {
                    this.panel.Controls.Remove(listbox[i]);
                }
                lstSayac = 0;
            }
        }
        private void ListBoxClear()
        {
            //lstMethod.Items.Clear();
            //lstClass.Items.Clear();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
            
        }
    }
}
