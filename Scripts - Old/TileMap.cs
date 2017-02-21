using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Mobile))]
    public class TileMap : MonoBehaviour
    {
        /* PUBLIC FIELDS */

        [Range(0f, 0.1f)]
        public float orthographicChangeAllowance = 0.001f;
        public int edgeTileBuffer = 1;
        public float scrollSpeed = 10;

        /* PRIVATE FIELDS */

        GameController gameCtrler;
        GameObject[,] tileArray;
        Vector2 lastOrthoSize;
        IntegerPair viewableTileDimensions;
        Mobile mobileScript;

        // focusLocation is the world map location, that the camera has focus on. focusIndices contains it's indices for the tilemap that is shown on screen, with 0,0
        // being the lower left most tile.
        // Both should represent the tile at the exact screen center
        Coordinates focusLoc;
        IntegerPair focusIndices;

        Coordinates lowerLeft;
        Coordinates upperRight;

        /* PROPERTIES */
        public GameWorld GameWorld
        {
            get
            {
                return gameCtrler.gameWorld;
            }
        }

        public GameController gameController
        {
            get
            {
                return gameCtrler;
            }
            set
            {
                gameCtrler = value;
            }
        }

        public Coordinates focusLocation
        {
            get
            {
                return focusLoc;
            }
        }

        public Mobile Mobile
        {
            get { return mobileScript; }
        }

        /* MONOBEHAVIOR METHODS */
        void Awake()
        {
            lastOrthoSize = new Vector2();
            viewableTileDimensions = new IntegerPair();
            focusIndices = new IntegerPair();
            mobileScript = gameObject.GetComponent<Mobile>();
        }

        void Start()
        {
            UpdateDimensions();
            ChangeFocus(GameWorld.SpawnLocation);
            CreateTileMap();
        }


        void Update()
        {
            if ((Mathf.Abs(lastOrthoSize.y - gameCtrler.OrthographicSize.y) >= orthographicChangeAllowance)
                || Mathf.Abs(lastOrthoSize.x - gameCtrler.OrthographicSize.x) >= orthographicChangeAllowance)
            {
                DestroyTileMap();
                UpdateDimensions();
                CreateTileMap();
            }
        }

        /* METHODS */

        /// <summary>
        /// Creates a tilemap to display in camera view, from terrain in pre loaded chunks,  based on the camera's focus on the world map
        /// </summary>
        public void CreateTileMap()
        {
            GameObject tileInstance;
            Vector3 position = Vector3.zero;

            tileArray = new GameObject[viewableTileDimensions.x, viewableTileDimensions.y];


            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    //The position to place the tile is the lower left corner (0,0) plus the indices in units
                    position.x = this.transform.position.x - viewableTileDimensions.i / 2 + i;
                    position.y = this.transform.position.x - viewableTileDimensions.j / 2 + j;

                    tileInstance = Instantiate(GameWorld.GetTerrainTile(GetCoordinatesAtIndices(new IntegerPair(i, j))), position, Quaternion.identity) as GameObject;
                    tileInstance.transform.SetParent(this.transform);
                    tileArray[i, j] = tileInstance;
                }
            }
        }

        /// <summary>
        /// Clear tile map from view
        /// </summary>
        void DestroyTileMap()
        {
            for (int x = 0; x < tileArray.GetLength(0); x++)
            {
                for (int y = 0; y < tileArray.GetLength(1); y++)
                {
                    Destroy(tileArray[x, y]);
                }
            }
        }

        /// <summary>
        /// To be called whenever the screen size changes, to recalculate the number of tiles to push to screen
        /// </summary>
        void UpdateDimensions()
        {

            lastOrthoSize = gameCtrler.OrthographicSize;

            //Calculate the number of tiles we'll need to fill the camera view
            //orthoSize is half the camera view, so we subtract half a unit to exclude the center tile.
            //We round up to determine how many full tiles we need on that half of the screen.
            //Double the amount and add one for the center tile, gives the amount to cover the full width/height of the view
            viewableTileDimensions.y = Mathf.CeilToInt(lastOrthoSize.y - 0.5f + edgeTileBuffer) * 2 + 1;
            viewableTileDimensions.x = Mathf.CeilToInt(lastOrthoSize.x - 0.5f + edgeTileBuffer) * 2 + 1;

            // To find the indices of the focused tile we, find the half way point, rounding down since indices start at (0,0)????? 
            focusIndices.y = Mathf.FloorToInt(viewableTileDimensions.y / 2);
            focusIndices.x = Mathf.FloorToInt(viewableTileDimensions.x / 2);
        }

        /// <summary>
        /// Returns world map coordinates from given tile position in camera view, with position (0,0) being lower left corner
        /// </summary>
        /// <param name="indices">Tile position in camera view of type Integer Pair</param>
        /// <returns>Location holding world map location</returns>
        public Coordinates GetCoordinatesAtIndices(IntegerPair indices)
        {
            Coordinates.World_Coordinates worldLocation = new Coordinates.World_Coordinates();

            worldLocation.X = indices.i - focusIndices.i + focusLoc.World.X;
            worldLocation.Y = indices.j - focusIndices.j + focusLoc.World.Y;

            return new Coordinates(worldLocation);
        }

        public IEnumerator Scroll(int horizontal, int vertical)
        {
            return Scroll(horizontal, vertical, scrollSpeed);
        }

        public IEnumerator Scroll(int horizontal, int vertical, float speed)
        {
            if (!mobileScript.isMoving)
            {
                mobileScript.defaultSpeed = speed;

                ChangeFocus(new Coordinates(focusLoc.World.X + horizontal, focusLoc.World.Y + vertical));

                yield return mobileScript.Move(-horizontal, -vertical);

                DestroyTileMap();

                transform.position = new Vector3(0, 0, -10);
                CreateTileMap();
            }
        }


        /// <summary>
        /// Changes the focus of the center of the view on to a different map location. If new chunks needed to be loaded, it will call for it.
        /// </summary>
        /// <param name="location"></param>
        public void ChangeFocus(Coordinates location)
        {
            // define the bounds of the map that will be in the view
            lowerLeft = new Coordinates(location.World.X - (viewableTileDimensions.x - 1) / 2, (location.World.Y - (viewableTileDimensions.y - 1) / 2));
            upperRight = new Coordinates(location.World.X + (viewableTileDimensions.x - 1) / 2, (location.World.Y + (viewableTileDimensions.x - 1) / 2));

            // if any part of the view is outside current loaded chunk distance load new chunks
            if (!GameWorld.InLoadedChunks(lowerLeft, upperRight))
            {
                GameWorld.LoadChunksAt(location);
            }
            focusLoc = location;
            Debug.Log("Focus Location: " + focusLoc.World.X + " " + focusLoc.World.Y);
        }
    }

}