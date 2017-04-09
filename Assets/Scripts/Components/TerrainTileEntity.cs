using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    public class TerrainTileEntity : EntityMember
    {
        public enum TerrainType {Land, Water, Air };
        public float speedModifier = 1;
        public TerrainType terrainType = TerrainType.Land;

        protected override void Start()
        {
            base.Start();
            tag = "Terrain Tile";
        }

    }
}