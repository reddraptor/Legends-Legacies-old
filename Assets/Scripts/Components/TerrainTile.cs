using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    public class TerrainTile : Entity
    {
        public enum TerrainType {Land, Water, Air };
        public float speedModifier = 1;
        public TerrainType terrainType = TerrainType.Land;

        private void Awake()
        {
            tag = "Terrain Tile";
        }

    }
}