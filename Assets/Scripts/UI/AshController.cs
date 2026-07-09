using UnityEngine;

public class AshController : MonoBehaviour
{
    public int ashQuantity;

    [SerializeField]
    private TMPro.TextMeshProUGUI ashText;
    void Awake()
    {
        ashQuantity = 0;
    }

    void Update()
    {
        
    }

    public void AddAsh(int amount)
    {
        ashQuantity += amount;
        Debug.Log("Actual ash : " + ashQuantity);
        ashText.text = "Total ash: " + ashQuantity.ToString();
    }
}
