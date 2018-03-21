﻿using UnityEngine;
 
// Be aware this will not prevent a non singleton constructor such as
// `T myT = new T();` To prevent that, add `protected T () {}` to your singleton
// class. Create as MonoBehavior to support Coroutines.
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	private static T _instance;

	private static object _lock = new object();

	public static T Instance {
		get {
			if (applicationIsQuitting) {
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
					"' already destroyed on application quit." +
					" Won't create again - returning null.");
				return null;
			}

			lock (_lock) {
				if (_instance == null) {
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1) {
						Debug.LogError("[Singleton] Something went really " +
							"wrong - there should never be more than 1 " +
							"singleton! Reopening the scene might fix it.");
						return _instance;
					}

					if (_instance == null) {
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) " + typeof(T).ToString();

						DontDestroyOnLoad(singleton);

						Debug.Log("[Singleton] An instance of " + typeof(T) +
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
					}
					else {
						Debug.Log("[Singleton] Using instance already " +
							"created: " + _instance.gameObject.name);
					}
				}

				return _instance;
			}
		}
	}

	private static bool applicationIsQuitting = false;
	// When Unity quits, it destroys objects in a random order. In principle, a 
	// Singleton is only destroyed when application quits. If any script calls
	// the Instance after it has been destroyed, it will create a buggy ghost
	// object that will stay in the Editor scene even after it has stopping
	// playing the Application. Very bad! So, this was made to be sure we're not
	// creating a buggy ghost object.
	public void OnDestroy() {
		applicationIsQuitting = true;
	}
}
