namespace DarkestLoadOrder.Pages
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using ModernWpf.Controls;

    public partial class ControlsPage
    {
        public ControlsPage()
        {
            InitializeComponent();
        }

        private void ReduceSpacing(Panel panel)
        {
            const double factor = 0.5;

            if (panel != null)
            {
                ReduceMargin(panel);

                if (panel is SimpleStackPanel sp)
                {
                    var spacing = sp.Spacing;
                    if (spacing != 0)
                    {
                        sp.Spacing = spacing * factor;
                    }
                }

                foreach (var child in panel.Children.OfType<FrameworkElement>())
                {
                    if (child is Panel childAsPanel)
                    {
                        ReduceSpacing(childAsPanel);
                    }
                    else
                    {
                        ReduceMargin(child);
                    }
                }
            }

            static void ReduceMargin(FrameworkElement element)
            {
                var margin = element.Margin;
                if (margin != new Thickness())
                {
                    element.Margin = new Thickness(
                        margin.Left * factor,
                        margin.Top * factor,
                        margin.Right * factor,
                        margin.Bottom * factor);
                }
            }
        }
    }
}
