using UnityEngine;
using Assets.Scripts.Components;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Managers
{
    public class PhysicsManager : Manager
    {

        protected override void Start()
        {
            base.Start();
        }

        internal Entity GetTargetRay(Entity caster, Vector2 direction, float distance)
        {
            Collider2D casterCollider = caster.GetComponent<Collider2D>();
            RaycastHit2D[] hits = new RaycastHit2D[1];
            if (casterCollider.Raycast(direction, hits, distance) > 0)
                return hits[0].collider.GetComponent<Entity>();
            else return null;
        }

        internal Entity GetObstacle(Coordinates coordinates)
        {
            Vector2 position = screenManager.GetScreenPositionAt(coordinates);
            Vector2 boxSize = new Vector2(0.9f, 0.9f);

            RaycastHit2D hit = Physics2D.BoxCast(position, boxSize, 0f, Vector2.zero);

            if (hit)
                return hit.collider.GetComponent<Entity>();
            else return null;
        }
    }

}