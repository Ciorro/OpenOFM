using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OpenOFM.Ui.Converters
{
    internal class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BooleanToVisibilityConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool parameterIsBool = bool.TryParse(parameter?.ToString(), out var param);

            return value as bool? == (parameterIsBool ? param : true) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
