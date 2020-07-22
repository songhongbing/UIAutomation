using Prism.Ioc;
using System.Windows;

namespace UIAutomation.Views
{
    /// <summary>
    /// Interaction logic for LittleHelperWindow.xaml
    /// </summary>
    public partial class LittleHelperWindow : Window
    {
        public LittleHelperWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
