using System;
using Assets.Scripts.Behaviors;

namespace Assets.Scripts.Serialization
{
    class PlayerSerializer : ISerializer<PlayerScript>
    {
        public ASerializableData<PlayerScript> GetSerializableData(PlayerScript player)
        {
            return new PlayerSerializableData(player);
        }

        public void SetDeserializedData(PlayerScript player, ASerializableData<PlayerScript> serializableData)
        {
            serializableData.SetDataIn(player);
        }
    }
}
