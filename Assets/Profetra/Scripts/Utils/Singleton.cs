using UnityEngine;

namespace Profetra
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static object _lock = new object();
		private static T _instance;
		public static T Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = FindObjectOfType<T>();
					}

					return _instance;
				}
			}
		}

		protected virtual void Awake()
		{
			if (Instance != this)
			{
				Destroy(gameObject);
			}
		}
		protected virtual void OnDestroy()
		{
			if (Instance == this)
			{
				_instance = null;
			}
		}
	}
}
