using System;
using System.IO;
using CommandLine;
using SixLabors.ImageSharp;

namespace Mappa
{
    class Program
    {
        class RunParameters
        {
            [Option('w', "west", Required = false, HelpText = "West longitude in decimal degrees")]
            public double? West { get; set; }

            [Option('n', "north", Required = false, HelpText = "North latitude in decimal degrees")]
            public double? North { get; set; }

            [Option('s', "south", Required = false, HelpText = "South latitude in decimal degrees")]
            public double? South { get; set; }

            [Option('e', "east", Required = false, HelpText = "East longitude in decimal degrees")]
            public double? East { get; set; }

            [Option('d', "debugtiles", Required = false, Default = false, HelpText = "If set, Mappa will output tiles even if the the image is not available on that tile, with debug information including the tile coordinates")]
            public bool DebugTiles { get; set; }

            [Option('i',"image", Required = false, HelpText = "The path to the image to tile")]
            public string ImagePath { get; set; }

            [Option('z',"zoom", Required = false, Default = 14, HelpText = "The zoom level to tile to (inclusive)")]
            public int Zoom { get; set; }

            [Option('o', "output", Required = true, HelpText = "The output directory in which to save the files")]
            public string OutputPath { get; set; }

        }


        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RunParameters>(args)
                   .WithParsed<RunParameters>(o =>
                   {
                       Console.WriteLine("Running tiler process");

                       if (o.DebugTiles)
                       {
                           DebugTileGenerator debugTileGenerator = new DebugTileGenerator(o.OutputPath);
                           debugTileGenerator.MaxLevelOfDetail = o.Zoom;
                           debugTileGenerator.OutputSaveMode = OutputSaveMode.Folders;

                           debugTileGenerator.CreateTiles();
                       }

                       if (!string.IsNullOrEmpty(o.ImagePath))
                       {
                           if (o.East == null || o.West == null || o.North == null || o.South == null)
                           {
                               Console.WriteLine("All coordinates must be specified to output an image.");
                               return;
                           }
                           GeoLocation upperRightBounds = GeoLocation.FromDegrees(o.North.Value, o.East.Value);
                           GeoLocation lowerLeftBounds = GeoLocation.FromDegrees(o.South.Value, o.West.Value);

                           Image inputImage = LoadInputImage(o.ImagePath);

                           ImageTileGenerator imageTileGenerator = new ImageTileGenerator(
                               upperRightBounds,
                               lowerLeftBounds,
                               inputImage,
                               o.OutputPath);

                           imageTileGenerator.MaxLevelOfDetail = o.Zoom;
                           imageTileGenerator.OutputSaveMode = OutputSaveMode.Folders;

                           imageTileGenerator.CreateTiles();
                       }


                   });
        }

        private static Image LoadInputImage(string imagePath)
        {

            Image inputImage;
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                inputImage = Image.Load(imagePath);
            }
            return inputImage;

        }

    }
}
