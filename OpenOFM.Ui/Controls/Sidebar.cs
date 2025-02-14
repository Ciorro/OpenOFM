using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OpenOFM.Ui.Controls
{
    public class Sidebar : ItemsControl
    {
        static Sidebar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Sidebar), new FrameworkPropertyMetadata(typeof(Sidebar)));
        }

        public override void OnApplyTemplate()
        {
            AddHandler(SidebarItem.NavigatedEvent, new NavigatedRoutedEventHandler((sender, args) =>
            {
                CurrentPageKey = args.PageKey;
            }));

            Width = ExpandedWidth;
            base.OnApplyTemplate();
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded",
            typeof(bool),
            typeof(Sidebar),
            new PropertyMetadata(true, OnExpandedStateChanged));


        public double ExpandedWidth
        {
            get => (double)GetValue(ExpandedWidthProperty);
            set => SetValue(ExpandedWidthProperty, value);
        }

        public static readonly DependencyProperty ExpandedWidthProperty = DependencyProperty.Register(
            "ExpandedWidth",
            typeof(double),
            typeof(Sidebar),
            new PropertyMetadata(260.0));


        public ObservableCollection<FrameworkElement> Footer
        {
            get => (ObservableCollection<FrameworkElement>)GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            "Footer",
            typeof(ObservableCollection<FrameworkElement>),
            typeof(Sidebar),
            new PropertyMetadata(new ObservableCollection<FrameworkElement>()));


        public object CurrentPageKey
        {
            get => (object)GetValue(CurrentPageKeyProperty);
            set => SetValue(CurrentPageKeyProperty, value);
        }

        public static readonly DependencyProperty CurrentPageKeyProperty = DependencyProperty.Register(
            "CurrentPageKey",
            typeof(object),
            typeof(Sidebar),
            new PropertyMetadata(OnCurrentPageKeyChanged));


        private static void OnCurrentPageKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Sidebar sidebar)
            {
                foreach (SidebarItem item in sidebar.Items)
                {
                    if (item.PageKey.Equals(e.NewValue))
                    {
                        item.IsChecked = true;
                    }
                }

                foreach (SidebarItem item in sidebar.Footer)
                {
                    if (item.PageKey.Equals(e.NewValue))
                    {
                        item.IsChecked = true;
                    }
                }
            }
        }

        private static void OnExpandedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Sidebar sidebar)
            {
                var animation = new DoubleAnimation();
                animation.From = sidebar.ActualWidth;
                animation.To = sidebar.IsExpanded ?
                    sidebar.ExpandedWidth : 40;
                animation.Duration = TimeSpan.FromSeconds(0.2);
                animation.EasingFunction = new CircleEase()
                {
                    EasingMode = EasingMode.EaseOut
                };

                sidebar.BeginAnimation(WidthProperty, animation);
            }
        }
    }
}
