namespace LearningOpenTK;

public static class Program {
    public static void Main() {
        using var game = new Game(1280, 700);
        game.Run();
    }
}