using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Components
{
    public class Client : MonoBehaviour
    {
        public EntityMember controlledEntity;

        public bool IsMoving
        {
            get
            {
                if (controlledEntity)
                {
                    Movable movable = controlledEntity.GetComponent<Movable>();
                    if (movable) return movable.IsMoving;
                }

                return false;
            }
        }

        public float MovementSpeed
        {
            get
            {
                if (controlledEntity)
                {
                    Movable movable = controlledEntity.GetComponent<Movable>();
                    if (movable) return movable.speed;
                }

                return 0f;
            }
        }
            
    }

}