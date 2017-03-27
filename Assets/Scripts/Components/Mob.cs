using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Attributes))]

    class Mob : Entity
    {


        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            tag = "Mob";
        }

    }
}
