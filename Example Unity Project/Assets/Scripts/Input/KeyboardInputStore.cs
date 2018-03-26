using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputStore : PersistentSingleton<KeyboardInputStore>
{

    // Keep singleton-only by disabling constructor
    protected KeyboardInputStore() { }

    private void Awake()
    {
        SetupGlobalDefaults();
        SetupPlayerDefaults();
    }

    // =================
    // Global Controls
    // =================

    public GlobalControls GlobalControls()
    {
        KeyCode exitKey = GetGlobalKeyboardCommand(InputCommand.Exit);

        return new GlobalControls(exitKey);
    }

    private string GlobalCommandConfigKey(InputCommand inputCommand)
    {
        return "BUTTON_" + "GLOBAL_" + inputCommand;
    }

    private KeyCode GetGlobalKeyboardCommand(InputCommand command)
    {
        string configKey = GlobalCommandConfigKey(command);

        string keyCodeString = PlayerPrefs.GetString(configKey);
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeString);
    }

    private void SetGlobalCommandDefault(InputCommand inputCommand, KeyCode defaultKey)
    {
        string configKey = GlobalCommandConfigKey(inputCommand);

        if (!PlayerPrefs.HasKey(configKey))
        {
            PlayerPrefs.SetString(configKey, defaultKey.ToString());
        }
    }

    private void SetupGlobalDefaults()
    {
        SetGlobalCommandDefault(InputCommand.Exit, KeyCode.Escape);

        PlayerPrefs.Save();
    }

    // =================
    // Player Controls
    // =================

    public PlayerKeyboardControls PlayerKeyboardControls(KeyboardConfigNumber keyboardConfigNumber)
    {
        KeyCode upKey = GetPlayerKeyboardCommand(keyboardConfigNumber, InputCommand.Up);
        KeyCode leftKey = GetPlayerKeyboardCommand(keyboardConfigNumber, InputCommand.Left);
        KeyCode downKey = GetPlayerKeyboardCommand(keyboardConfigNumber, InputCommand.Down);
        KeyCode rightKey = GetPlayerKeyboardCommand(keyboardConfigNumber, InputCommand.Right);

        return new PlayerKeyboardControls(keyboardConfigNumber, upKey, leftKey, downKey, rightKey);
    }

    private string PlayerCommandConfigKey(KeyboardConfigNumber keyboardConfigNumber, InputCommand inputCommand)
    {
        return "BUTTON_" + "CONFIG_" + (int)keyboardConfigNumber + "_" + inputCommand;
    }

    private KeyCode GetPlayerKeyboardCommand(KeyboardConfigNumber keyboardConfigNumber, InputCommand inputCommand)
    {
        string configKey = PlayerCommandConfigKey(keyboardConfigNumber, inputCommand);

        string keyCodeString = PlayerPrefs.GetString(configKey);
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeString);
    }

    private void SetPlayerCommandDefault(KeyboardConfigNumber keyboardConfigNumber, InputCommand inputCommand, KeyCode defaultKey)
    {
        string configKey = PlayerCommandConfigKey(keyboardConfigNumber, inputCommand);

        if (!PlayerPrefs.HasKey(configKey))
        {
            PlayerPrefs.SetString(configKey, defaultKey.ToString());
        }
    }

    private void SetupPlayerDefaults()
    {
        SetPlayerCommandDefault(KeyboardConfigNumber.One, InputCommand.Up, KeyCode.W);
        SetPlayerCommandDefault(KeyboardConfigNumber.One, InputCommand.Left, KeyCode.A);
        SetPlayerCommandDefault(KeyboardConfigNumber.One, InputCommand.Down, KeyCode.S);
        SetPlayerCommandDefault(KeyboardConfigNumber.One, InputCommand.Right, KeyCode.D);

        SetPlayerCommandDefault(KeyboardConfigNumber.Two, InputCommand.Up, KeyCode.UpArrow);
        SetPlayerCommandDefault(KeyboardConfigNumber.Two, InputCommand.Left, KeyCode.LeftArrow);
        SetPlayerCommandDefault(KeyboardConfigNumber.Two, InputCommand.Down, KeyCode.DownArrow);
        SetPlayerCommandDefault(KeyboardConfigNumber.Two, InputCommand.Right, KeyCode.RightArrow);

        SetPlayerCommandDefault(KeyboardConfigNumber.Three, InputCommand.Up, KeyCode.Y);
        SetPlayerCommandDefault(KeyboardConfigNumber.Three, InputCommand.Left, KeyCode.G);
        SetPlayerCommandDefault(KeyboardConfigNumber.Three, InputCommand.Down, KeyCode.H);
        SetPlayerCommandDefault(KeyboardConfigNumber.Three, InputCommand.Right, KeyCode.J);

        SetPlayerCommandDefault(KeyboardConfigNumber.Four, InputCommand.Up, KeyCode.P);
        SetPlayerCommandDefault(KeyboardConfigNumber.Four, InputCommand.Left, KeyCode.L);
        SetPlayerCommandDefault(KeyboardConfigNumber.Four, InputCommand.Down, KeyCode.Semicolon);
        SetPlayerCommandDefault(KeyboardConfigNumber.Four, InputCommand.Right, KeyCode.Quote);

        PlayerPrefs.Save();
    }

}
