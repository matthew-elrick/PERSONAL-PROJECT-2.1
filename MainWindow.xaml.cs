using PERSONAL_PROJECT_2.MVVM.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PERSONAL_PROJECT_2
{
    public partial class MainWindow : Window
    {
        private bool _transitionRunning = false;

        private const double StripeWidth = 75; //45
        private const double SkewAngle = -25;
        private static readonly Color ColorDark = (Color)ColorConverter.ConvertFromString("#212D40");
        private static readonly Color ColorLight = (Color)ColorConverter.ConvertFromString("#11151C");

        private double _groupWidth;

        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ViewChangeRequested += ChangeViewWithTransition;
            }

            Loaded += (s, e) => BuildStripes();
        }

        private void BuildStripes()
        {
            TransitionLines.Children.Clear();

            double canvasWidth = TransitionCanvas.ActualWidth > 0 ? TransitionCanvas.ActualWidth : 880;
            double canvasHeight = TransitionCanvas.ActualHeight > 0 ? TransitionCanvas.ActualHeight : 645;

            double skewOverhang = canvasHeight * Math.Abs(Math.Tan(SkewAngle * Math.PI / 180));
            _groupWidth = canvasWidth + skewOverhang + StripeWidth * 4;

            int stripeCount = (int)Math.Ceiling(_groupWidth / StripeWidth) + 2;

            for (int i = 0; i < stripeCount; i++)
            {
                var rect = new Rectangle
                {
                    Width = StripeWidth,
                    Height = canvasHeight + skewOverhang * 2 + 100,
                    Fill = new SolidColorBrush(i % 2 == 0 ? ColorDark : ColorLight),
                    RenderTransform = new SkewTransform { AngleX = SkewAngle }
                };

                Canvas.SetLeft(rect, i * StripeWidth);
                Canvas.SetTop(rect, -skewOverhang - 50);

                TransitionLines.Children.Add(rect);
            }
        }

        private async void ChangeViewWithTransition(object view)
        {
            if (_transitionRunning)
                return;

            _transitionRunning = true;

            await PlayTransition(view);

            _transitionRunning = false;
        }

        private async Task PlayTransition(object view)
        {
            if (_groupWidth == 0)
                BuildStripes();

            TransitionCanvas.Visibility = Visibility.Visible;

            double canvasWidth = TransitionCanvas.ActualWidth;

            double startX = -_groupWidth;       
            double coverX = (canvasWidth - _groupWidth) / 2;
            double endX = canvasWidth;                  

            await AnimateStripesAsync(startX, coverX, TimeSpan.FromMilliseconds(350));

            if (DataContext is MainViewModel vm)
            {
                vm.CurrentView = view;
            }

            await AnimateStripesAsync(coverX, endX, TimeSpan.FromMilliseconds(350));

            TransitionCanvas.Visibility = Visibility.Collapsed;
            StripeGroupTranslate.X = startX;
        }

        private Task AnimateStripesAsync(double from, double to, TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            animation.Completed += (s, e) => tcs.SetResult(true);

            StripeGroupTranslate.BeginAnimation(TranslateTransform.XProperty, animation);

            return tcs.Task;
        }
    }
}