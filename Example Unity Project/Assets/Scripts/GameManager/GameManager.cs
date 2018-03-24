using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager<P> : MonoBehaviour where P : Player
{

    private static GameManager<P> _instance;
    public static GameManager<P> Instance { get { return _instance; } }

    private const string _gameSelect = "GameSelect";

    protected GlobalControls _globalControls;
    protected P[] _players;

    protected void Awake()
    {
        // GameManager is a singleton, load only one per scene
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        _globalControls = GameControlsManager.Instance.GlobalControls();
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
        _players = FindObjectsOfType(typeof(P)) as P[];
    }

    protected void DoInput()
    {
        if (_globalControls.GetExitKeyDown())
        {
            SceneManager.LoadSceneAsync(_gameSelect);
        }
    }

    protected IEnumerator EndGameAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadSceneAsync(_gameSelect);
    }

}
