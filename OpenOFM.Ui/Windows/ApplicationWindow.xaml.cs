using System.Windows;

namespace OpenOFM.Ui.Windows
{
    public partial class ApplicationWindow : Window
    {
        public ApplicationWindow()
        {
            WindowHelper.ExtendGlassFrame(this);
            InitializeComponent();
        }
    }
}
