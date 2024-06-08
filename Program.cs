using OpenTK.Windowing.Desktop;

namespace LearningOpenTK;

public static class Program {
    public static void Main() {
        using var game = new Game(500, 500);
        game.Run();
    }
}

