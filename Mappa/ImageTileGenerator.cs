using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Mappa
{
    public class ImageTileGenerator : TileGenerator, IDisposable
    {
        private GeoLocation _upperRightBounds;
        private GeoLocation _lowerLeftBounds;
        private Image<Rgba32> _image;

        private Dictionary<int, Rectangle> ImageRectanglesAtZoomLevels = new Dictionary<int, Rectangle>();
        private Dictionary<int, Image> ScaledImagesAtZoomLevels = new Dictionary<int, Image>();

        public ImageTileGenerator(GeoLocation ur, GeoLocation ll, Image img, string outputPath) : base(outputPath)
        {
            _upperRightBounds = ur;
            _lowerLeftBounds = ll;
            _image = new Image<Rgba32>(img.Width, img.Height);
            _image.Mutate(x => x.DrawImage(img, 1.0f));
        }

        protected override void CreateTilesForDetailLevel(int detailLevel)
        {
            //at this zoom level, we find the pixel of the image bounds
            int imageUrX, imageUrY;
            int imageLlX, imageLlY;
            TileSystem.LatLongToPixelXY(this._upperRightBounds.LatitudeDegrees, this._upperRightBounds.LongitudeDegrees, detailLevel, out imageUrX, out imageUrY);
            TileSystem.LatLongToPixelXY(this._lowerLeftBounds.LatitudeDegrees, this._lowerLeftBounds.LongitudeDegrees, detailLevel, out imageLlX, out imageLlY);

            Rectangle imageRect = new Rectangle(imageLlX, imageUrY, (imageUrX - imageLlX), (imageLlY - imageUrY));
            lock (ImageRectanglesAtZoomLevels)
            {
                ImageRectanglesAtZoomLevels.Add(detailLevel, imageRect);
            }

            lock (ScaledImagesAtZoomLevels)
            {
                var scaledImage = this._image.Clone();
                scaledImage.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Sampler = new BicubicResampler(),
                    Size = new Size(imageRect.Width, imageRect.Height)
                }));

                ScaledImagesAtZoomLevels.Add(detailLevel, scaledImage);
            }


            base.CreateTilesForDetailLevel(detailLevel);
        }

        protected override void CreateIndividualTile(int z, int x, int y)
        {

            //now we find the tile rectangle at this zoom level
            int tilePixelX, tilePixelY;
            TileSystem.TileXYToPixelXY(x, y, out tilePixelX, out tilePixelY);
            Rectangle tileRect = new Rectangle(tilePixelX, tilePixelY, 256, 256);

            if (ImageRectanglesAtZoomLevels[z].IntersectsWith(tileRect))
            {
                using (var image = new Image<Rgba32>(256, 256))
                {
                    Point drawPoint = new Point(ImageRectanglesAtZoomLevels[z].X - tilePixelX, ImageRectanglesAtZoomLevels[z].Y - tilePixelY);
                    image.Mutate(x => x.DrawImage(ScaledImagesAtZoomLevels[z], drawPoint, 0.7f));

                    using (FileStream outputStream = new FileStream(GenerateTileSavePath(z, x, y), FileMode.Create, FileAccess.Write))
                    {
                        image.SaveAsPng(outputStream);
                    }
                }
            }
        }

        public void Dispose()
        {
            _image.Dispose();
            ScaledImagesAtZoomLevels.Clear();
            ImageRectanglesAtZoomLevels.Clear();
        }
    }
}
