using SFML.Graphics;
using SFML.System;
using SFML.Window;

static class Input
{
    public static Vector2i MousePositionOnWindow;

    public static void ProcessInput(RenderWindow window)
    {
        window.DispatchEvents();

        MousePositionOnWindow = Mouse.GetPosition() - window.Position;
    }
}