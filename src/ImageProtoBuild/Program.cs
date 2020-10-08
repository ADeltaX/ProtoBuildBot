using System;
using System.IO;
using BuildChecker;
using SkiaSharp;

namespace ImageProtoBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Draw(GenFake());
            Console.WriteLine("O K");
            Console.ReadKey();
        }

        static BuildInfo GenFake()
        {
            return new BuildInfo()
            {
                BuildLong = "10.0.18309.1000 (rs_lolololol.181220-1256)",
                FlightID = "NS:478A",
                DeviceFamily = "Desktop",
                Ring = "WIF",
                UpdateID = new Guid().ToString()
            };
        }

        static void Draw(BuildInfo info)
        {
            //TEST

            //TODO

            var bitmap = SKBitmap.Decode(Directory.GetCurrentDirectory() + @"\IMG\prerelease.jpg");
            var canvas = new SKCanvas(bitmap);

            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.ResetMatrix();

            //BUILD NUMBER

            var font = SKTypeface.FromFamilyName("Segoe UI", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

            var brush = new SKPaint
            {
                Typeface = font,
                TextSize = 156,
                IsLinearText = true,
                IsDither = true,
                IsEmbeddedBitmapText = true,
                LcdRenderText = true,
                IsAntialias = true,
                Color = new SKColor(255, 255, 255, 255),
                ImageFilter = SKImageFilter.CreateDropShadow(1, 1, 4, 4, SKColors.Black, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
            };
            var msNumber = brush.MeasureText(info.BuildLong.Split(' ')[0].Replace("10.0.", "").Split('.')[0]);

            //Draw Number
            canvas.DrawText(info.BuildLong.Split(' ')[0].Replace("10.0.", "").Split('.')[0], new SKPoint(24, 156 + 128), brush);

            //BUILD NUMBER REVISION

            font = SKTypeface.FromFamilyName("Segoe UI", SKFontStyleWeight.Thin, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

            brush.TextSize = 76;
            brush.Color = new SKColor(255, 255, 255, 180);

            //Draw Revision
            canvas.DrawText("." + info.BuildLong.Split(' ')[0].Replace("10.0.", "").Split('.')[1], new SKPoint(24 + msNumber, 156 + 128), brush);

            //BUILD BRANCH
            font = SKTypeface.FromFamilyName("Segoe UI", SKFontStyleWeight.Thin, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

            brush.TextSize = 32;
            brush.Color = new SKColor(255, 255, 255, 180);

            //Draw branch
            canvas.DrawText(info.BuildLong.Split(' ')[1].Replace("(", "").Replace(")", "").Split('.')[0], new SKPoint(24, 128 + 32), brush);

            //Draw compile date
            canvas.DrawText(info.BuildLong.Split(' ')[1].Replace("(", "").Replace(")", "").Split('.')[1], new SKPoint(72, 156 + 128 + 36), brush);

            canvas.Flush();

            var image = SKImage.FromBitmap(bitmap);
            var data = image.Encode(SKEncodedImageFormat.Png, 100);

            using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + @"\OUT\prerelease.jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                data.SaveTo(fs);
            }

            image.Dispose();
            canvas.Dispose();
            brush.Dispose();
            font.Dispose();
            bitmap.Dispose();
        }
    }
}
