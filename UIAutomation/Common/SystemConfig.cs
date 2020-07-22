using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UIAutomation.Models;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Xml;
using UIAutomation.Tools;
using Tool.Tools;

namespace UIAutomation.Common
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
         
        #region xml名称
        /// <summary>
        /// 路径
        /// </summary>
        public readonly static string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\UIAutomation\config";
        /// <summary>
        /// 文件
        /// </summary>
        public readonly static string filename = $@"{path}\db.dll";
        /// <summary>
        /// 目标进程名称
        /// </summary>
        public volatile static string targetProcessName = "";

        /// <summary>
        /// 测试路线XML名称
        /// </summary>
        public readonly static string xml_automation = "automation";
        /// <summary>
        /// 控件列表XML名称
        /// </summary>
        public readonly static string xml_control = "control";
        /// <summary>
        /// 控件png字典
        /// </summary>
        private static Dictionary<string, string> dicControlPng = new Dictionary<string, string>();

        public static Dictionary<string, string> DicControlPng
        {
            get
            {
                if (dicControlPng.Count==0)
                {
                    List<ComboBoxItemModel> list= GetControlList();
                    foreach (ComboBoxItemModel item in list)
                    {
                        dicControlPng.Add(item.Value,item.Data);
                    }
                }
                return dicControlPng;
            }
        }

        #endregion
       

        #region 方法
        /// <summary>
        /// 获取控件列表
        /// </summary>
        /// <returns></returns>
        public static List<ComboBoxItemModel> GetControlList()
        {
            XmlHelper.LoadXml(SystemConfig.xml_control, "/UIAutomation;component/Resources/Files/ControlList.xml");
            List<ComboBoxItemModel> list = XmlHelper.GetListByXmlDocument(SystemConfig.xml_control);
            return list;
        }

        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <returns></returns>
        public static ConfigModel GetConfig()
        {
            ConfigModel configModel = new ConfigModel();
            if (File.Exists(filename))
            {
                try
                { 
                    //读取加密并且保存
                    string value = File.ReadAllText(filename);
                    //字符串转二进制字符串
                    value = StringHelper.ByteStringToDecode(value);
                    //解密
                    string encryptResult = StringHelper.Decrypt(value); 
                    configModel = XmlDeserialize<ConfigModel>(encryptResult);
                }
                catch (Exception ex)
                {} 
            }
            return configModel;
        }

        /// <summary>
        /// 保存配置数据
        /// </summary>
        /// <returns></returns>
        public static void SaveConfig(ConfigModel configModel)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                XmlSerializer writer = new XmlSerializer(typeof(ConfigModel));
                FileStream file = File.Create(filename);
                writer.Serialize(file, configModel); 
                file.Close();

                //读取加密并且保存
                string value=File.ReadAllText(filename);
                //加密
                string result= StringHelper.Encrypt(value);
                //字符串转二进制字符串
                result = StringHelper.EncodeToByteString(result);
                File.WriteAllText(filename, result);
            }
            catch(Exception ex)
            { } 
        }

        /// <summary>
        /// xml格式字符串 反序列化为类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns> 
        public static T XmlDeserialize<T>(string xmlString)
        {
            T t = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigModel));
            using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                {
                    Object obj = xmlSerializer.Deserialize(xmlReader);
                    t = (T)obj;

                }
            }
            return t;
        }
        #endregion

    }
}
