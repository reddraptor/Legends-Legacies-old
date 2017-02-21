using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Serialization
{
    public class EntitySerializer : ISerializer<Entity>
    {
        GameController gameController;

        public EntitySerializer(GameController gameController)
        {
            this.gameController = gameController;
        }

        public ASerializableData<Entity> GetSerializableData(Entity entity)
        {
            return new EntitySerializableData(entity, gameController);
        }

        public void SetDeserializedData(Entity entity, ASerializableData<Entity> serializableData)
        {
            serializableData.SetDataIn(entity);
        }
    }
}
