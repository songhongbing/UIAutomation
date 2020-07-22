using HandyControl.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Prism.Ioc;
using Prism.Events;
using UIAutomation.Events;

namespace UIAutomation.ViewModels
{
    public class LittleHelperWindowViewModel : BindableBase
    {
        private IContainerExtension IContainer;
        private IEventAggregator IEvents;
        public LittleHelperWindowViewModel(IContainerExtension container)
        {
            IContainer = container;
            IEvents = IContainer.Resolve<IEventAggregator>();
            IEvents.GetEvent<ProgressbarEvent>().Subscribe(ChangeCircleProgressBarReceived);
            IEvents.GetEvent<TaskStateChangeEvent>().Subscribe(TaskStateChangeReceived);
        }

        public LittleHelperWindowViewModel() { }

        #region 属性

        /// <summary>
        /// 是否开启工作
        /// </summary>
        private bool isStartWorker=false;
        public bool IsStartWorker
        {
            get { return isStartWorker; }
            set { SetProperty(ref isStartWorker, value); }
        }

        /// <summary>
        /// 模式窗体位置
        /// </summary>
        public int WindowLeft
        {
            get { 
                return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width-200; 
            } 
        }

        /// <summary>
        /// 进度条
        /// </summary>
        private int _CircleProgressBarValue;
        public int CircleProgressBarValue
        {
            get { return _CircleProgressBarValue; }
            set { SetProperty(ref _CircleProgressBarValue, value); }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 双击显示窗口
        /// </summary>
        private DelegateCommand _MouseDoubleCommand;
        public DelegateCommand MouseDoubleCommand =>
            _MouseDoubleCommand ?? (_MouseDoubleCommand = new DelegateCommand(ExecuteMouseDoubleCommand));

        void ExecuteMouseDoubleCommand()
        {
            ShowMainWin();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        private DelegateCommand _PauseCommand;
        public DelegateCommand PauseCommand =>
            _PauseCommand ?? (_PauseCommand = new DelegateCommand(ExecutePauseCommand));

        void ExecutePauseCommand()
        {
            IEvents.GetEvent<SuspensionTaskEvent>().Publish();
            //显示主窗口
            ShowMainWin();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 接收进度值
        /// </summary>
        private void ChangeCircleProgressBarReceived(int value)
        {
            Application.Current.Dispatcher.Invoke(()=> {
                CircleProgressBarValue = value;
            }); 
        }

        /// <summary>
        /// 任务状态改变
        /// </summary>
        /// <param name="state"></param>
        private void TaskStateChangeReceived(bool state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsStartWorker = state;
            });
        }

        /// <summary>
        /// 显示主窗口
        /// </summary>
        private void ShowMainWin()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.Activate();
        }
        #endregion
    }
}
