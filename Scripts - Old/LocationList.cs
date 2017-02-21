using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.OldScripts
{ 
    public class LocationList //: _Serialization.IHasSerializable<LocationList>
    {
        /**** STATIC FIELDS ****/

        static LocationList masterList = new LocationList();
        static GameWorld gameWorld;

        /**** FIELDS ****/

        Dictionary<Coordinates, Location> dictionary;
        
        /**** PROPERTIES ****/

        //public _Serialization.ASerializable<LocationList> Serializable
        //{
        //    get { return new LocationListSerializable(this); }
        //    set { value.SetValuesIn(this); }
        //}

        static public GameWorld GameWorld
        {
            get
            {
                if (gameWorld == null) throw new InvalidOperationException("LocationList: Attempt to access GameWorld before it is assigned.");
                else return gameWorld;
            }
            set
            {
                if (gameWorld == null) gameWorld = value;
                else throw new InvalidOperationException("LocationList: Attempt to reassign GameWorld.");
            }
        }
        
        /// <summary>
        /// The MasterList contains all locations created by all LocationLists, which are just considered subsets of the MasterList. The MasterList should be the root class containing 
        /// all mobiles, terrain tile changes, prop changes, etc that make up the currently evolving world from it's original generated state. This should be included in the world save.
        /// </summary>
        public static LocationList MasterList
        {
            get{ return masterList; }
        }

        /**** CONSTRUCTORS ****/

        public LocationList()
        {
            dictionary = new Dictionary<Coordinates, Location>();
        }

        //public LocationList(LocationListSerializable locationListS)
        //{
        //    Serializable = locationListS;
        //}

        /**** METHODS ****/

        public bool ChangeAsset<TAsset>(TAsset asset, Coordinates coord) where TAsset : ScriptableObject
        {

            //if (typeof(TAsset) == typeof(Entity))
            //    return ChangeEntity((asset as Entity), GetLocationAt(coord));

            //else if (typeof(TAsset) == typeof(TerrainTile))
            //    return ChangeTerrainTile((asset as TerrainTile), GetLocationAt(coord));

            //else
            //{
            //    Debug.Log("ChangeAsset failed, asset type not implemented: " + typeof(TAsset).ToString());
            //    return false;
            //}
        }

        public bool RemoveAsset<TAsset>(Coordinates coordinates) where TAsset: ScriptableObject
        {
            return ChangeAsset<TAsset>(null, coordinates);
        }
        
        public bool MoveAsset<TAsset>(Coordinates oldCoords, Coordinates newCoords) where TAsset: ScriptableObject
        {
            TAsset asset = GetAsset<TAsset>(oldCoords);
            if (!asset)
            {
                Debug.Log("MoveAsset failed, no asset of that type at coordinates: " + oldCoords.ToString());
                return false;
            }
            if (ChangeAsset<TAsset>(asset, newCoords))
                return RemoveAsset<TAsset>(oldCoords);
            else
            {
                Debug.Log("MoveAsset failed, asset can not be moved to new coordinates: " + newCoords.ToString());
                return false;
            }
        }

        public TAsset GetAsset<TAsset>(Coordinates coordinates) where TAsset: ScriptableObject
        {
            Location location;
            if (!dictionary.TryGetValue(coordinates, out location))
            {
                Debug.Log("GetAsset returned null, coordinates no not exist in location list: " + coordinates.ToString());
                return null;
            }
            if (typeof(TAsset) == typeof(Entity))
                return location.Entity as TAsset;
            else if (typeof(TAsset) == typeof(TerrainTile))
                return location.TerrainTile as TAsset;
            else
            {
                Debug.Log("GetAsset returned null, asset type not implemented: " + typeof(TAsset).ToString());
                return null;
            }
        }
 
        bool ChangeEntity(Entity entity, Location location)
        {
            location.Entity = entity;
            if (location.AllPropertiesNull)
                return RemoveLocationAt(location.Coordinates);
            else return true;
        }

        bool ChangeTerrainTile(TerrainTile terrainTile, Location location)
        {
            location.TerrainTile = terrainTile;

            if (location.AllPropertiesNull)
                return RemoveLocationAt(location.Coordinates);
            else return true;
        }

        Location GetLocationAt(Coordinates coord)
        {
            if (dictionary.Keys.Contains(coord))
            {
                return dictionary[coord];
            }
            else
            { 
                Location newLoc = new Location(coord);
                dictionary[coord] = newLoc;
                masterList.dictionary[coord] = newLoc;
                return newLoc;
            }
        }

        bool RemoveLocationAt(Coordinates coord)
        {
            if (dictionary.Keys.Contains(coord))
            {
                dictionary.Remove(coord);
                masterList.dictionary.Remove(coord);
                return true;
            }
            else
            {
                Debug.Log("Attempt to remove location from list failed, coordinates do not exist in list:" + coord.ToString());
                return false;
            }
        }

        /**** NESTED CLASSES ****/
        //[System.Serializable]
        //public class LocationListSerializable : _Serialization.ASerializable<LocationList>
        //{
        //    List<Location.LocationSerializable> locationListS;

        //    public LocationListSerializable(LocationList list) : base(list)
        //    {
        //        locationListS = new List<Location.LocationSerializable>();

        //        foreach (Location location in list.dictionary.Values)
        //        {
        //            locationListS.Add((Location.LocationSerializable)location.Serializable);
        //        }
        //    }

            //public LocationListSerializable(SerializationInfo info, StreamingContext context) : base(info, context)
            //{

            //}

            //public override void GetObjectData(SerializationInfo info, StreamingContext context)
            //{
            //    throw new NotImplementedException();
            //}

            //public override void SetValuesIn(LocationList list)
            //{
            //    list.dictionary = new Dictionary<Coordinates, Location>();
            //    foreach (Location.LocationSerializable locationS in locationListS)
            //    {
            //        Location location = new Location(locationS);
            //        list.dictionary.Add(location.Coordinates, location);
            //    }
            //}
        //}
    }
}
