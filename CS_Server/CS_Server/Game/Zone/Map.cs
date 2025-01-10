using Newtonsoft.Json;
using ServerCore;

namespace CS_Server;

[Serializable]
public class MapData
{
    public required BoundsData Bounds;
    public required List<RowData> CollisionData;
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
        area.RemoveGameObject(gameObject);


        {
            var localPos = Bounds.ToLocalCoordinates(posInfo.PosX, posInfo.PosY);
            if (_mapCells[localPos.y, localPos.x].GameObject == gameObject)
                _mapCells[localPos.y, localPos.x].GameObject = null;
        }

        return true;
    }

    private void UpdateArea(GameObject gameObject, Vector2Int currPos, Vector2Int destPos)
    {
        var type = ObjectManager.GetObjectTypeById(gameObject.Id);

        // 현재와 다음 영역 계산
        var currArea = gameObject.Zone.GetArea(currPos);
        var nextArea = gameObject.Zone.GetArea(destPos);

        if (currArea == nextArea)
            return;

        // 영역 이동 처리
        currArea.RemoveGameObject(gameObject);
        nextArea.AddGameObject(gameObject);
    }

    public bool ApplyMove(GameObject gameObject, Vector2Int dest, bool checkObjects = true, bool isCollision = true)
    {
        var posInfo = gameObject.PosInfo;

        // 이동 가능 여부 확인
        if (CanGo(dest, checkObjects) == false)
            return false;

        // 충돌 처리
        if (isCollision)
        {
            var currlocalPos = Bounds.ToLocalCoordinates(posInfo.PosX, posInfo.PosY);
            if (_mapCells[currlocalPos.y, currlocalPos.x].GameObject == gameObject)
                _mapCells[currlocalPos.y, currlocalPos.x].GameObject = null;

            var newLocalPos = Bounds.ToLocalCoordinates(dest);
            _mapCells[newLocalPos.y, newLocalPos.x].GameObject = gameObject;
        }

        // 영역 이동 처리
        UpdateArea(gameObject, gameObject.CellPos, dest);

        // 실제 좌표 이동
        posInfo.PosX = dest.x;
        posInfo.PosY = dest.y;

        return true;
    }

    public void LoadMap(int mapId, string pathPrefix)
    {
        string mapName = $"Map_{mapId:D3}";
        var filePath = $"{pathPrefix}/{mapName}.json";
        if (!File.Exists(filePath))
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

    // 비용 관련 상수
    private const int CostMultiplier = 10;

    // 점수 매기기
    // F = G + H
    // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
    // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
    // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)
    public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool checkObjects = true, int maxDist = 10)
    {
        // 방문한 노드 집합
        HashSet<Pos> closedSet = new HashSet<Pos>();

        // 점수 정보 저장 (F 점수)
        Dictionary<Pos, int> openDict = new Dictionary<Pos, int>();
        Dictionary<Pos, Pos> parentDict = new Dictionary<Pos, Pos>();

        // 우선순위 큐 (F 값을 기준으로 정렬)
        PriorityQueue<PQNode, int> pq = new PriorityQueue<PQNode, int>();

        // 시작 및 도착 지점 변환
        Pos start = Bounds.CellToPos(startCellPos);
        Pos dest = Bounds.CellToPos(destCellPos);

        // 시작점 초기화
        var initalH = CostMultiplier * ManhattanDistance(dest, start);   // 맨하탄 거리 
        openDict.Add(start, initalH);
        pq.Enqueue(new PQNode { F = initalH, G = 0, Y = start.Y, X = start.X }, initalH);
        parentDict.Add(start, start);

        // 탐색 시작
        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode currentNode = pq.Dequeue();
            var currentPos = new Pos(currentNode.Y, currentNode.X);

            // 이미 방문한 곳이면 스킵
            if (closedSet.Contains(currentPos))
                continue;

            // 방문 처리
            closedSet.Add(currentPos);

            // 목적지 도착했으면 바로 종료
            if (currentPos.Equals(dest))
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(currentNode.Y + _deltaY[i], currentNode.X + _deltaX[i]);

                if (!IsValidNextPos(start, next, dest, maxDist, checkObjects, closedSet))
                    continue;

                // 비용 계산
                int g = 0;// node.G + _cost[i];
                int h = CostMultiplier * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
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

                pq.Enqueue(new PQNode { F = g + h, G = g, Y = next.Y, X = next.X }, g + h);

                if (parentDict.TryAdd(next, currentPos) == false)
                {
                    parentDict[next] = currentPos;
                }
            }
        }

        return BuildPathFromParent(parentDict, dest);
    }

    private void InitializeStartNode(Pos start, Pos dest, Dictionary<Pos, int> openDict, Dictionary<Pos, Pos> parentDict, PriorityQueue<PQNode, int> pq)
    {
        int initialH = CostMultiplier * ManhattanDistance(dest, start);
        openDict[start] = initialH;
        pq.Enqueue(new PQNode { F = initialH, G = 0, Y = start.Y, X = start.X }, initialH);
        parentDict[start] = start;
    }

    //private void ProcessNeighbors(PQNode currentNode, Pos start, Pos dest, int maxDist, bool checkObjects, HashSet<Pos> closedSet,
    //    Dictionary<Pos, int> openDict, Dictionary<Pos, Pos> parentDict, PriorityQueue<PQNode, int> pq)
    //{
    //    for (int i = 0; i < _deltaY.Length; i++)
    //    {
    //        Pos next = new Pos(currentNode.Y + _deltaY[i], currentNode.X + _deltaX[i]);

    //        if (!IsValidNextPos(start, next, dest, maxDist, checkObjects, closedSet))
    //            continue;

    //        int g = currentNode.G + CostMultiplier;
    //        int h = CostMultiplier * ManhattanDistance(dest, next);

    //        if (openDict.TryGetValue(next, out int currentScore) && currentScore <= g + h)
    //            continue;

    //        openDict[next] = g + h;
    //        pq.Enqueue(new PQNode { F = g + h, G = g, Y = next.Y, X = next.X }, g + h);
    //        parentDict[next] = currentNode;
    //    }
    //}

    private bool IsValidNextPos(Pos start, Pos next, Pos dest, int maxDist, bool checkObjects, HashSet<Pos> closedSet)
    {
        // 너무 멀면 스킵
        if (Math.Abs(start.Y - next.Y) + Math.Abs(start.X - next.X) >= maxDist)
            return false;

        // 유효 범위를 벗어났으면 스킵, 벽으로 막혀서 갈 수 없으면 스킵
        if (!next.Equals(dest) && !CanGo(Bounds.PosToCell(next), checkObjects))
            return false;

        // 이미 방문한 곳이면 스킵
        if (closedSet.Contains(next))
            return false;

        return true;
    }

    // 부모 노드 정보로 경로 생성
    List<Vector2Int> BuildPathFromParent(Dictionary<Pos, Pos> parentDict, Pos dest)
    {
        // 목적지가 parentDict에 없으면 가장 가까운 지점을 찾는다.
        if (!parentDict.ContainsKey(dest))
            dest = FindClosestToDest(parentDict, dest);

        List<Vector2Int> path = new List<Vector2Int>();
        var pos = dest;
        while (parentDict.TryGetValue(dest, out var parent) && !dest.Equals(pos))
        {
            path.Add(Bounds.PosToCell(dest));
            dest = parent;
        }
        path.Add(Bounds.PosToCell(pos));
        path.Reverse();

        return path;
    }

    // 도착하지 못했을 때, 가장 가까운 곳을 찾는다
    private Pos FindClosestToDest(Dictionary<Pos, Pos> parentDict, Pos dest)
    {
        var closest = new Pos();
        int bestDist = int.MaxValue;

        foreach (var pos in parentDict.Keys)
        {
            int dist = ManhattanDistance(dest, pos);
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = pos;
            }
        }
        return closest;
    }

    // 맨해튼 거리 계산 유틸리티 함수
    private int ManhattanDistance(Pos a, Pos b)
    {
        return Math.Abs(a.Y - b.Y) + Math.Abs(a.X - b.X);
    }
    #endregion
}
