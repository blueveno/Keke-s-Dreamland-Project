namespace KekeDreamLand
{
    /// <summary>
    /// Collectable. Easy to find.
    /// </summary>
    public class Feather : Item
    {
        protected override void DoActionWhenPick()
        {
            GameManager.instance.CurrentLevel.FeatherPickedUp++;
        }
    }
}
