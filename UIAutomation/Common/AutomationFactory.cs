using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using HandyControl.Controls;
using NLog;
using UIAutomation.Models;
using UIAutomation.Tools;

namespace UIAutomation.Common
{
    /// <summary>
    /// 自动化工程
    /// </summary>
    public class AutomationFactory
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static readonly ILogger _Logger = LogManager.GetLogger(nameof(AutomationFactory));

        /// <summary>
        /// 当前进程
        /// </summary>
        private static Process CurrentProcess { get; set; }

        public static void LoadProcess(Process process)
        {
            if(CurrentProcess?.Id != process.Id )
            {
                CurrentProcess = process;
            } 
        }

        public static object ExecuteAutomationTask(AutomationModel automation)
        {
            //非WPF控件类型列表
            string[] controlTypeArr = { "process", "mouse" };
            //在进程中查找控件的 AutomationElement
            AutomationElement automationElement = AutomationHelper.FindElementById(CurrentProcess, automation.ControlName);
            if (automationElement == null && !controlTypeArr.Contains(automation.ControlType.Trim().ToLower()))
            {
                MessageBox.Show("无效控件");
                return -1;
            }

            //按类型执行不同的任务
            switch (automation.ControlType.Trim().ToLower())
            {
                case "window":

                    break;
                case "textbox":
                    if (automation.Operation.ToLower().Trim() == "text")
                    {
                        AutomationHelper.SetValueToValuePattern(automationElement, automation.SetValue);
                    }
                    break;
                case "listbox":
                    if (automation.Operation.ToLower().Trim() == "cursorpos")
                    {
                        string[] arr = automation.SetValue.Split(',');
                        AutomationHelper.CursorPos(automationElement, Convert.ToInt32(arr[0]), Convert.ToInt32(arr[1]));
                    }
                    else if (automation.Operation.ToLower().Trim() == "select")
                    {
                        AutomationHelper.SelectionPatternToOperation(automationElement, automation.Operation, automation.SetValue);
                    }
                    break;
                case "button":
                    if (automation.Operation.ToLower().Trim() == "click")
                    {
                        AutomationHelper.InvokePatternToInvoke(automationElement);
                    }
                    break;
                case "combobox":
                    if (automation.Operation.ToLower().Trim() == "select")
                    {
                        AutomationHelper.SetSelectedComboBoxItem(automationElement, automation.SetValue);
                    }
                    break;
                case "checkbox":

                    break;
                case "radiobutton":

                    break;
                case "togglebutton":
                    if (automation.Operation.ToLower().Trim() == "toggle")
                    {
                        AutomationHelper.TogglePatternToToggle(automationElement);
                    }
                    break;
                case "scrollviewer":
                    if (automation.Operation.ToLower().Trim() == "scroll")
                    {
                        AutomationHelper.ScrollPatternToInvoke(automationElement, automation.SetValue);
                    }
                    break;
                case "messagebox":
                    if (automation.Operation.ToLower().Trim() == "click")
                    {
                        AutomationHelper.MessageBoxToInvoke(automationElement);
                    }
                    break;
                case "process":
                    if (automation.Operation.ToLower().Trim() == "kill")
                    {
                        ProcessHelper.KillProcess(automation.ControlName.Trim());
                    }
                    else if (automation.Operation.ToLower().Trim() == "start")
                    {
                        ProcessHelper.StartProcess(automation.SetValue.Trim());
                    }
                    break;
                case "mouse":
                    if (automation.Operation.ToLower().Trim() == "click")
                    {
                        AutomationHelper.MouseClick();
                    }
                    break;
            }
            return 0;
        }

    }
}
