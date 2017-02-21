using UnityEngine;
using UnityEditor;
using NUnit.Framework;

//public class Chunk_Test {

//    GameWorld gameWorld = ScriptableObject.CreateInstance<GameWorld>();
    

//    [Test]
//    public void ChunkTest1()
//    {
//        for (int i = 0; i < gameWorld.LoadedChunkWidth; i++)
//        {
//            for (int j = 0; j < gameWorld.LoadedChunkWidth; j++)
//            {
//                Coordinates location = gameWorld.GetLocationAtIndices(new IntegerPair(i, j));
//                Debug.Log("Chunk " +
//                    location.Chunk.I + " " +
//                    location.Chunk.J);
//                WriteTiles(location);
//                Debug.Log("\n");
//            }
//        }
//    }

//    void WriteTiles(Coordinates location)
//    {
//        for (int x = 0; x < Chunk.tileWidth; x++)
//        {
//            for (int y = 0; y < Chunk.tileWidth; y++)
//            {
//                Debug.Log("Tile " + x + " " + y + " - Type: " +
//                    gameWorld.GetTerrainTile(new Coordinates(location.World.X + x, location.World.Y + y)));
//            }
//        }
//    }
//}
