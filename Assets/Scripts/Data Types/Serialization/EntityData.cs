﻿using Assets.Scripts.Managers;
using Assets.Scripts.Components;
using System.Collections.Generic;

namespace Assets.Scripts.Data_Types.Serialization
{
    [System.Serializable]
    class EntityData
    {
        public List<PlayerData> playerData;
        // public List<OtherEntityData> otherEntityData;
        // ...


        public EntityData(EntitiesManager entityManager)
        {
            playerData = new List<PlayerData>();
            foreach (Entity entity in entityManager.playerCollection.Members)
            {
                if (entity.Type == "Player")
                    playerData.Add( new PlayerData(entity) );
            }

            // ... 

        }

        public void SetData(EntitiesManager entityManager)
        {
            foreach (PlayerData data in playerData)
            {
                Entity entity = entityManager.Spawn("Player", new Coordinates(data.coordX, data.coordY));
                if (entity)
                    data.SetData(entity);
            }

            // ...
        }
    }
}
