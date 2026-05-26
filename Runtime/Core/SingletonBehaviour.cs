using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Core
{
    /// <summary>
    /// MonoBehaviour 向けのシングルトン基底クラス。
    /// 継承することで <see cref="Instance"/> から唯一のインスタンスにアクセスできます。
    /// 重複インスタンスは自動的に破棄され、シーンをまたいで保持されます。
    /// </summary>
    /// <typeparam name="T">シングルトンとして管理する MonoBehaviour の型</typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
        /// <summary>
        /// 唯一のインスタンスを取得
        /// </summary>
		public static T Instance { get; private set; }

        /// <summary>
        /// シングルトンの初期化処理。
        /// オーバーライドする場合は必ず <c>base.Awake()</c> を呼び出してください。
        /// </summary>
		protected void Awake()
		{
			if (Instance == null)
			{
				Instance = this as T;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
			}
		}
	}
}