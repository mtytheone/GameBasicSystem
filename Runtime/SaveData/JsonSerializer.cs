using HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData
{
    public sealed class JsonSerializer<TObjectType> : ISerializer<TObjectType>
    {
        public string Serialize(TObjectType obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public TObjectType Deserialize(string json)
        {
            return JsonUtility.FromJson<TObjectType>(json);
        }
    }
}
