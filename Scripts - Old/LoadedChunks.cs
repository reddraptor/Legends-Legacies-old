using UnityEngine;
using System.Collections;

namespace Assets.OldScripts
{
    public class LoadedChunks
    {
        Chunk[,] chunks;
        Coordinates lowerLeftCorner;   // The location in the lower left corner of this chunk array
        GameWorld gameWorld;

        public LoadedChunks(GameWorld gameWorld)
        {
            this.gameWorld = gameWorld;
            chunks = new Chunk[gameWorld.LoadedChunkWidth, gameWorld.LoadedChunkWidth];
        }

        //public LoadedChunks Init(GameWorld gameWorld)
        //{
        //    this.gameWorld = gameWorld;
        //    chunks = new Chunk[gameWorld.LoadedChunkWidth, gameWorld.LoadedChunkWidth];
        //    //LoadChunksAt(center);

        //    return this;
        //}


        public bool ContainsArea(Coordinates lowerLeft, Coordinates upperRight)
        {
            if (lowerLeft.Chunk.I < lowerLeftCorner.Chunk.I)
                return false;
            else if (lowerLeft.Chunk.J < lowerLeftCorner.Chunk.J)
                return false;
            else if (upperRight.Chunk.I >= lowerLeftCorner.Chunk.I + (gameWorld.LoadedChunkWidth))
                return false;
            else if (upperRight.Chunk.J >= lowerLeftCorner.Chunk.J + (gameWorld.LoadedChunkWidth))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Loads up chunks around a new world location. Currently just generates new chunks. Eventually will load chunks from
        /// procedual generation code with changes recorded in a list of locations.
        /// </summary>
        /// <param name="center"></param>
        public void LoadChunksAt(Coordinates center)
        {
            lowerLeftCorner = new Coordinates(
                center.Chunk.I - gameWorld.LoadedChunkDistance,
                center.Chunk.J - gameWorld.LoadedChunkDistance, 0, 0);

            for (int i = 0; i < gameWorld.LoadedChunkWidth; i++)
            {
                for (int j = 0; j < gameWorld.LoadedChunkWidth; j++)
                {
                    //IntegerPair currentIndices = new IntegerPair(i, j);
                    chunks[i, j] = ScriptableObject.CreateInstance<Chunk>().Init(
                        new Coordinates.Chunk_Coordinates(lowerLeftCorner.Chunk.I + i, lowerLeftCorner.Chunk.J + j, 0, 0),
                        gameWorld.mapGenerator,
                        gameWorld.Seed);

                    // Connect our chunks with references to the adjacent chunks for easy navigation
                    if (i > 0) // Not in the first column
                    {
                        chunks[i, j].south = chunks[i - 1, j]; // chunk in the column to left
                        chunks[i - 1, j].north = chunks[i, j];
                    }
                    if (j > 0) // Not in the bottom row
                    {
                        chunks[i, j].west = chunks[i, j - 1]; // chunk in the row below
                        chunks[i, j - 1].east = chunks[i, j];
                    }
                }
            }
        }

        public GameObject GetTerrainTile(Coordinates location)
        {
            //Location.Chunk_Location chunkLocation = Location.WorldToChunk(worldLocation);

            IntegerPair indices = new IntegerPair(
                location.Chunk.I - lowerLeftCorner.Chunk.I,
                location.Chunk.J - lowerLeftCorner.Chunk.J);

            return chunks[indices.i, indices.j].GetTerrainTile(location);

        }

        public Coordinates GetLocationAtIndices(IntegerPair indices)
        {
            return new Coordinates(lowerLeftCorner.Chunk.I + indices.i, lowerLeftCorner.Chunk.J + indices.j, 0, 0);
        }
    } 
}