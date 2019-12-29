using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Raymarcher
{
    public static class Imaging
    {
        public static Bitmap RawToImage(byte[] pixels, int width, int height)
        {
            Vector2I res = Graphics.GetRenderResolution();
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            try
            {
                Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            }
            catch(Exception e)
            {
                Log.Print(e.Message);
            }

            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
