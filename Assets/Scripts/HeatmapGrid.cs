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

    public float GetHeat(Vector2Int cell)
    {
        return IsValid(cell) ? heatValues[cell.x, cell.y] : 0f;
    }

    public void SetHeat(Vector2Int cell, float value)
    {
        if (IsValid(cell)) heatValues[cell.x, cell.y] = value;
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
