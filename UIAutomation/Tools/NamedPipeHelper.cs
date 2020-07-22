using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UIAutomation.Tools
{
    public class NamedPipeHelper
    {
        public delegate void ReceivedMsg(string msg);
        public event ReceivedMsg ReceivedMsgEvent;

        #region 属性

        private readonly string PipeName = "UIAutomation";

        private NamedPipeClientStream pipeClient;
        private StreamWriter swClient;
        private StreamReader srClient;

        #endregion

        #region 方法
        /// <summary>
        /// 启动管道服务端
        /// </summary>
        public void StartService()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut))
                        {
                            pipeServer.WaitForConnection();
                            using (var reader = new StreamReader(pipeServer))
                            {
                                string cmd = reader.ReadLine();
                                ReceivedMsgEvent(cmd);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        if (ex.Message.Contains("所有的管道范例都在使用中"))
                        {
                            Application.Current.Dispatcher.Invoke(()=> {
                                Application.Current.Shutdown();
                            }); 
                        } 
                        return;
                    }
                }
            });
        }
        /// <summary>
        /// 启动管道客户端
        /// </summary>
        public void StartClient()
        {
            try
            {
                pipeClient = new NamedPipeClientStream("localhost", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
                pipeClient.Connect(5000);
                swClient = new StreamWriter(pipeClient);
                srClient = new StreamReader(pipeClient);
                swClient.AutoFlush = true;
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(string msg)
        {
            swClient?.WriteLine(msg);
        }

        #endregion
    }
}
