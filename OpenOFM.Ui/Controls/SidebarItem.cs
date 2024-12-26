using OpenOFM.Ui.Controls.Events;
using System.Windows;
using System.Windows.Controls;

namespace OpenOFM.Ui.Controls
{
    delegate void NavigatedRoutedEventHandler(object sender, NavigatedRoutedEventArgs args);

    class SidebarItem : RadioButton
    {
        static SidebarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SidebarItem), new FrameworkPropertyMetadata(typeof(SidebarItem)));
        }

        public event NavigatedRoutedEventHandler Navigated
        {
            add => AddHandler(NavigatedEvent, value);
            remove => RemoveHandler(NavigatedEvent, value);
        }

        public static readonly RoutedEvent NavigatedEvent = EventManager.RegisterRoutedEvent(
            "Navigated",
            RoutingStrategy.Bubble,
            typeof(NavigatedRoutedEventHandler),
            typeof(SidebarItem));

        public UIElement Icon
        {
            get => (UIElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(UIElement),
            typeof(SidebarItem));


        public object PageKey
        {
            get => (object)GetValue(PageKeyProperty);
            set => SetValue(PageKeyProperty, value);
        }

        public static readonly DependencyProperty PageKeyProperty = DependencyProperty.Register(
            "PageKey",
            typeof(object),
            typeof(SidebarItem));

        protected override void OnClick()
        {
            base.OnClick();

            RaiseEvent(new NavigatedRoutedEventArgs(NavigatedEvent, this)
            {
                PageKey = PageKey
            });
        }
    }
}
