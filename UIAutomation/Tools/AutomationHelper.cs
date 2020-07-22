using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace UIAutomation.Tools
{
    /// <summary>
    /// 自动化Helper类
    /// </summary>
    public class AutomationHelper
    { 
        /// <summary>
        /// Get the automation element by automation Id.
        /// </summary>
        /// <param name="windowName">Window name</param>
        /// <param name="automationId">Control automation Id</param>
        /// <returns>Automatin element searched by automation Id</returns>
        public static AutomationElement FindElementById(Process process, string automationId)
        {
            AutomationElement aeForm = FindWindowByProcessId(process);
            if (string.IsNullOrEmpty(automationId))
                return aeForm;
            AutomationElement tarFindElement = aeForm.FindFirst(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
            return tarFindElement;
        }

        /// <summary>
        /// Get the automation elemention of current form.
        /// </summary>
        /// <param name="processId">Process Id</param>
        /// <returns>Target element</returns>
        public static AutomationElement FindWindowByProcessId(Process process)
        {
  
            int count = 0;
            try
            { 
                AutomationElement targetWindow = AutomationElement.FromHandle(process.MainWindowHandle); 
                return targetWindow;
            }
            catch (Exception ex)
            { 
                StringBuilder sb = new StringBuilder();
                string message = sb.AppendLine(string.Format("Target window is not existing.try #{0}", count)).ToString(); 
                throw new InvalidProgramException(message, ex); 
            }
        }


        #region 鼠标点击

        public static void MouseClick()
        {
            WinAPIHelper.Click();
        }

        #endregion

        #region ExpandCollapsePattern helper

        /// <summary>
        /// Get ExpandCollapsePattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>ExpandCollapsePattern instance</returns>
        public static ExpandCollapsePattern GetExpandCollapsePattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the ExpandCollapsePattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as ExpandCollapsePattern;
        }
        #endregion

        #region ValuePattern helper

        /// <summary>
        /// To get the ValuePattern
        /// </summary>
        /// <param name="element">Target element</param>
        /// <returns>ValuePattern instance</returns>
        public static ValuePattern GetValuePattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(ValuePattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the ValuePattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as ValuePattern;
        }
        /// <summary>
        /// Write value to ValuePattern 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetValueToValuePattern(AutomationElement element,string value)
        {
            ValuePattern valuePattern = GetValuePattern(element);
             value = value.Replace("{Time}", DateTime.Now.ToString("yyyyMMddHHmmss"));
             valuePattern.SetValue(value);

        }
        #endregion

        #region WindowPattern helper

        /// <summary>
        /// Get WindowPattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>WindowPattern instance</returns>
        public static WindowPattern GetWindowPattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(WindowPattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the WindowPattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as WindowPattern;
        }

        #endregion


        #region

        public static void MessageBoxToInvoke(AutomationElement element)
        {
            double offset_top = 60;
            double offset_left = 35;
            double x = element.Current.BoundingRectangle.X;
            double y = element.Current.BoundingRectangle.Y;
            double width = element.Current.BoundingRectangle.Width;
            double height = element.Current.BoundingRectangle.Height;
            WinAPIHelper.MoveMouseToPoint((int)(x+ offset_left+ width /2), (int)(y+ offset_top + height / 2));
            Thread.Sleep(200);
            WinAPIHelper.Click();
        }

        #endregion

        #region ScrollPattern helper

        /// <summary>
        /// Get ScrollPattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>SelectItemPattern instance</returns>
        public static ScrollPattern GetScrollPattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(ScrollPattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the SelectionItemPattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as ScrollPattern;
        }

        public static void ScrollPatternToInvoke(AutomationElement element, string value)
        {
            ScrollPattern scrollPattern = GetScrollPattern(element);
            switch (value.ToLower().Trim())
            {
                case "largedecrement":
                    for (int i = 0; i < 50; i++)
                    {
                        try
                        {
                            scrollPattern.ScrollVertical(ScrollAmount.LargeDecrement);
                            return;
                        }
                        catch
                        {
                            Thread.Sleep(0);
                        }
                    }
                    break;
                case "largeincrement":
                    for (int i = 0; i < 50; i++)
                    {
                        try
                        {
                            scrollPattern.ScrollVertical(ScrollAmount.LargeIncrement);
                            return;
                        }
                        catch
                        {
                            Thread.Sleep(0);
                        }
                    }
                    break;
                case "noamount":
                    scrollPattern.ScrollVertical(ScrollAmount.NoAmount);
                    break;
                case "smalldecrement":
                    for (int i = 0; i < 50; i++)
                    {
                        try
                        {
                            scrollPattern.ScrollVertical(ScrollAmount.SmallDecrement);
                            return;
                        }
                        catch
                        {
                            Thread.Sleep(0);
                        }
                    }
                    break;
                case "smallincrement":
                    for (int i = 0; i < 50; i++)
                    {
                        try
                        {
                            scrollPattern.ScrollVertical(ScrollAmount.LargeIncrement);
                            return;
                        }
                        catch
                        {
                            Thread.Sleep(0);
                        }
                    }
                    break;
            }

        }
        #endregion




        #region InvokePattern helper

        /// <summary>
        /// Get InvokePattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>WindowPattern instance</returns>
        public static InvokePattern GetInvokePattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(InvokePattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the WindowPattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as InvokePattern;
        }

        /// <summary>
        ///  InvokePattern to Invoke
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>WindowPattern instance</returns>
        public static void InvokePatternToInvoke(AutomationElement element)
        {
            InvokePattern invokePattern = GetInvokePattern(element);
            invokePattern?.Invoke();
        }

        #endregion

        #region TogglePattern helper

        /// <summary>
        /// Get TogglePattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>WindowPattern instance</returns>
        public static TogglePattern GetTogglePattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(TogglePattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the WindowPattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as TogglePattern;
        }

        public static void TogglePatternToToggle(AutomationElement element)
        {
            TogglePattern togglePattern = GetTogglePattern(element);
            togglePattern?.Toggle();
        }

        #endregion

        #region SelectItemPattern

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="element"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public static void CursorPos(AutomationElement element, int left, int top)
        {
            double offset_top = top;
            double offset_left = left;
            double x = element.Current.BoundingRectangle.X;
            double y = element.Current.BoundingRectangle.Y;
            WinAPIHelper.MoveMouseToPoint((int)(x + offset_left), (int)(y + offset_top));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="automation"></param>
        public static int SelectionPatternToOperation(AutomationElement element,string operation,string value)
        {
            switch (operation.ToLower().Trim())
            {
                case "leftmouse":
                    SelectionPatternToMouseLeft(element,Convert.ToInt32(value));
                    return 0;
                case "if": 
                    return GetSelectionItemPatternCount(element);
                default:
                    SelectionPatternToSelect(element, Convert.ToInt32(value));
                    return 0;
            }
        }

        /// <summary>
        /// MouseLeft the list item
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <param name="targetIndex">item index collection</param> 
        public static void SelectionPatternToMouseLeft(AutomationElement element, int targetIndex)
        { 
            AutomationElementCollection rows =
               element.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));

            object multiSelect;
            if (rows[targetIndex - 1].TryGetCurrentPattern(SelectionItemPattern.Pattern, out multiSelect))
            {
                (multiSelect as SelectionItemPattern).AddToSelection();
                AutomationElement automationElement = (multiSelect as SelectionItemPattern).Current.SelectionContainer;
                int x = (int)automationElement.Current.BoundingRectangle.X;
                int y = (int)automationElement.Current.BoundingRectangle.Y;
                int width = (int)automationElement.Current.BoundingRectangle.Width;
                int height = (int)automationElement.Current.BoundingRectangle.Height;
                x = x + (width / 2) + 100;
                y = y + (height / 2);

                WinAPIHelper.MoveMouseToPoint(x, y);
                Thread.Sleep(1000);
                WinAPIHelper.RightClick(x, y);
            }
        }

        public static int GetSelectionItemPatternCount(AutomationElement element)
        {
            AutomationElementCollection rows =
             element.FindAll(TreeScope.Descendants,
              new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            return rows.Count;
        }

        /// <summary>
        /// Get SelectItemPattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>SelectItemPattern instance</returns>
        public static SelectionItemPattern GetSelectionItemPattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the SelectionItemPattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as SelectionItemPattern;
        }
        /// <summary>
        /// Get SelectPattern
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <returns>SelectItemPattern instance</returns>
        public static SelectionPattern GetSelectionPattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(SelectionPattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the SelectionItemPattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as SelectionPattern;
        }

        /// <summary>
        /// Bulk select the list item
        /// </summary>
        /// <param name="element">AutomationElement instance</param>
        /// <param name="targetIndex">item index collection</param> 
        public static void SelectionPatternToSelect(AutomationElement element,int targetIndex)
        {  
            AutomationElementCollection rows =
               element.FindAll(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));

            object multiSelect;
            if (rows[targetIndex-1].TryGetCurrentPattern(SelectionItemPattern.Pattern, out multiSelect))
            {
                (multiSelect as SelectionItemPattern).AddToSelection();
            } 
             
        }

        /// <summary>
        /// Combobox选中
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="index"></param>
        public static void SetSelectedComboBoxItem(AutomationElement comboBox, string index)
        { 
            Int32.TryParse(index,out int selectIndex); 
            AutomationPattern automationPatternFromElement = GetSpecifiedPattern(comboBox, "ExpandCollapsePatternIdentifiers.Pattern");
            ExpandCollapsePattern expandCollapsePattern = comboBox.GetCurrentPattern(automationPatternFromElement) as ExpandCollapsePattern;
            expandCollapsePattern.Expand();
            Thread.Sleep(1000); 
            AutomationElementCollection listItem = comboBox.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
            if (listItem.Count >= selectIndex)
            {
                AutomationElement automationElement = listItem[selectIndex - 1];
                AutomationPattern automationPatternFromElement_item = GetSpecifiedPattern(automationElement, "SelectionItemPatternIdentifiers.Pattern");
                SelectionItemPattern selectionItemPattern = automationElement.GetCurrentPattern(automationPatternFromElement_item) as SelectionItemPattern;
                selectionItemPattern.Select(); 
            } 
            expandCollapsePattern.Collapse();
        }


        private static AutomationElement[] GetCurrentSelectionProperty(AutomationElement selectionContainer)
        {
            try
            {
                return selectionContainer.GetCurrentPropertyValue(
                    SelectionPattern.SelectionProperty) as AutomationElement[];
            }
            // Container is not enabled
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private static AutomationPattern GetSpecifiedPattern(AutomationElement element, string patternName)
        {
            AutomationPattern[] supportedPattern = element.GetSupportedPatterns();

            foreach (AutomationPattern pattern in supportedPattern)
            {
                if (pattern.ProgrammaticName == patternName)
                    return pattern;
            }

            return null;
        }
        #endregion
    }
}
