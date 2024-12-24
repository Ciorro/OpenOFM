using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls
{
    internal class MediaControls : Control
    {
        static MediaControls()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MediaControls), new FrameworkPropertyMetadata(typeof(MediaControls)));
        }

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
