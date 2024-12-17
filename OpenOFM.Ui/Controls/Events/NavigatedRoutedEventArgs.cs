using System.Windows;

namespace OpenOFM.Ui.Controls.Events
{
    class NavigatedRoutedEventArgs : RoutedEventArgs
    {
        public required object PageKey { get; set; }

        public NavigatedRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public NavigatedRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}
