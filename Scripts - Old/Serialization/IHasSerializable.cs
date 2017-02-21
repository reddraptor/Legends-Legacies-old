namespace Assets.Scripts.Serialization
{
    /// <summary>
    /// Scripts that have data to be serialized for storage (in a world save, for example) implement this interface.
    /// <c>IHasSerializable</c> is an generic interface the script implements directly using itself as type, and <c></c>ASerialable</c>
    /// is a generic abstract class that must be implemented by a nested class with in the script using the script as type.
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
    /// <typeparam name="THasSerializable">A curiously reaccurring template pattern. Scripts implement <c>IHasSerializable</c> for their own type</typeparam>

    public interface IHasSerializable<THasSerializable> where THasSerializable : IHasSerializable<THasSerializable>
    {
        /// <summary>
        /// Use this property to access an object of readily serializable data for this script. This can 
        /// be provided directly to serialization methods like BinaryFormatter.Serialize. Also, setting this
        /// property to a serializable of the same type, modifies the data of the script from the
        /// serializable's values.
        /// </summary>

        ASerializableData<THasSerializable> Serializable { get; set; }
    }
}
