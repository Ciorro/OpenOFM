using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OpenOFM.Ui.Converters
{
    class ValueGreaterThanToBoolConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ValueGreaterThanToBoolConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            parameter = System.Convert.ChangeType(parameter, value.GetType(), CultureInfo.InvariantCulture);

            if (value is IComparable comparable)
            {
                return comparable.CompareTo(parameter as IComparable) < 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
