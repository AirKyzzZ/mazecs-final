using SylLab.MazeCS;

Vec2d Offset = new(0, 3);
Vec2d MazeSize = new(50, 20);

var InfoPos     = Offset    + new Vec2d(0, MazeSize.Y);
var WinEscPos   = InfoPos   + new Vec2d(0, 3);
var PressKeyPos = WinEscPos + new Vec2d(0, 5);

var grid = new CellType[MazeSize.X, MazeSize.Y];

const int HeaderPaddingX = 10;
const int WinPaddingX = 2;

const string HeaderMsg = "🏃 LABYRINTHE ASCII  C#  🏃";
const string InfoMsg   = "  [Z/↑] Haut   [S/↓] Bas   [Q/←] Gauche   [D/→] Droite   [Échap] Quitter";
const string WinMsg1   = "🎉  FÉLICITATIONS !  🎉";
const string WinMsg2   = "Vous avez trouvé la sortie !";
const string EscMsg = "\n  Partie abandonnée. À bientôt !";
const string PressKeyMsg = "  Appuyez sur une key pour quitter...";

const ConsoleColor WinColor      = ConsoleColor.Green;
const ConsoleColor EscColor      = ConsoleColor.Red;
const ConsoleColor HeaderColor   = ConsoleColor.Cyan;
const ConsoleColor InfoColor     = ConsoleColor.DarkCyan;

var player = Vec2d.Origin;
var mode = State.Playing;
var kbd = new KeyboardController();
using (var screen = new ConsoleScreen(Offset))
{
    GenerateMaze(grid, player);

    screen.DrawFrame(Vec2d.Origin, HeaderPaddingX, HeaderColor, HeaderMsg);
    screen.DrawMaze (grid);
    screen.DrawTextXY(InfoPos, InfoMsg, InfoColor);

    while (mode == State.Playing)
    {
        var newPlayer = player;

        kbd.WaitKey();
        newPlayer += kbd.DirectionPressed;
        if (kbd.IsEscapePressed)
            mode = State.Canceled;
        if (newPlayer.IsIn(MazeSize) && grid[newPlayer.X, newPlayer.Y] != CellType.Wall)
        {
            if (grid[newPlayer.X, newPlayer.Y] == CellType.Exit) mode = State.Won;

            UpdateCell(screen, player, CellType.Corridor);
            UpdateCell(screen, player = newPlayer, CellType.Player);
        }
    }
    if(mode == State.Won)
        screen.DrawFrame(WinEscPos, WinPaddingX, WinColor, WinMsg1, WinMsg2);
    else
        screen.DrawTextXY(WinEscPos, EscMsg, EscColor);
    screen.DrawTextXY(PressKeyPos, PressKeyMsg);
}
kbd.WaitKey();

#region Functions

void UpdateCell(ConsoleScreen screen, Vec2d mazePos, CellType type)
{
    SetTile(mazePos, type);
    screen.DrawCell(mazePos, type);
}

void SetTile(Vec2d pos, CellType type) =>
    grid[pos.X, pos.Y] = type;

void GenerateMaze(CellType[,] grid, Vec2d start)
{
    for (var pos = Vec2d.Origin; pos.IsIn(MazeSize); pos = pos.NextLTR(MazeSize.X))
    {
        SetTile(pos, CellType.Wall);
    }
    int[][] orders = [
        [ 0, 1, 2, 3 ], [ 0, 1, 3, 2 ], [ 0, 2, 1, 3 ], [ 0, 2, 3, 1 ], [ 0, 3, 1, 2 ], [ 0, 3, 2, 1 ],
        [ 1, 0, 2, 3 ], [ 1, 0, 3, 2 ], [ 1, 2, 0, 3 ], [ 1, 2, 3, 0 ], [ 1, 3, 0, 2 ], [ 1, 3, 2, 0 ],
        [ 2, 0, 1, 3 ], [ 2, 0, 3, 1 ], [ 2, 1, 0, 3 ], [ 2, 1, 3, 0 ], [ 2, 3, 0, 1 ], [ 2, 3, 1, 0 ],
        [ 3, 0, 1, 2 ], [ 3, 0, 2, 1 ], [ 3, 1, 0, 2 ], [ 3, 1, 2, 0 ], [ 3, 2, 0, 1 ], [ 3, 2, 1, 0 ]
    ];
    Vec2d[] dirs = [Vec2d.North * 2, Vec2d.East * 2, Vec2d.South * 2, Vec2d.West * 2];
    var rng = new Random();

    GenerateMazeRec(start);

    SetTile(start, CellType.Player);
    SetTile(

        (MazeSize + Vec2d.North + Vec2d.West).Even(),
        CellType.Exit
    );
    void GenerateMazeRec(Vec2d pos)
    {
        SetTile(pos, CellType.Corridor);
        foreach (var index in rng.GetItems(orders, 1).First())
        {
            var next = pos + dirs[index];

            if (next.IsIn(MazeSize) && grid[next.X, next.Y] == CellType.Wall)
            {
                SetTile((pos + next) / 2, CellType.Corridor);
                GenerateMazeRec(next);
            }
        }
    }
}
#endregion
