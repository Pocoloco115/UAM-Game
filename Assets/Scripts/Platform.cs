using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;
    private bool isSafe = true;

    private void Awake()
    {
        Transform spikes = transform.Find("SpikeArea");
        if (spikes != null)
        {
            isSafe = false;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CleanPlatform();
    }

    public bool IsSafe()
    {
        return isSafe;
    }

    public void CleanPlatform()
    {
        if(transform.position.y < player.position.y - 10)
        {
            Destroy(gameObject);
        }
    }
}
