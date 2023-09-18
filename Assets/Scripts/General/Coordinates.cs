public struct Coordinates
{
    public int x;
    public int y;
    public Coordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public override string ToString()
    {
        string s = $"({x}; {y})";
        UnityEngine.MonoBehaviour.print($"({x}; {y})");
        return s;
    }
    public static bool Equal(Coordinates a, Coordinates b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public void Print()
    {
        UnityEngine.MonoBehaviour.print(x + " " + y);
    }
}
