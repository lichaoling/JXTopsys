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
    }
}