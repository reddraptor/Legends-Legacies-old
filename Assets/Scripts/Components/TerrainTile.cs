using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Entity))]
    class TerrainTile : MonoBehaviour
    {
        /* EDITOR FIELDS */

        public float speedModifier = 1;

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