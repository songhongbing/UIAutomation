using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UIAutomation.Properties;

namespace UIAutomation.ViewModels
{
    /// <summary>
    /// 托盘图标 上下文
    /// </summary>
    public class NotifyIconViewModel : BindableBase
    {
        public NotifyIconViewModel()
        {

        }

        #region 属性

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProductName
        {
            get { return $"{Resources.ProductName}（V{Resources.Version}）"; }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 显示窗口
        /// </summary>
        private DelegateCommand _ShowWindowCommand;
        public DelegateCommand ShowWindowCommand =>
            _ShowWindowCommand ?? (_ShowWindowCommand = new DelegateCommand(ExecuteShowWindowCommand));

        void ExecuteShowWindowCommand()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.Activate();
        }
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        private DelegateCommand _HideWindowCommand;
        public DelegateCommand HideWindowCommand =>
            _HideWindowCommand ?? (_HideWindowCommand = new DelegateCommand(ExecuteHideWindowCommand));

        void ExecuteHideWindowCommand()
        {
            Application.Current.MainWindow.ShowInTaskbar = false;
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        private DelegateCommand _ExitApplicationCommand;
        public DelegateCommand ExitApplicationCommand =>
            _ExitApplicationCommand ?? (_ExitApplicationCommand = new DelegateCommand(ExecuteExitApplicationCommand));

        void ExecuteExitApplicationCommand()
        {
            Application.Current.Shutdown();
        }
        #endregion
    }
}
