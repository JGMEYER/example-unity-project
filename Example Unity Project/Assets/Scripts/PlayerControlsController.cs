using System.Collections;
using System.Collections.Generic;
using SharpConfig;
using System.IO;
using UnityEngine;

public class PlayerControlsController : MonoBehaviour {

	public string FileName = "cfg/controls.cfg";
	public Configuration cfg = new Configuration();

	void Start() {
		if (!File.Exists(FileName)) {
			Debug.Log("Setting up default controller config since no file found.");
			CreateNewConfig();
			SaveConfig();
		}

		cfg = Configuration.LoadFromFile(FileName);
	}

	private void SaveConfig() {
		Debug.Log("Saving controller config...");
		cfg.SaveToFile(FileName);
	}

	private void CreateNewConfig() {
		// Player 1
		cfg["Player1"]["Up"].StringValue = "W";
		cfg["Player1"]["Left"].StringValue = "A";
		cfg["Player1"]["Down"].StringValue = "S";
		cfg["Player1"]["Right"].StringValue = "D";

		// Player 2
		cfg["Player2"]["Up"].StringValue = "Y";
		cfg["Player2"]["Left"].StringValue = "G";
		cfg["Player2"]["Down"].StringValue = "H";
		cfg["Player2"]["Right"].StringValue = "J";

		// Player 3
		cfg["Player3"]["Up"].StringValue = "P";
		cfg["Player3"]["Left"].StringValue = "L";
		cfg["Player3"]["Down"].StringValue = "Semicolon";
		cfg["Player3"]["Right"].StringValue = "Quote";

		// Player 4
		cfg["Player4"]["Up"].StringValue = "UpArrow";
		cfg["Player4"]["Left"].StringValue = "LeftArrow";
		cfg["Player4"]["Down"].StringValue = "DownArrow";
		cfg["Player4"]["Right"].StringValue = "RightArrow";
	}

}
