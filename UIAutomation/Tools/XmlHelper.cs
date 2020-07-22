using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using UIAutomation.Models;

namespace Tool.Tools
{
    public class XmlHelper
    {
        private static Dictionary<string,XmlDocument> DocXml = new Dictionary<string,XmlDocument>(); 
        /// <summary>
        /// XML文档
        /// </summary>
        public static XmlDocument GetDoc(string key)
        {
            return DocXml[key]; 
        }  
        /// <summary>
        /// 载入Xml文件,嵌入资源
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="url">Xml文件路径</param>
        public static void LoadXml(string key,string url)
        {
            if (!DocXml.Keys.Contains(key))
            {
                DocXml.Add(key, new XmlDocument());
            }
            //判断是否为内部资源
            if (url.Contains(";component/"))
            {
                string result = GetXmlValue(url);
                SetDataXml(key, result); 
            }
            else
            {
                DocXml[key].Load(url);
            } 
        }
        /// <summary>
        /// 读取Xml文件,嵌入资源
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="content">数据内容</param>
        public static bool SetDataXml(string key, string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                return false;
            }
            try
            {
                if (!DocXml.Keys.Contains(key))
                {
                    DocXml.Add(key,new XmlDocument());
                }
                 
                DocXml[key].LoadXml(content.Trim());
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// XML文件验证
        /// </summary>
        /// <returns></returns>
        public static bool IsValidate(string xml, out string error)
        {
            error = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 跟据路径获取Xml文本
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetXmlValue(string url)
        {
            Uri uri = new Uri(url, UriKind.Relative);
            System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(uri);
            Stream stream = info.Stream;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            string result = System.Text.Encoding.ASCII.GetString(data);
            return result.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("???", ""); 
        }

        /// <summary>
        /// 获取XmlDocument的根节点
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <returns>返回的XmlElement元素根节点</returns>
        public static XmlElement GetXmlDocumentRoot(string key)
        {
            return DocXml[key].DocumentElement;
        }
        /// <summary>
        /// 获取指定元素的指定Attribute值
        /// </summary>
        /// <param name="xe">表示一个XmlElement</param>
        /// <param name="attr">表示Attribute的名字</param>
        /// <returns>返回获取的Attribute的值</returns>
        public static string GetAttribute(XmlElement xe, string attr)
        {
            return xe.GetAttribute(attr);
        }
        /// <summary>
        /// 获取指定节点的指定Attribute值
        /// </summary>
        /// <param name="xn">表示一个XmlNode</param>
        /// <param name="attr"></param>
        /// <returns>返回获取的Attribute的值</returns>
        public static string GetAttribute(XmlNode xn, string attr)
        {
            XmlElement xe = ExchangeNodeElement(xn);
            return xe.GetAttribute(attr);
        }
        /// <summary>
        /// XmlElement对象转换成XmlNode对象
        /// </summary>
        /// <param name="xe">XmlElement对象</param>
        /// <returns>返回XmlNode对象</returns>
        public static XmlNode ExchangeNodeElement(XmlElement xe)
        {
            return (XmlNode)xe;
        }
        /// <summary>
        /// XmlNode对象转换成XmlElement对象
        /// </summary>
        /// <param name="xe">XmlNode对象</param>
        /// <returns>返回XmlElement对象</returns>
        public static XmlElement ExchangeNodeElement(XmlNode xn)
        {
            return (XmlElement)xn;
        }
        /// <summary>
        /// 获取节点的文本
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="nodename">节点的名称</param>
        /// <returns></returns>
        public static string GetXmlNodeInnerText(XmlNode xn, string nodename)
        {
            XmlNode childxn = xn.SelectSingleNode(nodename);
            return childxn.InnerText;
        }
        /// <summary>
        /// 获取指定节点的子节点
        /// </summary>
        /// <param name="xn">节点对象</param>
        /// <returns>返回子节点数</returns>
        public static int GetXmlNodeCount(XmlNode xn)
        {
            return xn.ChildNodes.Count;
        }
        /// <summary>
        /// 获取元素的文本
        /// </summary>
        /// <param name="xn">XmlElement元素</param>
        /// <param name="nodename">元素的名称</param>
        /// <returns></returns>
        public static string GetXmlElementInnerText(XmlElement xn, string nodename)
        {

            XmlNode childxn = xn.SelectSingleNode(nodename);
            return childxn.InnerText;
        }
        /// <summary>
        /// 获取XmlNode是否具有指定Attribute值
        /// </summary>
        /// <param name="xn">XmlNode对象</param>
        /// <param name="attr">Attribute的名称</param>
        /// <param name="compare">Attribute的值</param>
        /// <returns>返回bool值</returns>
        public static bool GetXmlNodeByArrtibute(XmlNode xn, string attr, string compare)
        {
            if (GetAttribute(xn, attr) == compare)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取XmlElement是否具有指定Attribute值
        /// </summary>
        /// <param name="xn">XmlElement对象</param>
        /// <param name="attr">Attribute的名称</param>
        /// <param name="compare">Attribute的值</param>
        /// <returns>返回bool值</returns>
        public static bool GetXmlNodeByArrtibute(XmlElement xe, string attr, string compare)
        {
            if (GetAttribute(xe, compare) == attr)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取一个具有指定Attrtibute的XmlNode子节点
        /// </summary>
        /// <param name="xn">XmlNode对象</param>
        /// <param name="attr">Attrtibute的名称</param>
        /// <param name="compare">Attrtibute的值</param>
        /// <returns>返回相应的子节点</returns>
        public static XmlNode GetXmlChildNodeByAttrtibute(XmlNode xn, string attr, string compare)
        {
            foreach (XmlNode cxn in xn.ChildNodes)
            {
                if (GetXmlNodeByArrtibute(cxn, attr, compare))
                {
                    return cxn;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="xPath">节点路径 例如 BookStore/NewBook</param>
        /// <returns></returns>
        public static List<ComboBoxItemModel> GetListByXmlDocument(string key)
        { 
            List<ComboBoxItemModel> list = new List<ComboBoxItemModel>();
            XmlElement xmlElement = GetXmlDocumentRoot(key); 
            foreach (XmlNode xmlNodeOne in xmlElement.ChildNodes)
            {
                ComboBoxItemModel comboBoxItemModel = new ComboBoxItemModel();
                comboBoxItemModel.Value = xmlNodeOne.Attributes["Name"].Value;
                comboBoxItemModel.Data= xmlNodeOne.Attributes["Path"].Value;
                foreach (XmlNode xmlNodeTwo in xmlNodeOne.ChildNodes)
                { 
                    Boolean.TryParse(xmlNodeTwo.Attributes["IsSetValue"].Value,out bool state);
                    comboBoxItemModel.List.Add(new ComboBoxItemModel() { Value = xmlNodeTwo.Attributes["Name"].Value, State = state });
                }
                list.Add(comboBoxItemModel);
            }
            return list;
        }

        /// <summary>
        /// 修改节点与属性
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="xPath">节点路径 例如 BookStore/NewBook</param>
        /// <param name="roleid">路线ID</param>
        /// <param name="id">标识</param>
        /// <param name="dics">参数集合</param>
        public static void ModifyAttribute(string key,string xPath,string roleid,string id,Dictionary<string,string> dics)
        { 
            XmlNodeList xmlNodeList = DocXml[key].SelectNodes(xPath);
            foreach (XmlNode xmlNodeOne in xmlNodeList)
            {
                if(xmlNodeOne.Attributes["ID"].Value.Trim() == roleid)
                {
                    foreach (XmlNode xmlNodeTwo in xmlNodeOne.ChildNodes)
                    {
                        if (xmlNodeTwo.Attributes["ID"].Value.Trim() == id)
                        {
                            foreach (KeyValuePair<string, string> kv in dics)
                            {
                                try
                                {  
                                    xmlNodeTwo.Attributes[kv.Key].Value = kv.Value;
                                }
                                catch(Exception ex)
                                { } 
                            }
                            return;
                        }
                    }
                } 
            }
        }

        /// <summary>
        /// 替换行顺序
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="xPath">节点路径 例如 BookStore/NewBook</param>
        /// <param name="roleid">路线ID</param>
        /// <param name="oneXmlNode">节点1</param>
        /// <param name="twoXmlNode">节点2</param>
        public static void ChangeRowItemsIndex(string key, string xPath, string roleid, int oneXmlNodeIndex, int twoXmlNodeIndex)
        {
            XmlNodeList xmlNodeList = DocXml[key].SelectNodes(xPath);
            foreach (XmlNode xmlNodeOne in xmlNodeList)
            {
                if (xmlNodeOne.Attributes["ID"].Value.Trim() == roleid)
                {
                    XmlNode twoXmlNode = xmlNodeOne.ChildNodes[twoXmlNodeIndex];
                    XmlNode oneXmlNode = xmlNodeOne.ChildNodes[oneXmlNodeIndex];

                    xmlNodeOne.InsertBefore(twoXmlNode,oneXmlNode);
                }
            }
        }

        /// <summary>
        /// 添加节点与属性
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="xPath">节点路径 例如 BookStore/NewBook</param>
        /// <param name="roleid">路线ID</param>
        /// <param name="id">标识</param>
        /// <param name="dics">参数集合</param>
        public static void InsertAttribute(string key, string xPath, string roleid, string id, Dictionary<string, string> dics)
        {
            XmlNodeList xmlNodeList = DocXml[key].SelectNodes(xPath);
            foreach (XmlNode xmlNodeOne in xmlNodeList)
            {
                if (xmlNodeOne.Attributes["ID"].Value.Trim() == roleid)
                {
                    for (int i=0; i<xmlNodeOne.ChildNodes.Count;i++)
                    {
                        XmlNode xmlNodeTwo = xmlNodeOne.ChildNodes[i]; 
                        if (xmlNodeTwo.Attributes["ID"].Value.ToString() == id)
                        { 
                            XmlElement newXmlNode = DocXml[key].CreateElement("Automation");
                            foreach (var item in dics)
                            {
                                newXmlNode.SetAttribute(item.Key, item.Value);
                            }
                            xmlNodeOne.InsertAfter(newXmlNode, xmlNodeTwo);
                            return;
                        }
                         
                    }
                }
            }
        } 

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="key">唯一索引</param>
        /// <param name="xPath">节点路径 例如 BookStore/NewBook</param>
        /// <param name="roleid">路线ID</param>
        /// <param name="id">标识</param>
        /// <param name="dics">参数集合</param>
        public static void DeleteAttribute(string key, string xPath, string roleid, string id)
        {
            XmlNodeList xmlNodeList = DocXml[key].SelectNodes(xPath);
            foreach (XmlNode xmlNodeOne in xmlNodeList)
            {
                if (xmlNodeOne.Attributes["ID"].Value.Trim() == roleid)
                {
                    foreach (XmlNode xmlNodeTwo in xmlNodeOne.ChildNodes)
                    {
                        if (xmlNodeTwo.Attributes["ID"].Value.Trim() == id)
                        {
                            xmlNodeOne.RemoveChild(xmlNodeTwo);
                            return;
                        }
                    }
                }
            }
        }
    } 
}
