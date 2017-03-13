using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using System;

[RequireComponent(typeof(GameManager))]
public class MovementManager : MonoBehaviour
{
    public float movementDelay = 0.0075f;

    public EntityManager entityManager
    {
        get { return GetComponent<EntityManager>(); }
    }

    public TileMapManager tileMapManager
    {
        get { return GetComponent<TileMapManager>(); }
    }

    HashSet<Movement> movements;
    private float timeSinceLastRun = 0;

    /* UNITY MESSAGES */

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        movements = new HashSet<Movement>();
    }


    // Use this for initialization
    void Start()
    {

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
            movement.startPosition = movement.transform.position; //Set starting position to entity's current position
            movement.vector = addVector;
            //movement.speed = speed / movement.vector.magnitude;
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
        if (movement == null) yield break;

        movement.isMoving = true;
        Vector3 endPosition = movement.startPosition + movement.vector;

        float sqrRemainingDistance = (movement.transform.position - endPosition).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            if (movement == null) yield break;
            Vector3 newPosition = Vector3.MoveTowards(movement.transform.position, endPosition, movement.speed * Time.deltaTime);
            movement.rigidbody.MovePosition(newPosition);
            endPosition = CurrentEndPosition(movement);
            sqrRemainingDistance = (movement.transform.position - endPosition).sqrMagnitude;
            yield return null; 
        }

        movement.isMoving = false;
        SnapToGrid(movement);
        
        // Start Debug Code
        if (movement)
        {
            if (movement.transform.position == entityManager.GetPlayer("Player One").transform.position)
            {
                Debug.Log(
                    movement.name + " sharing place with player!" + "\n" +
                    "Player: " + entityManager.GetPlayer("Player One").coordinates + "\n" +
                    movement.name + " " + movement.entity.instanceId + ": " + movement.coordinates + "\n" +
                    "Focus: " + tileMapManager.focus
                    );
            }

            if (movement.entity.coordinates != tileMapManager.GetCoordinates(movement.transform.position) && movement.name != tileMapManager.tileMap.name)
            {
                Debug.Log(
                    "Coordinates and position do not match!" + "\n" +
                    movement.name + movement.entity.instanceId + ": " + movement.coordinates + "\n" +
                    tileMapManager.tileMap.name + ": " + tileMapManager.GetCoordinates(movement.transform.position)
                    );
            }

        }

        // End Debug Code

    }

    private void SnapToGrid(Movement movement)
    {
        if (!movement) return;

        if (movement.GetComponent<Entity>().type == EntityManager.EntityType.Map) return; // tileMapManager will handle map's position.

        if (movement.isMoving) return;
        
        if (tileMapManager)
        {
            Vector3 predictedPosition = tileMapManager.GetScreenPositionAt(movement.coordinates);
            Vector3 truePosition = movement.transform.position;
            if (predictedPosition != truePosition) movement.transform.position = predictedPosition;
        }

    }

    private Vector3 CurrentEndPosition(Movement movement)
    {
        return movement.startPosition + movement.vector;
    }
}
