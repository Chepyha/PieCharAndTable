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
using System.Net.Http.Json;
using System.Globalization;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;

using System.Runtime.CompilerServices;
using System.Drawing.Design;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

   

    public partial class MainWindow : Window
    {
        public List<LogIn> Logs = new List<LogIn>();
        public List<ExName> rep = new List<ExName>();
        public List<Table> Trep =   new List<Table>();
        public List<Table> ListEx = new List<Table>();
        public HttpClient client = new HttpClient();
        public int Time = 0;
        public int[] countElement= null;

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
            Logs.Add(new LogIn("https://localhost:7204/login", "1", "1", "https://localhost:7204/pe",5));
            Logs.Add(new LogIn("https://localhost:7204/login", "1", "1", "https://localhost:7204/rgp", 10));
            Logs.Add(new LogIn("http://vnrzfdocora01:3000/login", "Test", "Test!123", "http://vnrzfdocora01:3000/pe",5));
            //заполнение данных и заполнение листа
            countElement=new int[Logs.Count];

            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public class LogIn
        {
            public string name { get; set; }
            public string Log { get; set; }
            public string pass { get; set; }
            public string address { get; set; }
            public int timeSec { get; set; }
            public LogIn(string name = "", string Log = "", string pass = "", string address = "", int timeSec = 15)
            {
                this.name = name;
                this.Log = Log;
                this.pass = pass;
                this.address = address;
                this.timeSec = timeSec;
            }
        }

        //вспомогательный класс для вывода в DataGrid
        public class Table
        {
            public string Name { get; set; }
            public string ex { set; get; }
            public string Name1 { set; get; }
            public string fex { set; get; }
            public ImageBrush main { set; get; } = new ImageBrush();
            public int IfExe { get; set; }
            public List<DatePie> PieList { set; get; } = null;
            public int x { set; get; }
            public int y { set; get; }
            public Table(string Name = "Null", Image xe=null, string Name1 = "None", int IfExe = 0, string ex = "Error", string fex = "", List<DatePie> PieList = null, int x=0, int y=0)
            {
                this.Name = Name;
                if (xe != null)
                {
                    this.main.ImageSource = xe.Source;
                }
                else this.main = new ImageBrush();
                this.ex = ex;
                this.Name1 = Name1;
                this.fex = fex;
                if (PieList == null) PieList= new List<DatePie>();
                this.PieList = PieList;
                this.IfExe = IfExe;
                this.x = x;
                this.y = y;
            }
        }

        public class DatePie
        {
            public string NameP { get; set; }
            public int Value { get; set; }
            public DatePie(string NameP = "None", int Value=0)
            {
                this.NameP = NameP;
                this.Value = Value; 
            }
        }
        //Основной класс для заполнения
        public class ExName
        {
            public string Name  { get; set; }
            public string ex { set; get; }
            public string Name1 { set; get; }
            public string fex { set; get; }
            public Byte[] Img { set; get; }=null;
            public int IfExe { get; set; }
            public List<DatePie> PieList { set; get; } = null;
            public int x { set; get; }
            public int y { set; get; }
            public ExName(string Name = "null", Byte[] img = null, int IfExe = 0, string Name1="None", string ex="Error", string fex="", List<DatePie>PieList=null, int x=0, int y=0)
            {
                this.Name = Name;
                this.Img = img;
                this.IfExe = IfExe;
                this.Name1 = Name1; 
                this.ex = ex;   
                this.fex = fex;
                this.PieList=PieList;
                this.x = x;
                this.y = y;
            }
            public Table CreateTable()
            {
                Image Imp1 = new Image();

                Imp1 = ByteArrToImage(Img);

                return new Table(Name, Imp1,Name1,IfExe,ex,fex,PieList,x,y);
            }
        }

        //медот отправки запрос и возвращения данных
        public async Task<List<ExName>> GetClass(string address)
        {
            List < ExName> rep = new List<ExName>();
            try
            {
                rep = await client.GetFromJsonAsync<List<ExName>>(address);
                if (rep != null)
                {
                    return rep;
                }
            }
            catch (Exception ex)
            {
                rep.Add(new ExName(ex.Message, null,0));
            }
            return rep;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            bool b=false;
            //Заполнение и обновление данных в Trep 
            if (Time == 0)
            {
                for (int i = 0; i < Logs.Count; i++)
                {
                    avtoritiz(i);
                    Task<List<ExName>> serp = Task.Run(() => GetClass(Logs[i].address));
                    rep = serp.Result;
                    countElement[i] = rep.Count;
                    foreach (ExName ex in rep)
                    {
                        Trep.Add(ex.CreateTable());
                    }
                }
            }
            Time += 1;
            int summcount = 0;
            int rz = 0;
            for (int i = 0; i < Logs.Count; i++)
            {
                if (0 == Time % Logs[i].timeSec)
                {
                    avtoritiz(i);
                    Task<List<ExName>> serp = Task.Run(() => GetClass(Logs[i].address));
                    rep = serp.Result;
                    if (countElement[i] < rep.Count)
                    {
                        rz = rep.Count - countElement[i];
                        for (int j = 0; j < rz; j++)
                        {
                            Trep.Add(new Table());
                        }
                        for (int j = Trep.Count - rz - 1; j > summcount + countElement[i]; j--)
                        {
                            Trep[j + rz] = Trep[j];
                        }
                        countElement[i] = rep.Count;
                    }
                    if (countElement[i] > rep.Count)
                    {
                        rz = -rep.Count + countElement[i];
                        for (int j = 0; j < rz; j++)
                        {
                            Trep.RemoveAt(summcount + rep.Count);
                        }
                        countElement[i] = rep.Count;
                    }
                    for (int j = 0; j < countElement[i]; j++)
                    {
                        if (countElement[i] <= rep.Count)
                        {
                            Trep[j + summcount] = rep[j].CreateTable();
                        }
                        else
                        {
                            Trep[j + summcount] = new Table();
                        }
                    }
                    summcount += countElement[i];
                    b = true;
                }
            }
            //Конец Заполнения и обновления


            int[] MandM = MinAndMaks(Trep);
            Grid[] gr=new Grid[Trep.Count];
            RowDefinition[] row=new RowDefinition[MandM[1]+1];
            ColumnDefinition[] Column = new ColumnDefinition[MandM[0]+1];
            for (int i = 0; i < MandM[0]; i++) 
            { Column[i] = new ColumnDefinition(); Column[i].Width = new GridLength(128); Grid_View.ColumnDefinitions.Add(Column[i]); }
            for (int i=0; i< MandM[1]; i++) 
            { row[i] = new RowDefinition(); row[i].Height = new GridLength(128); Grid_View.RowDefinitions.Add(row[i]); }

            if (b)
            {
                for (int i = 0; i < Trep.Count; i++)
                {
                    gr[i] = new Grid();
                    Grid_View.Children.Add(gr[i]);
                    Grid.SetColumn(gr[i], Trep[i].x);
                    Grid.SetRow(gr[i], Trep[i].y);
                    gr[i].Height = 128;
                    gr[i].Width = 128;

                    switch (Trep[i].IfExe)
                    {
                        case 0:
                            {
                                TextBlock block = new TextBlock();
                                block.Text = Trep[i].ex + "   " + Trep[i].fex;
                                block.Background = Trep[i].main;
                                block.Width = 128;
                                block.Height = 128;
                                gr[i].Children.Add(block);
                                gr[i].UpdateLayout();
                                break;
                            }
                        case 1:
                            {

                                double[] d = new double[Trep[i].PieList.Count];
                                PieChart ch = new PieChart();
                                for (int j = 0; j < Trep[i].PieList.Count; j++)
                                {
                                    d[j] = (double)Trep[i].PieList[j].Value;
                                }
                                CreateChart(ch, d);
                                gr[i].Children.Add(ch.ChartBackground);
                                gr[i].UpdateLayout();
                                break;
                            }
                    }
                }
                b = false;
            }
            Grid_View.UpdateLayout();










            //Задания объектов для проприсовки Круговой диаграммы 
            //Dgrid.ItemsSource = Trep;
            //GrChart.Children.OfType<Canvas>().ToList().ForEach(p => GrChart.Children.Remove(p));
            //PieChart chart = null;
            //chart = new PieChart();
            //GrChart.Children.Add(chart.ChartBackground);
            //GrChart.UpdateLayout();
            //CreateChart(chart);
            //Dgrid.Items.Refresh();
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

        public async Task avtoritiz(int i)
        {
                var values = new Dictionary<string, string>
                {
                    { "email", Logs[i].Log},
                    { "password", Logs[i].pass }
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync(Logs[i].name, content);

                var responseString = await response.Content.ReadAsStringAsync();
        }


        private void CreateChart(PieChart chart, double[] n)
        {
            chart.Clear();
            chart.AddValue(n);
        }

        static int[] MinAndMaks(List<Table> tabl)
        {
            int maksx = 0;
            int maksy = 0;
            for (int i=0; i<tabl.Count;i++)
            {
                if (maksx < tabl[i].x) maksx = tabl[i].x;
                if (maksy < tabl[i].y) maksy = tabl[i].y;
            }
            return new int[] { maksx, maksy};
        }

        //private int Counti(int i)
        //{
        //    int count = 0;
        //    if (i == 2)
        //    {
        //        foreach (Table x in Trep)
        //        {
        //            if (x.ex == "Error")
        //                count++;
        //        }
        //    }
        //    if (i == 1)
        //    {
        //        foreach (Table x in Trep)
        //        {
        //            if (x.ex == "DataError")
        //                count++;
        //        }
        //    }
        //    if (i == 0)
        //    {
        //        foreach (Table x in Trep)
        //        {
        //            if (x.ex == "Ok")
        //                count++;
        //        }
        //    }
        //    return count;
        //}
    }
}
