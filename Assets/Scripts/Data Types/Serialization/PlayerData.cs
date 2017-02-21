using Assets.Scripts.Components;

namespace Assets.Scripts.Data_Types.Serialization
{
    [System.Serializable]
    class PlayerData
    {
        public string name;
        public int coordX;
        public int coordY;
        public int sightRange;

        public PlayerData (Entity entity)
        {
            Player player = entity.GetComponent<Player>();
            if (player)
            {
                name = player.name;
                sightRange = player.sightRange;
                coordX = entity.coordinates.World.X;
                coordY = entity.coordinates.World.Y;
            }
        }

        public void SetData(Entity entity)
        {
            Player player = entity.GetComponent<Player>();
            if (player)
            {
                entity.coordinates = new Coordinates(coordX, coordY);
                player.sightRange = sightRange;
                player.name = name;
            }
        }

    }
}
