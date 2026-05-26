namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    /// <summary>
    /// オブジェクトのシリアライズ・デシリアライズを行うインターフェース。
    /// カスタムシリアライズを実装する場合はこのインターフェースを実装し、<see cref="SaveDataManagerBase{T}"/> に渡してください。
    /// </summary>
    /// <typeparam name="T">シリアライズ対象の型</typeparam>
    public interface ISerializer<T>
    {
        /// <summary>オブジェクトを文字列にシリアライズ</summary>
        /// <param name="obj">シリアライズするオブジェクト</param>
        /// <returns>シリアライズされた文字列</returns>
        public string Serialize(T obj);

        /// <summary>文字列をオブジェクトにデシリアライズ</summary>
        /// <param name="json">デシリアライズする文字列</param>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public T Deserialize(string json);
    }
}
