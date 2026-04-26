using UnityEngine;

public class DataUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI coinsText;
    [SerializeField] private TMPro.TextMeshProUGUI counterText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coinsText.text = Player.Instance.coinCounter.ToString();
        counterText.text = Player.Instance.GetCounter().ToString();

    }
}
