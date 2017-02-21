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


        public EntityData(EntityManager entityManager)
        {
            playerData = new List<PlayerData>();
            foreach (Entity entity in entityManager.players.Values)
            {
                if (entity.type == EntityManager.EntityType.Player)
                    playerData.Add( new PlayerData(entity) );
            }

            // ... 

        }

        public void SetData(EntityManager entityManager)
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