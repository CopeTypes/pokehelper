using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Tesseract;

namespace PokeHelper.util
{
    public class OcrHelper
    { // reading text from the game screen, used mainly to recognize pop-ups/dialogs

        private readonly TesseractEngine _ocr;

        public OcrHelper()
        {
            _ocr = new TesseractEngine("C:\\Program Files\\Tesseract-OCR\\tessdata", "eng");
        }
        
        public string DumpScreen(Image img)
        { // dumps all text from a screenshot
            using (var pix = MakePix(img))
            {
                using (var page = _ocr.Process(pix))
                {
                    return page.GetText();
                }   
            }
        }

        private Pix MakePix(Image image)
        { // convert the Image -> Bitmap, enhance it for OCR, convert -> Pix
            var bmp = new Bitmap(image);
            var sw = new Stopwatch();
            sw.Start();
            //todo need to do more experimentation with this to see if its worthwile
            //bmp = EnhanceResolution(bmp);
            //Console.WriteLine("Upscaling took " + sw.ElapsedMilliseconds + "ms");
            //sw.Restart();
            ApplyContrast(bmp);
            Console.WriteLine("Applying contrast took " + sw.ElapsedMilliseconds + "ms");
            sw.Restart();
            InvertColors(bmp);
            Console.WriteLine("Inverting colors took " + sw.ElapsedMilliseconds + "ms");
            sw.Stop();
            return PixConverter.ToPix(bmp);
        }

        private const float Contrast = 1.5f;

        private static unsafe void ApplyContrast(Bitmap image)
        {
            var bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            var height = image.Height;
            var width = image.Width;
            
            Parallel.For(0, height, y =>
            {
                var row = (byte*)bmpData.Scan0 + (y * bmpData.Stride);

                for (var x = 0; x < width; x++)
                {
                    var offset = x * bytesPerPixel;
                    int blue = row[offset];
                    int green = row[offset + 1];
                    int red = row[offset + 2];

                    blue = (int) ((blue - 127.5) * Contrast + 127.5);
                    green = (int) ((green - 127.5) * Contrast + 127.5);
                    red = (int) ((red - 127.5) * Contrast + 127.5);

                    blue = Math.Max(0, Math.Min(255, blue));
                    green = Math.Max(0, Math.Min(255, green));
                    red = Math.Max(0, Math.Min(255, red));

                    row[offset] = (byte) blue;
                    row[offset + 1] = (byte) green;
                    row[offset + 2] = (byte) red;
                }
            });

            image.UnlockBits(bmpData);
        }

        
        private static Bitmap InvertColors(Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;
            var format = image.PixelFormat;
            
            var invertedBitmap = new Bitmap(width, height, format);
            var bmpDataSrc = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
            var bmpDataDst = invertedBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
            var bytesPerPixel = Image.GetPixelFormatSize(format) / 8;

            unsafe
            {
                for (var y = 0; y < height; y++)
                {
                    var srcRow = (byte*)bmpDataSrc.Scan0 + (y * bmpDataSrc.Stride);
                    var dstRow = (byte*)bmpDataDst.Scan0 + (y * bmpDataDst.Stride);

                    for (var x = 0; x < width; x++)
                    {
                        var offset = x * bytesPerPixel;
                        dstRow[offset] = (byte)(255 - srcRow[offset]);         // blue
                        dstRow[offset + 1] = (byte)(255 - srcRow[offset + 1]); // green
                        dstRow[offset + 2] = (byte)(255 - srcRow[offset + 2]); // red
                    }
                }
            }
            
            image.UnlockBits(bmpDataSrc);
            invertedBitmap.UnlockBits(bmpDataDst);
            return invertedBitmap;
        }

        
        private static Bitmap EnhanceResolution(Image image)
        {
            var targetWidth = (int) (image.Width * 1.75);
            var targetHeight = (int) (image.Height * 1.75);
            
            var upscaled = new Bitmap(targetWidth, targetHeight);
            using (var graphics = Graphics.FromImage(upscaled))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, new Rectangle(0, 0, targetWidth, targetHeight),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }

            return upscaled;
        }
    }
}