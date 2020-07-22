using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace UIAutomation.Models
{
    public class AutomationModel:BindableBase
    {

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 步骤名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }

        private string _Operation;
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Operation {
            get 
            {
                return _Operation;
            }
            set
            { 
                SetProperty(ref _Operation, value);
            }
        }
        /// <summary>
        /// 句柄
        /// </summary>
        public int Handle { get; set; }
        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType { get; set; } 
        /// <summary>
        /// 赋值
        /// </summary>
        public string SetValue { get; set; }

        /// <summary>
        /// 睡眠时间(单位:毫秒)
        /// </summary>
        public int SleepTime { get; set; } 

        /// <summary>
        /// 是否被选中
        /// </summary>
        private int _IsSelected;
        public int IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }
    }
}
