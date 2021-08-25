using System;
using System.IO;
using System.Threading.Tasks;

namespace Mappa
{
    public abstract class TileGenerator
    {
        public int MaxLevelOfDetail { get; set; } = 6; //default

        public string OutputDirectory { get; private set; }

        public TileGenerator(string outputPath)
        {
            this.OutputDirectory = outputPath;
        }


        public virtual void CreateTiles()
        {
            for (int i = 1; i <= this.MaxLevelOfDetail; i++)
            {
                Console.WriteLine($" [{this.GetType().Name}] Creating tiles for detail level {i}");
                CreateTilesForDetailLevel(i);
            }
        }

        protected virtual void CreateTilesForDetailLevel(int detailLevel)
        {
            int startTileX = 0;
            int startTileY = 0;
            int endTileX = TileSystem.NumberOfTiles(detailLevel) - 1;
            int endTileY = TileSystem.NumberOfTiles(detailLevel) - 1;


            CreateTilesForDetailLevel(detailLevel, startTileX, endTileX, startTileY, endTileY);

        }

        protected void CreateTilesForDetailLevel(int detailLevel, int startTileX, int endTileX, int startTileY, int endTileY)
        {
            Console.WriteLine($"Generating {(endTileX - startTileX) * (endTileY - startTileY)} tiles for detail level {detailLevel}");

            Parallel.For(startTileX, endTileX + 1 /* +1 for parallel inclusion silliness*/, tileX =>
            {
                //Parallel.For(startTileY, endTileY + 1 /* +1 for parallel silliness*/, tileY => { 
                for (int tileY = startTileY; tileY <= endTileY; tileY++)
                {
                    if (IsTileValid(detailLevel, tileX, tileY))
                    {
                        //Log.Debug($"generating tile x{tileX} y{tileY}");
                        CreateIndividualTile(detailLevel, tileX, tileY);
                        //Log.Debug($"generated tile x{tileX} y{tileY}");
                    }
                }
                //});
            });
        }

        protected abstract void CreateIndividualTile(int z, int x, int y);

        protected string GenerateTileSavePath(int detailLevel, int tileX, int tileY)
        {
            return Path.Combine(this.OutputDirectory, $"{TileSystem.TileXYToQuadKey(tileX, tileY, detailLevel)}.png");
        }


        public virtual bool IsTileValid(int z, int x, int y)
        {
            return true;
        }

        protected virtual bool IsTileAlreadyCreated(string tileSavePath)
        {
            return File.Exists(tileSavePath);
        }
    }
}
