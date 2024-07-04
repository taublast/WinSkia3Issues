using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace WinNotAntialiasedRepro
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();

            CanvasSimple.PaintSurface += OnPaintSurfaceSimple;

            Canvas.PaintSurface += OnPaintSurface;
        }

        (SKSurface, SKImage) CreateImage(int width, int height, float renderingScale)
        {
            var surfaceInfo = new SKImageInfo(width / 3, height / 3);
            var surface = SKSurface.Create(surfaceInfo);
            surface.Canvas.Clear(SKColors.White);

            // Calculate the margin
            float margin = 12 * renderingScale;

            // Define the rectangle inside the white rectangle
            var redRect = new SKRect(
                margin,
                margin,
                surfaceInfo.Width - margin,
                surfaceInfo.Height - margin
            );

            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.Red;
                surface.Canvas.DrawRect(redRect, paint);
            }

            surface.Flush();
            var image = surface.Snapshot();
            return (surface, image);
        }



        private void OnPaintSurfaceSimple(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (CanvasSimple.Height > 0 && CanvasSimple.Width > 0)
            {

                var canvas = e.Surface.Canvas;
                var res = CreateImage(e.Info.Width, e.Info.Height, (float)(e.Info.Width / CanvasSimple.Width));
                var image = res.Item2;

                //drawn image with rotation to illustrate missing antialiasing
                using var paint = new SKPaint() //max quality
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High,
                    IsDither = true
                };

                var count = canvas.Save();

                canvas.RotateDegrees((float)-10, (float)e.Info.Width / 2, (float)e.Info.Height / 2);

                canvas.DrawImage(image, 50, 50, paint);

                canvas.RestoreToCount(count);

                res.Item1.Dispose();
                res.Item2.Dispose();
            }

        }

        private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
        {
            if (Canvas.Height > 0 && Canvas.Width > 0)
            {
                var canvas = e.Surface.Canvas;
                var res = CreateImage(e.Info.Width, e.Info.Height, (float)(e.Info.Width / Canvas.Width));
                var image = res.Item2;

                //drawn image with rotation to illustrate missing antialiasing
                using var paint = new SKPaint() //max quality
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High,
                    IsDither = true
                };

                var count = canvas.Save();

                canvas.RotateDegrees((float)-10, (float)e.Info.Width / 2, (float)e.Info.Height / 2);

                canvas.DrawImage(image, 50, 50, paint); //DrawBitmap gives same edgy not-antialiased effect

                canvas.RestoreToCount(count);

                res.Item1.Dispose();
                res.Item2.Dispose();
            }

        }
    }

}
