namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    public interface ISerializer<ObjectType>
    {
        public string Serialize(ObjectType obj);
        public ObjectType Deserialize(string json);
    }
}
