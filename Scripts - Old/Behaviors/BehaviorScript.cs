using System;
using UnityEngine;
using Assets.Scripts.Serialization;

namespace Assets.Scripts.Behaviors
{
    public class BehaviorScript : ScriptableObject//, IHasSerializable<BehaviourScript>
    {
        /* PRIVATE FIELDS */
        Entity entityScript;

        public Entity Entity
        {
            get { return entityScript; }
            set
            {
                if (entityScript == null) entityScript = value;
                else Debug.Log("Attempt to reinitialize entityScript in Player script.");
            }
        }

        public Mobile Mobile
        {
            get { return entityScript.Mobile; }
            set
            {
                if (entityScript.Mobile == null) entityScript.Mobile = value;
                else Debug.Log("Attempt to reinitialize mobileScript in Player script.");
            }
        }

        //public ASerializableData<BehaviourScript> Serializable
        //{
        //    get { return new BehaviorSerializable(this); }
        //    set { value.SetValuesIn(this); }
        //}

        ///* SERIALIZABLE DATA CLASS */
        //[Serializable]
        //public class BehaviorSerializable : ASerializableData<BehaviourScript> 
        //{
        //    /// <summary>
        //    /// Constructor packages data from script into a new serializable object
        //    /// </summary>
        //    /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
        //    public BehaviorSerializable(BehaviourScript behaviorScript) : base(behaviorScript) { }

        //    /// <summary>
        //    /// Takes the values from the serializable object and sets the properties of a script.
        //    /// </summary>
        //    /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
        //    public override void SetValuesIn(BehaviourScript behaviorScript)
        //    {
        //        if (behaviorScript == null)
        //        {
        //            behaviorScript = CreateInstance<BehaviourScript>();
        //        }
        //    }
        //}

    }
}
