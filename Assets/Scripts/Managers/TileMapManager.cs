using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(MovementManager))]
    [RequireComponent(typeof(WorldManager))]
    public class TileMapManager : MonoBehaviour
    {
        public TileMap tileMapComponent;
        [Range(0f, 0.1f)]
        public int edgeTileBuffer = 1;
        public float scrollSpeed = 10;
        public float centerPositionX = 0;
        public float centerPositionY = 0;
        public GameObject tileCollection;
         
        public bool IsScrolling
        {
            get
            {
                Movable mapMovable = tileMap.GetComponent<Movable>();
                if (mapMovable) return mapMovable.IsMoving;
                else return false;
            }
        }

        public Coordinates Focus
        {
            get
            {
                if (!tileMap) Start();
                return tileMap.GetComponent<Entity>().Coordinates;
            }
            set
            {
                SetFocus(value);
            }
        }

        private WorldManager worldManager;
        private MovementManager movementManager;
        private EntityManager entityManager;
        private GameManager gameManager;
        private ScreenManager screenManager;
        private IntegerPair viewableTileDimensions;
        private IntegerPair centerIndices;
        private Vector2 centerPosition;
        private TileMap tileMap;

        // Use this for initialization
        void Start()
        {
            viewableTileDimensions = new IntegerPair();
            centerIndices = new IntegerPair();
            centerPosition = new Vector2(centerPositionX, centerPositionY);
            worldManager = GetComponent<WorldManager>();
            movementManager = GetComponent<MovementManager>();
            entityManager = GetComponent<EntityManager>();
            gameManager = GetComponent<GameManager>();
            screenManager = GetComponent<ScreenManager>();
            UpdateDimensions(screenManager.OrthographicSize);
            tileMap = tileMapComponent;
            ShowMap(false);
            tileMap.Position = centerPosition;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /* METHODS */

        internal void ShowMap(bool show)
        {
            if (!tileMap) Start();

            if (show)
            {
                Refresh();
                tileMap.Show = true;
            }
            else
            {
                tileMap.Show = false;
            }
        }

        internal void Refresh()
        {
            Refresh(Vector2.zero);
        }

        /// <summary>
        /// Instances terrain tiles to display, from terrain data in pre-loaded chunks,  based on the camera's focus on the world map
        /// </summary>
        private void Refresh(Vector2 offset)
        {
            GameObject tileInstance;
            Vector2 tilePos = Vector2.zero;
            Vector2 focusPos = screenManager.GetScreenPositionAt(Focus);

            GameObject[,] buffer = new GameObject[viewableTileDimensions.X, viewableTileDimensions.Y];

            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                for (int j = 0; j < buffer.GetLength(1); j++)
                {
                    //The position to place the tile is the lower left corner (0,0) plus the indices in units
                    tilePos = focusPos + offset - new Vector2(viewableTileDimensions.I / 2 - i, viewableTileDimensions.J / 2 - j);

                    tileInstance = Instantiate(worldManager.GetTilePrefab(GetCoordinates(new IntegerPair(i, j))), tilePos, Quaternion.identity, tileCollection.transform);
                    buffer[i, j] = tileInstance;
                }
            }
            DestroyTileArray();
            tileMap.tileArray = buffer;
        }


        /// <summary>
        /// Destroys all terrain tile instances
        /// </summary>
        void DestroyTileArray()
        {
            if (tileMap.tileArray != null)
            {
                for (int x = 0; x < tileMap.tileArray.GetLength(0); x++)
                {
                    for (int y = 0; y < tileMap.tileArray.GetLength(1); y++)
                    {
                        Destroy(tileMap.tileArray[x, y]);
                    }
                }
            }
        }

        /// <summary>
        /// To be called whenever the screen size changes, to recalculate the number of tiles to push to screen
        /// </summary>
        internal void UpdateDimensions(Vector2 orthographicSize)
        {
            //Calculate the number of tiles we'll need to fill the camera view
            //orthoSize is half the camera view, so we subtract half a unit to exclude the center tile.
            //We round up to determine how many full tiles we need on that half of the screen.
            //Double the amount and add one for the center tile, gives the amount to cover the full width/height of the view
            viewableTileDimensions.Y = Mathf.CeilToInt(orthographicSize.y - 0.5f + edgeTileBuffer) * 2 + 1;
            viewableTileDimensions.X = Mathf.CeilToInt(orthographicSize.x - 0.5f + edgeTileBuffer) * 2 + 1;

            // To find the indices of the focused tile we, find the half way point, rounding down since indices start at (0,0)????? 
            centerIndices.Y = Mathf.FloorToInt(viewableTileDimensions.Y / 2);
            centerIndices.X = Mathf.FloorToInt(viewableTileDimensions.X / 2);
        }

        /// <summary>
        /// Returns world map coordinates from given tile position in camera view, with position (0,0) being lower left corner
        /// </summary>
        /// <param name="tileArrayIndices">Tile position in camera view of type Integer Pair</param>
        /// <returns>Location holding world map location</returns>
        internal Coordinates GetCoordinates(IntegerPair tileArrayIndices)
        {
            long x = tileArrayIndices.I - centerIndices.I + Focus.InWorld.X;
            long y = tileArrayIndices.J - centerIndices.J + Focus.InWorld.Y;
            return new Coordinates(x, y);
        }

        internal Coordinates GetCoordinates(Vector2 position)
        {
            long x = Focus.InWorld.X + Mathf.FloorToInt(position.x);
            long y = Focus.InWorld.Y + Mathf.FloorToInt(position.y);
            return new Coordinates(x, y);
        }


        public void Scroll(Vector2 vector)
        {
            Scroll(vector, scrollSpeed);
        }

        public void Scroll(Vector2 vector, float speed)
        {
            if (!IsScrolling)
            {
                SetFocus(Focus.AtVector(vector));
                tileMap.transform.position += new Vector3(vector.x, vector.y, 0); //Access the transform position directly, because we need instant change of position
                Refresh(vector);

                movementManager.Assign(tileMap, -vector, speed);
                ScrollEntities(-vector, speed);
            }
        }

        internal void ScrollEntities(Vector2 vector, float speed)
        {
            foreach (Entity entity in entityManager.mobCollection.Members)
            {
                if (entity) movementManager.Assign(entity, vector, speed);
            }
        }

        /// <summary>
        /// Changes the focus of the center of the view on to a different map location. If new chunks needed to be loaded, it will call for it.
        /// </summary>
        /// <param name="coordinates"></param>
        public void SetFocus(Coordinates coordinates)
        {
            // If new coordinates are outside current chunk, or no chunks have been loaded, load chunks at new coordinates
            if (coordinates.InChunks.X != Focus.InChunks.X || coordinates.InChunks.Y != Focus.InChunks.Y || !worldManager.HasLoadedChunks)
            {
                worldManager.LoadChunksAt(coordinates);
            }

            Entity mapEntity = tileMap.GetComponent<Entity>();

            mapEntity.SetLocation(worldManager.GetLoadedChunk(coordinates), new IntegerPair(coordinates.InChunks.I, coordinates.InChunks.J));
         }

    }

}