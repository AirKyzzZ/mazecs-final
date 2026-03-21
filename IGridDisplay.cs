namespace SylLab.MazeCS;

public interface IGridDisplay
{
    void DrawGridCell(Vec2d pos, (string Content, ConsoleColor Color) cellInfo);
}
