using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(GameManager))]
    public class MovementManager : MonoBehaviour
    {
        public float movementDelay = 0f;

        private EntityManager entityManager;
        private TileMapManager tileMapManager;
        private ScreenManager screenManager;

        HashSet<Movement> movements;
        private float timeSinceLastRun = 0;

        /* UNITY MESSAGES */

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            movements = new HashSet<Movement>();
            entityManager = GetComponent<EntityManager>();
            tileMapManager = GetComponent<TileMapManager>();
            screenManager = GetComponent<ScreenManager>();
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastRun += Time.deltaTime;
            if (timeSinceLastRun > movementDelay)
            {
                DoMoves();
                timeSinceLastRun = 0;
            }
        }

        /* METHODS */
        public void Add(Movement movement, Vector2 addVector, float speed)
        {
            // If movement already in movement set, update movement vector
            if (movements.Contains(movement))
            {
                Vector2 currentSpeedVector = movement.vector.normalized * movement.speed;
                Vector2 addSpeedVector = addVector.normalized * speed;

                movement.speed = (currentSpeedVector + addSpeedVector).magnitude;
                movement.vector += addVector;
            }
            // Else add new movement to movement set
            else
            {
                movement.startPosition = movement.rigidbody.position; //Set starting position to entity's current predicted position
                movement.vector = addVector;
                movement.speed = speed;
                movements.Add(movement);
            }
        }

        void DoMoves()
        {
            foreach (Movement movement in movements)
            {
                StartCoroutine(Move(movement));
            }
            movements.Clear();

        }

        IEnumerator Move(Movement movement)
        {
            if (!movement) yield break;

            movement.isActive = true;
            Vector2 endPosition = CurrentEndPosition(movement);

            float sqrRemainingDistance = (movement.rigidbody.position - endPosition).sqrMagnitude;

            while (sqrRemainingDistance > float.Epsilon)
            {
                if (movement == null) yield break;
                Vector3 newPosition = Vector3.MoveTowards(movement.rigidbody.position, endPosition, movement.speed * Time.deltaTime);
                movement.rigidbody.MovePosition(newPosition);
                endPosition = CurrentEndPosition(movement);
                sqrRemainingDistance = (movement.rigidbody.position - endPosition).sqrMagnitude;
                yield return null;
            }

            Reset(movement);
            SnapToGrid(movement);
            movement.isActive = false;

            // Start Debug Code
            if (movement)
            {
                if (movement.rigidbody.position == entityManager.GetPlayer("Player One").Position && movement.Entity.Type != "Map")
                {
                    Debug.Log(
                        movement.Entity + " sharing place with player!" + "\n" +
                        "Player: " + entityManager.GetPlayer("Player One").Coordinates + "\n" +
                        movement.Entity + ": " + movement.Coordinates + "\n" +
                        "Focus: " + tileMapManager.Focus
                        );
                }

                if (movement.Entity.Coordinates != tileMapManager.GetCoordinates(movement.rigidbody.position) && movement.name != tileMapManager.tileMapComponent.name)
                {
                    Debug.Log(
                        "Coordinates and position do not match!" + "\n" +
                        movement.Entity + ": " + movement.Coordinates + "\n" +
                        tileMapManager.tileMapComponent + ": " + tileMapManager.GetCoordinates(movement.rigidbody.position)
                        );
                }

            }

            // End Debug Code

        }

        private void Reset(Movement movement)
        {
            movement.vector = Vector2.zero;
            movement.speed = 0;
            movement.startPosition = movement.rigidbody.position;
        }

        private void SnapToGrid(Movement movement)
        {
            if (!movement) return;

            //if (movement.Entity.Type == "Map") return; // tileMapManager will handle map's position.

           // if (movement.isMoving) return;

            if (tileMapManager)
            { 
                Vector3 predictedPosition = screenManager.GetScreenPositionAt(movement.Coordinates);
                Vector3 truePosition = movement.rigidbody.position;
                if (predictedPosition != truePosition) movement.transform.position = predictedPosition;
            }

        }

        private Vector3 CurrentEndPosition(Movement movement)
        {
            return movement.startPosition + movement.vector;
        }
    }

}