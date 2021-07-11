using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 internal class Tile
{
    internal int type = 0;
    internal GameObject prefab;
    internal bool isWall = true;
}

public class Room
{
    public float w, h, x ,y;
    public GameObject gameObject;
    public int roomType = 0,level;
    public Room(float x, float y, float w, float h, int level)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.level = level;
    }
}

public class MapGen : MonoBehaviour
{
    [SerializeField]
    internal GameObject TilePrefab, WallPrefab, RoomTriggerPrefab;
    [SerializeField]
    internal Material tileMaterial;

    Tile[,] mapa;
    List<PointInt> PointInts = new List<PointInt>();
    List<Room> rooms = new List<Room>();

    public float playerX, playerY;
    public float bossX, bossY;
    private int w, h;
    int level = 0;
    internal void Generate(int w, int h, int n, int minRoomSize, float chanceFill, int level)
    {
        this.level = level;
        this.w = w;
        this.h = h;
        mapa = new Tile[w, h];
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)mapa[i, j] = new Tile();

        Split(n, new PointInt(0, 0), new PointInt(w - 1, h - 1), minRoomSize, chanceFill);
        ConnectPointInts();
        CreateObjects(w, h);
    }

    void CreateObjects(int w, int h)
    {
        GameObject tilesObject = new GameObject();
        tilesObject.name = "Generated Tiles";
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (!mapa[i, j].isWall)
                {
                    GameObject til = Instantiate(TilePrefab, new Vector3(i * 4f, 0, j * 4f), Quaternion.identity, tilesObject.transform);
                    til.isStatic = true;
                    Material m = new Material(tileMaterial);
                    m.color = Color.HSVToRGB(mapa[i, j].type / 100f, 1f, 1f);
                    til.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<Renderer>().material = m;
                }
                else
                {
                    bool stop = true;
                    try { if (!mapa[i, j-1].isWall) stop = false; } catch { }
                    try { if (!mapa[i-1, j].isWall) stop = false; } catch { }
                    try { if (!mapa[i, j+1].isWall) stop = false; } catch { }
                    try { if (!mapa[i+1, j].isWall) stop = false; } catch { }
                    if (stop) continue;
                    GameObject til = Instantiate(WallPrefab, new Vector3(i * 4f, 2f, j * 4f), Quaternion.identity, tilesObject.transform);
                    til.isStatic = true;
                    Material m = new Material(tileMaterial);
                    m.color = Color.HSVToRGB(mapa[i, j].type / 100f, 1f, .4f);
                    til.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<Renderer>().material = m;
                }
            }
        }
        foreach(Room room in rooms)
        {
            GameObject roomObject = Instantiate(RoomTriggerPrefab, new Vector3(room.x * 4f, 0, room.y * 4f), Quaternion.identity, tilesObject.transform);
            roomObject.GetComponent<RoomBehaviour>().roomType = room.roomType;
            roomObject.GetComponent<RoomBehaviour>().w = room.w;
            roomObject.GetComponent<RoomBehaviour>().h = room.h;
            roomObject.GetComponent<RoomBehaviour>().level = room.level;

            BoxCollider bc = roomObject.GetComponent<BoxCollider>();
            bc.size = new Vector3(room.w, 10f, room.h);
            room.gameObject = roomObject;
        }
    }
    //TODO random player start   BOSS furtherst distance
    void Split(int n, PointInt p1, PointInt p2, int minRoomSize, float chanceFill)
    {
        const int size = 10;
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        n--;
        if(n < 0 || dx <= size || dy <= size)
        {
            if (/*dx / dy > 3 || dy / dx > 3 || */UnityEngine.Random.value > chanceFill) return;
            PointInts.Add(new PointInt(p1.x + dx / 2, p1.y + dy / 2));

            int c = (int)(UnityEngine.Random.value*100f);
            /*DrawLine(p1,new PointInt(p1.x, p2.y), c, false);
            DrawLine(p1,new PointInt(p2.x, p1.y), c, false);
            DrawLine(p2,new PointInt(p2.x, p1.y), c, false);
            DrawLine(p2,new PointInt(p1.x, p2.y), c, false);*/
            // DrawRect(PointInts[PointInts.Count - 1], (int)(dx-1-Math.Round(UnityEngine.Random.value*.9f)), (int)(dy -1 - Math.Round(UnityEngine.Random.value*.9f)), c, false);
            int w = (int)((dx - 1) * (UnityEngine.Random.value / 2 + 0.5f));
            int h = (int)((dy - 1) * (UnityEngine.Random.value / 2 + 0.5f));
            DrawRect(PointInts[PointInts.Count - 1], w, h, c, false);
            float rx = PointInts[PointInts.Count - 1].x;
            float ry = PointInts[PointInts.Count - 1].y;
            if (w % 2 == 0) rx += .5f;
            if (h % 2 == 0) ry += .5f;
                rooms.Add(new Room(rx, ry, w *4f, h*4f, level));
            return;
        }
        int splitPointInt;
        if(dx > dy)
        {
            splitPointInt = (int)(  p1.x + Math.Round(dx / 2) + Math.Round((1 - 2 * UnityEngine.Random.value) * 0.1f * dx)  );
            Split(n, p1, new PointInt(splitPointInt - 1, p2.y), minRoomSize, chanceFill);
            Split(n, new PointInt(splitPointInt, p1.y), p2, minRoomSize, chanceFill);
        }
        else
        {
            splitPointInt = (int)(p1.y + Math.Round(dy / 2) + Math.Round((1 - 2 * UnityEngine.Random.value) * 0.1f * dy));
            Split(n, p1, new PointInt(p2.x, splitPointInt - 1), minRoomSize, chanceFill);
            Split(n, new PointInt(p1.x, splitPointInt), p2, minRoomSize, chanceFill);
        }
    }

    private void ConnectPointInts()
    {
        foreach (PointInt p in PointInts)
        {
            mapa[p.x, p.y].type = (mapa[p.x, p.y].type + 50) % 100;
            mapa[p.x, p.y].isWall = false;
        }

        int[,] pointsIndex = new int[w, h];
        Point[] ps = new Point[PointInts.Count];
        for (int i = 0; i < PointInts.Count; i++) { 
             ps[i] = new Point(PointInts[i].x, PointInts[i].y);
             pointsIndex[PointInts[i].x, PointInts[i].y] = i;
         }

        HashSet<int>[] graph = new HashSet<int>[PointInts.Count];
        for (int i = 0; i < PointInts.Count; i++) graph[i] = new HashSet<int>();

        DelaunayTriangulator d = new DelaunayTriangulator();
        IEnumerable<Triangle> triangles = d.BowyerWatson(ps);
        
        foreach (Triangle t in triangles)
        {
            try { graph[pointsIndex[(int)t.Vertices[0].X, (int)t.Vertices[0].Y]].Add(pointsIndex[(int)t.Vertices[1].X, (int)t.Vertices[1].Y]); }catch { }
            try { graph[pointsIndex[(int)t.Vertices[1].X, (int)t.Vertices[1].Y]].Add(pointsIndex[(int)t.Vertices[0].X, (int)t.Vertices[0].Y]); }catch { }
            try { graph[pointsIndex[(int)t.Vertices[2].X, (int)t.Vertices[2].Y]].Add(pointsIndex[(int)t.Vertices[1].X, (int)t.Vertices[1].Y]); }catch { }
            try { graph[pointsIndex[(int)t.Vertices[1].X, (int)t.Vertices[1].Y]].Add(pointsIndex[(int)t.Vertices[2].X, (int)t.Vertices[2].Y]); }catch { }
            try { graph[pointsIndex[(int)t.Vertices[0].X, (int)t.Vertices[0].Y]].Add(pointsIndex[(int)t.Vertices[2].X, (int)t.Vertices[2].Y]); }catch { }
            try { graph[pointsIndex[(int)t.Vertices[2].X, (int)t.Vertices[2].Y]].Add(pointsIndex[(int)t.Vertices[0].X, (int)t.Vertices[0].Y]); }catch { }
        }

        /*for(int p1 = 0; p1 < graph.Length; p1++)
        {
            foreach(int p2 in graph[p1])
            {
                DrawLine(ps[p1], ps[p2], 80, false);
            }
        }/**/
        int playerNodeIndex = (int)(UnityEngine.Random.value * (ps.Length-1));
        playerX = PointInts[playerNodeIndex].x*4f;
        playerY = PointInts[playerNodeIndex].y*4f;

        // BEGIN DIJKSTRA //
        List<int> Q = new List<int>();
        int[] dist = new int[graph.Length];
        int[] prev = new int[graph.Length];

        for(int i = 0; i < graph.Length; i++)
        {
            dist[i] = 99999;
            prev[i] = -1;
            Q.Add(i);
        }
        dist[playerNodeIndex] = 0;
        while (Q.Count > 0)
        {
            int minVal = 99999, minIndex = -1;
            for (int i = 0; i < Q.Count; i++)
            {
                if (dist[Q[i]] <= minVal)
                {
                    minVal = dist[Q[i]];
                    minIndex = Q[i];
                }
            }
            int u = minIndex;
            for (int i = Q.Count - 1; i >= 0; i--)if (Q[i] == u) Q.RemoveAt(i);
            foreach (int v in graph[u])
            {
                int alt = dist[u] + (int)((ps[u].X-ps[v].X)* (ps[u].X - ps[v].X) + (ps[u].Y - ps[v].Y)* (ps[u].Y - ps[v].Y));//1 = dlugosc polaczenia
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        // BEGIN BOSS //
        int bossNodeIndex = -1, bossDist = 0;
        for (int i = 0; i < graph.Length; i++)
        {
            if (dist[i] > bossDist)
            {
                bossDist = dist[i];
                bossNodeIndex = i;
            }
        }
        int bossX = (int)ps[bossNodeIndex].X;
        int bossY = (int)ps[bossNodeIndex].Y;

        foreach (Room room in rooms)
        {
            if ((int)room.x == bossX && (int)room.y == bossY) room.roomType = 2;
            if ((int)room.x == (int)ps[playerNodeIndex].X && (int)room.y == (int)ps[playerNodeIndex].Y) room.roomType = 1;
        }
        // END BOSS //

        List<int> S = new List<int>();
        int target = bossNodeIndex;
        while (target >= 0)
        {
            S.Add(target);
            target = prev[target];
        }
        // END DIJSKTRA //

        
        //TODO graph triangulacja   graf polaczenss 
        // BEGIN DRAW CONNECTIONS //
        Queue<int> q = new Queue<int>();
        int[] parents = new int[graph.Length];
        for (int i = 0; i < parents.Length; i++) parents[i] = -1;
        q.Enqueue(playerNodeIndex);
        while (q.Count > 0)
        {
            int u = q.Dequeue();
            foreach (int v in graph[u])
            {
                if (parents[v] >= 0) continue;
                parents[v] = u;
                q.Enqueue(v);
            }
        }
        for (int i = 0; i < parents.Length; i++)
        {
            DrawLine2D(ps[i], ps[parents[i]], 70, false);
        }
        // END DRAW CONNECTIONS //
        // BEGIN DRAW BOSS PATH //
        for (int i = 0; i < S.Count - 1; i++) DrawLine2D(ps[S[i]], ps[S[i + 1]], 80000, false);
        // END DRAW BOSS PATH //
    }


    void DrawLine(PointInt p1, PointInt p2, int type, bool isWall)
    {
        if (p1.x == p2.x && p1.y == p2.y) return;
        int X0 = p1.x;
        int Y0 = p1.y;
        int X1 = p2.x;
        int Y1 = p2.y;
        int dx = X1 - X0;
        int dy = Y1 - Y0;

        int steps = Math.Abs(dx) > Math.Abs(dy) ? Math.Abs(dx) : Math.Abs(dy);

        float Xinc = (float)dx / (float)steps;
        float Yinc = (float)dy / (float)steps;

        float X = X0;
        float Y = Y0;
        for (int i = 0; i <= steps; i++)    
        {
            if(type>=0)mapa[(int)X, (int)Y].type = type;
            mapa[(int)X, (int)Y].isWall = isWall;

            X += Xinc;
            Y += Yinc;
        }
    }
    void DrawLine(Point p1, Point p2, int type, bool isWall)
    {
        DrawLine(new PointInt((float)p1.X, (float)p1.Y), new PointInt((float)p2.X, (float)p2.Y), type, isWall);
    }

    void DrawLine2D(Point p1, Point p2, int type, bool isWall)
    {
        DrawLine(new PointInt((float)p1.X, (float)p1.Y), new PointInt((float)p2.X, (float)p1.Y), type, isWall);
        DrawLine(new PointInt((float)p2.X, (float)p1.Y), new PointInt((float)p2.X, (float)p2.Y), type, isWall);
    }


    void DrawRect(PointInt p1, int w, int h, int type, bool isWall)
    {
        int x0 = p1.x - w / 2;
        int y0 = p1.y - h / 2;
        int x1 = p1.x + w / 2;
        int y1 = p1.y + h / 2;
        if (w % 2 == 0) x0++;
        if (h % 2 == 0) y0++;
        for (int  i = y0; i <= y1; i++)
        {
            for (int j = x0; j <= x1; j++)
            {
                 mapa[j, i].type = type;
                 mapa[j, i].isWall = isWall;
            }
        }
    }
}