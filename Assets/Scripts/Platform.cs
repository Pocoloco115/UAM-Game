using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Player Reference")]
    private bool isSafe = true;
    private Camera mainCamera;
    private Transform player;

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
        mainCamera = Camera.main;
        player = Player.Instance.transform;
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

    private void CleanPlatform()
    {
        Vector2 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool isBellowViewport = viewportPoint.y < 0;
        bool isHorizontallyWithinViewport = viewportPoint.x >= 0 && viewportPoint.x <= 1;
        if (isBellowViewport && isHorizontallyWithinViewport)
        {
            StartCoroutine(DestroyPlatform());
        }
    }

    private IEnumerator DestroyPlatform()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
