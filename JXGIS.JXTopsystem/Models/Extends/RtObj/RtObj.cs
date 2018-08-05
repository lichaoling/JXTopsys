using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends.RtObj
{
    public class RtObj
    {
        // 默认的异常信息配置
        private static IRtObjCfg _rtObjCfg = new DefaultRtObjCfg();

        // 异常信息配置，可自定义，没有提供的情况下使用默认配置
        public static IRtObjCfg RtObjCfg
        {
            get
            {
                if (_rtObjCfg == null)
                    _rtObjCfg = new DefaultRtObjCfg();
                return _rtObjCfg;
            }
            set
            {
                _rtObjCfg = value;
            }
        }

        /// <summary>
        /// 默认的异常信息，由 IRtObjCfg 接口提供
        /// </summary>
        private static string defaultErrorMessage
        {
            get
            {
                return RtObjCfg.GetDefaultErrorMessage();
            }
        }

        /// <summary>
        /// 是否显示原始错误信息
        /// 调试状态下始终显示原始错误
        /// 非调试状态下通过 定义的  IRtObjCfg 接口获取配置确定是否显示原始错误
        /// </summary>
        private bool ShowOrginErrorMessage
        {
            get
            {
#if DEBUG
                return true;
#endif
                return RtObjCfg.GetShowOrginErrorMessage();
            }

        }

        /// <summary>
        /// 数据对象，默认为 Dictionary<string, object>类型
        /// </summary>
        public object Data { get; set; }

        // 是否为Error类型的自定义错误
        private bool isError = false;

        private Exception ex = null;

        public RtObj()
        {

        }
        public RtObj(Exception ex)
        {
            this.ex = ex;
            this.isError = ex.GetType() == typeof(Error);
            // 记录日志
            RtObj.Log(ex);
        }
        public RtObj(object obj)
        {
            this.Data = obj;
        }
        public static void Log(Exception ex)
        {
            RtObjCfg.Log(ex);
        }

        /// <summary>
        /// 在错误为自定义错误（即类型为Error的错误）或者开启了显示原始错误选项的情况下，返回原始错误，否则返回全局错误
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                if (ex != null)
                {
                    if (this.isError || ShowOrginErrorMessage)
                        return ex.Message;
                    else
                        return defaultErrorMessage;
                }
                return null;
            }
        }

        /// <summary>
        /// 添加数据，返回Dictionary<string,object>类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Dictionary<string, object> AddData(string key, object value)
        {
            if (this.Data == null || this.Data.GetType() != typeof(Dictionary<string, object>))
            {
                this.Data = new Dictionary<string, object>();
            }
            (this.Data as Dictionary<string, object>)[key] = value;
            return (this.Data as Dictionary<string, object>);
        }
    }
}