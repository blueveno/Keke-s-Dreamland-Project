namespace KekeDreamLand
{
    /// <summary>
    /// Collectable. Easy to find.
    /// </summary>
    public class Feather : Item
    {
        protected override void DoActionWhenPick()
        {
            // FEEDBACK -> sound, particles...

            GameManager.instance.levelMgr.FeatherCollected++;
        }
    }
}
