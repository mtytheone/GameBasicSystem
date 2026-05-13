using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Core
{
	public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance { get; private set; }


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