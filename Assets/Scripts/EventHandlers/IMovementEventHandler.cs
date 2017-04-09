using UnityEngine.EventSystems;
using Assets.Scripts.Components;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.EventHandlers
{
    public interface IMovementEventHandler : IEventSystemHandler
    {
        void OnMovementCreate(Movement movement);

        void OnMovementStart(Movement movement);

        void OnMovementEnd(Movement movement);

        void OnMovementChange(Movement movement);
    }

}