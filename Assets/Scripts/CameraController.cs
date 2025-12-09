using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Auto Scroll")]
    public float autoScrollSpeed = 1f;  

    [Header("Follow Player")]
    public float followSpeed = 2f; 
    public float followThreshold = 1f; 

    private float highestY;

    void Start()
    {
        highestY = transform.position.y;
    }

    void Update()
    {
        float newY = transform.position.y;

        newY += autoScrollSpeed * Time.deltaTime;

        if (player.position.y > newY + followThreshold)
        {
            newY = Mathf.Lerp(newY, player.position.y - followThreshold, followSpeed * Time.deltaTime);
        }

        if (newY < highestY)
        {
            newY = highestY;
        }

        highestY = newY;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void FreezeCamera()
    {
        autoScrollSpeed = 0f;
    }
}
