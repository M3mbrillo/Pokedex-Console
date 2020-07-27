using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex_Console
{
    public static class DrawerImage
    {
        public static async Task FromUrl(string url)
        {

            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            System.Net.WebResponse response = request.GetResponse();

            using System.IO.Stream responseStream = response.GetResponseStream();

            Bitmap bitmap = new Bitmap(responseStream);

            DrawerImage.FromBitmap(
                await OptimizeBitmap(bitmap)
                );
        }

        public static void FromBitmap(Bitmap bitmap)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, bitmap.Width), decimal.Divide(sMax, bitmap.Height));
            Size resSize = new Size((int)(bitmap.Width * percent), (int)(bitmap.Height * percent));
            Func<System.Drawing.Color, int> ToConsoleColor = c =>
            {
                int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
                index |= (c.R > 64) ? 4 : 0;
                index |= (c.G > 64) ? 2 : 0;
                index |= (c.B > 64) ? 1 : 0;
                return index;
            };
            Bitmap bmpMin = new Bitmap(bitmap, resSize.Width, resSize.Height);
            Bitmap bmpMax = new Bitmap(bitmap, resSize.Width * 2, resSize.Height * 2);
            for (int i = 0; i < resSize.Height; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("    ");

                for (int j = 0; j < resSize.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2, i * 2));
                    Console.BackgroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2, i * 2 + 1));
                    Console.Write("▀");

                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2 + 1, i * 2));
                    Console.BackgroundColor = (ConsoleColor)ToConsoleColor(bmpMax.GetPixel(j * 2 + 1, i * 2 + 1));
                    Console.Write("▀");
                }
                System.Console.WriteLine();
            }

        }

        static async Task<Bitmap> OptimizeBitmap(Bitmap bitmap)
        {
            var color = bitmap.GetPixel(0, 0);
            
            // when i write this, I was drunk on a Saturday night 🍻🍺, and...
            // my original idea was a "Solve, and SolveReverse", no 4 functions.
            Func<Bitmap, Color, int> solveY = (image, color) =>
            {
                var y = 0;
                for (int line = 0; line < image.Height; line++)
                {
                    Color pixel;
                    for (int col = 0; col < image.Width; col++)
                    {
                        pixel = image.GetPixel(col, line);
                        if (pixel != color && y == 0)
                        { // find the top
                            y = line;
                            break;
                        }
                    }
                    if (y != 0)
                        break;
                }
                return y;
            };

            Func<Bitmap, Color, int> solveYReversed = (image, color) =>
            {
                var y = 0;
                var countLine = 0;
                for (int line = image.Height; line > 0; line--)
                {
                    Color pixel;
                    for (int col = 0; col < image.Width; col++)
                    {
                        pixel = image.GetPixel(col, line - 1);
                        if (pixel != color && y == 0)
                        { // find the top
                            y = countLine;
                            break;
                        }
                    }
                    countLine++;
                    if (y != 0)
                        break;
                }
                return y;
            };

            Func<Bitmap, Color, int> solveX = (image, color) =>
            {
                var x = 0;
                for (int col = 0; col < image.Width; col++)
                {
                    Color pixel;
                    for (int line = 0; line < image.Height; line++)
                    {
                        pixel = image.GetPixel(col, line);
                        if (pixel != color && x == 0)
                        { // find the top
                            x = col;
                            break;
                        }
                    }
                    if (x != 0)
                        break;
                }
                return x;
            };

            Func<Bitmap, Color, int> solveXReversed = (image, color) =>
            {
                var x = 0;
                var countCol = 0;
                for (int col = 0; col < image.Width; col++)
                {
                    Color pixel;
                    for (int line = image.Height; line > 0; line--)
                    {
                        pixel = image.GetPixel(col, line - 1);
                        if (pixel != color && x == 0)
                        { // find the top
                            x = countCol;
                            break;
                        }
                    }
                    countCol++;
                    if (x != 0)
                        break;
                }
                return x;
            };

            //todo make the resource bitmap multi thread safe...
            //bitmap is not friendly to multi thread
            var tY = Task.Run(() => solveY(bitmap, color));
            var y = await tY;
            var tYReversed = Task.Run(() => solveYReversed(bitmap, color));
            var height = bitmap.Height - y - await tYReversed;

            var tX = Task.Run(() => solveX(bitmap, color));
            var x = await tX;
            var tXReversed = Task.Run(() => solveYReversed(bitmap, color));
            var width = bitmap.Width - y - await tXReversed;


            return bitmap.Clone(new Rectangle(x, y, width, height), bitmap.PixelFormat);
        }
    }
}
