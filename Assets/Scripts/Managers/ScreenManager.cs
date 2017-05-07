using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;

namespace Assets.Scripts.Managers
{
    public class ScreenManager : Manager
    {
        public float orthographicChangeAllowance = 0.001f;
        public Camera mainCameraComponent;

        public Vector2 OrthographicSize
        {
            get
            {
                return new Vector2()
                {
                    y = mainCamera.orthographicSize,
                    x = mainCamera.orthographicSize * mainCamera.aspect
                };
            }
        }

        public Vector2 GetScreenPositionAt(Coordinates coordinates)
        {
            return new Vector2()
            {
                x = (coordinates.InWorld.X - cameraEntity.Coordinates.InWorld.X),
                y = (coordinates.InWorld.Y - cameraEntity.Coordinates.InWorld.Y)
            };
        }

        /// <summary>
        /// Moves camera focus to coordinates and updates map rendering
        /// </summary>
        /// <param name="coordinates">coordinates</param>
        internal void CenterOn(Coordinates coordinates)
        {
            mainCamera.transform.position = GetScreenPositionAt(coordinates);
            tileMapManager.SetFocus(coordinates);
            tileMapManager.Refresh();
        }

        internal void SetScroll(Vector2 destinationVector, float speed)
        {
            if (!cameraMovable.IsMoving)
            {
                movementManager.Set(cameraMovable, destinationVector, speed);
            }
        }

        private Camera mainCamera;
        private CameraEntity cameraEntity;
        private Movable cameraMovable;
        private Vector2 lastOrthoSize = Vector2.zero;

        protected override void Start()
        {
            base.Start();

            mainCamera = mainCameraComponent;
            cameraEntity = mainCamera.GetComponent<CameraEntity>();
            cameraMovable = mainCamera.GetComponent<Movable>();

            lastOrthoSize = OrthographicSize;            
            tileMapManager.UpdateDimensions(OrthographicSize);
        }

        private void Update()
        {
            if ((Mathf.Abs(lastOrthoSize.y - OrthographicSize.y) >= orthographicChangeAllowance)
                 || Mathf.Abs(lastOrthoSize.x - OrthographicSize.x) >= orthographicChangeAllowance)
            {
                tileMapManager.UpdateDimensions(OrthographicSize);
                lastOrthoSize = OrthographicSize;
            }
        }
    }
}