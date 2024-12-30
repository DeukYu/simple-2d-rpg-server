using Google.Protobuf.Enum;
using Newtonsoft.Json;
using ServerCore;
using System.Diagnostics;

namespace CS_Server;

[Serializable]
public class MapData
{
    public BoundsData Bounds;
    public List<RowData> CollisionData;
}

[Serializable]
public class BoundsData
{
    public int MinX;
    public int MaxX;
    public int MinY;
    public int MaxY;
}

[Serializable]
public class RowData
{
    public List<int> Columns = new List<int>();

    public RowData(int size)
    {
        Columns = new List<int>(new int[size]);
    }
}

public class Map
{
    public Bounds Bounds { get; set; }

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
        if (gameObject.Zone == null)
            return false;

        if (gameObject.Zone.Map != this)
            return false;

        var posInfo = gameObject.Info.PosInfo;
        if (Bounds.Contains(posInfo.PosX, posInfo.PosY) == false)
            return false;

        // Area
        var area = gameObject.Zone.GetArea(gameObject.CellPos);
        area.Remove(gameObject);


        {
            var localPos = Bounds.ToLocalCoordinates(posInfo.PosX, posInfo.PosY);
            if (_mapCells[localPos.y, localPos.x].GameObject == gameObject)
                _mapCells[localPos.y, localPos.x].GameObject = null;
        }

        return true;
    }

    public bool ApplyMove(GameObject gameObject, Vector2Int dest, bool checkObjects = true, bool isCollision = true)
    {
        var posInfo = gameObject.Info.PosInfo;
        if (CanGo(dest, checkObjects) == false)
            return false;

        if (isCollision)
        {
            {
                var localPos = Bounds.ToLocalCoordinates(posInfo.PosX, posInfo.PosY);
                if (_mapCells[localPos.y, localPos.x].GameObject == gameObject)
                    _mapCells[localPos.y, localPos.x].GameObject = null;
            }
            {
                var localPos = Bounds.ToLocalCoordinates(dest);
                _mapCells[localPos.y, localPos.x].GameObject = gameObject;
            }
        }

        var type = ObjectManager.GetObjectTypeById(gameObject.Id);
        if (type == GameObjectType.Player)
        {
            var player = gameObject as Player;

            var currArea = player.Zone.GetArea(new Vector2Int(posInfo.PosX, posInfo.PosY));
            var nextArea = player.Zone.GetArea(dest);
            if (currArea != nextArea)
            {
                currArea.Players.Remove(player);
                nextArea.Players.Add(player);
            }
        }
        else if (type == GameObjectType.Monster)
        {
            var monster = gameObject as Monster;
            var currArea = monster.Zone.GetArea(new Vector2Int(posInfo.PosX, posInfo.PosY));
            var nextArea = monster.Zone.GetArea(dest);
            if (currArea != nextArea)
            {
                currArea.Monsters.Remove(monster);
                nextArea.Monsters.Add(monster);
            }
        }
        else if (type == GameObjectType.Projectile)
        {
            var projectile = gameObject as Projectile;
            var currArea = projectile.Zone.GetArea(new Vector2Int(posInfo.PosX, posInfo.PosY));
            var nextArea = projectile.Zone.GetArea(dest);
            if (currArea != nextArea)
            {
                currArea.Projectiles.Remove(projectile);
                nextArea.Projectiles.Add(projectile);
            }
        }


        // 실제 좌표 이동
        posInfo.PosX = dest.x;
        posInfo.PosY = dest.y;

        return true;
    }

    public void LoadMap(int mapId, string pathPrefix)
    {
        string mapName = $"Map_{mapId:D3}";
        var filePath = $"{pathPrefix}/{mapName}.json";
        if(!File.Exists(filePath))
        {
            Log.Error($"File not found. {filePath}");
            return;
        }

        try
        {
            string jsonText = File.ReadAllText(filePath);
            var mapData = JsonConvert.DeserializeObject<MapData>(jsonText);

            // Set bounds
            Bounds = new Bounds(
                mapData.Bounds.MinX,
                mapData.Bounds.MaxX,
                mapData.Bounds.MinY,
                mapData.Bounds.MaxY
            );

            int xCount = Bounds.SizeX;
            int yCount = Bounds.SizeY;

            // Initialize map cells
            _mapCells = new MapCell[yCount, xCount];

            for (int y = 0; y < yCount; y++)
            {
                var row = mapData.CollisionData[y];
                for (int x = 0; x < xCount; x++)
                {
                    _mapCells[y, x] = new MapCell
                    {
                        Collision = row.Columns[x] == 1
                    };
                }
            }

            Log.Info($"Map {mapName} loaded successfully from {filePath}");
        }
        catch (IOException ex)
        {
            Log.Error($"Failed to load map: {filePath}, Error: {ex.Message}");
        }
    }

    #region A* PathFinding

    // U D L R
    int[] _deltaY = new int[] { 1, -1, 0, 0 };
    int[] _deltaX = new int[] { 0, 0, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10 };

    public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool checkObjects = true, int maxDist = 10)
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
        HashSet<Pos> closedSet = new HashSet<Pos>();

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        Dictionary<Pos, int> openDict = new Dictionary<Pos, int>();
        Dictionary<Pos, Pos> parentDict = new Dictionary<Pos, Pos>();

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode, int> pq = new PriorityQueue<PQNode, int>();

        // CellPos -> ArrayPos
        Pos pos = Bounds.CellToPos(startCellPos);
        Pos dest = Bounds.CellToPos(destCellPos);

        // 시작점 발견 (예약 진행)
        openDict.Add(pos, 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)));
        pq.Enqueue(new PQNode()
        {
            F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)),
            G = 0,
            Y = pos.Y,
            X = pos.X
        }
        , 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X))
        );
        parentDict.Add(pos, pos);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode pqNode = pq.Dequeue();
            var node = new Pos(pqNode.Y, pqNode.X);
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closedSet.Contains(node))
                continue;

            // 방문한다
            closedSet.Add(node);
            // 목적지 도착했으면 바로 종료
            if (pqNode.Y == dest.Y && pqNode.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)

            {
                Pos next = new Pos(pqNode.Y + _deltaY[i], pqNode.X + _deltaX[i]);

                // 너무 멀면 스킵
                if (Math.Abs(pos.Y - next.Y) + Math.Abs(pos.X - next.X) >= maxDist)
                    continue;

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Bounds.PosToCell(next), checkObjects) == false) // CellPos
                        continue;
                }

                // 이미 방문한 곳이면 스킵
                if (closedSet.Contains(next))
                    continue;

                // 비용 계산
                int g = 0;// node.G + _cost[i];
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if (openDict.TryGetValue(next, out int value) == false)
                {
                    value = int.MaxValue;
                }
                if (value < g + h)
                    continue;

                // 예약 진행
                if (openDict.TryAdd(next, g + h) == false)
                {
                    openDict[next] = g + h;
                }

                pq.Enqueue(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X }, g + h);

                if (parentDict.TryAdd(next, node) == false)
                {
                    parentDict[next] = node;
                }
            }
        }

        return CalcCellPathFromParent(parentDict, dest);
    }

    List<Vector2Int> CalcCellPathFromParent(Dictionary<Pos, Pos> parentDict, Pos dest)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        if (parentDict.ContainsKey(dest) == false)
        {
            var bestPos = new Pos();
            int bestDist = int.MaxValue;

            foreach (var pos in parentDict.Keys)
            {
                int dist = Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestPos = pos;
                }
            }
            dest = bestPos;
        }

        {
            Pos pos = dest;
            while (parentDict[pos] != pos)
            {
                cells.Add(Bounds.PosToCell(pos));
                pos = parentDict[pos];
            }
            cells.Add(Bounds.PosToCell(pos));
            cells.Reverse();
        }


        return cells;
    }
    #endregion
}
