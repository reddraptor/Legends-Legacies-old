using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Data_Types.Serialization
{
    [System.Serializable]
    class WorldData
    {
        public int seed;
        public EntityData entityData;

        public WorldData(WorldManager worldManager)
        {
            seed = worldManager.Seed;

            EntitiesManager entityManager = worldManager.GetComponent<EntitiesManager>();
            if (entityManager)
                entityData = new EntityData(entityManager);
        }

        public void SetData(WorldManager worldManager)
        {
            EntitiesManager entityManager = worldManager.GetComponent<EntitiesManager>();
            World world = ScriptableObject.CreateInstance<World>();

            if (world && entityManager)
            {
                worldManager.world = world;
                worldManager.Seed = seed;
                entityData.SetData(entityManager);
            }
        }

    }
}
