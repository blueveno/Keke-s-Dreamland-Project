namespace KekeDreamLand
{
    /// <summary>
    /// A special item is a rare and unique collectable item.
    /// </summary>
    public class SpecialItem : Item
    {
        /*
         * Key 0
         * Raisin bread 1 - Obtained where all feathers are collected.
         * Chocolatine 2 - Present in each secret level except. (Hidden by foreground tile).
         * Sunflower seed 3 - Present in each level except secret where they are replace by Chocolatine.
         */

        public int specialItemIndex;

        protected override void DoActionWhenPick()
        {
            // TODO FEEDBACK -> sound, particles...

            // Notify that the item has been pick on the current level and update HUD.
            GameManager.instance.levelMgr.PickSpecialItem(specialItemIndex, true);
        }
    }

}