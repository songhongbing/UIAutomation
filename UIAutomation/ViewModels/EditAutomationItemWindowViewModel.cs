using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Resources;
using Tool.Tools;
using UIAutomation.Common;
using UIAutomation.Events;
using UIAutomation.Models;
using UIAutomation.Views;

namespace UIAutomation.ViewModels
{
    public class EditAutomationItemWindowViewModel : BindableBase
    {
        private readonly IContainerExtension IContainer;
        private readonly IEventAggregator IEvents;
        public EditAutomationItemWindowViewModel(IContainerExtension container)
        {
            IContainer = container;
            IEvents = IContainer.Resolve<IEventAggregator>();

            IEvents.GetEvent<EditAutomationItemEvent>().Subscribe(EditAutomationItemEventReceived);
        }

        #region 属性

        public EditAutomationItemWindow EditAutomationItemWindow { get; set; }

        /// <summary>
        /// 窗口类型
        /// </summary>
        private WindowType _WindowType;
        public WindowType WindowType
        {
            get { return _WindowType; }
            set
            {
                SetProperty(ref _WindowType, value);
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        /// <summary>
        /// 窗口标题
        /// </summary>
        public string WindowTitle
        {
            get
            {
                switch (WindowType)
                {
                    case WindowType.Add:
                        return "添加自动化步骤";
                    case WindowType.Edit:
                        return "编辑自动化步骤";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 当前选中的测试路线ID
        /// </summary>
        private string _CurrentRoleID;
        public string CurrentRoleID
        {
            get { return _CurrentRoleID; }
            set { SetProperty(ref _CurrentRoleID, value); }
        }

        /// <summary>
        /// 测试子项
        /// </summary>
        private AutomationModel _AutomationModel = new AutomationModel();
        public AutomationModel AutomationModel
        {
            get { return _AutomationModel; }
            set { SetProperty(ref _AutomationModel, value); }
        }

        /// <summary>
        /// 绑定控件类型列表
        /// </summary>
        private ObservableCollection<ComboBoxItemModel> typeList;
        public ObservableCollection<ComboBoxItemModel> ControlTypeList
        {
            get
            {

                if (typeList == null)
                {
                    List<ComboBoxItemModel> list = SystemConfig.GetControlList();
                    ControlTypeList = new ObservableCollection<ComboBoxItemModel>(list);
                }
                return typeList;
            }
            set
            {
                typeList = value;
                SetProperty(ref typeList, value);
            }
        }

        /// <summary>
        /// 事件列表
        /// </summary>
        private ObservableCollection<ComboBoxItemModel> _EventList = new ObservableCollection<ComboBoxItemModel>();
        public ObservableCollection<ComboBoxItemModel> EventList
        {
            get { return _EventList; }
            set { SetProperty(ref _EventList, value); }
        }

        /// <summary>
        /// 当前选中的类型
        /// </summary>
        private ComboBoxItemModel currentComboBoxItemModel;
        public ComboBoxItemModel CurrentComboBoxItemModel
        {
            get { return currentComboBoxItemModel; }
            set
            {
                SetProperty(ref currentComboBoxItemModel, value);
                if (currentComboBoxItemModel != null)
                {
                    AutomationModel.ControlType = currentComboBoxItemModel.Value;
                }
            }
        }
        /// <summary>
        /// 上一个ID
        /// </summary>
        private string _OldId;
        public string OldId
        {
            get { return _OldId; }
            set { SetProperty(ref _OldId, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 加载事件
        /// </summary>
        private DelegateCommand<EditAutomationItemWindow> _fieldName;
        public DelegateCommand<EditAutomationItemWindow> LoadedCommand =>
            _fieldName ?? (_fieldName = new DelegateCommand<EditAutomationItemWindow>(ExecuteLoadedCommand));

        void ExecuteLoadedCommand(EditAutomationItemWindow win)
        {
            EditAutomationItemWindow = win;
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        private DelegateCommand _SaveConfigCommand;
        public DelegateCommand SaveConfigCommand =>
            _SaveConfigCommand ?? (_SaveConfigCommand = new DelegateCommand(ExecuteSaveConfigCommand));

        void ExecuteSaveConfigCommand()
        {
            if (AutomationModel != null)
            {
                switch (WindowType)
                {
                    case WindowType.Add:
                        Insert();
                        break;
                    case WindowType.Edit:
                        Edit();
                        break;
                }

                EditAutomationItemWindow.DialogResult = true;
            }
            EditAutomationItemWindow.Close();

        }

        /// <summary>
        /// 取消窗口
        /// </summary>
        private DelegateCommand _CannelCommand;
        public DelegateCommand CannelCommand =>
            _CannelCommand ?? (_CannelCommand = new DelegateCommand(ExecuteCannelCommand));

        void ExecuteCannelCommand()
        {
            EditAutomationItemWindow.DialogResult = false;
            EditAutomationItemWindow.Close();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="automationModel"></param>
        public void Insert()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["ID"] = AutomationModel.ID;
            dic["ControlName"] = AutomationModel.ControlName.ToString();
            dic["ControlType"] = AutomationModel.ControlType.ToString();
            dic["Handle"] = AutomationModel.Handle.ToString();
            dic["Name"] = AutomationModel.Name.ToString();
            dic["Operation"] = AutomationModel.Operation?.ToString();
            dic["SetValue"] = AutomationModel.SetValue?.ToString();
            dic["SleepTime"] = AutomationModel.SleepTime.ToString();
            XmlHelper.InsertAttribute(SystemConfig.xml_automation, "UIAutomation/Road", CurrentRoleID, OldId, dic);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="automationModel"></param>
        public void Edit()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["ID"] = AutomationModel.ID.ToString();
            dic["ControlName"] = AutomationModel.ControlName.ToString();
            dic["ControlType"] = AutomationModel.ControlType.ToString();
            dic["Handle"] = AutomationModel.Handle.ToString();
            dic["Name"] = AutomationModel.Name.ToString();
            dic["Operation"] = AutomationModel.Operation?.ToString();
            dic["SetValue"] = AutomationModel.SetValue?.ToString();
            dic["SleepTime"] = AutomationModel.SleepTime.ToString();
            XmlHelper.ModifyAttribute(SystemConfig.xml_automation, "UIAutomation/Road", CurrentRoleID, AutomationModel.ID.ToString(), dic);
        }

        /// <summary>
        /// 接收实体
        /// </summary>
        /// <param name="model"></param>
        public void EditAutomationItemEventReceived(Dictionary<string, object> dic)
        {
            //窗口类型
            WindowType = (WindowType)dic["Type"];
            //路线ID
            CurrentRoleID = dic["RodeID"].ToString();
            switch (WindowType)
            {
                case WindowType.Edit:
                    AutomationModel = (AutomationModel)dic["AutomationModel"];
                    CurrentComboBoxItemModel = ControlTypeList.FirstOrDefault(o => o.Value == AutomationModel.ControlType);
                    break;
                case WindowType.Add:
                    AutomationModel.ID = dic["AutomationID"].ToString();
                    OldId = dic["OldId"].ToString();
                    break;
            }

        }

        #endregion
    }
}
