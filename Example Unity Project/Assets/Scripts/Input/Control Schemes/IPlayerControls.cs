public interface IPlayerControls
{

    float GetMovementHorizontal();
    float GetMovementVertical();
    bool GetSubmitDown();
    bool GetCancelDown();
    bool GetExitDown();
    bool GetJoinGameDown();
    bool GetJump();
    bool GetUpKey();
    bool GetUpKeyDown();
    bool GetLeftKey();
    bool GetLeftKeyDown();
    bool GetDownKey();
    bool GetDownKeyDown();
    bool GetRightKey();
    bool GetRightKeyDown();
    bool GetActionKey();
    bool GetActionKeyDown();

    string GetJoinGameKeyName();

}
