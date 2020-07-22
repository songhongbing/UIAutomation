using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace UIAutomation.Models
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [Serializable]
    public class ConfigModel
    {
        private string _TargetSoftwarePath;
        /// <summary>
        /// 目标软件
        /// </summary>
        public string TargetSoftwarePath {
            get 
            {
                return _TargetSoftwarePath;
            }
            set
            {
                _TargetSoftwarePath = value;
            }
        }

        private ObservableCollection<FileData> fileDatas = new ObservableCollection<FileData>(); 
        /// <summary>
        /// 文件历史记录
        /// </summary>
        public ObservableCollection<FileData> HistoryFileData
        {
            get { return fileDatas; }
            set { fileDatas = value; }
        }

        private string _AutomationRoadData;
        /// <summary>
        /// 动画数据
        /// </summary>
        public string AutomationRoadData
        {
            get { return _AutomationRoadData; }
            set { _AutomationRoadData = value; }
        }

        private bool _IsLoopState;
        /// <summary>
        /// 是否循环执行任务
        /// </summary>
        public bool IsLoopState
        {
            get { return _IsLoopState; }
            set { _IsLoopState = value; }
        }

        /// <summary>
        /// 添加历史记录
        /// </summary>
        public void AddHistoryFile(string path)
        {
            FileData fileData = HistoryFileData.FirstOrDefault(o => o.FilePath.Trim() == path.Trim());
            if (fileData == null)
            {
                HistoryFileData.Add(new FileData { CreateTime = DateTime.Now, FilePath = path });
            }
            else
            {
                fileData.CreateTime = DateTime.Now;
            }
        }
    }

    [Serializable]
    public class FileData
    {
        public DateTime CreateTime { get; set; }
        public string FilePath { get; set; }
    }
}
