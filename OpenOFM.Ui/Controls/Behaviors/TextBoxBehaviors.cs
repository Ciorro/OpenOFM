using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls.Behaviors
{
    class TextBoxBehaviors
    {
        public static bool GetIsNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericProperty);
        }

        public static void SetIsNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericProperty, value);
        }

        public static readonly DependencyProperty IsNumericProperty = DependencyProperty.RegisterAttached(
            "IsNumeric",
            typeof(bool),
            typeof(TextBox),
            new PropertyMetadata()
            {
                PropertyChangedCallback = OnIsNumericChanged
            });

        private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (e.NewValue as bool? == true)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    DataObject.AddPastingHandler(d, OnPasted);
                }
                else
                {
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    DataObject.RemovePastingHandler(d, OnPasted);
                }
            }
        }

        private static void OnPasted(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = e.DataObject.GetData(DataFormats.Text) as string;
                if (text is not null && !IsTextValid(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!IsTextValid(e.Text))
            {
                e.Handled = true;
            }
        }

        private static bool IsTextValid(string text)
        {
            return text.All(char.IsDigit);
        }
    }
}
