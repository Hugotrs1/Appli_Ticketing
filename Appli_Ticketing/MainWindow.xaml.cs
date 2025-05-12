using System.Windows;

namespace Appli_Ticketing
{
    public partial class MainWindow : Window
    {
        public System.Windows.Controls.Frame MainFrame { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            MainFrame = new System.Windows.Controls.Frame();
            Content = MainFrame; // Set the frame as the main content of the window
        }
    }
}
