using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PERSONAL_PROJECT_2.MVVM.View
{
    public partial class HomeView : UserControl
    {
        private Grid[] sections;

        public HomeView()
        {
            InitializeComponent();

            sections = new Grid[]
            {
                Section0,
                Section1,
                Section2,
                Section3,
                Section4,
                Section5,
                Section6,
                Section7,
                Section8
            };
        }

        private void MainScrollViewer_ScrollChanged(
            object sender,
            ScrollChangedEventArgs e)
        {
            foreach (Grid section in sections)
            {
                Point position = section.TransformToAncestor(MainScrollViewer)
                                         .Transform(new Point(0, 0));

                double sectionCenter = position.Y + section.ActualHeight / 2;
                double viewerCenter = MainScrollViewer.ViewportHeight / 2;
                double distance = Math.Abs(sectionCenter - viewerCenter);
                double fadeDistance = 400;
                double opacity = 1 - (distance / fadeDistance);

                opacity = Math.Max(0, Math.Min(1, opacity));
                section.Opacity = opacity;
            }
        }
    }
}
