using System.Windows;

namespace Appli_Ticketing.Views
{
    public partial class ReponseAdmin : Window
    {
        public string ResponseText { get; private set; }

        public ReponseAdmin()
        {
            InitializeComponent();
        }

        private void OnSendClick(object sender, RoutedEventArgs e)
        {
            ResponseText = ResponseTextBox.Text;
            DialogResult = true;
            Close();
        }
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
