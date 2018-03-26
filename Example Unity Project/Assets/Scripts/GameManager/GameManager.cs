using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager<P> : MonoBehaviour where P : Player
{

    private static GameManager<P> instance;
    public static GameManager<P> Instance { get { return instance; } }

    private const string GameSelect = "GameSelect";

    protected GlobalControls globalControls;
    protected P[] players;

    protected void Awake()
    {
        // GameManager is a singleton, load only one per scene
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        globalControls = InputManager.Instance.GlobalControls();
    }

    protected void Start()
    {
        FetchPlayersFromScene();
    }

    protected void Update()
    {
        DoInput();
    }

    protected void FetchPlayersFromScene()
    {
        players = FindObjectsOfType(typeof(P)) as P[];
    }

    protected void DoInput()
    {
        if (globalControls.GetExitKeyDown())
        {
            SceneManager.LoadSceneAsync(GameSelect);
        }
    }

    protected IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync(GameSelect);
    }

}
