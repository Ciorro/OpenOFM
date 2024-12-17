using OpenOFM.Ui.Controls.Events;
using System.Windows;

namespace OpenOFM.Ui.Windows
{
    public partial class ApplicationWindow : Window
    {
        public ApplicationWindow()
        {
            InitializeComponent();
            WindowHelper.ExtendGlassFrame(this);
        }
    }
}
