using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq; 
using System.Web;

namespace Warehouse.GlobalClass
{
    public class Barcode
    {
        internal Image getBarCode(string Code, int W, int H)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            BarcodeLib.TYPE type = BarcodeLib.TYPE.UNSPECIFIED;
            type = BarcodeLib.TYPE.CODE128;
            b.IncludeLabel = false;
            //===== Encoding performed here ===== 
            return b.Encode(type, Convert.ToInt64(Code).ToString("D12"), Color.Black, Color.White, W, H);
        }
        internal byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        /*function to count digits*/
        public int countDigits(decimal num)
        {
            int count = 0;
            while (num > 1)
            {
                count++;
                num /= 10;
            }
            return count;
        }


    }
}