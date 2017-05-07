using UnityEngine.EventSystems;
using Assets.Scripts.Components;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.EventHandlers
{
    public interface IMovementEventHandler : IEventSystemHandler
    {
        void OnMovementSet(Movable movable);

        void OnMovementStart(Movable movable);

        void OnMovementEnd(Movable movable);
    }

}