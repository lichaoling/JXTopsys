using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class WxUtils
    {
        private static string wxCfgFileName = "wxConfig";
        private static string wxCachePathConfigPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, wxCfgFileName + ".json");
        private static string accessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        public static string tickedUrl = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";
        private static DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public string appId { get; set; }

        public string appSecret { get; set; }

        private string _access_token;

        public string access_token
        {
            get
            {
                if (string.IsNullOrEmpty(_access_token) || (DateTime.Now - at_time).TotalSeconds >= at_expires_in)
                {
                    this.RefreshAccessToken();
                    this.SaveWxConfig();
                }
                return _access_token;
            }
            set
            {
                _access_token = value;
            }
        }
        public int at_expires_in { get; set; }
        public DateTime at_time { get; set; }

        public  WxUtils GetConfig()
        {
            WxUtils wxConfig = null;
            try
            {
                using (StreamReader sr = new StreamReader(wxCachePathConfigPath))
                {
                    wxConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<WxUtils>(sr.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取微信配置缓存失败！");
            }
            return wxConfig;
        }

        private void SaveWxConfig()
        {
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            using (StreamWriter sw = new StreamWriter(wxCachePathConfigPath, false))
            {
                sw.Write(s);
            }
        }


        public void RefreshAccessToken()
        {
            try
            {
                WebClient wc = new WebClient();
                var url = string.Format(accessTokenUrl, this.appId, this.appSecret);
                var accessToken = wc.DownloadString(url);

                var at = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(accessToken);
                if (at == null || string.IsNullOrEmpty(at.access_token))
                {
                    //throw new Exception("获取AccessToken失败");
                    throw new Exception(accessToken);
                }
                this._access_token = at.access_token;
                this.at_expires_in = at.expires_in;
                at_time = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class AccessToken
        {
            public string access_token;
            public int expires_in;
        }



    }
}