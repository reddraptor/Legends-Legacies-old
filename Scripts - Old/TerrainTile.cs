using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

//[CreateAssetMenu(menuName = "Legend Legacy/Terrain Tile")]
public class TerrainTile : MonoBehaviour
{

    public float speedModifier;

    Location location;

    public Location Location
    {
        get { return location; }
        set { if (value.TerrainTile == this) location = value; }
    }

}