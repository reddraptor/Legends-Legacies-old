using System;
using Assets.Scripts.Behaviors;


namespace Assets.Scripts.Serialization
{
    [System.Serializable]
    class PlayerSerializableData : ASerializableData<PlayerScript>
    {
        public string playerId;

        public PlayerSerializableData(PlayerScript player) : base(player)
        {
            playerId = player.playerId;
        }

        public override void SetDataIn(PlayerScript player)
        {
            player.playerId = playerId;
        }
    }
}
