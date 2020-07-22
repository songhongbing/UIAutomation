using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace UIAutomation.Models
{
    public class RoadsModel:BindableBase
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 路线名称
        /// </summary>
        public string RoadName { get; set; }

        /// <summary>
        /// 动画列表
        /// </summary>
        private ObservableCollection<AutomationModel> _AutomationList=new ObservableCollection<AutomationModel>();
        public ObservableCollection<AutomationModel> AutomationList
        {
            get { return _AutomationList; }
            set { SetProperty(ref _AutomationList, value); }
        }
    }
}
