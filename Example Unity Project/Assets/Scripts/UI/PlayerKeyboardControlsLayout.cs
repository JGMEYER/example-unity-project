using UnityEngine;
using UnityEngine.UI;

public class PlayerKeyboardControlsLayout : MonoBehaviour {

    public void Init(PlayerKeyboardControls playerKeyboardControls)
    {
        // Hacky: grabbing panels by thier image
        Image[] uiKeys = GetComponentsInChildren<Image>();

        foreach (Image uiKey in uiKeys)
        {
            // Hacky: key UIs should be individually responsible for setting their own text by key name
            Text uiKeyText = uiKey.GetComponentInChildren<Text>();

            // Hacky: key names should be enums or standardized in some way
            switch (uiKey.name)
            {
                case "SubmitKey":
                    KeyCode submitKey = playerKeyboardControls.ActionKey;
                    uiKeyText.text = KeyCodeCharacter.For(submitKey).ToString();
                    break;
                case "UpKey":
                    KeyCode upKey = playerKeyboardControls.UpKey;
                    uiKeyText.text = KeyCodeCharacter.For(upKey).ToString();
                    break;
                case "LeftKey":
                    KeyCode leftKey = playerKeyboardControls.LeftKey;
                    uiKeyText.text = KeyCodeCharacter.For(leftKey).ToString();
                    break;
                case "DownKey":
                    KeyCode downKey = playerKeyboardControls.DownKey;
                    uiKeyText.text = KeyCodeCharacter.For(downKey).ToString();
                    break;
                case "RightKey":
                    KeyCode rightKey = playerKeyboardControls.RightKey;
                    uiKeyText.text = KeyCodeCharacter.For(rightKey).ToString();
                    break;
            }
        }
    }

}
