using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "LegendsLegacies/Map Generation Data")]
public class MapGenData : ScriptableObject
{
    /* EDITOR FIELDS */
    public int maxElevation = 100;              // Each chunk has an random elevation value. This is the maximum value that can be assigned
    public int mountainElevationMinimum = 50;   // Mountains will be generated above this elevation
    public int valleyElevationMaximum = 90;     // The maximum elevation that rivers will be generated at
    public int waterBodyElevationMax = 50;      // The maximum elevation bodies of water will be generated at

    public GameObject waterTilePrefab;
    public GameObject stoneTilePrefab;
    public GameObject grassTilePrefab;
}
