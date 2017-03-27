using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "LegendsLegacies/World")]
    public class World : ScriptableObject
    {
        public int seed = 07271975;
        public MapGenData mapGenerationData;
        public int loadedChunkDistance = 3;
        public long playerSpawnLocationX = 0, playerSpawnLocationY = 0;

        public int LoadedChunkWidth
        {
            get { return loadedChunkDistance * 2 + 1; }
        }
        public Coordinates PlayerSpawnCoordinates
        {
            get { return new Coordinates(playerSpawnLocationX, playerSpawnLocationY); }
            set { playerSpawnLocationX = value.InWorld.X; playerSpawnLocationY = value.InWorld.Y; }
        }
        public LoadedChunks LoadedChunks
        {
            get { return loadedChunks; }
        }
        
        private LoadedChunks loadedChunks;
        
        private void OnEnable()
        {
            loadedChunks = new LoadedChunks(this);
        }
    }

}