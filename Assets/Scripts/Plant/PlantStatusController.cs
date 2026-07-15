using System.Collections;
using UnityEngine;

public class PlantStatusController : MonoBehaviour
{
    [SerializeField] private int ashQuantity = 0;
    [SerializeField] private float infestationTime = 3f;

    public PlantStatus currentStatus;
    private PlantStatus startStatus;
    private PlantStatus lastStatus;
    private SpriteRenderer spriteRenderer;

    private GardenManager gardenManager;
    private Vector2Int myCell;

    void Awake()
    {
        startStatus = PlantStatus.Healthy;
        currentStatus = startStatus;
        lastStatus = startStatus;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        gardenManager = FindAnyObjectByType<GardenManager>();
        myCell = gardenManager.WorldToHeatmapCell(transform.position);
        Debug.Log($"Pianta registrata in cella: {myCell}");
        gardenManager.RegisterPlant(myCell, this);
        UpdateHeat();
    }

    void Update()
    {
        switch (ashQuantity)
        {
            case 0: currentStatus = PlantStatus.Healthy; break;
            case 1: currentStatus = PlantStatus.Infesting; break;
            case 2: currentStatus = PlantStatus.Infested; break;
        }

        if (currentStatus != lastStatus)
        {
            ApplyStatusEffects();
            UpdateHeat();
        }

        lastStatus = currentStatus;
    }

    void UpdateHeat()
    {
        float heat = currentStatus switch
        {
            PlantStatus.Healthy => 0f,
            PlantStatus.Infesting => 1f,
            PlantStatus.Infested => 0.1f,
            _ => 0f
        };
        gardenManager.Grid.SetHeat(myCell, heat);
    }

    public void Water()
    {
        StopAllCoroutines();
        ashQuantity = 0;
        currentStatus = PlantStatus.Healthy;
        ApplyStatusEffects();
        UpdateHeat();
    }

    void ApplyStatusEffects()
    {
        switch (currentStatus)
        {
            case PlantStatus.Healthy:
                spriteRenderer.color = Color.yellow;
                break;
            case PlantStatus.Infesting:
                spriteRenderer.color = Color.cyan;
                StartCoroutine(InfestingStatus());
                break;
            case PlantStatus.Infested:
                spriteRenderer.color = Color.black;
                break;
        }
    }

    IEnumerator InfestingStatus()
    {
        yield return new WaitForSeconds(infestationTime);
        ashQuantity = 2;
        currentStatus = PlantStatus.Infested;
    }
}
