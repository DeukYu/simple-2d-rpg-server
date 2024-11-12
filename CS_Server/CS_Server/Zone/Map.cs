namespace CS_Server;

public class Map
{
    Bounds Bounds { get; set; }
    private MapCell[,] _mapCells;

    public bool CanGo(Vector2Int cellPos, bool checkObject = true)
    {
        if (Bounds.Contains(cellPos) == false)
            return false;

        var localPos = Bounds.ToLocalCoordinates(cellPos);
        return _mapCells[localPos.y, localPos.x].CanMoveTo(checkObject);
    }

    public GameObject? Find(Vector2Int cellPos)
    {
        if (Bounds.Contains(cellPos) == false)
            return null;

        var localPos = Bounds.ToLocalCoordinates(cellPos);
        return _mapCells[localPos.y, localPos.x].GameObject;
    }

    public bool ApplyLeave(GameObject gameObject)
    {
        var posInfo = gameObject.Info.PosInfo;
        if (Bounds.Contains(posInfo.PosX, posInfo.PosY) == false)
            return false;

        {
            var localPos = Bounds.ToLocalCoordinates(posInfo.PosX, posInfo.PosY);
            if (_mapCells[localPos.y, localPos.x].GameObject == gameObject)
                _mapCells[localPos.y, localPos.x].GameObject = null;
        }

        return true;
    }

    public bool ApplyMove(GameObject gameObject, Vector2Int dest)
    {
        ApplyLeave(gameObject);

        var posInfo = gameObject.Info.PosInfo;
        if (CanGo(dest, true) == false)
            return false;

        {
            var localPos = Bounds.ToLocalCoordinates(dest);
            _mapCells[localPos.y, localPos.x].GameObject = gameObject;
        }

        // 실제 좌표 이동
        posInfo.PosX = dest.x;
        posInfo.PosY = dest.y;

        return true;
    }

    public void LoadMap(int mapId, string pathPrefix)
    {
        string mapName = "Map_" + mapId.ToString("000");

        var text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
        var reader = new StringReader(text);

        Bounds = new Bounds(
            int.Parse(reader.ReadLine()!),
            int.Parse(reader.ReadLine()!),
            int.Parse(reader.ReadLine()!),
            int.Parse(reader.ReadLine()!)
        );

        int xCount = Bounds.SizeX;
        int yCount = Bounds.SizeY;

        _mapCells = new MapCell[yCount, xCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine()!;
            for (int x = 0; x < xCount; x++)
            {
                _mapCells[y, x].Collision = (line[x] == '1' ? true : false);
            }
        }
    }

    #region A* PathFinding

    // U D L R
    int[] _deltaY = new int[] { 1, -1, 0, 0 };
    int[] _deltaX = new int[] { 0, 0, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10 };

    public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool ignoreDestCollision = false)
    {
        List<Pos> path = new List<Pos>();

        var SizeX = Bounds.SizeX;
        var SizeY = Bounds.SizeY;
        // 점수 매기기
        // F = G + H
        // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
        // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
        // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

        // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        bool[,] closed = new bool[SizeY, SizeX]; // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        int[,] open = new int[SizeY, SizeX]; // OpenList
        for (int y = 0; y < SizeY; y++)
            for (int x = 0; x < SizeX; x++)
                open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[SizeY, SizeX];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode, int> pq = new PriorityQueue<PQNode, int>();

        // CellPos -> ArrayPos
        Pos pos = Bounds.CellToPos(startCellPos);
        Pos dest = Bounds.CellToPos(destCellPos);

        // 시작점 발견 (예약 진행)
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Enqueue(new PQNode()
        {
            F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)),
            G = 0,
            Y = pos.Y,
            X = pos.X
        }
        , 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X))
        );
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Dequeue();
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closed[node.Y, node.X])
                continue;

            // 방문한다
            closed[node.Y, node.X] = true;
            // 목적지 도착했으면 바로 종료
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)

            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Bounds.PosToCell(next)) == false) // CellPos
                        continue;
                }

                // 이미 방문한 곳이면 스킵
                if (closed[next.Y, next.X])
                    continue;

                // 비용 계산
                int g = 0;// node.G + _cost[i];
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if (open[next.Y, next.X] < g + h)
                    continue;

                // 예약 진행
                open[dest.Y, dest.X] = g + h;
                pq.Enqueue(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X }, g + h);
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
            }
        }

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector2Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        int y = dest.Y;
        int x = dest.X;
        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            cells.Add(Bounds.PosToCell(new Pos(y, x)));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }
        cells.Add(Bounds.PosToCell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }
    #endregion
}
