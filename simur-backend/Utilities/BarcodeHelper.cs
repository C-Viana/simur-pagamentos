
using SkiaSharp;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;

namespace simur_backend.Utilities
{
    public class BarcodeHelper
    {
        public static byte[] CreateBarcode(string barcodeNumbers)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.ITF,
                Options = new EncodingOptions
                {
                    Width = 140,
                    Height = 80,
                    Margin = 2
                }
            };

            using var bitmap = writer.Write(barcodeNumbers);
            using var ms = new MemoryStream();

            //bitmap.Save(ms, ImageFormat.Png);
            bitmap.Encode(SKEncodedImageFormat.Jpeg, 0).SaveTo(ms);
            return ms.ToArray();
        }
    }
}
