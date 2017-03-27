using UnityEngine;
using Assets.Scripts.Components;
using System.Collections.Generic;

namespace Assets.Scripts.Data_Types
{
    public class Chunk
    {
        /* PUBLIC FIELDS */

        public static int chunkTileWidth = 32;

        public GameObject[,] tileArray;

        public Coordinates lowerLeft;

        public Chunk north = null;
        public Chunk northEast = null;
        public Chunk east = null;
        public Chunk southEast = null;
        public Chunk south = null;
        public Chunk southWest = null;
        public Chunk west = null;
        public Chunk northWest = null;

        public HashSet<Entity> entitySet;
        
        /* CONSTRUCTORS */
        public Chunk(GameObject[,] tileArray)
        {
            this.tileArray = tileArray;
            entitySet = new HashSet<Entity>();
        }

        public override string ToString()
        {
            return "(" + lowerLeft.InChunks.X + ", " + lowerLeft.InChunks.Y + ")";
        }

    }

}