using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace UIAutomation.Tools
{
    public class WinAPIHelper
    {

        private const UInt32 MouseEventLeftDown = 0x0002;
        private const UInt32 MouseEventLeftUp = 0x0004;
        private const UInt32 MouseEventRightDown = 0x0008;
        private const UInt32 MouseEventRightUp = 0x00010;


        [DllImport("user32.dll")]

        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        /// <summary>
        /// 引用user32.dll动态链接库（windows api），
        /// 使用库中定义 API：SetCursorPos 
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);
        /// <summary>
        /// 移动鼠标到指定的坐标点
        /// </summary>
        public static void MoveMouseToPoint(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void Click()
        {
            mouse_event(MouseEventLeftDown, 0, 0, 0, IntPtr.Zero);
            mouse_event(MouseEventLeftUp, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(100);
        }

        public static void RightClick(double x, double y)
        {
            mouse_event(MouseEventRightDown, (UInt32)x, (UInt32)y, 0, IntPtr.Zero);
            mouse_event(MouseEventRightUp, (UInt32)x, (UInt32)y, 0, IntPtr.Zero);
            Thread.Sleep(100); 
        }
    }
}
