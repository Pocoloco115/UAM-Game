using UnityEngine;

public class LevelBackground : MonoBehaviour
{
    [Header("Camera reference")]
    [SerializeField] private Camera camera;

    [Header("Settings")]
    [SerializeField] private float speed;

    private Transform transform;
    private float spriteWidth;
    private float cameraWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform = GetComponent<Transform>();
        spriteWidth = transform.localScale.x;
        cameraWidth = camera.orthographicSize * camera.aspect * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if(transform.position.x <= -cameraWidth/2f - spriteWidth * 2)
        {
            transform.position = new Vector3(cameraWidth / 2f + spriteWidth/2f, transform.position.y, transform.position.z);
        }
    }
}
