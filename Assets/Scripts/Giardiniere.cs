using System.Collections.Generic;
using UnityEngine;

public class Giardiniere : MonoBehaviour
{
    [SerializeField] private GardenManager gardenManager;
    [SerializeField] private int searchRadius = 5;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int neighborRadius = 1;

    private HeatmapGrid heatmap;
    private List<Vector2Int> currentPath;
    private int pathIndex;
    private Vector2Int currentTarget;
    private bool isBusy; 

    void Start()
    {
        heatmap = gardenManager.Grid;
        heatmap.OnHeatChanged += HandleHeatChanged;

        
        HandleHeatChanged(default, 0f);
    }

    void OnDisable()
    {
        if (heatmap != null)
            heatmap.OnHeatChanged -= HandleHeatChanged;
    }

    Vector2Int GetMyCell()
    {
        return gardenManager.WorldToHeatmapCell(transform.position);
    }

    void HandleHeatChanged(Vector2Int changedCell, float newHeat)
    {
        Vector2Int myCell = GetMyCell();
        Vector2Int bestCell = heatmap.FindHottestCell(myCell, searchRadius, neighborRadius);

        if (bestCell != currentTarget)
        {
            RecalculatePath(myCell, bestCell);
        }
    }

    void RecalculatePath(Vector2Int myCell, Vector2Int bestCell)
    {
        currentTarget = bestCell;
        currentPath = AStarPathfinding.FindPath(heatmap, myCell, bestCell);
        pathIndex = 0;
        isBusy = false;
    }

    void Update()
    {
        if (isBusy) return;
        if (currentPath == null || pathIndex >= currentPath.Count) return;

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Vector3 targetWorldPos = gardenManager.HeatmapCellToWorld(currentPath[pathIndex]);
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

        if (HasReachedCurrentWaypoint(targetWorldPos))
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
            {
                isBusy = true;
                // TODO: animazione/logica di annaffiatura
            }
        }
    }

    bool HasReachedCurrentWaypoint(Vector3 targetWorldPos)
    {
        return Vector3.Distance(transform.position, targetWorldPos) < 0.05f;
    }
}