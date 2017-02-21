using UnityEngine;
using Assets.Scripts.Serialization;
using Assets.Scripts.Behaviors;

namespace Assets.Scripts
{

    public class Entity : ScriptableObject//, IHasSerializable<Entity>
    {
        /* PUBLIC FIELDS */
        public string name = "Default";
        public float baseSpeed = 10;
        public float sightRange = 32;
        public int prefabIndex = 0;
        public int behaviorIndex = 0;
        public BehaviorScript behaviorScript;

        /* ENUMERATIONS */
        //public enum BehaviorType
        //{
        //    Player,
        //    NPC,
        //    Monster,
        //    Animal
        //}

        /* PRIVATE FIELDS */
        Location location;
        Mobile mobileScript;
        //BehaviourScript behaviorScript;



        /**** PROPERTIES ****/
        public Mobile Mobile
        {
            get { return mobileScript; }
            set
            {
                if (mobileScript == null) mobileScript = value;
                else Debug.Log("Attempt to reinitialize mobileScript in Entity script.");
            }
        }


        //public ASerializableData<Entity> Serializable
        //{
        //    get { return new EntitySerializable(this); }
        //    set { value.SetValuesIn(this); }
        //}

        public Location Location
        {
            get { return location; }
            set { if (value.Entity == this) location = value; }
        }

        public Coordinates Coordinates
        {
            get { return location.Coordinates; }
        }


        /**** Serializable Data Class ****/
        //[System.Serializable]
        //public class EntitySerializable : ASerializableData<Entity>
        //{
        //    int SPrefabIndex;
        //    //BehaviourScript.BehaviorSerializable SBehavior;
            

        //    public EntitySerializable(Entity entity) : base(entity)
        //    {
        //        SPrefabIndex = entity.prefabIndex;
        //        //SBehavior = (BehaviourScript.BehaviorSerializable)entity.behaviorScript.Serializable;
                
        //    }

        //    public override void SetValuesIn(Entity entity)
        //    {
        //        entity.prefabIndex = SPrefabIndex;
        //        //SBehavior.SetValuesIn(entity.behaviorScript);
        //    }
        //}

    }
}
