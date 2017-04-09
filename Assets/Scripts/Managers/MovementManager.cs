using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.Data_Types;
using Assets.Scripts.EventHandlers;
using System.Collections;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(GameManager))]
    public class MovementManager : MonoBehaviour
    {
        private EntityManager entityManager;
        private TileMapManager tileMapManager;
        private ScreenManager screenManager;

        Queue<Movement> movementQueue = new Queue<Movement>();

        /* UNITY MESSAGES */

        private void Start()
        {
            entityManager = GetComponent<EntityManager>();
            tileMapManager = GetComponent<TileMapManager>();
            screenManager = GetComponent<ScreenManager>();
        }

        void Update()
        {
            DoMoves();
        }

        /* METHODS */
        public void Assign(Entity entity, Vector2 direction, float speed)
        {
            if (entity)
            {
                Movable movable = entity.GetComponent<Movable>();
                if (movable && movable.rigidbody)
                {
                    if (movementQueue.Contains(movable.nextMovement))
                    {
                        movable.nextMovement.Add(direction, speed);
                        ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementChange(movable.nextMovement));
                    }

                    // Else add new movement to movement queue
                    else
                    {
                        Vector2 startPosition;
                        if (movable.moving)
                            startPosition = movable.currentMovement.EndPosition;
                        else
                            startPosition = movable.rigidbody.position;

                        Movement nextMovement = new Movement(movable, startPosition, direction, speed);
                        movable.nextMovement = nextMovement;
                        movementQueue.Enqueue(nextMovement);
                        ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementCreate(nextMovement));
                    }
                }
                //else Debug.Log("Attempt to add movement to entity with no moveable component.");
            }
        }

        void DoMoves()
        {
            int queueSize = movementQueue.Count;
            int queuePosition = 0;

            while (queuePosition < queueSize)
            {
                Movement nextMovement = movementQueue.Dequeue();

                if (nextMovement.movable.IsMoving)
                    movementQueue.Enqueue(nextMovement);
                else
                {
                    nextMovement.movable.currentMovement = nextMovement;
                    StartCoroutine(Move(nextMovement));
                }

                queuePosition++;
            }
        }

        IEnumerator Move(Movement movement)
        {
            if (movement == null || !movement.movable || movement.direction == Vector2.zero || !movement.movable.rigidbody) yield break;
            
            ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementStart(movement));
            movement.movable.moving = true;

            Vector2 endPosition = movement.startPosition + movement.direction;

            float sqrRemainingDistance = (movement.movable.rigidbody.position - endPosition).sqrMagnitude;

            while (sqrRemainingDistance > float.Epsilon)
            {
                if (movement.movable == null) break;

                Vector3 newPosition = Vector3.MoveTowards(movement.movable.rigidbody.position, endPosition, movement.speed * Time.deltaTime);
                movement.movable.rigidbody.MovePosition(newPosition);

                yield return null;

                sqrRemainingDistance = (movement.movable.rigidbody.position - endPosition).sqrMagnitude;
            }

            ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementEnd(movement));
            movement.movable.moving = false;

            //Reset(movement);
            //SnapToGrid(movement);
            //movement.isActive = false;

            // Start Debug Code
            //if (movement.movable)
            //{
            //    if (movement.movable.rigidbody.position == GetComponent<GameManager>().client.controlledEntity.Position && movement.movable.entity.Type != "Map")
            //    {
            //        Debug.Log(
            //            movement.movable.entity + " sharing place with player!" + "\n" +
            //            "Player: " + GetComponent<GameManager>().client.controlledEntity.Coordinates + "\n" +
            //            movement.movable.entity + ": " + movement.movable.Coordinates + "\n" +
            //            "Focus: " + tileMapManager.Focus
            //            );
            //    }

            //    if (movement.movable.entity.Coordinates != tileMapManager.GetCoordinates(movement.movable.rigidbody.position) && movement.movable.name != tileMapManager.tileMapComponent.name)
            //    {
            //        Debug.Log(
            //            "Coordinates and position do not match!" + "\n" +
            //            movement.movable.entity + ": " + movement.movable.Coordinates + "\n" +
            //            tileMapManager.tileMapComponent + ": " + tileMapManager.GetCoordinates(movement.movable.rigidbody.position)
            //            );
            //    }

            //}

            // End Debug Code

        }

        //private void Reset(Moveable movement)
        //{
        //    movement.vector = Vector2.zero;
        //    movement.speed = 0;
        //    movement.startPosition = movement.Rigidbody.position;
        //}

        private void SnapToGrid(Movable movement)
        {
            if (!movement) return;

            //if (movement.Entity.Type == "Map") return; // tileMapManager will handle map's position.

           // if (movement.isMoving) return;

            if (tileMapManager)
            { 
                Vector3 predictedPosition = screenManager.GetScreenPositionAt(movement.Coordinates);
                Vector3 truePosition = movement.rigidbody.position;
                if (predictedPosition != truePosition) movement.rigidbody.position = predictedPosition;
            }

        }

        private Vector3 CurrentEndPosition(Movement movement)
        {
            return movement.startPosition + movement.direction;
        }
    }

}