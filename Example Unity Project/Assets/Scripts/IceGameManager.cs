using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGameManager : MonoBehaviour {

    public int numPlayers;

	// Use this for initialization
	void Start () {
        GameObject[] players = GameObject.FindGameObjectsWithTag("IcePlayer");
        for (int i = 3; i >= 0 + numPlayers; i--)
        {
            Destroy(players[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
