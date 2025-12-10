using UnityEngine;

public class CounterManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject counter;
    [SerializeField] private TMPro.TextMeshProUGUI counterText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counterText.text = counter.GetComponent<Player>().GetCounter().ToString();
    }
}
