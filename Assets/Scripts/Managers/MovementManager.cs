using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using System;

[RequireComponent(typeof(GameManager))]
public class MovementManager : MonoBehaviour
{
    HashSet<Movement> movements;
    

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
        DoMoves();
    }

    /* METHODS */
    public void Add(Movement movement, int horizontal, int vertical, float speed)
    {
        if (movement.isMoving) return;

        if (movements.Contains(movement))
        {
            float horizontalVector = movement.horizontal * movement.speed + horizontal * speed;
            float verticalVector = movement.vertical * movement.speed + vertical * speed;
            movement.speed = Mathf.Sqrt(Mathf.Pow(horizontalVector, 2) + Mathf.Pow(verticalVector, 2));
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

        if (movement.isMoving) yield break; //If mobile already in motion, ignore move request
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

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(movement.transform.position, end, movement.speed * Time.deltaTime);
            movement.Rigidbody.MovePosition(newPosition);
            sqrRemainingDistance = (movement.transform.position - end).sqrMagnitude;
            yield return null;
        }

        movement.isMoving = false;
        FixPosition(movement);
    }

    private void FixPosition(Movement movement)
    {
        if (movement.isMoving) return;

        if (GetComponent<TileMapManager>())
        {
            Vector3 predictedPosition = GetComponent<TileMapManager>().GetScreenPositionAt(movement.coordinates);
            Vector3 truePosition = movement.transform.position;
            if (predictedPosition != truePosition) movement.transform.position = predictedPosition;
        }

    }

}
