public interface IPlayerMovementComponent
{
    /// <summary>
    /// Initializes the component with the specified player movement controller.
    /// </summary>
    /// <param name="movementController">The player movement controller to use for initialization.</param>
    public void Initialize(PlayerMovementController movementController);
}
