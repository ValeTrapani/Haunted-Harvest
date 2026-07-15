using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GardenManager : MonoBehaviour
{
    [Header("Riferimenti Tilemap")]
    [SerializeField] private Tilemap wallTilemap;
    private List<Vector2Int> testPath;

    private HeatmapGrid heatmap;
    private BoundsInt bounds;

    public HeatmapGrid Grid => heatmap;


    void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        if (wallTilemap == null)
        {
            Debug.LogError("Assegna la Wall Tilemap nell'inspector di GardenManager!");
            return;
        }

        wallTilemap.CompressBounds();
        bounds = wallTilemap.cellBounds;

        heatmap = new HeatmapGrid(bounds.size.x, bounds.size.y);

        ScanMap();
    }

    void ScanMap()
    {
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);

                bool hasWall = wallTilemap.HasTile(tilePos);
                heatmap.SetWalkable(new Vector2Int(x, y), !hasWall);
            }
        }
    }

    void Start()
    {
        if (heatmap.IsWalkable(new Vector2Int(1, 1)))
        {
            heatmap.SetHeat(new Vector2Int(1, 1), 1.0f);
            heatmap.SetHeat(new Vector2Int(2, 1), 0.6f);
            heatmap.SetHeat(new Vector2Int(5, 6), 1.0f);
            heatmap.SetHeat(new Vector2Int(5, 2), 1.0f);
            heatmap.SetHeat(new Vector2Int(5, 7), 1.0f);
        }
    }
    public Vector2Int WorldToHeatmapCell(Vector3 worldPos)
    {
        Vector3Int cell = wallTilemap.WorldToCell(worldPos);
        return new Vector2Int(cell.x - bounds.xMin, cell.y - bounds.yMin);
    }

    public Vector3 HeatmapCellToWorld(Vector2Int cell)
    {
        Vector3Int realCell = new Vector3Int(cell.x + bounds.xMin, cell.y + bounds.yMin, 0);
        return wallTilemap.GetCellCenterWorld(realCell);
    }
    void OnDrawGizmos()
    {
        
        if (wallTilemap == null) return;

        BoundsInt currentBounds = wallTilemap.cellBounds;

        int width = heatmap != null ? heatmap.Width : currentBounds.size.x;
        int height = heatmap != null ? heatmap.Height : currentBounds.size.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Vector3Int tilePos = new Vector3Int(x + currentBounds.xMin, y + currentBounds.yMin, 0);

                Vector3 worldPos = wallTilemap.GetCellCenterWorld(tilePos);

                if (heatmap != null)
                {
                    if (!heatmap.IsWalkable(cell))
                    {
                        Gizmos.color = new Color(0, 0, 0, 0.7f); // Muro
                    }
                    else if (testPath != null && testPath.Contains(cell))
                    {
                        Gizmos.color = new Color(0f, 0.8f, 1f, 0.8f); // Cella del percorso (Azzurro)
                    }
                    else
                    {
                        Color heatColor = Color.Lerp(Color.green, Color.red, heatmap.GetHeat(cell));
                        heatColor.a = 0.5f;
                        Gizmos.color = heatColor;
                    }
                }


                Gizmos.DrawCube(worldPos, new Vector3(0.9f, 0.9f, 0.1f));
            }
        }
    }
}