using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Managers;

public class InputManager : Manager
{
    /* EDITOR FIELDS */

    [Header("Input Settings")]
    [Range(0.05f, 0.001f)]
    public float mouseScrollEdgePercentage = 0.05f; //Percent of the total screen size at edge where mouse scroll is active.
    public float mouseScrollCenterPercent = 0.1f;
    public GameObject mouseScrollReminder;
    public KeyCode mainMenuKey = KeyCode.Escape;
    public KeyCode mouseScrollKey = KeyCode.LeftControl;
    public float inputRepeatDelay = 0.5f;


    /* PRIVATE FIELDS */
    private Vector2 inputDirection;
    private bool menuKeyUp;
    private bool mouseScrollKeyDown;
    private bool mouseScrollActive = false;
    private bool mouseScrollKeyUp;
    private Vector2 mousePosition;
    private Vector2 mouseScrollDirection;
    private float timeSinceLastInput;
    
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        mouseScrollReminder.GetComponent<Canvas>().enabled = false;
        timeSinceLastInput = inputRepeatDelay;
    }

    // Update is called once per frame
    private void Update()
    {
        timeSinceLastInput += Time.deltaTime;

        if (timeSinceLastInput > inputRepeatDelay)
        {
            timeSinceLastInput = 0;
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            menuKeyUp = Input.GetKeyUp(mainMenuKey);
            mouseScrollKeyDown = Input.GetKeyDown(mouseScrollKey);
            mouseScrollKeyUp = Input.GetKeyUp(mouseScrollKey);
            mousePosition = Input.mousePosition;
            mouseScrollDirection = GetMouseScrollDirection(mousePosition);

            if (menuKeyUp)
            {
                if (menuManager)
                {
                    if (!menuManager.IsAnyOpen)
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

            else if (!gameManager.client.IsMoving && (!menuManager || !menuManager.IsAnyOpen))
            {
                if ((inputDirection.x != 0 || inputDirection.y != 0) && gameManager.client.controlledEntity != null)
                {
                    if (entitiesManager.SetMovement(gameManager.client.controlledEntity, inputDirection))
                    {
                        tileMapManager.SetFocus(gameManager.client.controlledEntity.Coordinates);
                        tileMapManager.Refresh();
                        screenManager.SetScroll(inputDirection, gameManager.client.MovementSpeed);
                    }

                    // START DEBUG CODE
                    if (tileMapManager.Focus != gameManager.client.controlledEntity.Coordinates)
                    {
                        Debug.Log(
                            "Focus and player coordinates do not match. \n" +
                            "Focus: " + tileMapManager.Focus + "\n" +
                            "Player: " + gameManager.client.controlledEntity.Coordinates
                            );
                    }

                    if (true)
                    {
                        Debug.Log(
                    "Map Focus: " + tileMapManager.Focus + "\n" +
                    "Player: " + gameManager.client.controlledEntity.Coordinates);

                    }                // END DEBUG CODE

                    // Run mob behaviors
                    behaviorsManager.RunBehaviors();

                }

                else if (mouseScrollActive)
                {
                    if ((mouseScrollDirection.x != 0 || mouseScrollDirection.y != 0))
                    {
                        // Ensure map doesn't scroll horizontally out of player's sight range.
                        //Coordinates newFocusHorizontal = new Coordinates(tileMapManager.focus.inWorld.x + mouseScrollDirection.x, tileMapManager.focus.inWorld.y);
                        //if (mouseScrollDirection.x != 0
                        //    && gameManager.player.entity.coordinates.Range(newFocusHorizontal) > gameManager.player.attributes.sightRange - worldManager.mapGenerator.chunkTileWidth / 2)
                        //{
                        //    mouseScrollDirection.x = 0;
                        //}

                        //// Ensure map doesn't scroll vertically out of player's sight range.
                        //Coordinates newFocusVertical = new Coordinates(tileMapManager.focus.inWorld.x, tileMapManager.focus.inWorld.y + mouseScrollDirection.y);
                        //if (mouseScrollDirection.y != 0
                        //    && gameManager.player.entity.coordinates.Range(newFocusVertical) > gameManager.player.attributes.sightRange - worldManager.mapGenerator.chunkTileWidth / 2)
                        //{
                        //    mouseScrollDirection.y = 0;
                        //}

                        //// If scrolling out of players sight range both directions then return.
                        //if (mouseScrollDirection.x == 0 && mouseScrollDirection.y == 0) return;

                        //Coordinates newFocusLocation
                        //    = new Coordinates(tileMapManager.focus.inWorld.x + mouseScrollDirection.x, tileMapManager.focus.inWorld.y + mouseScrollDirection.y);

                        //// Ensure the scrolling of both directions would not be out of player's sight. Not sure if this is necessary?!?!
                        //if (gameManager.player.entity.coordinates.Range(newFocusLocation) > gameManager.player.attributes.sightRange - worldManager.mapGenerator.chunkTileWidth / 2) return;

                        //movementManager.Add(gameManager.player.movement, -mouseScrollDirection, tileMapManager.scrollSpeed);
                        ////entityManager.SetMoveAll(-mouseScrollDirection.x, -mouseScrollDirection.y, tileMapManager.scrollSpeed);
                        //tileMapManager.Scroll(mouseScrollDirection.x, mouseScrollDirection.y);
                    }
                }

                else if (!mouseScrollActive)
                {
                    if (gameManager.client.controlledEntity)
                    {
                        if (!gameManager.client.controlledEntity.IsCentered)
                        {
                            screenManager.CenterOn(gameManager.client.controlledEntity.Coordinates);
                        }
                    }
                }
            }
        }
    }

    private Vector2 GetMouseScrollDirection(Vector2 mousePos)
    {
        Vector2 direction = Vector2.zero;
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