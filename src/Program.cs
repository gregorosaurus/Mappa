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
            [Option('w', "west", Required = true, HelpText = "West longitude in decimal degrees")]
            public double West { get; set; }

            [Option('n', "north", Required = true, HelpText = "North latitude in decimal degrees")]
            public double North { get; set; }

            [Option('s', "south", Required = true, HelpText = "South latitude in decimal degrees")]
            public double South { get; set; }

            [Option('e', "east", Required = true, HelpText = "East longitude in decimal degrees")]
            public double East { get; set; }

            [Option('i',"image", Required = true, HelpText = "The path to the image to tile")]
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

                       GeoLocation upperRightBounds = GeoLocation.FromDegrees(o.North, o.West);
                       GeoLocation lowerLeftBounds = GeoLocation.FromDegrees(o.South, o.East);

                       Image inputImage = LoadInputImage(o.ImagePath);

                       ImageTileGenerator imageTileGenerator = new ImageTileGenerator(
                            upperRightBounds,
                            lowerLeftBounds,
                            inputImage,
                            o.OutputPath);

                        imageTileGenerator.MaxLevelOfDetail = o.Zoom;

                        imageTileGenerator.CreateTiles();
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
