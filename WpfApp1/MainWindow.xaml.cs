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
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Globalization;



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
            address.Add("https://localhost:7204");
            address.Add("");
            address.Add("");
            address.Add("");


            rep =new ExName[address.Count];
            Trep = new Table[address.Count];
            //заполнение данных и заполнение листа
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


        }

        //вспомогательный класс для вывода в DataGrid
        public class Table
        {
            public string name { get; set; }
            public string col { set; get; }
            public string Exep { get; set; }          
            public Table(string Name = "", string ex = "", string ExeptEx = "")
            {
                this.name = Name;
                this.Exep = ExeptEx;
                this.col = ex;
            }
        }

        //Основной класс для заполнения
        public class ExName
        {
            public string Name 
            { get; set; }
            public string ex { set; get; }
            public bool IfExe { get; set; }
            public string ExeptEx { get; set; }
            public ExName(string Name = "", string ex = "", bool IfExe = true, string ExeptEx = "")
            {
                this.Name = Name;
                switch (ex)
                {
                    case "Error":
                        this.ex = ex; break;
                    case "DataError":
                        this.ex = ex; break;
                    case "Ok":
                        this.ex = ex;break;
                    default: this.ex = "not Information"; break;
                }
                this.IfExe = IfExe;
                this.ExeptEx = ExeptEx;
            }

            public Table CreateTable()
            {
                return new Table(Name, ex, ExeptEx);
            }
        }

        //медот отправки запрос и возвращения данных
        public async Task<ExName> GetClass(string address)
        {
            HttpClient client = new HttpClient();
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
                return new ExName("None", "Error", false, "");
            }
            return new ExName("None", "Error", false, "");
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ListEx.Clear();
            for (int i = 0; i < address.Count; i++)
            {
                Task<ExName> serp = Task.Run(() => GetClass(address[i]));
                rep[i] = serp.Result;
                Trep[i] = rep[i].CreateTable();
                ListEx.Add(Trep[i]);
            }
            //вывод данных  в DataGrid

            Dgrid.ItemsSource = ListEx;
            GrChart.Children.OfType<Canvas>().ToList().ForEach(p => GrChart.Children.Remove(p));
            PieChart chart = null;
            chart = new PieChart();
            GrChart.Children.Add(chart.ChartBackground);
            GrChart.UpdateLayout();

            CreateChart(chart);
            Dgrid.Items.Refresh();
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
            if (i==2)
            {
                foreach(var x in rep)
                {
                    if (x.ex=="Error")
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
