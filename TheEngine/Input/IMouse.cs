using TheMaths;

namespace TheEngine.Input
{
    public interface IMouse
    {
        bool IsMouseDown(MouseButton button);
        Vector2 Position { get; }
    }
}