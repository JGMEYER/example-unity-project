using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LoadSceneOnClick : MonoBehaviour {

	public void OnClick (string sceneName) {
		EditorSceneManager.LoadSceneAsync(sceneName);
	}

}
