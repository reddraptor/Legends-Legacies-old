using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Movement))]
    public class TileMap : MonoBehaviour
    {
        /* EDITOR FIELDS */


        /* PRIVATE FIELDS */
        public GameObject[,] tileArray;

        /* PROPERTIES */
        public Movement movement
        {
            get
            {
                return GetComponent<Movement>();
            }
        }

        /* UNITY MESSAGES */

        // Use this for initialization
        void Awake()
        {
            tileArray = new GameObject[0, 0];
            GetComponent<Entity>().type = EntityManager.EntityType.Map;
        }

    }

}