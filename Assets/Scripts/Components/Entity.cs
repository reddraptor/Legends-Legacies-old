﻿using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Managers;
using EntityType = EntityManager.EntityType;

namespace Assets.Scripts.Components
{
    public class Entity : MonoBehaviour
    {
        /* INSPECTOR FIELDS */
        public EntityManager entityManager;

        /* PUBLIC FIELDS */
        public Coordinates coordinates = new Coordinates(0, 0);
        [HideInInspector] public bool placed = false;
        

        /* PRIVATE FIELDS */

        EntityType entityType = EntityType.Undefined;


        //Coordinates coords = new Coordinates(0, 0);


        /* PROPERTIES */

        public EntityType type
        {
            get { return entityType; }
            set
            {
                if (entityType == EntityType.Undefined)
                    entityType = value;
            }
        }
        
        //public Coordinates coordinates
        //{
        //    get { return coords; }
        //    set
        //    {
        //        if (entityType == EntityType.Player)
        //        {
        //            entityManager.players.Remove(coords);
        //            entityManager.players[value] = this;
        //            coords = value;
        //        }
        //    }
        //}

        public bool IsCentered
        {
            get
            {
                if (transform.position == Vector3.zero) return true;
                else return false;
            }
        }

        /* UNITY MESSAGES */
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}