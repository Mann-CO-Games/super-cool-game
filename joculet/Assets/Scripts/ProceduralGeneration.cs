using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class BFS
{
    int[,] grid;
    Vector2Int startPos;
    Vector2Int endPos;
    int gridWidth;
    int gridHeight;

    public BFS(int[,] grid, Vector2Int startPos, Vector2Int endPos, int gridWidth, int gridHeight)
    {
        this.grid = grid;
        this.startPos = startPos;
        this.endPos = endPos;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
    }

    public List<Vector2Int> FindPath()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        queue.Enqueue(startPos);
        cameFrom[startPos] = startPos;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == endPos)
            {
                return ReconstructPath(cameFrom, current);
            }

            List<Vector2Int> neighbors = GetNeighbors(current);
            foreach (Vector2Int neighbor in neighbors)
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(current);

        while (current != startPos)
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int[,] directions = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        for (int i = 0; i < 4; i++)
        {
            int x = current.x + directions[i, 0];
            int y = current.y + directions[i, 1];

            if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight && grid[x, y] == 0)
            {
                neighbors.Add(new Vector2Int(x, y));
            }
        }

        return neighbors;
    }
}


public class ProceduralGeneration : MonoBehaviour
{ 
    public Tilemap tilemap = new Tilemap();

    public TileBase[] walkableTiles;
    public TileBase[] wallTiles;
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float fillPercentage = 0.4f;
    public Vector2Int startPos;
    public Vector2Int endPos;
    public GameObject walkableTile;
    public GameObject obstacleTile;
    [SerializeField] private int iterations;
    private int[,] grid;
    public Tile testTile;
    private void Start()
    {
        
        

       
        startPos = new Vector2Int(0, 0);
        endPos = new Vector2Int(gridWidth-1, gridHeight-1);


        do
        {
            ConstructGrid();
        } while (!ConstructGrid());



    }

    private bool ConstructGrid()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        grid = new int[gridWidth, gridHeight];
        FillGrid();
        ApplyAutomata();
        InstantiateTiles();
        
        var startpositions = getBottomStartingPoints();
        var endpositions = getTopEndPoints();
        foreach(var s in startpositions)
        foreach (var e in endpositions)
        {
            BFS bfs = new BFS(grid,s,e,gridWidth,gridHeight);
            path = bfs.FindPath();
            if (path != null )
            {
                foreach (var p in path)
                {
                    tilemap.SetTile(new Vector3Int(p.x, p.y, 0), testTile);
                }

                return true;
            }

        }

        return false;
    }

    private List<Vector2Int> getBottomStartingPoints()
    {
        List<Vector2Int> startingPositions = new List<Vector2Int>();

        for(int i=0;i<gridWidth;i++)
            if(grid[0,i] == 0)
                startingPositions.Add(new Vector2Int(i,0));
                

        return startingPositions;
    }
    private List<Vector2Int> getTopEndPoints()
    {
        List<Vector2Int> startingPositions = new List<Vector2Int>();

        for(int i=0;i<gridWidth;i++)
            if(grid[gridHeight-1,i]==0)
                startingPositions.Add(new Vector2Int(i,gridHeight-1));
                

        return startingPositions;
    }
    private void FillGrid()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (i == startPos.x && j == startPos.y)
                {
                    grid[i, j] = 0;
                }
                else if (i == endPos.x && j == endPos.y)
                {
                    grid[i, j] = 0;
                }
                else if (Random.value < fillPercentage)
                {
                    grid[i, j] = 1;
                }
                else
                {
                    grid[i, j] = 0;
                }
            }
        }
    }

    private void ApplyAutomata()
    {
        for (int i = 0; i < iterations; i++)
        {
            int[,] newGrid = new int[gridWidth, gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    int neighborWallTiles = GetSurroundingWallCount(x, y);

                    if (neighborWallTiles > 4)
                        newGrid[x, y] = 1;
                    else if (neighborWallTiles < 4)
                        newGrid[x, y] = 0;
                    else
                        newGrid[x, y] = grid[x, y];
                }
            }

            grid = newGrid;
        }
    }

    private int GetSurroundingWallCount(int x, int y)
    {
        int wallCount = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX >= 0 && neighborX < gridWidth && neighborY >= 0 && neighborY < gridHeight)
                {
                    if (grid[neighborX, neighborY] == 1)
                    {
                        wallCount++;
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    

    private void InstantiateTiles()
    {
        
        
        for (int i = 0; i < gridWidth; i++)
        {   GameObject tile = new GameObject();
            for (int j = 0; j < gridHeight; j++)
            {
                
                Vector2 tilePosition = new Vector3(i, j);

                if (grid[i, j] == 0)
                {
                   
                    
                        if (Random.value < 0.3f) // 30% chance of spawning an obstacle
                        {
                            int x = Random.Range(0, tilemap.size.x);
                            int y = Random.Range(0, tilemap.size.y);
                            int randomTileIndex = Random.Range(0, wallTiles.Length);
                            tilemap.SetTile(new Vector3Int(i, j, 0), wallTiles[randomTileIndex]);
                        }
                        else
                        {
                            int x = Random.Range(0, tilemap.size.x);
                            int y = Random.Range(0, tilemap.size.y);
                            int randomTileIndex = Random.Range(0, walkableTiles.Length);
                            tilemap.SetTile(new Vector3Int(i, j, 0), walkableTiles[randomTileIndex]);
                        }
                    }
                

                else
                {
                    int x = Random.Range(0, tilemap.size.x);
                    int y = Random.Range(0, tilemap.size.y);
                    int randomTileIndex = Random.Range(0, wallTiles.Length);
                    tilemap.SetTile(new Vector3Int(i, j, 0), wallTiles[randomTileIndex]);
                }

                tile.transform.parent = this.transform;
            }
        }
    }

}

