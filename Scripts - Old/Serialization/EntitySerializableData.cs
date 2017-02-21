

namespace Assets.Scripts.Serialization
{
    [System.Serializable]
    public class EntitySerializableData : ASerializableData<Entity>
    {
        public string name;
        public int prefabIndex;
        public int behaviorIndex;

        GameController gamecontroller;

        public EntitySerializableData(Entity entity, GameController gamecontroller) : base(entity)
        {
            name = entity.name;
            prefabIndex = entity.prefabIndex;
            behaviorIndex = entity.behaviorIndex;
            this.gamecontroller = gamecontroller;
        }

        public override void SetDataIn(Entity entity)
        {
            entity.name = name;
            entity.prefabIndex = prefabIndex;
            gamecontroller.BehaviorController.SetNewBehavior(entity, behaviorIndex);
                
        }
    }
}
