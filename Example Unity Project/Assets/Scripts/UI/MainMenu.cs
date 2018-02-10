using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

	public void QuitGame() {
        Debug.Log("Hit quit btn");
        Application.Quit();
    }

    public void LoadScene(string sceneName) {
        Debug.Log("Hit free-play btn");
        SceneManager.LoadScene(sceneName);
    }
}
