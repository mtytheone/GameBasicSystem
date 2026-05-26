namespace HatzeLaboratory.GameBasicSystem.Runtime.Core
{
    /// <summary>
    /// MonoBehaviourを持たないクラス向けのシングルトン基底クラス。
    /// 継承することで <see cref="Instance"/> から唯一のインスタンスにアクセスできます。
    /// </summary>
    /// <typeparam name="T">シングルトンとして管理するクラスの型</typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;

        /// <summary>
        /// 唯一のインスタンスを取得します。
        /// 初回アクセス時に自動生成されます。
        /// </summary>
        public static T Instance => _instance ??= new T();
    }
}   
