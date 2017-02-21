using UnityEngine;

namespace Assets.Scripts.Data_Types
{
    public class Chunk
    {
        /* PUBLIC FIELDS */
        public GameObject[,] tileArray;

        public Chunk north = null;
        public Chunk northEast = null;
        public Chunk east = null;
        public Chunk southEast = null;
        public Chunk south = null;
        public Chunk southWest = null;
        public Chunk west = null;
        public Chunk northWest = null;


        /* CONSTRUCTORS */
        public Chunk(GameObject[,] tileArray)
        {
            this.tileArray = tileArray;
        }
    }

}