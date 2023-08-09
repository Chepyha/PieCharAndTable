using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace WpfApp1
{
    class BarChart 
    {
        private double _gap = 5;
        private readonly double _factor = 0.666666666666667;
        protected readonly double PaddingChart = 10;
        public double WidthChart;
        public double HeightChart;

        public Canvas ChartBackground = new Canvas();

        public BarChart()
        {
            ChartBackground.Margin = new Thickness(0);
            ChartBackground.SizeChanged += ChartBackground_SizeChanged;
        }

        private void ChartBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Инициализация ширины и высоты графиков.
            WidthChart = e.NewSize.Width - (PaddingChart * 2);

            // Верхний предел графика на 10 линии снизу.
            HeightChart = e.NewSize.Height * _factor;
        }

        public void AddValue(double value)
        {
            // Получаем все значения которые уже есть в графике.
            List<double> listValues = ChartBackground.Children.OfType<Rectangle>().Select(p => (double)p.Tag).ToList();

            // Добавляем новое значение в график.
            listValues.Add(value);

            // Вычисляем новую ширину бара, чтобы график поместился 
            // полностью на ширину поля.
            double widthBar = (WidthChart - ((listValues.Count - 1) * _gap)) / listValues.Count;

            // Для ограничения высоты графика, вне зависимости от абсолютных значений,
            // вычислим общий знаменатель. И самое большое значение будет на максимальной
            // допустимой высоте, остальные пропорционально ниже.
            double maxValue = listValues.Max();
            double denominator = maxValue / HeightChart;

            // Удалим текущие элементы графика.
            Clear();

            //Random random = new();

            foreach (double val in listValues)
            {
                int count = ChartBackground.Children.OfType<Rectangle>().Count();

                // Относительная высота точки от нижнего края.
                // Для этого все абсолютные значения делятся на общий знаменатель,
                // чтобы максимальная высота точек не выходила выше установленной.
                double heightPoint = val / denominator;

                // Для улучшения визуального восприятия.
                if (heightPoint < 3)
                {
                    heightPoint = 3;
                }

                //// Координата X расположения полосы, координата Y равна 0:
                //// полоса начинается от нижнего края.
                //double x = (count * (widthBar + _gap)) + (ChartBackground.ActualWidth - WidthChart) / 2;

                //// Создание полосы.
                //Rectangle bar = CreateBar(x, heightPoint, widthBar, val);
                //_ = ChartBackground.Children.Add(bar);

                //// Надпись над полосой.
                //Label title = CreateTitle(x, bar.Height, widthBar, val);
                //_ = ChartBackground.Children.Add(title);
            }
        }

        //public static explicit operator BarChart(UIElement v)
        //{
        //    throw new NotImplementedException();
        //}

        public void Clear() => ChartBackground.Children.Clear();


        #region Private


        /// <summary>
        /// Создание полосы графика
        /// </summary>
        /// <param name="x">x координата</param>
        /// <param name="height">высота</param>
        /// <param name="width">ширина</param>
        /// <param name="value">абсолютное значение</param>
        /// <returns></returns>
        private Rectangle CreateBar(double x, double height, double width, double value)
        {
            Random random = new();

            Rectangle bar = new()
            {
                Stroke = Brushes.Black,
                Fill = new SolidColorBrush(Color.FromArgb(255, (byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256))),
                Height = height,
                Width = width,
                StrokeThickness = 0.5,
                Tag = value
            };

            Canvas.SetLeft(bar, x);
            Canvas.SetBottom(bar, 0);

            return bar;
        }



        /// <summary>
        /// Создание текстовой надписи над полосой графика.
        /// </summary>
        /// <param name="x">x координата</param>
        /// <param name="y">y координата</param>
        /// <param name="width">ширина поля надписи</param>
        /// <param name="value">абсолютное значение выводится как текст</param>
        /// <returns></returns>
        //private Label CreateTitle(double x, double y, double width, double value)
        //{
        //    Label title = new()
        //    {
        //        Content = value,
        //        HorizontalContentAlignment = HorizontalAlignment.Center,
        //        Width = width,
        //        Padding = new Thickness(0, 0, 0, 10)
        //    };

        //    Canvas.SetLeft(title, x);
        //    Canvas.SetBottom(title, y);

        //    return title;
        //}


        #endregion

    }
}
