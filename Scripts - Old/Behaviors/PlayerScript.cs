using UnityEngine;
using System;
using Assets.Scripts.Serialization;


namespace Assets.Scripts.Behaviors
{
    public class PlayerScript : BehaviorScript//, Serialization.IHasSerializable<Player>
    {
        /* PUBLIC FIELDS */
        public string playerId = "Player One";

        /* PRIVATE FIELDS */

        //public new PlayerSerializable Serializable
        //{
        //    get { return new PlayerSerializable(this); }
        //    set { value.SetValuesIn(this); }
        //}

        /* SCRIPTABLEOBJECT METHODS */

        private void OnEnable()
        {

        }


        /* METHODS */



        ///* SERIALIZABLE DATA CLASS */
        //[Serializable]
        //public class PlayerSerializable : BehaviorSerializable
        //{
        //    string SPlayerId;

        //    /// <summary>
        //    /// Constructor packages data from script into a new serializable object
        //    /// </summary>
        //    /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
        //    public PlayerSerializable(PlayerScript playerScript) : base(playerScript)
        //    {
        //        SPlayerId = playerScript.playerId;
        //    }

        //    /// <summary>
        //    /// Takes the values from the serializable object and sets the properties of a script.
        //    /// </summary>
        //    /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
        //    public override void SetValuesIn(BehaviourScript behaviorScript)
        //    {
        //        if (behaviorScript.GetType() != typeof(PlayerScript))
        //        {

        //        }
        //        base.SetValuesIn(behaviorScript);
        //        ((PlayerScript)behaviorScript).playerId = SPlayerId;
        //    }
        //}


    }

}