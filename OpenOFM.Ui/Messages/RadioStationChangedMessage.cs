using CommunityToolkit.Mvvm.Messaging.Messages;
using OpenOFM.Core.Models;

namespace OpenOFM.Ui.Messages
{
    internal class RadioStationChangedMessage : ValueChangedMessage<RadioStation>
    {
        public RadioStationChangedMessage(RadioStation value)
            : base(value)
        { }
    }
}
