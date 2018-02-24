using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionPlayer : MonoBehaviour {

    public KeyCode upKey;
    public KeyCode leftKey;
    public KeyCode downKey;
    public KeyCode rightKey;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(upKey)) {
            GameObject manager = GameObject.Find("ReactionGameManager");
            ReactionGameManager reactionManager = (ReactionGameManager)manager.GetComponent(typeof(ReactionGameManager));
            reactionManager.Grab();
        }
    }
}
