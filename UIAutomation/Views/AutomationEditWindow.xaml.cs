using Prism.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using UIAutomation.Common;
using UIAutomation.Models;
using HandyMessageBox = HandyControl.Controls.MessageBox;

namespace UIAutomation.Views
{
    /// <summary>
    /// Interaction logic for AutomationEditWindow.xaml
    /// </summary>
    public partial class AutomationEditWindow : Window, INotifyPropertyChanged
    {
        public AutomationEditWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region 属性 

        /// <summary>
        /// 配置实体
        /// </summary>
        private ConfigModel _ConfigModel;
        public ConfigModel ConfigModel
        {
            get
            {
                return _ConfigModel;
            }
            set
            {
                _ConfigModel = value;
                RaisePropertyChanged("ConfigModel");
            }
        }

        #endregion

        #region 命令 

        /// <summary>
        /// 保存数据
        /// </summary>
        private DelegateCommand _SaveFileCommand;
        public DelegateCommand SaveFileCommand =>
            _SaveFileCommand ?? (_SaveFileCommand = new DelegateCommand(ExecuteSaveFileCommand));

        void ExecuteSaveFileCommand()
        {
            TextRange t = new TextRange(TxtContent.Document.ContentStart,
                                    TxtContent.Document.ContentEnd);

            bool result = Tool.Tools.XmlHelper.IsValidate(t.Text, out string error);
            if (result == false)
            {
                HandyMessageBox.Show($"XML文件错误:{error}", Properties.Resources.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ConfigModel.AutomationRoadData = t.Text;
            SystemConfig.SaveConfig(ConfigModel);
            this.DialogResult = true;
            this.Close();
        }
        /// <summary>
        /// 取消
        /// </summary>
        private DelegateCommand _CloseCommand;

        public DelegateCommand CloseCommand =>
            _CloseCommand ?? (_CloseCommand = new DelegateCommand(ExecuteCloseCommandCommand));

        void ExecuteCloseCommandCommand()
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == null)
            {
                MessageBoxResult messageBoxResult = HandyMessageBox.Show("您确定要关闭吗?", Properties.Resources.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void win_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigModel = SystemConfig.GetConfig();
            LoadXMLText();
        }

        private void LoadXMLText()
        {
            string doc = ConfigModel.AutomationRoadData.ToString().Replace("<", "|<").Replace("\n", "");
            string[] lines = doc.Split('|');
            TxtContent.Document.Blocks.Clear();
            foreach (string line in lines)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.LineHeight = 8;
                string[] arr = line.Split(' ');
                foreach (string value in arr)
                {
                    List<Run> list = XmlFormart(value);
                    foreach (Run run in list)
                    {
                        paragraph.Inlines.Add(run);
                    }
                }
                TxtContent.Document.Blocks.Add(paragraph);
            }
        }

        public List<Run> XmlFormart(string value)
        {
            List<Run> runs = new List<Run>();
            string[] items = value.Split(' ');
            foreach (string item in items)
            {
                if (item.Contains('<'))
                {
                    string str1 = item.Substring(0, item.IndexOf('<') + 1);
                    Run run = new Run();
                    run.Foreground = System.Windows.Media.Brushes.Gray;
                    run.Text = str1;
                    runs.Add(run);

                    string str2 = item.Substring(item.IndexOf('<') + 1, item.Length - item.IndexOf('<') - 1);
                    Run run2 = new Run();
                    run2.Foreground = System.Windows.Media.Brushes.Blue;
                    run2.Text = str2;
                    runs.Add(run2);
                }

                if (item.Contains("/>"))
                {
                    string str3 = item.Substring(item.IndexOf("/>"), item.Length - item.IndexOf("/>"));
                    Run run3 = new Run();
                    run3.Foreground = System.Windows.Media.Brushes.Gray;
                    run3.Text = str3;
                    runs.Add(run3);
                }

                if (item.Contains("?>"))
                {
                    string str3 = item.Substring(item.IndexOf("?>"), item.Length - item.IndexOf("?>"));
                    Run run3 = new Run();
                    run3.Foreground = System.Windows.Media.Brushes.Gray;
                    run3.Text = str3;
                    runs.Add(run3);
                }

                if (item.Contains("="))
                {
                    string[] items3 = item.Split('=');
                    Run run4 = new Run();
                    run4.Foreground = System.Windows.Media.Brushes.BlueViolet;
                    run4.Text = items3[0];
                    runs.Add(run4);
                    Run run5 = new Run();
                    run5.Foreground = System.Windows.Media.Brushes.Gray;
                    run5.Text = "=";
                    runs.Add(run5);
                    Run run6 = new Run();
                    run6.Foreground = System.Windows.Media.Brushes.Black;
                    run6.Text = items3[1];
                    runs.Add(run6);
                }
                Run run7 = new Run();
                run7.Text = " ";
                runs.Add(run7);
            }
            return runs;
        }
    }
}
