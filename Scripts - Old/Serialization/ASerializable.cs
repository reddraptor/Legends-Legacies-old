using System;

namespace Assets.Scripts.Serialization
{
    /// <summary>
    /// Scripts that implement <c>IHasSerializable</c> need to contain a class implementing <c>ASerializable</c> to package it's data for it's serializable property.
    /// <c>IHasSerializable</c> is an generic interface the script implements directly using itself as type, and <c>ASerialable</c>
    /// is a generic abstract class that must be implememnted by a nested class with in the script using the script as type.
    /// 
    /// Following is suggested usage in your script:
    /// <code>
    ///
    /// [System.Serializable]
    /// public Serialization.ASerializable<THasSerializable> serializable
    /// {
    ///         get { return new Serializable(this); }
    ///         set { value.SetValuesIn(this); }
    /// }
    ///
    /// public class Serializable : Serialization.ASerializable<THasSeriazable>
    /// {
    ///     // serializable properties here
    ///
    ///     public Serializable(THasSerialzable hasSerializable) : base(location)
    ///     {
    ///         // assign serializable property values from hasSerializable to Serializable's properties here
    ///     }
    ///
    ///     public override void SetValuesIn(THasSerialzable hasSerializable)
    ///     {
    ///        // assign Serializable's property values to hadSerialable's properties here
    ///     }
    ///}
    /// </code>
    /// </summary>
    /// <typeparam name="TObject">A script that implements IHasSerizable</typeparam>

    [Serializable]
    public abstract class ASerializableData<TObject> //where THasSerializable : IHasSerializable<THasSerializable>
    {
        /// <summary>
        /// Constructor packages data from script into a new serializable object
        /// </summary>
        /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
        protected ASerializableData(TObject theObject) { }

        public abstract void SetDataIn(TObject theObject);
    }

    //public abstract class ASerializable<THasSerializable> where THasSerializable : IHasSerializable<THasSerializable>
    //{
    //    /// <summary>
    //    /// Constructor packages data from script into a new serializable object
    //    /// </summary>
    //    /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
    //    protected ASerializable(THasSerializable hasSerializable) { }

    //    /// <summary>
    //    /// Takes the values from the serializable object and sets the properties of a script.
    //    /// </summary>
    //    /// <param name="hasSerializable">A script that implements this class' IHasSerializable type.</param>
    //    public abstract void SetValuesIn(THasSerializable hasSerializable);
    //}
}
