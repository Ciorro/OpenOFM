using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OpenOFM.Ui.Converters
{
    class TimeSpanLessThanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TimeSpanLessThanToVisibilityConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var maxTimeSpan = (parameter as TimeSpan?) ?? TimeSpan.Zero;

            return value is TimeSpan timeSpan && timeSpan < maxTimeSpan ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
