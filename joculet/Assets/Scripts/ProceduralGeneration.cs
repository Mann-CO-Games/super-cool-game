using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    private void Start()
    {
        startPos = new Vector2Int(0, 0);
        endPos = new Vector2Int(gridWidth-1, gridHeight-1);
        
        grid = new int[gridWidth, gridHeight];
        FillGrid();
        ApplyAutomata();
        InstantiateTiles();
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

