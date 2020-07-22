using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Tool.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Sign_Loaded); 
        }
          

        private void Sign_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Application.Current.MainWindow.ShowInTaskbar = false;
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}
