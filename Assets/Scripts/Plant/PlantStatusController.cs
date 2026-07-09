using System.Collections;
using System.ComponentModel;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class PlantStatusController : MonoBehaviour
{
    //Remove when ash is implemented
    [SerializeField]
    private int ashQuantity = 0;

    
    public PlantStatus currentStatus;
    private PlantStatus startStatus;
    private PlantStatus lastStatus;
    private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        startStatus = PlantStatus.Healthy;
        currentStatus = startStatus;
        lastStatus = startStatus;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch (ashQuantity)
        {
            case 0:
                currentStatus = PlantStatus.Healthy;
                break;
            case 1:
                currentStatus = PlantStatus.Infesting;
                break;
            case 2:
                currentStatus = PlantStatus.Infested;
                break;
        }

        if (currentStatus != lastStatus)
        {
            ApplyStatusEffects();
        }

        lastStatus = currentStatus;
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
        yield return new WaitForSeconds(3);
        ashQuantity = 2;
        currentStatus = PlantStatus.Infested;
    }
}
