using System;

namespace Pixel3D;

public static class Program
{
    [STAThread]
    private static void Main()
    {
        using (var game = new Game1())
        {
            // Console.WriteLine($"Using {Environment.ProcessorCount} processors.");
            
            game.Run();
            
            // Console.WriteLine($"AVG FPS: {game.FrameCount / game.ElapsedTime}");
        }
    }
}