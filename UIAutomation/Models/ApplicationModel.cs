using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace UIAutomation.Models
{
    public class ApplicationModel
    {

    }
    /// <summary>
    /// ComboBox 子项
    /// </summary>
    public partial class ComboBoxItemModel: BindableBase
    {
        private string _Value;
        public string Value {
            get 
            {
                return _Value;
            }
            set 
            {
                _Value = value;
                SetProperty(ref _Value, value);
            }
        }

        private string _data;
        public string Data {
            get 
            {
                return _data;
            }
            set
            {
                _data = value;
                SetProperty(ref _data, value);
            }
        }

        private bool _state;
        public bool State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        private List<ComboBoxItemModel> list = new List<ComboBoxItemModel>();
        public List<ComboBoxItemModel> List { 
            get 
            { 
                return list; 
            }
            set 
            { 
                list = value;
                SetProperty(ref list, value);
            }
        }
    }

    /// <summary>
    /// 窗口类型枚举
    /// </summary>
    public enum WindowType
    {
        Add=0,
        Edit=1
    }
}
