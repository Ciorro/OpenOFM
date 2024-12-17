using Microsoft.Xaml.Behaviors;
using System.Windows.Input;

namespace OpenOFM.Ui.Navigation.Events
{
    internal class NextButtonEventTrigger : EventTrigger
    {
        protected override void OnEvent(EventArgs eventArgs)
        {
            if (eventArgs is not MouseButtonEventArgs mouseEventArgs)
                return;

            if (mouseEventArgs.ChangedButton == MouseButton.XButton2)
                InvokeActions(eventArgs);
        }
    }
}
