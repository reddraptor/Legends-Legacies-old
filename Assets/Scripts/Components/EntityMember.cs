namespace  Assets.Scripts.Components
{

    /// <summary>
    /// An EntityMember represents a single entity gameobject like a player or mob. It can be an item in a EntityCollection becoming a node in a entity hierarchy
    /// </summary>
    public class EntityMember : Entity
    {
        internal Behavior behavior;

        protected override void Start()
        {
            base.Start();
            behavior = GetComponent<Behavior>();
        }
    }
}