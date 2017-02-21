using Assets.Scripts.ScriptableObjects;
using UnityEngine;


namespace Assets.Scripts.Data_Types.Serialization
{
    [System.Serializable]
    class WorldData
    {
        public int seed;
        public EntityData entityData;

        public WorldData(WorldManager worldManager)
        {
            seed = worldManager.world.seed;

            EntityManager entityManager = worldManager.GetComponent<EntityManager>();
            if (entityManager)
                entityData = new EntityData(entityManager);
        }

        public void SetData(WorldManager worldManager)
        {
            EntityManager entityManager = worldManager.GetComponent<EntityManager>();
            World world = ScriptableObject.CreateInstance<World>();

            if (world && entityManager)
            {
                worldManager.world = world;
                worldManager.world.seed = seed;
                entityData.SetData(entityManager);
            }
        }

    }
}
