using Assets.Scripts.ScriptableObjects;

namespace Assets.Scripts.Data_Types
{
    public class LoadedChunks
    {
        public Coordinates lowerLeftCorner; // The location in the lower left corner of this chunk array

        /* PRIVATE FIELDS */

        Chunk[,] chunks;
    
        /* CONSTRUCTORS */

        public LoadedChunks(World world)
        {
            chunks = new Chunk[world.loadedChunkWidth, world.loadedChunkWidth];
        }

        /* PROPERTIES */

        public Chunk[,] Chunks
        {
            get { return chunks; }
        }
        
    }
}