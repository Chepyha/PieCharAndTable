using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Globalization;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

   

    public partial class MainWindow : Window
    {
        public List<string> address = new List<string>();
        public ExName[] rep = new ExName[0];
        public Table[] Trep = new Table[0];
        public List<Table> ListEx = new List<Table>();
        public HttpClient client = new HttpClient();
        public List<LogIn> Logs= new List<LogIn>();

        public class LogIn
        {
            public string name { get; set; }
            public string Log { get; set; }
            public string pass { get; set; }
            public LogIn(string name="", string Log = "", string pass = "")
            {
                this.name = name;
                this.Log = Log;
                this.pass   = pass;
            }
        }



        public MainWindow()
        {
            //маштабирование по всему экрану
            double width = System.Windows.SystemParameters.PrimaryScreenWidth+22;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight+10;
            InitializeComponent();
            var crd = System.Windows.SystemParameters.WorkArea.TopLeft;
            this.Left = crd.X-10;
            this.Top = crd.Y-1;
            HostingWfInWpf.Width = width;
            HostingWfInWpf.Height = height;
            //создание переменных

            //заполнение адресов
            address.Add("https://localhost:7204/rgp");
            address.Add("https://localhost:7204/pe");
            address.Add("");
            address.Add("");
            Logs.Add(new LogIn("https://localhost:7204/login", "1", "1"));
            rep = new ExName[address.Count];
            Trep = new Table[address.Count];
            //заполнение данных и заполнение листа
            avtoritiz();


            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


        }

        //вспомогательный класс для вывода в DataGrid
        public class Table
        {
            public string Name { get; set; }
            public ImageBrush col { set; get; }=new ImageBrush();
            public ImageBrush main { set; get; } = new ImageBrush();
            public string ex { set; get; }
            public string Name1 { set; get; }
            public string fex { set; get; }
            public Table(string Name = "Null", Image vv=null, Image xe=null, string Name1 = "None", string ex = "Error", string fex = "")
            {
                this.Name = Name;
                if (vv != null)
                {
                    this.col.ImageSource = vv.Source;
                }
                else this.col = new ImageBrush();

                if (xe != null)
                {
                    this.main.ImageSource = xe.Source;
                }
                else this.main = new ImageBrush();
                this.ex = ex;
                this.Name1 = Name1;
                this.fex = fex;
            }
        }

        //Основной класс для заполнения
        public class ExName
        {
            public string Name  { get; set; }
            public Byte[] Img { set; get; }=null;
            public Byte[] Img1 { set; get; }= null;
            public string ex { set; get; }
            public string Name1 { set; get; }
            public string fex { set; get; }
            public int IfExe { get; set; }

            public ExName(string Name = "null", Byte[] img = null,Byte[] img1=null, int IfExe = 0, string Name1="None", string ex="Error", string fex="")
            {
                this.Name = Name;
                this.Img = img;
                this.Img1 = img1;
                this.IfExe = IfExe;
                this.Name1 = Name1; 
                this.ex = ex;   
                this.fex = fex;
            }

            public Table CreateTable()
            {
                Image Imp1 = new Image();
                Image Imp2 = new Image();
                Imp1 = ByteArrToImage(Img);
                Imp2 = ByteArrToImage(Img1);
                return new Table(Name, Imp1,Imp2,Name1,ex,fex);
            }
        }

        //медот отправки запрос и возвращения данных
        public async Task<ExName> GetClass(string address)
        {
            ExName rep = new ExName();
            try
            {
                rep = await client.GetFromJsonAsync<ExName>(address);
                if (rep != null)
                {
                    return rep;
                }
            }
            catch (Exception ex)
            {
                return new ExName(ex.Message, null,null,0);
            }
            return new ExName("vvvv", null,null,0);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ListEx.Clear();
            //ImageBrush bra = new ImageBrush();
            //Image im = new Image();
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource=new Uri(@"C:\Users\Админ\Desktop\practic\WpfApp1\WpfApp1\Properties\2590a1a6759841581e6e1ed7fc91376d.jpg");
            //bitmap.EndInit();
            //im.Source = bitmap;

            for (int i = 0; i < address.Count; i++)
            {
                Task<ExName> serp = Task.Run(() => GetClass(address[i]));
                    rep[i] = serp.Result;
                    Trep[i] = rep[i].CreateTable();
                    ListEx.Add(Trep[i]);
            }

            Dgrid.ItemsSource = ListEx;


            GrChart.Children.OfType<Canvas>().ToList().ForEach(p => GrChart.Children.Remove(p));
            PieChart chart = null;
            chart = new PieChart();
            GrChart.Children.Add(chart.ChartBackground);
            GrChart.UpdateLayout();

            CreateChart(chart);


            Dgrid.Items.Refresh();
        }

        static Image ByteArrToImage(Byte[] byteArr)
        {
            if (byteArr != null)
            {
                Image img = new Image();
                MemoryStream stream = new MemoryStream(byteArr);
                img.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return img;
            }
            return null;
        }

        static Byte[] ImageToByteArr(BitmapImage img)
        {
            if (img == null) return null;
            MemoryStream stream = new MemoryStream();
            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(img));
            jpg.Save(stream);
            return stream.ToArray();
        }

        public async Task avtoritiz()
        {
            foreach (var x in Logs)
            {
                var values = new Dictionary<string, string>
                {
                    { "email", x.Log},
                    { "password", x.pass }
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync(x.name, content);

                var responseString = await response.Content.ReadAsStringAsync();
            }
        }


        private void CreateChart(PieChart chart)
        {
            chart.Clear();

            for (int i = 0; i < 3; i++)
            {
                chart.AddValue(Counti(i));
            }
        }

        private int Counti(int i)
        {
            int count = 0;
            if (i == 2)
            {
                foreach (var x in rep)
                {
                    if (x.ex == "Error")
                        count++;
                }
            }
            if (i == 1)
            {
                foreach (var x in rep)
                {
                    if (x.ex == "DataError")
                        count++;
                }
            }
            if (i == 0)
            {
                foreach (var x in rep)
                {
                    if (x.ex == "Ok")
                        count++;
                }
            }
            return count;
        }
    }
}
