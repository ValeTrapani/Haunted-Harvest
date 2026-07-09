using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlantAshFarming : MonoBehaviour
{
    [SerializeField]
    private int ashFarmingRate = 1;

    [SerializeField]
    private float ashFarmingInterval = 1f;

    AshController ashController;

    private PlantStatusController plantStatus;

    private bool isFarmingAsh = false;

    void Awake()
    {
        plantStatus = GetComponent<PlantStatusController>();

        ashController = FindAnyObjectByType<AshController>();

    }

    void Update()
    {
        if (plantStatus.currentStatus == PlantStatus.Infested && !isFarmingAsh)
        {
            isFarmingAsh = true;
            StartCoroutine(AshFarming());
        }
        
    }

    IEnumerator AshFarming()
    {
        Debug.Log("Coroutine");
        yield return new WaitForSeconds(ashFarmingInterval);

        ashController.AddAsh(ashFarmingRate);

        isFarmingAsh = false;
    }
}
