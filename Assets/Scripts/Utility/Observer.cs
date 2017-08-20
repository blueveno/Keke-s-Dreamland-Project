namespace KekeDreamLand
{

    /// <summary>
    /// Interface to implement if you want to observe when Boing jump.
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// Notify that the player has jump.
        /// </summary>
        void NotifyJump();
    }

}