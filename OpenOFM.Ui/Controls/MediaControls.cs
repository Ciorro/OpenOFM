using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenOFM.Ui.Controls
{
    internal class MediaControls : Control
    {
        static MediaControls()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MediaControls), new FrameworkPropertyMetadata(typeof(MediaControls)));
        }

        public override void OnApplyTemplate()
        {
            var prevButton = GetTemplateChild("PART_PreviousButton");
            if (prevButton is Button pb)
            {
                pb.Click += (_, __) => RaiseEvent(new RoutedEventArgs(PreviousClickedEvent, this));
            }

            var nextButton = GetTemplateChild("PART_NextButton");
            if (nextButton is Button nb)
            {
                nb.Click += (_, __) => RaiseEvent(new RoutedEventArgs(NextClickedEvent, this));
            }

            base.OnApplyTemplate();
        }

        public event RoutedEventHandler PreviousClicked
        {
            add => AddHandler(PreviousClickedEvent, value);
            remove => RemoveHandler(PreviousClickedEvent, value);
        }

        public static readonly RoutedEvent PreviousClickedEvent = EventManager.RegisterRoutedEvent(
            "PreviousClicked",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(MediaControls));


        public event RoutedEventHandler NextClicked
        {
            add => AddHandler(NextClickedEvent, value);
            remove => RemoveHandler(NextClickedEvent, value);
        }

        public static readonly RoutedEvent NextClickedEvent = EventManager.RegisterRoutedEvent(
            "NextClicked",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(MediaControls));


        public bool IsPaused
        {
            get => (bool)GetValue(IsPausedProperty);
            set => SetValue(IsPausedProperty, value);
        }

        public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register(
            "IsPaused", 
            typeof(bool), 
            typeof(MediaControls), 
            new FrameworkPropertyMetadata(false, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }

        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register(
            "IsMuted", 
            typeof(bool), 
            typeof(MediaControls), 
            new FrameworkPropertyMetadata(false, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public float Volume
        {
            get => (float)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            "Volume", 
            typeof(float), 
            typeof(MediaControls), 
            new FrameworkPropertyMetadata(1f, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public TimeSpan Delay
        {
            get => (TimeSpan)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay", 
            typeof(TimeSpan), 
            typeof(MediaControls), 
            new FrameworkPropertyMetadata(TimeSpan.Zero, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
