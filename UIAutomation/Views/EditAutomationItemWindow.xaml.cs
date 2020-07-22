using System.Text.RegularExpressions;
using System.Windows;

namespace UIAutomation.Views
{
    /// <summary>
    /// Interaction logic for EditAutomationItemWindow.xaml
    /// </summary>
    public partial class EditAutomationItemWindow : Window
    {
        public EditAutomationItemWindow()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
