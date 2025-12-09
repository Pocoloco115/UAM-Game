using UnityEngine;

public class LevelBackground : MonoBehaviour
{
    [Header("Camera reference")]
    [SerializeField] private Camera camera;

    [Header("Settings")]
    [SerializeField] private float speed;

    private SpriteRenderer spriteRenderer;
    private float spriteWidth;
    private float cameraWidth;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = spriteRenderer.sprite.bounds.size.x * transform.lossyScale.x;
        cameraWidth = camera.orthographicSize * 2f * camera.aspect;
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x + spriteWidth / 2f <= -cameraWidth / 2f)
        {
            transform.position = new Vector3(cameraWidth / 2f + spriteWidth / 2f, transform.position.y, transform.position.z);
        }
    }
}
