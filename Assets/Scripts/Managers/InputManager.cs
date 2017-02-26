using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Managers;

public class InputManager : MonoBehaviour
{
    /* EDITOR FIELDS */

    [Header("Input Settings")]
    [Range(0.05f, 0.001f)]
    public float mouseScrollEdgePercentage = 0.05f; //Percent of the total screen size at edge where mouse scroll is active.
    public float mouseScrollCenterPercent = 0.1f;
    public GameObject mouseScrollReminder;

    public KeyCode mainMenuKey = KeyCode.Escape;
    public KeyCode mouseScrollKey = KeyCode.LeftControl;


    /* PRIVATE FIELDS */
    IntegerPair inputDirection;
    bool menuKeyUp;
    bool mouseScrollKeyDown;
    bool mouseScrollActive = false;
    bool mouseScrollKeyUp;
    Vector2 mousePosition;
    IntegerPair mouseScrollDirection;
    GameManager gameManager;
    MenuManager menuManager;
    TileMapManager tileMapManager;
    EntityManager entityManager;
    WorldManager worldManager;
    MovementManager movementManager;
    BehaviorManager behaviorManager;


    // Use this for initialization
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        menuManager = GetComponent<MenuManager>();
        tileMapManager = GetComponent<TileMapManager>();
        entityManager = GetComponent<EntityManager>();
        worldManager = GetComponent<WorldManager>();
        movementManager = GetComponent<MovementManager>();
        behaviorManager = GetComponent<BehaviorManager>();
        mouseScrollReminder.GetComponent<Canvas>().enabled = false;


    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = new IntegerPair((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));
        menuKeyUp = Input.GetKeyUp(mainMenuKey);
        mouseScrollKeyDown = Input.GetKeyDown(mouseScrollKey);
        mouseScrollKeyUp = Input.GetKeyUp(mouseScrollKey);
        mousePosition = Input.mousePosition;
        mouseScrollDirection = GetMouseScrollDirection(mousePosition);

        if (menuKeyUp)
        {
            if (menuManager)
            {
                if (!menuManager.anyOpen)
                    menuManager.Open("Main Menu");
                else
                    menuManager.Return();
            }
        }

        else if (mouseScrollKeyDown)
        {
            if (mouseScrollActive == false) mouseScrollActive = true;
            mouseScrollReminder.GetComponent<Canvas>().enabled = true;
        }

        else if (mouseScrollKeyUp)
        {
            if (mouseScrollActive == true) mouseScrollActive = false;
            mouseScrollReminder.GetComponent<Canvas>().enabled = false;
        }

        else if (!tileMapManager.IsMoving && !menuManager.anyOpen)
        {
            if ((inputDirection.x != 0 || inputDirection.y != 0) && gameManager.player != null)
            {
                if (inputDirection.x < 0)
                {
                    entityManager.Place(gameManager.player.entity, gameManager.player.coordinates.West(1));
                }
                else if (inputDirection.x > 0)
                {
                    entityManager.Place(gameManager.player.entity, gameManager.player.coordinates.East(1));
                }

                if (inputDirection.y < 0)
                {
                    entityManager.Place(gameManager.player.entity, gameManager.player.coordinates.South(1));
                }
                else if (inputDirection.y > 0)
                {
                    entityManager.Place(gameManager.player.entity, gameManager.player.coordinates.North(1));
                }

                if (inputDirection.x != 0 || inputDirection.y != 0)
                {
                    if (!gameManager.player.entity.IsCentered)
                    {
                        entityManager.Center(gameManager.player.entity);
                    }

                    float speed = gameManager.player.speed * worldManager.SpeedModifierAt(gameManager.player.coordinates);
                    tileMapManager.Scroll(inputDirection.x, inputDirection.y, speed);
                    behaviorManager.RunBehaviors();
                }
            }

            else if (mouseScrollActive)
            {
                if ((mouseScrollDirection.x != 0 || mouseScrollDirection.y != 0))
                {
                    // Ensure map doesn't scroll horizontally out of player's sight range.
                    Coordinates newFocusHorizontal = new Coordinates(tileMapManager.focus.World.X + mouseScrollDirection.x, tileMapManager.focus.World.Y);
                    if (mouseScrollDirection.x != 0
                        && gameManager.player.entity.coordinates.Range(newFocusHorizontal) > gameManager.player.sightRange - worldManager.MapGenerator.chunkTileWidth / 2)
                    {
                        mouseScrollDirection.x = 0;
                    }

                    // Ensure map doesn't scroll vertically out of player's sight range.
                    Coordinates newFocusVertical = new Coordinates(tileMapManager.focus.World.X, tileMapManager.focus.World.Y + mouseScrollDirection.y);
                    if (mouseScrollDirection.y != 0
                        && gameManager.player.entity.coordinates.Range(newFocusVertical) > gameManager.player.sightRange - worldManager.MapGenerator.chunkTileWidth / 2)
                    {
                        mouseScrollDirection.y = 0;
                    }

                    // If scrolling out of players sight range both directions then return.
                    if (mouseScrollDirection.x == 0 && mouseScrollDirection.y == 0) return;

                    Coordinates newFocusLocation
                        = new Coordinates(tileMapManager.focus.World.X + mouseScrollDirection.x, tileMapManager.focus.World.Y + mouseScrollDirection.y);

                    // Ensure the scrolling of both directions would not be out of player's sight. Not sure if this is necessary?!?!
                    if (gameManager.player.entity.coordinates.Range(newFocusLocation) > gameManager.player.sightRange - worldManager.MapGenerator.chunkTileWidth / 2) return;

                    movementManager.Add(gameManager.player.movement, -mouseScrollDirection.x, -mouseScrollDirection.y, tileMapManager.scrollSpeed);
                    //entityManager.SetMoveAll(-mouseScrollDirection.x, -mouseScrollDirection.y, tileMapManager.scrollSpeed);
                    tileMapManager.Scroll(mouseScrollDirection.x, mouseScrollDirection.y);
                }
            }

            else if (!mouseScrollActive)
            {
                if (gameManager.player)
                {
                    if (!gameManager.player.entity.IsCentered)
                    {
                        entityManager.Center(gameManager.player.entity);
                    }
                }
            }
        }
    }

    IntegerPair GetMouseScrollDirection(Vector2 mousePos)
    {
        IntegerPair direction = new IntegerPair(0, 0);
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 lowerLeftOfCenter = new Vector2(screenCenter.x - Screen.width * mouseScrollCenterPercent / 2, screenCenter.y - Screen.height * mouseScrollCenterPercent / 2);
        Vector2 upperRightOfCenter = new Vector2(screenCenter.x + Screen.width * mouseScrollCenterPercent / 2, screenCenter.y + Screen.height * mouseScrollCenterPercent / 2);

        // if mouse position is outside screen limits return zero direction
        if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height) return direction;

        // determine directions to scroll
        if (mousePos.x < lowerLeftOfCenter.x)
            direction.x = -1;

        else if (mousePos.x > upperRightOfCenter.x)
            direction.x = 1;

        if (mousePos.y < lowerLeftOfCenter.y)
            direction.y = -1;

        else if (mousePos.y > upperRightOfCenter.y)
            direction.y = 1;

        return direction;
    }

}