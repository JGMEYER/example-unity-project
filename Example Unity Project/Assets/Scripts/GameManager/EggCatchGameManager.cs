public class EggCatchGameManager : GameManager<EggCatchPlayer> {

	private new void Start()
    {
        base.Start();

        StartCoroutine(StartRoundAfterDelay());
    }

}
