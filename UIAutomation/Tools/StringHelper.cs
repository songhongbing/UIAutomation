using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UIAutomation.Tools
{
    public class StringHelper
    {
        #region 加密字符串 
        static string encryptKey = "qwe123!@#";//字符串加密密钥(注意：密钥只能是4位)

        public static string Encrypt(string str)
        {//加密字符串

            try
            {
                byte[] key = Encoding.Unicode.GetBytes(encryptKey);//密钥
                byte[] data = Encoding.Unicode.GetBytes(str);//待加密字符串

                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//加密、解密对象
                MemoryStream MStream = new MemoryStream();//内存流对象

                //用内存流实例化加密流对象
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
                CStream.Write(data, 0, data.Length);//向加密流中写入数据
                CStream.FlushFinalBlock();//将数据压入基础流
                byte[] temp = MStream.ToArray();//从内存流中获取字节序列
                CStream.Close();//关闭加密流
                MStream.Close();//关闭内存流

                return Convert.ToBase64String(temp);//返回加密后的字符串
            }
            catch
            {
                return str;
            }
        }
        #endregion

        #region 解密字符串  
        public static string Decrypt(string str)
        {//解密字符串

            try
            {
                byte[] key = Encoding.Unicode.GetBytes(encryptKey);//密钥
                byte[] data = Convert.FromBase64String(str);//待解密字符串

                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//加密、解密对象
                MemoryStream MStream = new MemoryStream();//内存流对象

                //用内存流实例化解密流对象
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
                CStream.Write(data, 0, data.Length);//向加密流中写入数据
                CStream.FlushFinalBlock();//将数据压入基础流
                byte[] temp = MStream.ToArray();//从内存流中获取字节序列
                CStream.Close();//关闭加密流
                MStream.Close();//关闭内存流

                return Encoding.Unicode.GetString(temp);//返回解密后的字符串
            }
            catch
            {
                return str;
            }
        }
        #endregion

        /// <summary>
        /// 将 字符串 转成 二进制 “10011100000000011100011111111101”
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeToByteString(string s)
        {
            byte[] data = Encoding.Unicode.GetBytes(s);
            StringBuilder result = new StringBuilder(data.Length * 8);

            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }
        /// <summary>
        /// 将二进制 “10011100000000011100011111111101” 转成 字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ByteStringToDecode(string s)
        {
            System.Text.RegularExpressions.CaptureCollection cs =
                System.Text.RegularExpressions.Regex.Match(s, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }
    }
}
