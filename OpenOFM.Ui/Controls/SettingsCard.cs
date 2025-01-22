using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls
{
    class SettingsCard : HeaderedContentControl
    {
        static SettingsCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SettingsCard), new FrameworkPropertyMetadata(typeof(SettingsCard)));
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", 
            typeof(string), 
            typeof(SettingsCard));


        public UIElement Icon
        {
            get => (UIElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", 
            typeof(UIElement), 
            typeof(SettingsCard));
    }
}
