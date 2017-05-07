using UnityEngine;
using System.Collections;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(Movable))]
    [RequireComponent(typeof(EntityMember))]
    public class CameraEntity : EntityMember
    {
        protected override void Start()
        {
            base.Start();
            tag = "MainCamera";
        }
    }
    
}