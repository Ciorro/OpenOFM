using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls
{
    internal class InfoBar : ContentControl
    {
        public enum MessageSeverity
        {
            Info, Success, Warning, Error
        }

        static InfoBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(InfoBar), new FrameworkPropertyMetadata(typeof(InfoBar)));
        }

        public override void OnApplyTemplate()
        {
            var closeButton = GetTemplateChild("PART_CloseButton");
            if (closeButton is Button button)
            {
                button.Click += (_, __) =>
                {
                    IsOpen = false;
                };
            }

            base.OnApplyTemplate();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(InfoBar));


        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(InfoBar));


        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen",
            typeof(bool),
            typeof(InfoBar),
            new PropertyMetadata(true));


        public bool IsClosable
        {
            get => (bool)GetValue(IsClosableProperty);
            set => SetValue(IsClosableProperty, value);
        }

        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register(
            "IsClosable",
            typeof(bool),
            typeof(InfoBar),
            new PropertyMetadata(true));


        public MessageSeverity Severity
        {
            get => (MessageSeverity)GetValue(SeverityProperty);
            set => SetValue(SeverityProperty, value);
        }

        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
            "Severity",
            typeof(MessageSeverity),
            typeof(InfoBar),
            new PropertyMetadata(MessageSeverity.Info));
    }
}
