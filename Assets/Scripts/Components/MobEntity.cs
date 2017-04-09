using UnityEngine;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Movable))]
    [RequireComponent(typeof(Attributes))]

    class MobEntity : EntityMember
    {
        internal Attributes attributes;

        // Awake is called when the script instance is being loaded
        protected override void Start()
        {
            base.Start();
            tag = "Mob";
            attributes = GetComponent<Attributes>();
        }

    }
}
