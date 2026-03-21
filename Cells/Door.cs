namespace SylLab.MazeCS.Cells;

using SylLab.MazeCS.Collectables;

internal class Door : Cell
{
    public override ConsoleColor Color => ConsoleColor.Magenta;
    public override string Content => "/";
    public override bool TryTraverse(ICollection<ICollectable> withItems) =>
        _opened = _opened || withItems.Remove(_key);
    public Key CloseAndTakeKey()
    {
        _opened = false;
        return _key;
    }
    private bool _opened = true;
    private readonly Key _key = new Key();
}
