using UnityEngine;
using Assets.Scripts;

namespace Assets.Scripts.Behaviors
{
    [RequireComponent(typeof(GameController))]

    public class BehaviorController: MonoBehaviour
    {
        public BehaviorScript[] behaviorScripts;

        public void SetNewBehavior(Entity entity, int index)
        {
            entity.behaviorScript = Instantiate(behaviorScripts[index]);
            entity.behaviorIndex = index;
        }
    }
}
