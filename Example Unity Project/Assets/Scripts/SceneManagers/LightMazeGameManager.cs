using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LightMazeGameManager : MonoBehaviour {

    private string SceneSelect = "MainMenu";

	void Start () {
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(SceneSelect);
		}
	}
}
