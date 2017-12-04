namespace KekeDreamLand
{
    /// <summary>
    /// Implement this if you want that an action of the game can be stored in action list, then cancel or delete.
    /// </summary>
    public interface IAction
    {
        void DeleteAction();

        void CancelAction();
    }
}