using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "LegendsLegacies/World")]
    public class World : ScriptableObject
    {
        /* EDITABLE FIELDS */

        public int seed = 07271975;
        public MapGenData mapGenerationData;
        public int loadedChunkDistance = 3;
        public long playerSpawnLocationX = 0, playerSpawnLocationY = 0;

        /* PRIVATE FIELDS */

        LoadedChunks _loadedChunks;

        /* PROPERTIES */

        public int loadedChunkWidth
        {
            get { return loadedChunkDistance * 2 + 1; }
        }

        public Coordinates playerSpawnCoordinates
        {
            get { return new Coordinates(playerSpawnLocationX, playerSpawnLocationY); }
            set { playerSpawnLocationX = value.inWorld.x; playerSpawnLocationY = value.inWorld.y; }
        }

        public LoadedChunks loadedChunks
        {
            get { return _loadedChunks; }
        }

        /* UNITY MESSAGES */

        private void OnEnable()
        {
            //masterLocationList = LocationList.Master;
            _loadedChunks = new LoadedChunks(this);
        }
    }

}