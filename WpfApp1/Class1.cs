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
    class PieChart
    {
        private readonly double _factor = 0.666666666666667;
        protected readonly double PaddingChart = 10;
        public double WidthChart;
        public double HeightChart;

        public Canvas ChartBackground = new Canvas();

        public PieChart()
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

        public void AddValue(double[] value)
        {
            StoredValues[] listValues = new StoredValues[value.Length];
            listValues = СalculateSectorAngle(value);

            // Удалим все элементы перед созданием новых актуальных.
            Clear();

            // Размещение секторов вычисленного размера для создания Pie Chart.
            for (int i = 0; i < listValues.Length; i++)
            {
                StoredValues sv = listValues[i];

                // Каждый Path-элемент будет хранить данные сектора для последующих вычислений.
                Path p = CreateSector(sv.Degree, sv.Offset, sv.Value, i);
                ChartBackground.Children.Add(p);

                // Числовые значения секторов диска.
                //Label label = new Label()
                //{
                //    Content = sv.Value
                //};

                //// Цветовые метки перед числовыми
                //Rectangle r = new Rectangle()
                //{
                //    Width = 16,
                //    Height = 12,
                //    Fill = p.Fill,
                //    Stroke = Brushes.White,
                //    StrokeThickness = 1
                //};

                //StackPanel sp = new StackPanel()
                //{
                //    Orientation = Orientation.Horizontal
                //};
                //sp.Children.Add(r);
                //sp.Children.Add(label);
                //Canvas.SetLeft(sp, 10);
                //Canvas.SetTop(sp, 20 * i);
                //_ = ChartBackground.Children.Add(sp);
            }
        }


        public void Clear()
        {
            ChartBackground.Children.Clear();
        }


        private Path CreateSector(double degree, double offset, double value, int k)
        {
            Random random = new Random();
            SolidColorBrush Fl = new SolidColorBrush
                (Color.FromRgb(Convert.ToByte(random.Next(0,255)), Convert.ToByte(random.Next(0, 255)), Convert.ToByte(random.Next(0, 255)) ));



            Path path = new Path()
            {
                StrokeThickness = 5,
                Stroke = Brushes.White,
                Fill = Fl,

                Data = new PathGeometry()
                {
                    Figures = new PathFigureCollection()
                    {
                        SectorGeometry(degree, offset)
                    }
                }
            };
            return path;
        }


        public PathFigure SectorGeometry(double degree, double offset)
        {
            double _radius = 54;
            bool islarge = false;
            if (degree > 180) islarge = true;
            if (degree >= 360) degree = 359.999;
            Point centerPoint = new Point(64, 64);
            Point startPoint = new Point(centerPoint.X, centerPoint.Y + _radius);
            Point endPoint = startPoint;
            RotateTransform rotateStartPoint = new RotateTransform(offset)
            {
                CenterX = centerPoint.X,
                CenterY = centerPoint.Y
            };
            startPoint = rotateStartPoint.Transform(startPoint);
            RotateTransform rotateEndPoint = new RotateTransform(offset + degree)
            {
                CenterX = centerPoint.X,
                CenterY = centerPoint.Y
            };
            endPoint = rotateEndPoint.Transform(endPoint);
            PathFigure sector = new PathFigure()
            {
                StartPoint = startPoint,
                Segments = new PathSegmentCollection()
                {
                    new ArcSegment()
                    {
                        Point = endPoint,
                        Size = new Size(_radius, _radius),
                        SweepDirection = SweepDirection.Clockwise,
                        IsLargeArc = islarge,
                        IsStroked = true
                    },

                    new PolyLineSegment()
                    {
                        Points = new PointCollection() { endPoint, centerPoint, startPoint },
                        IsStroked = false
                    }
                }
            };

            return sector;
        }

        private StoredValues[] СalculateSectorAngle(double[] value)
        {
            StoredValues[] listValues= new StoredValues[value.Length];
            double sum = value.Sum();
            double denominator = sum / 360;
            for (int i = 0; i < value.Count(); i++)
            {
                double degree = Math.Round(value[i] / denominator, 2);
                double offset = 0;
                if (i > 0)
                {
                    offset = listValues[i - 1].Degree + listValues[i - 1].Offset;
                }

                listValues[i]=new StoredValues(degree, offset, value[i]);
            }

            return listValues;
        }

        public class StoredValues
        {
            public double Degree;
            public double Offset;
            public double Value;
            public StoredValues(double degree, double offset, double value)
            {
                Degree = degree;
                Offset = offset;
                Value = value;
            }
        }
    }
}
