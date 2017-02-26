using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using System;

[RequireComponent(typeof(GameManager))]
public class MovementManager : MonoBehaviour
{
    HashSet<Movement> movements;
    EntityManager entityManager;
    
    /* UNITY MESSAGES */

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        movements = new HashSet<Movement>();
        entityManager = GetComponent<EntityManager>();
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DoMoves();
    }

    /* METHODS */
    public void Add(Movement movement, int horizontal, int vertical, float speed)
    {
        if (movements.Contains(movement))
        {
            float horizontalSpeed = movement.horizontal * movement.speed + horizontal * speed;
            float verticalSpeed = movement.vertical * movement.speed + vertical * speed;
            movement.speed = Mathf.Sqrt(Mathf.Pow(horizontalSpeed, 2) + Mathf.Pow(verticalSpeed, 2));
            movement.horizontal += horizontal; 
            movement.vertical += vertical;
        }       
        else
        {
            movement.horizontal = horizontal; movement.vertical = vertical;
            movement.speed = speed / Mathf.Sqrt(Mathf.Pow(horizontal, 2) + Mathf.Pow(vertical, 2));
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
        movement.isMoving = true;
        Vector3 end = movement.transform.position;

        if (movement.horizontal < 0)
            end.x -= 1.0f;
        else if (movement.horizontal > 0)
            end.x += 1.0f;

        if (movement.vertical < 0)
            end.y -= 1.0f;
        else if (movement.vertical > 0)
            end.y += 1.0f;

        float sqrRemainingDistance = (movement.transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(movement.transform.position, end, movement.speed * Time.deltaTime);
            movement.rigidbody.MovePosition(newPosition);
            sqrRemainingDistance = (movement.transform.position - end).sqrMagnitude;
            yield return null;
        }
        CorrectPosition(movement);
        movement.isMoving = false;
    }

    internal void MoveEntities(int horizontal, int vertical, float speed)
    {
        foreach (Entity entity in entityManager.mobCollection)
        {
            Add(entity.GetComponent<Movement>(), horizontal, vertical, speed);
        }
    }

    private void CorrectPosition(Movement movement)
    {
        if (movement.isMoving) return;

        if (movement.GetComponent<Entity>().type == EntityManager.EntityType.Map) return; // tileMapManager will correct position.

        TileMapManager tileMapManager = GetComponent<TileMapManager>();

        if (tileMapManager)
        {
            Vector3 predictedPosition = tileMapManager.GetScreenPositionAt(movement.coordinates);
            Vector3 truePosition = movement.transform.position;
            if (predictedPosition != truePosition) movement.transform.position = predictedPosition;
        }

    }



}
