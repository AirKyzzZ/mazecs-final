namespace SylLab.MazeCS;

public class ConsoleScreen : IDisposable
{
    public readonly ConsoleColor[] CellTypeColors = [
        ConsoleColor.DarkGray, // Wall
        ConsoleColor.DarkBlue, // Corridor
        ConsoleColor.Yellow,   // Player
        ConsoleColor.Green     // Exit
    ];
    public const string CellTypeSymbols = ".█@★";

    public ConsoleScreen(Vec2d mazeOrigin)
    {
        MazeOrigin = mazeOrigin;
        Console.Clear();
        Console.CursorVisible = false;
    }
    public void Dispose() => 
        Console.CursorVisible = true;

    public Vec2d MazeOrigin { get; }

    public void DrawTextXY(Vec2d pos, string text, ConsoleColor? color = null)
    {
        Console.SetCursorPosition(pos.X, pos.Y);
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
        }
        Console.Write(text);
        Console.ResetColor();
    }

    public void DrawFrame(Vec2d pos, int paddingX, ConsoleColor color, params string [] lines)
    {
        Vec2d IncPos() => pos = pos with { Y = pos.Y + 1 };
        
        var width = lines.Max(s => s.Length + 2*paddingX);
        var horizontal = new string('═', width);
        
        DrawTextXY(pos, $"╔{ horizontal}╗", color);
        foreach (var line in lines)
        {
            var dif = width - line.Length;
            var left = new string(' ', dif / 2);
            var right = dif%2==0 ? left : left+' ';

            DrawTextXY(IncPos(), $"║{left}{line}{right}║", color);
        }
        DrawTextXY(IncPos(), $"╚{horizontal}╝", color);
    }

    public void DrawCell(Vec2d mazePos, CellType type) => DrawTextXY(
        MazeOrigin + mazePos,
        CellTypeSymbols[(int)type].ToString(),
        CellTypeColors[(int)type]
    );
  
    public void DrawMaze(CellType[,] grid)
    {
        var mazeSize = new Vec2d(grid.GetLength(0), grid.GetLength(1));
        
        for (var pos = Vec2d.Origin; pos.IsIn(mazeSize); pos = pos.NextLTR(mazeSize.X))
        {
            DrawCell(pos, grid[pos.X, pos.Y]);
        }
    }
}
