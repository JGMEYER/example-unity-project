using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigator : MonoBehaviour {

    [SerializeField]
    private string previousScene;

    private Button[] buttons;
    private Button currentSelection;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        currentSelection = buttons.Length > 0 ? buttons[0] : null;
    }

    private void Start()
    {
        changeSelection(currentSelection);
    }

    private void OnEnable()
    {
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedUp, NavigateUp);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedLeft, NavigateLeft);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedDown, NavigateDown);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedRight, NavigateRight);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedSubmit, Submit);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedCancel, Back);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedExit, Back);
    }

    private void OnDisable()
    {
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedUp, NavigateUp);
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedLeft, NavigateLeft);
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedDown, NavigateDown);
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedRight, NavigateRight);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedSubmit, Submit);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedCancel, Back);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedExit, Back);
    }

    private void changeSelection(Button newSelection)
    {
        if (newSelection != null)
        {
            newSelection.Select();
            currentSelection = newSelection;
        }
    }

    private void NavigateUp()
    {
        Selectable upSelection = currentSelection.FindSelectableOnUp();
        if (upSelection && upSelection is Button)
        {
            changeSelection((Button)upSelection);
        }
    }

    private void NavigateLeft()
    {
        Selectable leftSelection = currentSelection.FindSelectableOnLeft();
        if (leftSelection && leftSelection is Button)
        {
            changeSelection((Button)leftSelection);
        }
    }

    private void NavigateDown()
    {
        Selectable downSelection = currentSelection.FindSelectableOnDown();
        if (downSelection && downSelection is Button)
        {
            changeSelection((Button)downSelection);
        }
    }

    private void NavigateRight()
    {
        Selectable rightSelection = currentSelection.FindSelectableOnRight();
        if (rightSelection && rightSelection is Button)
        {
            changeSelection((Button)rightSelection);
        }
    }

    private void Submit()
    {
        currentSelection.onClick.Invoke();
    }

    private void Back()
    {
        if (previousScene != null)
        {
            SceneManager.LoadSceneAsync(previousScene);
        }
    }

}
