using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class SecurityUtils
    {
        private static RSACryptoService _rsaCryptoService = null;
        private static MD5 md5 = null;

        public static string PublicKey
        {
            get
            {
                return SystemUtils.Config.Security.PublicKey.ToString();
            }
        }

        public static string PrivateKey
        {
            get
            {
                return SystemUtils.Config.Security.PrivateKey.ToString();
            }
        }
        public static RSACryptoService RSACryptoService
        {
            get
            {
                if (_rsaCryptoService == null)
                {
                    _rsaCryptoService = new RSACryptoService(PrivateKey, PublicKey);
                }
                return _rsaCryptoService;
            }
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string text)
        {
            return RSACryptoService.Encrypt(text);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string RSADecrypt(string cipherText)
        {
            return RSACryptoService.Decrypt(cipherText);
        }

        public static MD5 MD5
        {
            get
            {
                if (md5 == null)
                {
                    md5 = MD5.Create();
                }
                return md5;
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string text)
        {
            byte[] bytes = SecurityUtils.MD5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// DES对称加密，密钥必须为8位
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DESEncryt(string textToEncrypt, string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length != 8)
            {
                throw new Error("请提供有效的8位密钥");
            }

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(textToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// DES对称解密，密钥必须为8位
        /// </summary>
        /// <param name="textToDecrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DESDecrypt(string textToDecrypt, string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length != 8)
            {
                throw new Error("请提供有效的8位密钥");
            }
            byte[] inputByteArray = Convert.FromBase64String(textToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }
    }
}