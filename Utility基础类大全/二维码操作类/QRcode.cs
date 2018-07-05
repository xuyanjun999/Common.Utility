using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;
using System.Web;

namespace Common.Utility
{
    public class QRcode
    {
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="size">图片大小</param>
        /// <param name="path">图片位置 
        /// 例如  /abc/abc/
        /// </param>
        /// <returns>返回生成的二维码图片路径</returns>
        public string Create(string str, int size, string path)
        {
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                //设置编码模式
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                //设置编码测量度
                qrCodeEncoder.QRCodeScale = size;
                //设置编码版本
                qrCodeEncoder.QRCodeVersion = 8;
                //设置编码错误纠正
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                System.Drawing.Image image = qrCodeEncoder.Encode(str);

                string filename = "~" + path + Guid.NewGuid() + ".jpg";
                image.Save(HttpContext.Current.Request.MapPath(filename));

                return filename.Replace("~", "");
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>    
        /// 调用此函数后使此两种图片合并，类似相册，有个    
        /// 背景图，中间贴自己的目标图片    
        /// </summary>    
        /// <param name="imgBack">粘贴的源图片</param>    
        /// <param name="destImg">粘贴的目标图片</param>    
        public static Image CombinImage(Image imgBack, string destImg)
        {
            Image img = Image.FromFile(destImg);        //照片图片      
            if (img.Height != 65 || img.Width != 65)
            {
                img = KiResizeImage(img, 65, 65, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);     

            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框    

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);    

            g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
            GC.Collect();
            return imgBack;
        }

        /// <summary>    
        /// Resize图片    
        /// </summary>    
        /// <param name="bmp">原始Bitmap</param>    
        /// <param name="newW">新的宽度</param>    
        /// <param name="newH">新的高度</param>    
        /// <param name="Mode">保留着，暂时未用</param>    
        /// <returns>处理以后的图片</returns>    
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量    
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        
        #region 生成base64编码格式的二维码
    /// <summary>
    /// 生成base64编码格式的二维码
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string MakeBase64QrCode(string content, int qrCodeVersion = 0, int qrCodeScale = 8)
    {
        string result = string.Empty;
        //初始化二维码生成工具
        QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
        qrCodeEncoder.QRCodeVersion = qrCodeVersion;
        qrCodeEncoder.QRCodeScale = qrCodeScale;

        //将字符串生成二维码图片
        using (Bitmap image = qrCodeEncoder.Encode(content, Encoding.Default))
        {
            //保存为PNG到内存流  
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);

                //输出二维码图片
                result = Convert.ToBase64String(ms.GetBuffer());
            }
        }
        return result;
    }
    #endregion

    }
}

/*
 * 引用ThoughtWorks.QRCode.dll
  QRcode qrCode = new QRcode();
    UserQrCode = qrCode.Create("http://www.kuiyu.net/" , 4, "/Content/Images/QrCode/");
 */
