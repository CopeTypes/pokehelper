using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PokeHelper.util
{
    public class UIHelper
    {

        private static Color GenericButtonColor = Color.FromArgb(46, 205, 167);
        
        public static Pos FindGenericOkButton(Image image)
        { // locate the generic green confirmation buttons with exact pixel matching
            // tested on a few buttons and dialogs and works fine, need to see if the green x close button would interfere at all
            try
            {
                var pos = FindColor(new Bitmap(image), GenericButtonColor);
                return pos.IsSet() ? pos : null;
            }
            catch (Exception e)
            {
                Console.WriteLine("UIHelper FindGenericButton error:\n" + e.StackTrace);
                return null;
            }
        }
        
        
        private static unsafe Pos FindColor(Bitmap image, Color color)
        {
            var bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;
            var scan0 = (byte*) bitmapData.Scan0.ToPointer();

            for (var y = 0; y < heightInPixels; y++)
            {
                var currentLine = scan0 + (y * bitmapData.Stride);
                for (var x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    // Get the color of the current pixel
                    var blue = currentLine[x];
                    var green = currentLine[x + 1];
                    var red = currentLine[x + 2];

                    var pixelColor = Color.FromArgb(red, green, blue);

                    // Check if the color at the current pixel matches the target color
                    if (pixelColor.R != color.R || pixelColor.G != color.G || pixelColor.B != color.B) continue;
                    image.UnlockBits(bitmapData);
                    return new Pos(x / bytesPerPixel, y, "color_match");
                }
            }
            // If the color is not found, return (-1, -1)
            image.UnlockBits(bitmapData);
            return new Pos(-1, -1, "color_match");
        }
        
    }
}