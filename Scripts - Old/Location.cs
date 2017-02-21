using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.OldScripts
{
    public class Location //: Serialization.IHasSerializable<Location>
    {
        /**** FIELDS ****/
        
        Coordinates coord;
        Entity entity;
        TerrainTile terrainTile;
        //Prop _prop;
        //Doorway _doorway;

        /**** PROPERTIES ****/

        public Coordinates Coordinates
        {
            get { return coord; }
        }

        public Entity Entity
        {
            get { return entity; }
            set
            {
                entity = value;
                if (value != null) entity.Location = this;
            }
        }

        public TerrainTile TerrainTile
        {
            get {
                    // implement if terrain_tile == null, return chunkmap terrain_tile if loaded
                    return terrainTile;
                }
            set
            {
                terrainTile = value;
                if (value != null) terrainTile.Location = this;
            }
        }

        public bool AllPropertiesNull
        {
            get
            {
                return (
                    entity == null &&
                    terrainTile == null
                    );
            }
        }

        //public Serialization.ASerializable<Location> Serializable
        //{
        //    get { return new LocationSerializable(this); }
        //    set { value.SetValuesIn(this); }
        //}

        /**** CONSTRUCTORS ****/

        public Location(Coordinates coordinates) 
        {
            coord = coordinates;
        }

        //public Location(LocationSerializable locationS)
        //{
        //    Serializable = locationS;
        //}

        /**** METHODS ****/


        /**** NESTED CLASSES ****/

        //[System.Serializable]
        //public class LocationSerializable : Serialization.ASerializable<Location>
        //{
        //    int x;
        //    int y;
        //    Mobile.MobileSerializable mobileS;

        //    public LocationSerializable(Location location) : base(location)
        //    {
        //        x = location.Coordinates.World.X;
        //        y = location.Coordinates.World.Y;
        //        if (location.Mobile != null)
        //            mobileS = (Mobile.MobileSerializable)location.Mobile.Serializable;
        //    }

        //    //public LocationSerializable(SerializationInfo info, StreamingContext context) : base(info, context)
        //    //{

        //    //}

        //    public override void SetValuesIn(Location location)
        //    {
        //        location.coord = new Coordinates(x, y);
        //        //if (mobileS.GetType() == typeof(Player.PlayerSerializable))
        //        //    location.Mobile.Serializable = (Player.PlayerSerializable)mobileS;
        //        if (mobileS != null)
        //        {
                   
        //        }
                        
        //    }

        //    //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //    //{
         
        //    //}
        //}
    }
}
