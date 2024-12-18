using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls
{
    class MediaControls : Control
    {
        static MediaControls()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
               typeof(MediaControls), new FrameworkPropertyMetadata(typeof(MediaControls)));
        }

        public event RoutedEventHandler Back
        {
            add => AddHandler(BackEvent, value);
            remove => RemoveHandler(BackEvent, value);
        }

        public static readonly RoutedEvent BackEvent = EventManager.RegisterRoutedEvent(
            "Back",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(MediaControls));


        public event RoutedEventHandler Forward
        {
            add => AddHandler(ForwardEvent, value);
            remove => RemoveHandler(ForwardEvent, value);
        }

        public static readonly RoutedEvent ForwardEvent = EventManager.RegisterRoutedEvent(
            "Forward",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(MediaControls));

        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(
            "IsPlaying", 
            typeof(bool), 
            typeof(MediaControls));


        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }

        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register(
            "IsMuted", 
            typeof(bool), 
            typeof(MediaControls), 
            new PropertyMetadata(false));


        public float Volume
        {
            get => (float)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            "Volume",
            typeof(float), 
            typeof(MediaControls), 
            new PropertyMetadata(1f));


        public TimeSpan Delay
        {
            get => (TimeSpan)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay", 
            typeof(TimeSpan), 
            typeof(MediaControls), 
            new PropertyMetadata(TimeSpan.Zero));
    }
}
