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

    private readonly struct Candidate
    {
        public readonly Vector2Int Cell;
        public readonly float Heat;
        public readonly int Distance;
        public readonly float LocalHeat;

        public Candidate(Vector2Int cell, float heat, int distance, float localHeat)
        {
            Cell = cell;
            Heat = heat;
            Distance = distance;
            LocalHeat = localHeat;
        }
    }

    public Vector2Int FindHottestCell(Vector2Int currentCell, int searchRadius, int neighborRadius = 0, float minCurableHeat = 0f)
    {
        Candidate best = default;
        bool hasBest = false;

        for (int x = -searchRadius; x <= searchRadius; x++)
        {
            for (int y = -searchRadius; y <= searchRadius; y++)
            {
                Vector2Int cell = currentCell + new Vector2Int(x, y);

                if (!IsWalkable(cell))
                    continue;

                float heat = GetHeat(cell);
                if (heat < minCurableHeat)
                    continue;

                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                float localHeat = neighborRadius > 0 ? GetLocalHeatSum(cell, neighborRadius) : 0f;

                Candidate candidate = new Candidate(cell, heat, distance, localHeat);

                if (!hasBest || IsBetterCandidate(candidate, best))
                {
                    best = candidate;
                    hasBest = true;
                }
            }
        }

        return hasBest ? best.Cell : currentCell;
    }

    private bool IsBetterCandidate(Candidate candidate, Candidate currentBest)
    {
        if (candidate.Heat > currentBest.Heat) return true;
        if (candidate.Heat < currentBest.Heat) return false;

        if (candidate.Distance < currentBest.Distance) return true;
        if (candidate.Distance > currentBest.Distance) return false;

        return candidate.LocalHeat > currentBest.LocalHeat;
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
