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

namespace JXGIS.JXTopsystem.Controllers
{
    public class SaveQRCodeController : Controller
    {
        public ActionResult SaveQRCodeImgs(int? Code)
        {
            RtObj rt = null;
            try
            {
                string QRFilePath = Path.Combine(FileController.uploadBasePath, FileController.RPQRCodeRelativePath);

                string strCode = $"http://www.cristbin.com/DMQuery_wx/LPYH/BaseInfo?Id={Code}";
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
        //[LoggerFilter(Description = "二维码图片下载")]
        public ActionResult DownloadQRCode(int StartCode, int EndCode)
        {
            RtObj rt = null;
            try
            {
                rt = new RtObj();
                string QRFilePath = Path.Combine(FileController.uploadBasePath, FileController.RPQRCodeRelativePath);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = null;
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。
                    for (var i = StartCode; i <= EndCode; i++)
                    {
                        string fileName = i + ".jpg";
                        string sourceFile = Path.Combine(QRFilePath, fileName);
                        file.Add(sourceFile);
                    }
                    file.CommitUpdate();
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
                Response.AddHeader("content-disposition", "attachment;filename=二维码" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt, JsonRequestBehavior.AllowGet);
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