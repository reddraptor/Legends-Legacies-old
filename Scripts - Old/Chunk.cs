using UnityEngine;
using System.Collections;

namespace Assets.OldScripts
{
    public class Chunk : ScriptableObject
    {
        public const int DEFAULT_CHUNK_TILE_WIDTH = 32; // how many tiles fit across a chunk


        GameObject[,] chunkMap;
        public Chunk north = null;
        public Chunk east = null;
        public Chunk south = null;
        public Chunk west = null;

        static int tile_width = DEFAULT_CHUNK_TILE_WIDTH;

        Coordinates.Chunk_Coordinates lowerLeftTile;
        Map_Generator mapGenerator;

        public Chunk Init(Coordinates.Chunk_Coordinates chunkLocation, MapGenerator mapGenerator, int seed)
        {
            this.mapGenerator = mapGenerator;
            lowerLeftTile = chunkLocation;
            chunkMap = mapGenerator.GenerateChunkMap(lowerLeftTile, seed);

            return this;
        }

        public static int tileWidth
        {
            get
            {
                return tile_width;
            }
        }

        //return location of lower left corner tile
        public Coordinates.Chunk_Coordinates location
        {
            get
            {
                return lowerLeftTile;
            }
        }

        /// <summary>
        /// This procedually generates a new chunks based on adjacent generated chunks.. eventually.
        /// </summary>
        //public void Generate()
        //{
        //    chunkMap = mapGenerator.GenerateChunkMap(lowerLeftTile);
        //}

        public GameObject GetTerrainTile(Coordinates location)
        {
            return chunkMap[location.Chunk.X, location.Chunk.Y];
        }
    }

}