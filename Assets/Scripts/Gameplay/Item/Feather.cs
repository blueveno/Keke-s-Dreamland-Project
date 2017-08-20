namespace KekeDreamLand
{
    public class Feather : Item
    {
        protected override void DoActionWhenPick()
        {
            GameManager.instance.FeatherPickedUp++;
        }
    }
}
