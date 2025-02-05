using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OpenOFM.Ui.Converters
{
    class AllValuesEqualToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new AllValuesEqualToVisibilityConverter();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool allEquals = values.All(value =>
            {
                if (parameter is null)
                {
                    return value is null;
                }

                parameter = System.Convert.ChangeType(parameter, value.GetType(), CultureInfo.InvariantCulture);
                return value.Equals(parameter);
            });

            return allEquals ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
