using Prism.Commands;
using Prism.Mvvm;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using Tool.Tools;
using Prism.Events;
using Prism.Regions;
using Tool.Views; 
using System.Windows.Navigation;
using System.Windows.Controls;
using UIAutomation.Views;
using System.Diagnostics;
using UIAutomation.Models;

namespace Tool.ViewModels
{
    public class MainWindowViewModel : BindableBase 
    {
        /// <summary>
        /// 主窗口
        /// </summary>
        private MainWindow CurrentMainWindow;
        /// <summary>
        /// 导航服务
        /// </summary>
        private NavigationService navService;
        /// <summary>
        /// IOC容器
        /// </summary>
        private readonly IContainerExtension _Container;
        /// <summary>
        /// Region 管理
        /// </summary>
        private readonly IRegionManager _RegionManager;

        /// <summary>
        /// 事件聚合器
        /// </summary>
        private IEventAggregator _EventBus; 


        public MainWindowViewModel(IContainerExtension container)
        {
            _Container = container;
            _RegionManager = _Container.Resolve<IRegionManager>();
            _EventBus = _Container.Resolve<IEventAggregator>();
            _Container.RegisterForNavigation<MainAutomationControl>();
        }

      

        #region 命令
        /// <summary>
        /// 加载绑定事件
        /// </summary>
        private DelegateCommand<MainWindow> _loadBindCommand;
        public DelegateCommand<MainWindow> LoadBindCommand =>
            _loadBindCommand ?? (_loadBindCommand = new DelegateCommand<MainWindow>(ExecuteLoadBindCommand));

        void ExecuteLoadBindCommand(MainWindow mainWindow)
        {
            CurrentMainWindow = mainWindow;
            _RegionManager.RequestNavigate("ContentRegion", nameof(MainAutomationControl));

            var littleHelperWindow = _Container.Resolve<LittleHelperWindow>();
            littleHelperWindow.Show();
        } 

        /// <summary>
        /// 导航返回
        /// </summary>
        private DelegateCommand<Button> _backNavigationCommand;
        public DelegateCommand<Button> BackNavigationCommand =>
            _backNavigationCommand ?? (_backNavigationCommand = new DelegateCommand<Button>(ExecuteBackNavigationCommand));

        void ExecuteBackNavigationCommand(Button btn)
        {
            ToNavigation();
        }

        /// <summary>
        /// 返回Home页
        /// </summary>
        private DelegateCommand _homeNavigationCommand;
        public DelegateCommand HomeNavigationCommand =>
            _homeNavigationCommand ?? (_homeNavigationCommand = new DelegateCommand(ExecuteHomeNavigationCommand));

        void ExecuteHomeNavigationCommand()
        { 

        }
       

        #endregion

        #region 方法
        /// <summary>
        /// 返回导航
        /// </summary>
        /// <param name="type"></param>
        void ToNavigation(string type="")
        { 
        } 
        #endregion 
    }
}
