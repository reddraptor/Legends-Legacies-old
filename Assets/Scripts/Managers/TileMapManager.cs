using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;

namespace Assets.Scripts.Managers
{
    [RequireComponent(typeof(MovementManager))]
    [RequireComponent(typeof(WorldManager))]
    public class TileMapManager : Manager
    {
        public TileMap tileMapComponent;
        [Range(0f, 0.1f)]
        public int edgeTileBuffer = 1;
        public float scrollSpeed = 10;
        public float centerPositionX = 0;
        public float centerPositionY = 0;
        public GameObject tileCollection;
         
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

        /// <summary>
        /// Returns world map coordinates from given tile position in the current tile array, with position (0,0) being lower left corner
        /// </summary>
        /// <param name="tileArrayIndices">Tile position in tile array of type IntegerPair</param>
        /// <returns>Coordinates holding world map location</returns>
        public Coordinates GetCoordinates(IntegerPair tileArrayIndices)
        {
            long x = tileArrayIndices.I - centerIndices.I + Focus.InWorld.X;
            long y = tileArrayIndices.J - centerIndices.J + Focus.InWorld.Y;
            return new Coordinates(x, y);
        }

        public Coordinates GetCoordinates(Vector2 position)
        {
            long x = Focus.InWorld.X + Mathf.FloorToInt(position.x);
            long y = Focus.InWorld.Y + Mathf.FloorToInt(position.y);
            return new Coordinates(x, y);
        }

        /// <summary>
        /// Changes the focus of the center of the tile array to a different map location. If new chunks needed to be loaded, it will call for it.
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
        /// To be called whenever the screen size changes, to recalculate the number of tiles to be loaded
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

        private IntegerPair viewableTileDimensions;
        private IntegerPair centerIndices;
        private Vector2 centerPosition;
        private TileMap tileMap;

        protected override void Start()
        {
            base.Start();

            viewableTileDimensions = new IntegerPair();
            centerIndices = new IntegerPair();
            centerPosition = new Vector2(centerPositionX, centerPositionY);
            UpdateDimensions(screenManager.OrthographicSize);
            tileMap = tileMapComponent;
            ShowMap(false);
            tileMap.Position = centerPosition;
        }

        /// <summary>
        /// Instances terrain tiles to display, from terrain data in pre-loaded chunks, based on the tilemap's focus coordinates on the world map.
        /// </summary>
        /// <param name="offset">Can instance tiles offset from the focus coordinates by the given offset vector. Vector2.zero would be no offset. Probably not required
        /// in the current tile map implementation.</param>
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
        private void DestroyTileArray()
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



    }

}