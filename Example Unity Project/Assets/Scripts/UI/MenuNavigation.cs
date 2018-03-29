using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour {

    private Button[] buttons;
    private Button currentSelection;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        currentSelection = buttons[0];
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
    }

    private void OnDisable()
    {
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedUp, NavigateUp);
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedLeft, NavigateLeft);
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedDown, NavigateDown);
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedRight, NavigateRight);
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedSubmit, Submit);
    }

    private void changeSelection(Button newSelection)
    {
        newSelection.Select();
        currentSelection = newSelection;
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

}
