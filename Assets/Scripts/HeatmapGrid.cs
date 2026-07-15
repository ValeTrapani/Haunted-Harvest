using System;
using UnityEngine;

public class HeatmapGrid
{
    private float [,] heatValues;
    private int width, height;
    private bool[,] walkable;

    public HeatmapGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        walkable = new bool[width, height];
        heatValues = new float[width, height];
    }

    public Vector2Int FindHottestCell(Vector2Int currentCell, int searchRadius, int neighborRadius = 0)
    {
        Vector2Int best = currentCell;
        float bestHeat = -1f;
        int bestDistance = int.MaxValue;
        float bestLocalHeat = -1f;

        for (int x = -searchRadius; x <= searchRadius; x++)
        {
            for (int y = -searchRadius; y <= searchRadius; y++)
            {
                Vector2Int cell = currentCell + new Vector2Int(x, y);

                if (!IsWalkable(cell))
                    continue;

                float heat = GetHeat(cell);
                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                float localHeat = neighborRadius > 0 ? GetLocalHeatSum(cell, neighborRadius) : 0f;

                bool isBetter = false;

                if (heat > bestHeat)
                {
                    isBetter = true;
                }
                else if (heat == bestHeat)
                {
                    if (distance < bestDistance)
                    {
                        isBetter = true;
                    }
                    else if (distance == bestDistance)
                    {
                        if (neighborRadius > 0 && localHeat > bestLocalHeat)
                        {
                            isBetter = true;
                        }
                    }
                }

                if (isBetter)
                {
                    best = cell;
                    bestHeat = heat;
                    bestDistance = distance;
                    bestLocalHeat = localHeat;
                }
            }
        }

        return best;
    }

    private float GetLocalHeatSum(Vector2Int cell, int radius)
    {
        float sum = 0f;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int neighbor = cell + new Vector2Int(x, y);
                if (IsWalkable(neighbor))
                    sum += GetHeat(neighbor);
            }
        }

        return sum;
    }

    public float GetHeat(Vector2Int cell)
    {
        return IsValid(cell) ? heatValues[cell.x, cell.y] : 0f;
    }

    public event Action<Vector2Int, float> OnHeatChanged;

    public void SetHeat(Vector2Int cell, float value)
    {
        if (IsValid(cell))
        {
            heatValues[cell.x, cell.y] = value;
            OnHeatChanged?.Invoke(cell, value);
        }
    }

    public void SetWalkable(Vector2Int cell, bool value)
    {
        if (IsValid(cell)) walkable[cell.x, cell.y] = value;
    }

    public bool IsWalkable(Vector2Int cell) =>
        IsValid(cell) && walkable[cell.x, cell.y];
    public bool IsValid(Vector2Int cell) =>
        cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;

    public int Width => width;
    public int Height => height;
}
