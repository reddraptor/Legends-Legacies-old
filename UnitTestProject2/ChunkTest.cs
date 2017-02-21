using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChunkTest
{
    [TestClass]
    public class ChunkTest
    {
        GameWorld gameWorld = new GameWorld();

        [TestMethod]
        public void ChunkTest1()
        {
            gameWorld.Start();
            
            for (int i = 0; i < gameWorld.LoadedChunkWidth; i++)
            {
                for (int j = 0; j < gameWorld.LoadedChunkWidth; j++)
                {
                    Location.Chunk_Location chunkLocation = gameWorld.GetChunkLocationAtIndices(new IntegerPair(i, j));
                    Console.WriteLine("Chunk {0}, {1}",
                        chunkLocation.I,
                        chunkLocation.J);
                    WriteTiles(chunkLocation);
                    Console.WriteLine();
                }                 
            }
        }

        void WriteTiles(Location.Chunk_Location chunkLocation)
        { 
            Location.World_Location worldLocation = Location.ChunkToWorld(chunkLocation);
                   
            for (int x = 0; x < Chunk.TileWidth; x++)
            {
                for (int y = 0; y < Chunk.TileWidth; y++)
                {
                    Console.Write("Tile {0}, {1} - Type: {2} ;", x, y,
                        gameWorld.GetTerrainType(new Location.World_Location(worldLocation.X + x, worldLocation.Y + y)));
                }
            }
        }
    }
}
