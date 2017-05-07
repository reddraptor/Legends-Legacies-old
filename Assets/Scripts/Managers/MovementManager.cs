using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Assets.Scripts.Components;
using UnityEngine.EventSystems;
using Assets.Scripts.EventHandlers;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(GameManager))]
    public class MovementManager : Manager
    {
        public void Set(Movable movable, Vector2 destinationVector, float speed)
        {
            if (movable && !movableQueue.Contains(movable))
            {
                movable.destinationVector = destinationVector;
                movable.speed = speed;
                movableQueue.Enqueue(movable);
                ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementSet(movable));
             }
        }

        private Queue<Movable> movableQueue = new Queue<Movable>();


        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            DoMoves();
        }


        private void DoMoves()
        {
            int queueSize = movableQueue.Count;
            int queuePosition = 0;

            while (queuePosition < queueSize)
            {
                StartCoroutine(Move(movableQueue.Dequeue()));

                queuePosition++;
            }
        }

        private IEnumerator Move(Movable movable)
        {
            if (movable && movable.RigidBody)
            {
                movable.moving = true;
                ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementStart(movable));

                Vector2 newPosition;
                Vector2 endPosition = movable.RigidBody.position + movable.destinationVector;

                float sqrRemainingDistance = 1.0f;

                while (sqrRemainingDistance > float.Epsilon)
                {
                    if (movable && movable.RigidBody)
                    {
                        newPosition = Vector2.MoveTowards(movable.RigidBody.position, endPosition, movable.speed * Time.deltaTime);
                        movable.RigidBody.MovePosition(newPosition);

                        sqrRemainingDistance = (movable.RigidBody.position - endPosition).sqrMagnitude;

                        yield return null;
                    }
                    else yield break;
                }

                movable.moving = false;
                ExecuteEvents.Execute<IMovementEventHandler>(gameObject, null, (x, y) => x.OnMovementEnd(movable));
                movable.destinationVector = Vector2.zero;
                movable.speed = 0f;
            }
        }
    }
}