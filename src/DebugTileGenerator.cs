using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappa
{
    public class DebugTileGenerator : TileGenerator
    {
        private Font _font;
        private Color _color;
        public DebugTileGenerator(string outputPath) : base(outputPath)
        {
            string executionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string executionDirectory = System.IO.Path.GetDirectoryName(executionPath);
            string fontPath = System.IO.Path.Combine(executionDirectory, "Roboto-Regular.ttf");
            FontCollection collection = new FontCollection();
            collection.Install(fontPath);

            _font = collection.Families.First().CreateFont(16.0f);

            _color = Color.Teal;
        }

        protected override void CreateIndividualTile(int z, int x, int y)
        {
            using (var image = new Image<Rgba32>(256, 256))
            {
                string tileText = $"z:{z} x:{x} y:{y}";
                image.Mutate(x => x.DrawText(tileText, _font, Color.Teal, new PointF(5, 5)));

                IPen pen = Pens.Solid(_color, 3.0f);
                Rectangle borderPath = new Rectangle(0, 0, 256, 256);
                image.Mutate(x => x.Draw(pen, borderPath));

                using (FileStream outputStream = new FileStream(GenerateTileSavePath(z, x, y), FileMode.Create, FileAccess.Write))
                {
                    image.SaveAsPng(outputStream);
                }
            }
        }
    }
}
