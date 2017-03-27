using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;

namespace Assets.Scripts.Managers
{
    public class ScreenManager : MonoBehaviour
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

        private Camera mainCamera;
        private Vector2 lastOrthoSize = Vector2.zero;
        private TileMapManager tileMapManager;
        private EntityManager entityManager;

        private void Start()
        {
            mainCamera = mainCameraComponent;
            lastOrthoSize = OrthographicSize;            
            tileMapManager = GetComponent<TileMapManager>();
            entityManager = GetComponent<EntityManager>();
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

        public Vector2 GetScreenPositionAt(Coordinates coordinates)
        {
            return new Vector2()
            {
                x = (coordinates.InWorld.X - tileMapManager.Focus.InWorld.X),
                y = (coordinates.InWorld.Y - tileMapManager.Focus.InWorld.Y)
            };
        }

        /// <summary>
        /// Moves camera focus with coordinates in the center.
        /// </summary>
        /// <param name="coordinates">coordinates</param>
        internal void CenterOn(Coordinates coordinates)
        {
            tileMapManager.SetFocus(coordinates);

            foreach (Entity entity in entityManager.PlayerCollection)
            {
                entity.transform.position = GetScreenPositionAt(entity.Coordinates);
            }
            foreach (Entity entity in entityManager.MobCollection)
            {
                entity.transform.position = GetScreenPositionAt(entity.Coordinates);
            }

            // ...
            
        }
    }
}