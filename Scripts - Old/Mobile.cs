using UnityEngine;
using Assets.Scripts;
using System.Collections;
using System.Runtime.Serialization;
using System;

namespace Assets.Scripts
{

    public class Mobile : MonoBehaviour //, Serialization.IHasSerializable<Mobile>
    {
        //public GameController gameController;
        public float defaultSpeed = 2;

        //Location location;
        protected Rigidbody2D rb;
        bool moving = false;
        //int prefabIndex = 0;
        //SpawnController spawnController;

        public bool IsCentered
        {
            get
            {
                if (transform.position == Vector3.zero) return true;
                else return false;
            }
        }



        //public Coordinates Coordinates
        //{
        //    get { return location.Coordinates; }
        //}

        //public Location Location
        //{
        //    get { return location; }
        //    set { if (value.Mobile == this) location = value; }
        //}

        //public Serialization.ASerializable<Mobile> Serializable
        //{
        //    get { return new MobileSerializable(this); }
        //    set { value.SetValuesIn(this); }
        //}

        //public int PrefabIndex
        //{
        //    get { return prefabIndex; }
        //    set
        //    {
        //        if (prefabIndex == 0)
        //        {
        //            prefabIndex = value;
        //        }
        //        else
        //        {
        //            Debug.Log(this.ToString() + "There was an attempt to reinitialize PrefabIndex.");
        //        }
        //   }
        //}

        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            //spawnController = gameController.gameObject.GetComponent<SpawnController>();
        }

        public bool isMoving
        {
            get
            {
                return moving;
            }
        }

        public IEnumerator Move(int horizontal, int vertical, float speed)
        {

            if (moving) yield break; //If mobile already in motion, ignore move request
            moving = true;
            Vector3 end = transform.position;

            if (horizontal < 0)
                end.x -= 1.0f;
            else if (horizontal > 0)
                end.x += 1.0f;

            if (vertical < 0)
                end.y -= 1.0f;
            else if (vertical > 0)
                end.y += 1.0f;

            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            while (sqrRemainingDistance > float.Epsilon)
            {
                // Vector3 newPosition = Vector3.MoveTowards(rb.position, end, speed * Time.deltaTime);
                Vector3 newPosition = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
                rb.MovePosition(newPosition);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }

            moving = false;
        }

        public IEnumerator Move(int horizontal, int vertical)
        {
            return Move(horizontal, vertical, defaultSpeed);
        }

        public void Center()
        {
            transform.position = Vector3.zero;
        }


        //[System.Serializable]
        //public class MobileSerializable : Serialization.ASerializable<Mobile>
        //{
        //    protected int prefabIndex;

        //    //public int PrefabIndex
        //    //{
        //    //    get { return prefabIndex; }
        //    //}

        //    public MobileSerializable(Mobile mobile) : base(mobile)
        //    {
        //        prefabIndex = mobile.PrefabIndex;
        //    }

        //    //public MobileSerializable(SerializationInfo info, StreamingContext context) : base (info, context)
        //    //{

        //    //}

        //    //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //    //{
        //    //    //info.AddValue(id, )
        //    //}

        //    public override void SetValuesIn(Mobile mobile)
        //    {
        //        mobile.PrefabIndex = prefabIndex;
        //    }
        //}
    }

}