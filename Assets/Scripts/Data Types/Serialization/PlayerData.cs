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
                sightRange = player.Attributes.SightRange;
                coordX = entity.Coordinates.InWorld.X;
                coordY = entity.Coordinates.InWorld.Y;
            }
        }
         
        public void SetData(Entity entity)
        {
            Player player = entity.GetComponent<Player>();
            if (player)
            {
               // entity.coordinates = new Coordinates(coordX, coordY);
                //player.attributes.sightRange = sightRange;
                player.name = name;
            }
        }

    }
}
