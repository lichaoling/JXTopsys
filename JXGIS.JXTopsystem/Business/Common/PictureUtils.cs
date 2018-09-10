using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class PictureUtils
    {
        public static Image GetHvtThumbnail(Image image, int max)
        {
            int oh = image.Height;
            int ow = image.Width;

            int h = 0;
            int w = 0;

            if (oh > ow)
            {
                h = max;
                w = (int)((double)ow / oh * h);
            }
            else
            {
                w = max;
                h = (int)((double)oh / ow * w);
            }

            Bitmap m_hovertreeBmp = new Bitmap(w, h);
            Graphics m_HvtGr = Graphics.FromImage(m_hovertreeBmp);
            m_HvtGr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            m_HvtGr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            m_HvtGr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Rectangle rectDestination = new Rectangle(0, 0, w, h);

            m_HvtGr.DrawImage(image, rectDestination, 0, 0, ow, oh, GraphicsUnit.Pixel);
            return m_hovertreeBmp;
        }
    }
}