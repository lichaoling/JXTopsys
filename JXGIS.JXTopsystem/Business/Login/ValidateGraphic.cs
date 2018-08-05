using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class ValidateGraphic
    {
        private int codeLength = 6;

        /// <summary>
        /// 验证验证码，验证成功则当前验证码失效，重新生成验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Validate(string code, bool refreshAfterSuccess = false)
        {
            var b = false;
            if (string.IsNullOrEmpty(code)) return false;
            b = code.ToLower() == this.Code.ToLower();
            // 验证成功后需刷新
            if (b && refreshAfterSuccess) this.Refresh();
            return b;
        }

        public ValidateGraphic()
        {
            this.createValidateGraphic();
        }

        public ValidateGraphic(int codeLength)
        {
            this.codeLength = codeLength;
            this.createValidateGraphic();
        }

        public void Refresh()
        {
            this.createValidateGraphic();
        }

        /// <summary>
        /// 随机验证码
        /// </summary>
        public string Code { get; set; }

        public byte[] CodeBytes { get; set; }

        private void createValidateGraphic()
        {
            try
            {
                this.Code = CreateRandomCode(this.codeLength);
                this.CodeBytes = CreateValidateGraphic(this.Code);
            }
            catch
            {
                this.Code = null;
                this.CodeBytes = null;
            }
        }

        /// <summary>
        /// 创建随机字符串
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string CreateRandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,a,b,c,d,e,f,g,h,i,g,k,l,m,n,o,p,q,r,F,G,H,I,G,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                randomCode += allCharArray[rand.Next(0, allCharArray.Length - 1)];
            }
            return randomCode;
        }

        /// <summary>
        /// 创建图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 14.0), 27);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, x2, y1, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);

                //画图片的前景干扰线
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);

                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}