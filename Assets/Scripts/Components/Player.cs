using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Attributes))]

    public class Player : Entity
    {

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            name = "Player One";
            tag = "Player";
        }
    }

}