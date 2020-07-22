
using HandyControl.Data;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Tool.Tools;
using Tool.Views;
using Prism.Ioc;
using UIAutomation.Common;
using UIAutomation.Models;
using UIAutomation.Views;
using Prism.Events;
using UIAutomation.Events;
using HandyMessageBox = HandyControl.Controls.MessageBox;
using System.Threading.Tasks;
using System.IO;
using UIAutomation.Tools;

namespace UIAutomation.ViewModels
{
    public class MainAutomationControlViewModel : BindableBase
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _Logger = LogManager.GetLogger(nameof(MainAutomationControlViewModel));

        private readonly IContainerExtension IContainer;
        private readonly IEventAggregator IEvents;

        public MainAutomationControlViewModel(IContainerExtension container)
        {
            IContainer = container;
            IEvents = IContainer.Resolve<IEventAggregator>();
            IEvents.GetEvent<SuspensionTaskEvent>().Subscribe(SuspensionTaskRecevied);
            IEvents.GetEvent<ChangeRowOrderEvent>().Subscribe(ChangeRowGridReceived);
        }

        #region 属性 
        /// <summary>
        /// 进程有效 时间控件
        /// </summary>
        private Timer checkProcessTimer;

        /// <summary>
        /// 初始没有锁
        /// </summary>
        static ManualResetEventSlim _mainEvent = new ManualResetEventSlim(true);

        /// <summary>
        /// 系统配置文件
        /// </summary>
        private ConfigModel _ConfigModel = new ConfigModel();
        public ConfigModel ConfigModel
        {
            get { return _ConfigModel; }
            set
            {
                SetProperty(ref _ConfigModel, value);
                OnPropertyChanged(nameof(FileName));
            }
        }

        /// <summary>
        /// 目标文件名称
        /// </summary>
        public string FileName
        {
            get
            {
                //设置测试目标进程名称
                SystemConfig.targetProcessName = Path.GetFileNameWithoutExtension(ConfigModel.TargetSoftwarePath);

                if (String.IsNullOrEmpty(ConfigModel.TargetSoftwarePath))
                    return "无目标";

                try
                {
                    return Path.GetFileName(ConfigModel.TargetSoftwarePath);
                }
                catch (Exception ex)
                {
                    return "无目标";
                }
            }
        }

        /// <summary>
        /// 自动化路线
        /// </summary>
        private ObservableCollection<RoadsModel> _RoadsList = new ObservableCollection<RoadsModel>();
        public ObservableCollection<RoadsModel> RoadsList
        {
            get { return _RoadsList; }
            set { SetProperty(ref _RoadsList, value); }
        }

        /// <summary>
        /// Vibox状态
        /// </summary>
        private bool _ViboxState = false;
        public bool ViboxState
        {
            get { return _ViboxState; }
            set { SetProperty(ref _ViboxState, value); }
        }


        /// <summary>
        /// 是否循环执行
        /// </summary>
        private bool _IsLoopState;
        public bool IsLoopState
        {
            get { return _IsLoopState; }
            set
            {
                SetProperty(ref _IsLoopState, value);
                ConfigModel.IsLoopState = value;
                SystemConfig.SaveConfig(ConfigModel);
            }
        }

        /// <summary>
        /// UI自动化详细路线
        /// </summary>
        private ObservableCollection<AutomationModel> _AutomationList = new ObservableCollection<AutomationModel>();
        public ObservableCollection<AutomationModel> AutomationList
        {
            get { return _AutomationList; }
            set { SetProperty(ref _AutomationList, value); }
        }

        /// <summary>
        /// 选中路线
        /// </summary>
        private RoadsModel _CurrentRoads;
        public RoadsModel CurrentRoads
        {
            get { return _CurrentRoads; }
            set
            {
                SetProperty(ref _CurrentRoads, value);
                AutomationList = _CurrentRoads?.AutomationList;
                if (_CurrentRoads?.RoadName != null)
                {
                    CurrentRoadName = _CurrentRoads?.RoadName;
                }
            }
        }

        /// <summary>
        /// 当前选中的路线名称
        /// </summary>
        private string _CurrentRoadName;
        public string CurrentRoadName
        {
            get { return _CurrentRoadName; }
            set { SetProperty(ref _CurrentRoadName, value); }
        }

        /// <summary>
        /// 动画状态
        /// </summary>
        private bool _AutomationState = false;
        public bool AutomationState
        {
            get { return _AutomationState; }
            set
            {
                if (value == false)
                {
                    IsWorkingState = true;
                }
                SetProperty(ref _AutomationState, value);
            }
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        private bool _IsWorkingState = true;
        public bool IsWorkingState
        {
            get { return _IsWorkingState; }
            set
            {
                if (value == true)
                {
                    _mainEvent.Reset();
                    IEvents.GetEvent<TaskStateChangeEvent>().Publish(false);
                }
                else
                {
                    _mainEvent.Set();
                    IEvents.GetEvent<TaskStateChangeEvent>().Publish(true);
                }

                SetProperty(ref _IsWorkingState, value);
            }
        }

        /// <summary>
        /// 滚动条
        /// </summary>
        public ScrollViewer svProperty { get; set; }

        public MainAutomationControl mainAutomationControl { get; set; }
        #endregion


        #region 命令

        /// <summary> 
        /// 数据绑定命令
        /// </summary>
        private DelegateCommand<MainAutomationControl> _LoadedBindDataCommand;
        public DelegateCommand<MainAutomationControl> LoadedBindDataCommand =>
            _LoadedBindDataCommand ?? (_LoadedBindDataCommand = new DelegateCommand<MainAutomationControl>(ExecuteLoadedBindDataCommand));
        void ExecuteLoadedBindDataCommand(MainAutomationControl mac)
        {
            mainAutomationControl = mac;
            //获取系统设置
            ConfigModel = SystemConfig.GetConfig();
            //加载数据
            LoadRoadList();
            // 检测进程任务
            CheckProcessTask();
            //滚动
            svProperty = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(mainAutomationControl.DataGrid1, 0), 0) as ScrollViewer;

            //设置委托
            setCurrentTestFileInfoExec += SetCurrentTestFile;
        }

        #region 菜单 

        /// <summary>
        /// 添加
        /// </summary>
        private DelegateCommand<AutomationModel> _AddAutomationCommand;
        public DelegateCommand<AutomationModel> AddAutomationCommand =>
            _AddAutomationCommand ?? (_AddAutomationCommand = new DelegateCommand<AutomationModel>(ExecuteAddAutomationCommand));

        void ExecuteAddAutomationCommand(AutomationModel automationModel)
        {
            EditAutomationItemWindow editAutomationItemWindow = IContainer.Resolve<EditAutomationItemWindow>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["Type"] = WindowType.Add;
            dic["RodeID"] = CurrentRoads.ID;
            dic["OldId"] = automationModel.ID;
            dic["AutomationID"] = DateTime.Now.ToString("yyyyMMddHHmmss");
            IEvents.GetEvent<EditAutomationItemEvent>().Publish(dic);
            bool? result = editAutomationItemWindow.ShowDialog();
            if (result == true)
            {
                ConfigModel.AutomationRoadData = XmlHelper.GetDoc(SystemConfig.xml_automation).InnerXml;
                //保存配置数据
                SystemConfig.SaveConfig(ConfigModel);
                LoadRoadList();
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        private DelegateCommand<AutomationModel> _EditAutomationCommand;
        public DelegateCommand<AutomationModel> EditAutomationCommand =>
            _EditAutomationCommand ?? (_EditAutomationCommand = new DelegateCommand<AutomationModel>(ExecuteEditAutomationCommand));

        void ExecuteEditAutomationCommand(AutomationModel automationModel)
        {
            EditAutomation(automationModel);
        }

        /// <summary>
        /// 双击编辑事件
        /// </summary>
        private DelegateCommand<AutomationModel> _MouseDoubleGridCommand;
        public DelegateCommand<AutomationModel> MouseDoubleGridCommand =>
            _MouseDoubleGridCommand ?? (_MouseDoubleGridCommand = new DelegateCommand<AutomationModel>(ExecuteMouseDoubleGridCommand));

        void ExecuteMouseDoubleGridCommand(AutomationModel gridItemData)
        {
            EditAutomation(gridItemData);
        }

        /// <summary>
        /// 删除
        /// </summary>
        private DelegateCommand<AutomationModel> _MinusAutomationCommand;
        public DelegateCommand<AutomationModel> MinusAutomationCommand =>
            _MinusAutomationCommand ?? (_MinusAutomationCommand = new DelegateCommand<AutomationModel>(ExecuteMinusAutomationCommand));

        void ExecuteMinusAutomationCommand(AutomationModel automationModel)
        {
            MessageBoxResult messageBoxResult = HandyMessageBox.Show("确定要删除吗?",Properties.Resources.ProductName,MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (messageBoxResult==MessageBoxResult.Yes)
            { 
                XmlHelper.DeleteAttribute(SystemConfig.xml_automation, "UIAutomation/Road", CurrentRoads.ID, automationModel.ID);
                ConfigModel.AutomationRoadData = XmlHelper.GetDoc(SystemConfig.xml_automation).InnerXml;
                //保存配置数据
                SystemConfig.SaveConfig(ConfigModel);
                LoadRoadList();
            }
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        private DelegateCommand<AutomationModel> _MoveUpCommand;
        public DelegateCommand<AutomationModel> MoveUpCommand =>
            _MoveUpCommand ?? (_MoveUpCommand = new DelegateCommand<AutomationModel>(ExecuteMoveUpCommand));

        void ExecuteMoveUpCommand(AutomationModel automationModel)
        { 
            //如果是第一个则取消向上移动
            int num=AutomationList.IndexOf(automationModel);
            if (num==0)
            {
                return;
            }
            Dictionary<string, int> dics = new Dictionary<string, int>();
            dics["prevRowIndex"]=num-1;
            dics["index"]=num;
            ChangeRowGridReceived(dics);
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        private DelegateCommand<AutomationModel> _MoveDownCommand;
        public DelegateCommand<AutomationModel> MoveDownCommand =>
            _MoveDownCommand ?? (_MoveDownCommand = new DelegateCommand<AutomationModel>(ExecuteMoveDownCommand));

        void ExecuteMoveDownCommand(AutomationModel automationModel)
        { 
            //如果是最后一个则取消向下移动
            int num = AutomationList.IndexOf(automationModel);
            if(num == AutomationList.Count()-1)
            {
                return;
            }
            Dictionary<string, int> dics = new Dictionary<string, int>();
            dics["prevRowIndex"] = num+1;
            dics["index"] = num;
            ChangeRowGridReceived(dics);
        }
        #endregion

        #region 启动程序
        /// <summary>
        /// 启动程序
        /// </summary>
        private DelegateCommand _ExecProcessCommand;
        public DelegateCommand ExecProcessCommand =>
            _ExecProcessCommand ?? (_ExecProcessCommand = new DelegateCommand(ExecuteCommandName));

        void ExecuteCommandName()
        {
            if (File.Exists(ConfigModel.TargetSoftwarePath))
            {
                Process.Start(ConfigModel.TargetSoftwarePath);
            }
            else
            {
                HandyMessageBox.Show("目标文件不存在", Properties.Resources.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region UI自动化测试控制

        Thread automationThread;
        /// <summary>
        /// 执行测试口令
        /// </summary>
        private DelegateCommand _AutomationCommand;
        public DelegateCommand AutomationCommand =>
            _AutomationCommand ?? (_AutomationCommand = new DelegateCommand(ExecuteAutomationCommand));
        void ExecuteAutomationCommand()
        {
            try
            {
                if (AutomationState == true)
                {
                    ///执行
                    automationThread = new Thread(ExecuteAutomation);
                    automationThread.IsBackground = true;
                    automationThread.Start();
                    _Logger.Info("开始执行测试");
                    IsWorkingState = false;
                }
                else
                {
                    _mainEvent.Set();
                    ///结束
                    automationThread?.Interrupt();
                    _Logger.Info("结束执行测试");
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "线程操作异常");
            }
        }


        /// <summary>
        /// 重新启动UI测试流程
        /// </summary>
        private DelegateCommand _ReStartUITestCommand;
        public DelegateCommand ReStartUITestCommand =>
            _ReStartUITestCommand ?? (_ReStartUITestCommand = new DelegateCommand(ExecuteReStartUITestCommand));

        void ExecuteReStartUITestCommand()
        {
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ///停止任务
                    AutomationState = false;
                });
                Thread.Sleep(500);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AutomationState = true;
                    ExecuteAutomationCommand();
                });
            });

        }
        #endregion

        #region 编辑配置文件
        /// <summary>
        /// 刷新数据
        /// </summary>
        private DelegateCommand _RefreshConfigFileCommand;
        public DelegateCommand RefreshConfigFileCommand =>
            _RefreshConfigFileCommand ?? (_RefreshConfigFileCommand = new DelegateCommand(ExecuteRefreshConfigFileCommand));
        void ExecuteRefreshConfigFileCommand()
        {
            //加载数据
            LoadRoadList();
            //重置数据状态
            ResetAutomationState();
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        private DelegateCommand _OpenConfigFileCommand;
        public DelegateCommand OpenConfigFileCommand =>
            _OpenConfigFileCommand ?? (_OpenConfigFileCommand = new DelegateCommand(ExecuteOpenConfigFileCommand));
        void ExecuteOpenConfigFileCommand()
        {
            AutomationEditWindow automationEditWindow = IContainer.Resolve<AutomationEditWindow>();
            bool? result = automationEditWindow.ShowDialog();
            if (result == true)
            {
                ConfigModel = SystemConfig.GetConfig();
                LoadRoadList();
            }
        }
        #endregion

        #region 文件

        /// <summary>
        /// 浏览文件
        /// </summary>
        private DelegateCommand _BrowseFileCommand;
        public DelegateCommand BrowseFileCommand =>
             _BrowseFileCommand ?? (_BrowseFileCommand = new DelegateCommand(ExecuteBrowseFileCommand));

        void ExecuteBrowseFileCommand()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetCurrentTestFile(dialog.FileName);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        public delegate void SetCurrentTestFileInfo(string path);

        public static SetCurrentTestFileInfo setCurrentTestFileInfoExec;

        /// <summary>
        /// 选中需要测试软件  注意:本方法类型为 static ，主要是因为这个方法需要在 style里面调用，如果是一般的类型，在style 则调用不到  
        /// </summary>
        private static DelegateCommand<string> _SelectFileCommand;
        public static DelegateCommand<string> SelectFileCommand =>
            _SelectFileCommand ?? (_SelectFileCommand = new DelegateCommand<string>(ExecuteSelectFileCommand));

        static void ExecuteSelectFileCommand(string path)
        {
            setCurrentTestFileInfoExec(path);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        private DelegateCommand _ExportDataCommand;
        public DelegateCommand ExportDataCommand =>
            _ExportDataCommand ?? (_ExportDataCommand = new DelegateCommand(ExecuteExportDataCommand));

        void ExecuteExportDataCommand()
        {

            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Title = "导出数据";
            dlg.Filter = "xml(*.xml)|*.xml";
            dlg.FileName = "data";
            dlg.RestoreDirectory = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string path = dlg.FileName;
                    FileStream fileStream = File.Create(path);
                    fileStream.Close();
                    FileHelper.WriteFile(path, ConfigModel.AutomationRoadData);
                }
                catch (Exception ex)
                { }
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        private DelegateCommand _ImportDataCommand;
        public DelegateCommand ImportDataCommand =>
            _ImportDataCommand ?? (_ImportDataCommand = new DelegateCommand(ExecuteImportDataCommand));

        void ExecuteImportDataCommand()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = "导入数据";
            dlg.Filter = "xml(*.xml)|*.xml";
            dlg.RestoreDirectory = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string path = dlg.FileName;
                    ConfigModel.AutomationRoadData = File.ReadAllText(path);
                    SystemConfig.SaveConfig(ConfigModel);
                    LoadRoadList();
                    HandyMessageBox.Show("导入成功", Properties.Resources.ProductName);
                }
                catch (Exception ex)
                { }
            }
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        private DelegateCommand _ExitSystemCommand;
        public DelegateCommand ExitSystemCommand =>
            _ExitSystemCommand ?? (_ExitSystemCommand = new DelegateCommand(ExecuteExitSystemCommand));
        void ExecuteExitSystemCommand()
        {
            MessageBoxResult messageBoxResult = HandyMessageBox.Show("确定要退出系统吗?", Properties.Resources.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion
         
        #endregion

        #region 方法

        /// <summary>
        /// 编辑动画
        /// </summary>
        /// <param name="gridItemData"></param>
        private void EditAutomation(AutomationModel gridItemData)
        {
            EditAutomationItemWindow editAutomationItemWindow = IContainer.Resolve<EditAutomationItemWindow>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["Type"] = WindowType.Edit;
            dic["RodeID"] = CurrentRoads.ID;
            dic["AutomationModel"] = gridItemData;
            IEvents.GetEvent<EditAutomationItemEvent>().Publish(dic);
            bool? result = editAutomationItemWindow.ShowDialog();
            if (result == true)
            {
                ConfigModel.AutomationRoadData = XmlHelper.GetDoc(SystemConfig.xml_automation).InnerXml;
                //保存配置数据
                SystemConfig.SaveConfig(ConfigModel);
                LoadRoadList();
            }
        }

        private volatile AutomationModel oldAutomationModel = null;
        /// <summary>
        /// 行改变
        /// </summary>
        /// <param name="dics"></param>
        private void ChangeRowGridReceived(Dictionary<string,int> dics)
        {
            int prevRowIndex = dics["prevRowIndex"];
            int index = dics["index"];
            AutomationModel movedEmps = AutomationList[prevRowIndex];
            AutomationList.RemoveAt(prevRowIndex); 
            AutomationList.Insert(index, movedEmps); 
            XmlHelper.ChangeRowItemsIndex(SystemConfig.xml_automation, "UIAutomation/Road",CurrentRoads.ID, index, prevRowIndex);
            ConfigModel.AutomationRoadData = XmlHelper.GetDoc(SystemConfig.xml_automation).InnerXml;
            //保存配置数据
            SystemConfig.SaveConfig(ConfigModel);
            LoadRoadList();
        }

        /// <summary>
        /// 选择目标测试的文件
        /// </summary>
        /// <param name="fileName"></param>
        private void SetCurrentTestFile(string fileName)
        {
            ConfigModel.TargetSoftwarePath = fileName;
            OnPropertyChanged(nameof(FileName));
            //记录历史记录
            ConfigModel.AddHistoryFile(ConfigModel.TargetSoftwarePath);
            SystemConfig.SaveConfig(ConfigModel);
        }

        /// <summary>
        /// 重置动画状态
        /// </summary>
        private void ResetAutomationState()
        {
            AutomationState = false;
            IsWorkingState = false;
        }

        /// <summary>
        /// 执行动画
        /// </summary>
        public void ExecuteAutomation()
        {
            try
            {
                bool IsContinue = false;
                for (int i = 0; i < AutomationList.Count(); i++)
                {
                    AutomationModel automation = AutomationList[i];

                    //清空行选中状态
                    ResetAutomationRowState();
                    //选中行选中状态
                    SelectAutomationRowState(automation);
                    //添加动态判断数据逻辑
                    if (automation.Operation.ToLower() == "if")
                    {

                        //发布进度更新
                        ChanageProgressValue(AutomationList.IndexOf(automation));
                        int count = (int)AutomationFactory.ExecuteAutomationTask(automation);

                        if (count > Convert.ToInt32(automation.SetValue))
                            IsContinue = true;
                    }
                    //判断语句结束则恢复执行任务
                    if (automation.Operation.ToLower() == "endif")
                    {
                        IsContinue = false;
                        continue;
                    }

                    if (IsContinue)
                        continue;

                    //发布进度更新
                    ChanageProgressValue(AutomationList.IndexOf(automation));
                    //执行任务
                    ExecAutomationTask(automation);

                    //任务等待
                    Thread.Sleep(automation.SleepTime);
                    _mainEvent.Wait();

                    //如果动画结束那就退出任务
                    if (AutomationState == false)
                        return;
                }
                //如果动画结束那就退出任务
                if (AutomationState == true && IsLoopState == true)
                {
                    ExecuteAutomation();    //递归循环执行
                }
                AutomationState = false;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "任务步骤执行失败");
            }
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        public void SuspensionTaskRecevied()
        {
            IsWorkingState = true;
        }

        /// <summary>
        /// 更新进度值
        /// </summary>
        /// <param name="num"></param>
        public void ChanageProgressValue(int num)
        {
            //滚动条
            ScrollToVerticalOffset(num);

            double percent = (num + 1) % AutomationList.Count();
            //发布进度
            IEvents.GetEvent<ProgressbarEvent>().Publish((int)percent);
        }

        /// <summary>
        /// 执行动画任务
        /// </summary>
        /// <param name="automation">任务载体</param>
        /// <param name="IsSecond">设置为true 如果失败了，默认1秒后执行一次递归</param>
        public void ExecAutomationTask(AutomationModel automation, bool IsSecond = false)
        {
            try
            {
                //执行具体任务的方法
                int result = (int)AutomationFactory.ExecuteAutomationTask(automation);
                if (result == -1)
                {
                    //暂停任务 把按钮标记为继续状态
                    IsWorkingState = true;
                }
            }
            catch (Exception ex)
            {
                if (IsSecond = true)
                {
                    //暂停任务 把按钮标记为继续状态
                    IsWorkingState = true;
                    MessageBox.Show(ex.Message);
                }
                else
                {
                    //如果失败了，默认1秒后执行一次递归
                    Thread.Sleep(1000);
                    ExecAutomationTask(automation, true);
                } 
            }
        }

        /// <summary>
        /// 重置动画行的状态
        /// </summary>
        public void ResetAutomationRowState()
        {
            if (oldAutomationModel != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    oldAutomationModel.IsSelected = 0;
                });
            }
        }

        /// <summary>
        /// 滚动条滚动
        /// </summary>
        /// <param name="num">滚动条地址</param>
        private void ScrollToVerticalOffset(int num)
        {
            //根据当前执行的任务，调整滚动条
            Application.Current.Dispatcher.Invoke(() =>
            {
                svProperty.ScrollToVerticalOffset(num);
            });
        }

        /// <summary>
        /// 选中动画的状态
        /// </summary>
        /// <param name="automation"></param>
        public void SelectAutomationRowState(AutomationModel automation)
        {
            //暂时存储当前动画实体
            oldAutomationModel = automation;
            //设置选中
            Application.Current.Dispatcher.Invoke(() =>
            {
                automation.IsSelected = 1;
            });
        }

        /// <summary>
        /// 加载数据方法
        /// </summary>
        private void LoadRoadList()
        {
            //在刷新数据前，先清空数据
            RoadsList.Clear();
            //加载数据文件
            bool result = XmlHelper.SetDataXml(SystemConfig.xml_automation,ConfigModel.AutomationRoadData);
            if (result == true)
            {
                XmlElement xmlElement = XmlHelper.GetXmlDocumentRoot(SystemConfig.xml_automation);
                for (int num = 0; num < xmlElement.ChildNodes.Count; num++)
                {
                    XmlNode XmlNode1 = xmlElement.ChildNodes[num];
                    RoadsModel roadsModel = new RoadsModel();

                    if (XmlNode1.Attributes["ID"] != null)
                    {
                        roadsModel.ID = XmlNode1.Attributes["ID"].Value;
                    }
                    else
                    {
                        roadsModel.ID = num.ToString();
                    }

                    roadsModel.RoadName = XmlNode1.Attributes["Name"].Value;
                    for (int i = 0; i < XmlNode1.ChildNodes.Count; i++)
                    {
                        try
                        {
                            var XmlNodeTemp = XmlNode1.ChildNodes[i];
                            AutomationModel automationModel = new AutomationModel();
                            if (XmlNodeTemp.Attributes["ID"] != null)
                            {
                                automationModel.ID = XmlNodeTemp.Attributes["ID"].Value;
                            }
                            else
                            {
                                automationModel.ID =i.ToString();
                            }
                            automationModel.ControlName = XmlNodeTemp.Attributes["ControlName"].Value;
                            automationModel.ControlType = XmlNodeTemp.Attributes["ControlType"].Value;
                            automationModel.Operation = XmlNodeTemp.Attributes["Operation"].Value;
                            automationModel.Name = XmlNodeTemp.Attributes["Name"].Value;
                            if (XmlNodeTemp.Attributes["Handle"] != null)
                            {
                                int.TryParse(XmlNodeTemp.Attributes["Handle"].Value, out int handle);
                                automationModel.Handle = handle;
                            }
                            else
                            {
                                automationModel.Handle = 0;
                            }
                           
                            automationModel.SetValue = XmlNodeTemp.Attributes["SetValue"].Value;
                            Int32.TryParse(XmlNodeTemp.Attributes["SleepTime"].Value, out int sleeptime);
                            automationModel.SleepTime = sleeptime;
                            roadsModel.AutomationList.Add(automationModel);
                        }
                        catch (Exception ex) { }
                    }
                    RoadsList.Add(roadsModel);
                }
                //默认选中第一个
                if (CurrentRoads == null)
                {
                    CurrentRoads = RoadsList.FirstOrDefault(o => o.RoadName == CurrentRoadName);
                    if (CurrentRoads == null)
                    {
                        CurrentRoads = RoadsList.FirstOrDefault();
                    }
                }
            }
        }

        /// <summary>
        /// 刷新检测Vibox状态
        /// </summary>
        private void RefurbishViboxState(object state)
        {
            try
            {
                Process[] process = System.Diagnostics.Process.GetProcesses();
                Process currentProcess = process.FirstOrDefault(o => o.ProcessName.Contains(SystemConfig.targetProcessName));
                if (currentProcess != null)
                {
                    ViboxState = true;
                    //在自动化工厂加载进程
                    AutomationFactory.LoadProcess(currentProcess);
                }
                else
                {
                    ViboxState = false;
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "检测Vibox进程报错");
            }
            var nextTime = DateTime.Now.AddSeconds(2);
            checkProcessTimer.Change(nextTime.Subtract(DateTime.Now), Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// 检测进程任务
        /// </summary>
        public void CheckProcessTask()
        {
            if (checkProcessTimer == null)
            {
                checkProcessTimer = new Timer(RefurbishViboxState, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
            //立即执行一次
            checkProcessTimer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        #endregion
    }
}
