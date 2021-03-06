﻿using JXGIS.JXTopsystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class SystemUtils
    {
        private static readonly object _lockObject3 = new object();
        private static SqlDBContext _EFDbContext;
        private static readonly object _lockObject4 = new object();
        public static SqlDBContext EFDbContext
        {
            get
            {
                if (SystemUtils._EFDbContext == null)
                    lock (_lockObject3)
                        SystemUtils._EFDbContext = new SqlDBContext();
                return SystemUtils._EFDbContext;
            }
        }

        public static SqlDBContext NewEFDbContext
        {
            get
            {
                return new SqlDBContext();
            }
        }


        private static string baseUrl = null;
        public static string BaseUrl
        {
            get
            {
                if (baseUrl == null)
                {
                    string appPath = HttpContext.Current == null ? string.Empty : HttpContext.Current.Request.ApplicationPath;
                    baseUrl = appPath == "/" ? string.Empty : appPath;
                }
                return baseUrl;
            }
        }
        public static OracleDBContext NewOrclEFDbContext
        {
            get
            {
                return new OracleDBContext();
            }
        }

        /// <summary>
        /// 调试配置文件地址
        /// </summary>
        private static string debugConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config\\SystemParameters.json";
        private static dynamic _Config;
        private static readonly object _lockObject1 = new object();
        public static dynamic Config
        {
            get
            {
                if (_Config == null)
                {
                    lock (_lockObject1)
                    {
                        using (StreamReader sr = new StreamReader(debugConfigPath))
                        {
                            string json = sr.ReadToEnd();
                            _Config = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        }
                    }
                }
                return _Config;
            }
        }
    }
}