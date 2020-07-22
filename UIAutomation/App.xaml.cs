using Hardcodet.Wpf.TaskbarNotification;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tool.Views;
using UIAutomation.Tools;
using UIAutomation.Views;

namespace Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           
        }

        /// <summary>
        /// 定义托盘图标
        /// </summary>
        private TaskbarIcon _taskbar;

        private NamedPipeHelper namedPipeHelper;
        protected override void OnStartup(StartupEventArgs e)
        {  
            //判断互斥量，限制只能启动一个程序
            var m = new Mutex(true, "UIAutomation", out bool createdNew);
            if (!createdNew)
            {
                //启动客户端管道服务，向另外一个进程发送信号，激活显示已经存在的进程
                namedPipeHelper = new NamedPipeHelper();
                namedPipeHelper.StartClient();
                namedPipeHelper.SendMsg("启动");
                m.ReleaseMutex();
                Application.Current.Shutdown();
                return;
            }
            //实现激活另外一个进程 有两个方案
            //1:使用窗口句柄(方便，简单，但是不灵活)  
            //2:使用命名管道(相对负责，但是很灵活) 当前选择的方式
            //启动管道服务，用来接收重复启动进程的消息
            namedPipeHelper = new NamedPipeHelper();
            namedPipeHelper.StartService();
            namedPipeHelper.ReceivedMsgEvent += NamedPipeReceived;
            _taskbar = (TaskbarIcon)FindResource("Taskbar");
            base.OnStartup(e);
        }

        /// <summary>
        /// 接收管道消息，用来激活当前窗口
        /// </summary>
        /// <param name="msg"></param>
        public void NamedPipeReceived(string msg)
        {
            if (msg == "启动")
            {
                Application.Current.Dispatcher.Invoke(() => {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    Application.Current.MainWindow.ShowInTaskbar = true;
                    Application.Current.MainWindow.Activate();
                });
            }
        }
    }
}
