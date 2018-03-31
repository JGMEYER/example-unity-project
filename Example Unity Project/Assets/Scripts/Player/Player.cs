using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    protected PlayerNumber playerNumber;

    protected IPlayerControls controls { get; private set; }
    protected bool active;

    protected void Awake()
    {
        controls = InputManager.Instance.PlayerControls(playerNumber);
        active = false;
    }

    protected void OnValidate()
    {
        if (playerNumber == 0)
        {
            throw new System.ArgumentException("Player is missing PlayerNumber assignment.");
        }
    }

    public PlayerNumber GetPlayerNumber()
    {
        // Could be a public { get; protected set;} instead, if we add a custom inspector.
        return playerNumber;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

}
