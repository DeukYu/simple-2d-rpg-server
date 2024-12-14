namespace CS_Server;

public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;

    }

    public static Vector2Int up { get { return new Vector2Int(0, 1); } }
    public static Vector2Int down { get { return new Vector2Int(0, -1); } }
    public static Vector2Int left { get { return new Vector2Int(-1, 0); } }
    public static Vector2Int right { get { return new Vector2Int(1, 0); } }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }
    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }
    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public float magnitude { get { return (float)Math.Sqrt(sqrMagnitude); } }
    public int sqrMagnitude { get { return x * x + y * y; } }
    public int cellDistance { get { return Math.Abs(x) + Math.Abs(y); } }
}

public struct Pos
{
    public Pos(int y, int x) { Y = y; X = x; }
    public int Y;
    public int X;

    public static bool operator==(Pos lts, Pos rhs)
    {
        return lts.X == rhs.X && lts.Y == rhs.Y;
    }
    public static bool operator !=(Pos lhs, Pos rhs)
    {
        return lhs.X != rhs.X || lhs.Y != rhs.Y;
    }
    public override bool Equals(object obj)
    {
        return obj is Pos pos && this == pos;
    }
    public override int GetHashCode()
    {
        long value = (Y << 32) | X;
        return value.GetHashCode();
    }
    public override string ToString()
    {
        return $"({Y}, {X})";
    }
}

public readonly struct Bounds
{
    public int MinX { get; }
    public int MaxX { get; }
    public int MinY { get; }
    public int MaxY { get; }

    public int SizeX => MaxX - MinX + 1;
    public int SizeY => MaxY - MinY + 1;

    public Bounds(int minX, int maxX, int minY, int maxY)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }
    public bool Contains(int x, int y)
    {
        return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
    }

    public bool Contains(Vector2Int position)
    {
        return Contains(position.x, position.y);
    }

    public Vector2Int ToLocalCoordinates(int x, int y)
    {
        int localX = x - MinX;
        int localY = MaxY - y;
        return new Vector2Int(localX, localY);
    }

    public Vector2Int ToLocalCoordinates(Vector2Int globalPosition)
    {
        return ToLocalCoordinates(globalPosition.x, globalPosition.y);
    }

    public Pos CellToPos(Vector2Int cell)
    {
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }
    public Vector2Int PosToCell(Pos pos)
    {
        return new Vector2Int(pos.X + MinX, MaxY - pos.Y);
    }
}
public struct MapCell
{
    public bool Collision;
    public GameObject? GameObject;

    public MapCell(bool collision, GameObject obj)
    {
        Collision = collision;
        GameObject = obj;
    }
    public bool CanMoveTo(bool checkObject)
    {
        return !Collision && (!checkObject || GameObject == null);
    }
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}