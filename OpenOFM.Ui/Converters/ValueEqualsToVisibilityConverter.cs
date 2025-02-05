using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OpenOFM.Ui.Converters
{
    class ValueEqualsToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ValueEqualsToVisibilityConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is null)
            {
                return value is null ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }

            parameter = System.Convert.ChangeType(parameter, value.GetType(), CultureInfo.InvariantCulture);

            return value.Equals(parameter) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
