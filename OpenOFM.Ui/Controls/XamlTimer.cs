using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenOFM.Ui.Controls
{
    internal class XamlTimer : Control
    {
        private readonly DispatcherTimer _timer;

        static XamlTimer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(XamlTimer), new FrameworkPropertyMetadata(typeof(XamlTimer)));
        }

        public XamlTimer()
        {
            IsEnabledChanged += XamlTimer_IsEnabledChanged;

            _timer = new DispatcherTimer();
            _timer.Tick += (sender, args) =>
            {
                RaiseEvent(new RoutedEventArgs(TickEvent, this));
            };
            _timer.Start();
        }

        public event RoutedEventHandler Tick
        {
            add => AddHandler(TickEvent, value);
            remove => RemoveHandler(TickEvent, value);
        }

        public static readonly RoutedEvent TickEvent = EventManager.RegisterRoutedEvent(
            "Tick",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(XamlTimer));


        public TimeSpan Interval
        {
            get { return (TimeSpan)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            "Interval", 
            typeof(TimeSpan), 
            typeof(XamlTimer), 
            new PropertyMetadata(TimeSpan.FromSeconds(1), OnIntervalChanged));


        private void XamlTimer_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue as bool? == true)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is XamlTimer xamlTimer)
            {
                xamlTimer._timer.Interval = (TimeSpan)e.NewValue;
            }
        }
    }
}
