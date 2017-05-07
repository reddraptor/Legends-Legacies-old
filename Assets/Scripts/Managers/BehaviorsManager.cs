using UnityEngine;
using Assets.Scripts.Components;
using Assets.Scripts.EventHandlers;
using Assets.Scripts.Data_Types;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Managers
{
    public class BehaviorsManager : Manager, IMovementEventHandler
    {
        public void OnMovementSet(Movable movable)
        {
            // Do Nothing
            //Debug.Log("Set: " + movable);
        }

        public void OnMovementStart(Movable movable)
        {
            // Do nothing
            // Debug.Log("Started: " + movable);
        }

        public void OnMovementEnd(Movable movable)
        {
            //Debug.Log("Ended: " + movable);
            Behaviors behavior = null;
            if (movable.Entity is EntityMember) behavior = GetBehaviors((EntityMember)movable.Entity);
            if (behavior) behavior.state = Behaviors.State.Idle;
        }

        internal System.Random randomizer = new System.Random();

        internal void RunBehaviors()
        {
            List<Behaviors> behaviorsList = GetBehaviorsList(); //TODO: Optimize by adding and removing behaviors to a static list on spawn and despawn

            // Run behaviors
            foreach (Behaviors behaviors in behaviorsList)
            {
                if (behaviors.attributes && behaviors.attributes.HitPoints < 0 && behaviors.entityMember)
                {
                    entitiesManager.Despawn(behaviors.entityMember);
                }
                else if (behaviors.state == Behaviors.State.Idle && behaviors.idleBehavior)
                {
                    behaviors.idleBehavior.Run(this, behaviors);
                }
            }
        }

        internal bool Move(EntityMember entityMember, Vector2 directionVector)
        {
            if (entitiesManager & entityMember) return entitiesManager.SetMovement(entityMember, directionVector);
            else return false;
        }

        protected override void Start()
        {
            base.Start();
        }

        private List<Behaviors> GetBehaviorsList()
        {
            Behaviors behaviors;
            List<Behaviors> behaviorsList = new List<Behaviors>();

            foreach (EntityMember entityMember in entitiesManager.mobCollection.Members)
            {
                if (entityMember)
                {
                    behaviors = entityMember.GetComponent<Behaviors>();
                    if (behaviors) behaviorsList.Add(behaviors);
                }
            }
            return behaviorsList;
        }


        private Behaviors GetBehaviors(EntityMember entityMember)
        {
            if (entityMember) return entityMember.GetComponent<Behaviors>();
            else return null;
        }

    }
}
