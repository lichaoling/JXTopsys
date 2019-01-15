using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using JXGIS.JXTopsystem.App_Start;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using JXGIS.JXTopsystem.Business;

namespace JXGIS.JXTopsystem.Controllers
{
    public class ResultTicket
    {
        public string ticket { get; set; }
        public string expire_seconds { get; set; }
        public string url { get; set; }
    }
    public class SaveQRCodeController : Controller
    {
        public ActionResult SaveQRCodeImgs(int? Code)
        {
            RtObj rt = null;
            try
            {
                WxUtils wx = new Business.WxUtils();
                wx = wx.GetConfig();
                WebClient wc = new WebClient();
                var url = string.Format(WxUtils.tickedUrl, wx.access_token);
                string postData = "{\"expire_seconds\": 604800, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\":" + Code + "}}}";
                var ticket = ServiceUtils.Post(url, postData);
                var re = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultTicket>(ticket);
                if (re == null || string.IsNullOrEmpty(re.url))
                    throw new Exception(ticket);

                string QRFilePath = Path.Combine(StaticVariable.basePath, StaticVariable.RPQRCodeRelativePath);

                //string strCode = $"http://www.cristbin.com/DMQuery_wx/LPYH/BaseInfo?Id={Code}";
                string strCode = re.url;

                QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrcode.GetGraphic(10, Color.Black, Color.White, null, 960, 960, false);
                Image img = qrCodeImage;

                //位图宽高
                int Margin = 60;
                int width = img.Width + Margin;
                int height = img.Height + Margin;
                Bitmap BitmapResult = new Bitmap(width, height);
                Graphics Grp = Graphics.FromImage(BitmapResult);
                SolidBrush b = new SolidBrush(Color.White);//这里修改颜色
                Grp.FillRectangle(b, 0, 0, width, height);
                System.Drawing.Rectangle Rec = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                //向矩形框内填充Img
                Grp.DrawImage(img, Margin / 2, Margin / 2, Rec, GraphicsUnit.Pixel);
                //返回位图文件
                Grp.Dispose();
                GC.Collect();

                string fileName = Code + ".jpg";
                string fileNameThum = "t-" + Code + ".jpg";
                string path = Path.Combine(QRFilePath, fileName);
                string pathThum = Path.Combine(QRFilePath, fileNameThum);
                if (!Directory.Exists(QRFilePath))
                {
                    Directory.CreateDirectory(QRFilePath);
                }
                BitmapResult.Save(path);
                Image imgThum = PictureUtils.GetHvtThumbnail(BitmapResult, 200);
                imgThum.Save(pathThum);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);

        }

        public void test1(int? Code)
        {
            try
            {
                WxUtils wx = new Business.WxUtils();
                wx = wx.GetConfig();
                WebClient wc = new WebClient();
                var url = string.Format(WxUtils.tickedUrl, wx.access_token);
                string postData = "{\"expire_seconds\": 604800, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\":" + Code + "}}}";
                var ticket = ServiceUtils.Post(url, postData);
                var re = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultTicket>(ticket);
                if (re == null || string.IsNullOrEmpty(re.url))
                    throw new Exception(ticket);

                string QRFilePath = Path.Combine(StaticVariable.basePath, StaticVariable.RPQRCodeRelativePath);

                //string strCode = $"http://www.cristbin.com/DMQuery_wx/LPYH/BaseInfo?Id={Code}";
                string strCode = re.url;

                QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrcode.GetGraphic(10, Color.Black, Color.White, null, 960, 960, false);
                Image img = qrCodeImage;

                //位图宽高
                int Margin = 60;
                int width = img.Width + Margin;
                int height = img.Height + Margin;
                Bitmap BitmapResult = new Bitmap(width, height);
                Graphics Grp = Graphics.FromImage(BitmapResult);
                SolidBrush b = new SolidBrush(Color.White);//这里修改颜色
                Grp.FillRectangle(b, 0, 0, width, height);
                System.Drawing.Rectangle Rec = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                //向矩形框内填充Img
                Grp.DrawImage(img, Margin / 2, Margin / 2, Rec, GraphicsUnit.Pixel);
                //返回位图文件
                Grp.Dispose();
                GC.Collect();

                string fileName = Code + ".jpg";
                string fileNameThum = "t-" + Code + ".jpg";
                string path = Path.Combine(QRFilePath, fileName);
                string pathThum = Path.Combine(QRFilePath, fileNameThum);
                if (!Directory.Exists(QRFilePath))
                {
                    Directory.CreateDirectory(QRFilePath);
                }
                BitmapResult.Save(path);
                Image imgThum = PictureUtils.GetHvtThumbnail(BitmapResult, 200);
                imgThum.Save(pathThum);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult test()
        {
            RtObj rt = null;
            try
            {
                for (int i = 1; i <= 829; i++)
                    test1(i);
                for (int i = 1001; i <= 1040; i++)
                    test1(i);
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
    }


    public class MyNameTransfom : ICSharpCode.SharpZipLib.Core.INameTransform
    {

        #region INameTransform 成员

        public string TransformDirectory(string name)
        {
            return null;
        }

        public string TransformFile(string name)
        {
            return Path.GetFileName(name);
        }

        #endregion
    }
}