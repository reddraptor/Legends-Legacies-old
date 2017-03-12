using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    public class TerrainTile : MonoBehaviour
    {
        public enum TerrainType {Land, Water, Air };

        /* EDITOR FIELDS */


        public float speedModifier = 1;
        public TerrainType terrainType = TerrainType.Land;

        /* PROPERTIES */
        public Coordinates coordinates
        {
            get {
                if (GetComponent<Entity>())
                {
                    return GetComponent<Entity>().coordinates;
                }
                else return new Coordinates(0,0);
            }
        }

    }
}