using Assets.Scripts.Components;

namespace Assets.Scripts.Data_Types.Serialization
{
    [System.Serializable]
    class PlayerData
    {
        public string name;
        public long coordX;
        public long coordY;
        public int sightRange;

        public PlayerData (Entity entity)
        {
            Player player = entity.GetComponent<Player>();
            if (player)
            {
                name = player.name;
                sightRange = player.attributes.sightRange;
                coordX = entity.coordinates.inWorld.x;
                coordY = entity.coordinates.inWorld.y;
            }
        }
         
        public void SetData(Entity entity)
        {
            Player player = entity.GetComponent<Player>();
            if (player)
            {
                entity.coordinates = new Coordinates(coordX, coordY);
                //player.attributes.sightRange = sightRange;
                player.name = name;
            }
        }

    }
}
