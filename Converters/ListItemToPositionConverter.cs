namespace DarkestLoadOrder.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ListItemToPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ListBoxItem item)
                return null;

            var lb = FindAncestor<ListBox>(item);

            var index = lb?.Items.IndexOf(item.Content);

            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static T FindAncestor<T>(DependencyObject from) where T : class
        {
            while (true)
                switch (from)
                {
                    case null:
                        return null;

                    case T candidate:
                        return candidate;

                    default:
                        from = VisualTreeHelper.GetParent(from);

                        break;
                }
        }
    }
}
