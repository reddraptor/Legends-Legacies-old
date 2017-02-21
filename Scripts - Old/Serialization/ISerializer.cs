namespace Assets.Scripts.Serialization
{
    public interface ISerializer<TObject>
    {
        ASerializableData<TObject> GetSerializableData(TObject theObject);

        void SetDeserializedData(TObject theObject, ASerializableData<TObject> serializableData);
    }
}
