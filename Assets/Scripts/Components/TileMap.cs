using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Components
{
    public class TileMap : EntityCollection
    {
        public GameObject[,] tileArray;

        public new bool Show
        {
            get { return show; }
            set
            {
                SetTilesShow(value);
                show = value;
            }
        }

        private bool show = false;

        private void Awake()
        {
            tag = "Map";
            tileArray = new GameObject[0, 0];

        }

        private void SetTilesShow(bool value)
        {
            if (tileArray != null)
            {
                for (int i = 0; i < tileArray.GetLength(0); i++)
                {
                    for (int j = 0; j < tileArray.GetLength(1); j++)
                    {
                        GameObject gO = tileArray[i, j];
                        if (gO)
                        {
                            TerrainTileEntity tile = gO.GetComponent<TerrainTileEntity>();
                            if (tile) tile.Show = value;
                        }
                    }
                }
            }
        }

    }

}