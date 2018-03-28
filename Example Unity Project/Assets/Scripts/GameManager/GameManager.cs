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
    protected int numPlayers { get; private set; }

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
        numPlayers = InputManager.Instance.NumPlayersRegistered();
    }

    protected void Start()
    {
        InitializePlayers();
    }

    protected void Update()
    {
        DoInput();
    }

    protected void InitializePlayers()
    {
        List<P> playersInScene = new List<P>(FindObjectsOfType(typeof(P)) as P[]);

        for (int i = playersInScene.Count - 1; i >= 0; i--)
        {
            P player = playersInScene[i];

            if ((int)player.GetPlayerNumber() > numPlayers)
            {
                playersInScene.RemoveAt(i);
                Destroy(player.gameObject);
            }
        }

        players = playersInScene.ToArray();
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
