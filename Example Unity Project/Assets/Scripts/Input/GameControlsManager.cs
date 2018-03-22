using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControlsManager : PersistentSingleton<GameControlsManager> {

	// Keep singleton-only by disabling constructor
	protected GameControlsManager() {}

	void Awake() {
		SetupGlobalDefaults();
		SetupPlayerDefaults();
	}

	// =================
	// Global Controls
	// =================

	string GlobalConfigKey(InputCommand command) {
		return "BUTTON_" + command;
	}

	void SetupGlobalDefaults() {
		if (!PlayerPrefs.HasKey("BUTTON_" + InputCommand.EXIT)) {
			PlayerPrefs.SetString("BUTTON_" + InputCommand.EXIT, "Escape");
		}

		PlayerPrefs.Save();
	}

	// Snapshot of global controls. Does not keep sync with PlayerPrefs.
	public GlobalControls GlobalControls() {
		KeyCode exitKey = GetGlobalCommandKey(InputCommand.EXIT);

		return new GlobalControls(exitKey);
	}

	// Game components should not use these directly. Use GlobalControls object.
	KeyCode GetGlobalCommandKey(InputCommand command) {
		string configKey = GlobalConfigKey(command);
		string keyCodeString = PlayerPrefs.GetString(configKey);

		return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeString);
	}

	// =================
	// Player Controls
	// =================

	string PlayerConfigKey(PlayerNumber playerNumber, InputCommand command) {
		return "BUTTON_" + "PLAYER" + (int)playerNumber + "_" + command;
	}

	void SetupPlayerDefaults() {
		if (!PlayerCommandKeySet(PlayerNumber.ONE, InputCommand.UP)) {
			SetPlayerCommandKey(PlayerNumber.ONE, InputCommand.UP, KeyCode.W);
		}
		if (!PlayerCommandKeySet(PlayerNumber.ONE, InputCommand.LEFT)) {
			SetPlayerCommandKey(PlayerNumber.ONE, InputCommand.LEFT, KeyCode.A);
		}
		if (!PlayerCommandKeySet(PlayerNumber.ONE, InputCommand.DOWN)) {
			SetPlayerCommandKey(PlayerNumber.ONE, InputCommand.DOWN, KeyCode.S);
		}
		if (!PlayerCommandKeySet(PlayerNumber.ONE, InputCommand.RIGHT)) {
			SetPlayerCommandKey(PlayerNumber.ONE, InputCommand.RIGHT, KeyCode.D);
		}

		if (!PlayerCommandKeySet(PlayerNumber.TWO, InputCommand.UP)) {
			SetPlayerCommandKey(PlayerNumber.TWO, InputCommand.UP, KeyCode.UpArrow);
		}
		if (!PlayerCommandKeySet(PlayerNumber.TWO, InputCommand.LEFT)) {
			SetPlayerCommandKey(PlayerNumber.TWO, InputCommand.LEFT, KeyCode.LeftArrow);
		}
		if (!PlayerCommandKeySet(PlayerNumber.TWO, InputCommand.DOWN)) {
			SetPlayerCommandKey(PlayerNumber.TWO, InputCommand.DOWN, KeyCode.DownArrow);
		}
		if (!PlayerCommandKeySet(PlayerNumber.TWO, InputCommand.RIGHT)) {
			SetPlayerCommandKey(PlayerNumber.TWO, InputCommand.RIGHT, KeyCode.RightArrow);
		}

		if (!PlayerCommandKeySet(PlayerNumber.THREE, InputCommand.UP)) {
			SetPlayerCommandKey(PlayerNumber.THREE, InputCommand.UP, KeyCode.Y);
		}
		if (!PlayerCommandKeySet(PlayerNumber.THREE, InputCommand.LEFT)) {
			SetPlayerCommandKey(PlayerNumber.THREE, InputCommand.LEFT, KeyCode.G);
		}
		if (!PlayerCommandKeySet(PlayerNumber.THREE, InputCommand.DOWN)) {
			SetPlayerCommandKey(PlayerNumber.THREE, InputCommand.DOWN, KeyCode.H);
		}
		if (!PlayerCommandKeySet(PlayerNumber.THREE, InputCommand.RIGHT)) {
			SetPlayerCommandKey(PlayerNumber.THREE, InputCommand.RIGHT, KeyCode.J);
		}

		if (!PlayerCommandKeySet(PlayerNumber.FOUR, InputCommand.UP)) {
			SetPlayerCommandKey(PlayerNumber.FOUR, InputCommand.UP, KeyCode.P);
		}
		if (!PlayerCommandKeySet(PlayerNumber.FOUR, InputCommand.LEFT)) {
			SetPlayerCommandKey(PlayerNumber.FOUR, InputCommand.LEFT, KeyCode.L);
		}
		if (!PlayerCommandKeySet(PlayerNumber.FOUR, InputCommand.DOWN)) {
			SetPlayerCommandKey(PlayerNumber.FOUR, InputCommand.DOWN, KeyCode.Semicolon);
		}
		if (!PlayerCommandKeySet(PlayerNumber.FOUR, InputCommand.RIGHT)) {
			SetPlayerCommandKey(PlayerNumber.FOUR, InputCommand.RIGHT, KeyCode.Quote);
		}

		PlayerPrefs.Save();
	}

	// Snapshot of player's controls. Does not keep sync with PlayerPrefs.
	public PlayerControls PlayerControls(PlayerNumber playerNumber) {
		KeyCode upKey = GetPlayerCommandKey(playerNumber, InputCommand.UP);
		KeyCode leftKey = GetPlayerCommandKey(playerNumber, InputCommand.LEFT);
		KeyCode downKey = GetPlayerCommandKey(playerNumber, InputCommand.DOWN);
		KeyCode rightKey = GetPlayerCommandKey(playerNumber, InputCommand.RIGHT);

		return new PlayerControls(upKey, leftKey, downKey, rightKey);
	}

	bool PlayerCommandKeySet(PlayerNumber playerNumber, InputCommand command) {
		return PlayerPrefs.HasKey(PlayerConfigKey(playerNumber, command));
	}

	public void SetPlayerCommandKey(PlayerNumber playerNumber, InputCommand command, KeyCode keyCode) {
		string configKey = PlayerConfigKey(playerNumber, command);
		PlayerPrefs.SetString(configKey, keyCode.ToString());
	}

	// Game components should not use these directly. Use PlayerControls object.
	KeyCode GetPlayerCommandKey(PlayerNumber playerNumber, InputCommand command) {
		string configKey = PlayerConfigKey(playerNumber, command);
		string keyCodeString = PlayerPrefs.GetString(configKey);

		return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeString);
	}

}
