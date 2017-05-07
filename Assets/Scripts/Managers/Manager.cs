using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Managers
{
    public class Manager : MonoBehaviour
    {
        protected MenuManager menuManager;
        protected InputManager inputManager;
        protected MovementManager movementManager;
        protected BehaviorsManager behaviorsManager;
        protected EntitiesManager entitiesManager;
        protected ScreenManager screenManager;
        protected WorldManager worldManager;
        protected GameManager gameManager;
        protected PersistanceManager persistanceManager;
        protected TileMapManager tileMapManager;
        protected PhysicsManager physicsManager;

        // Use this for initialization
        protected virtual void Start()
        {
            menuManager = GetComponent<MenuManager>();
            inputManager = GetComponent<InputManager>();
            movementManager = GetComponent<MovementManager>();
            behaviorsManager = GetComponent<BehaviorsManager>();
            entitiesManager = GetComponent<EntitiesManager>();
            screenManager = GetComponent<ScreenManager>();
            worldManager = GetComponent<WorldManager>();
            gameManager = GetComponent<GameManager>();
            persistanceManager = GetComponent<PersistanceManager>();
            tileMapManager = GetComponent<TileMapManager>();
            physicsManager = GetComponent<PhysicsManager>();
        }
    }

}