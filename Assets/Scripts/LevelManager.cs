using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string levelName = "Level 1";
    [SerializeField] private GameObject levelCompleteUI;
    private BoxCollider2D endCollider;
    private bool hasBeenCompleted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenCompleted)
        {
            hasBeenCompleted = true;
            GameData.Instance.CompleteLevel(levelName);
            levelCompleteUI.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log($"[LevelManager] Level '{levelName}' completed!");
        }
    }
}
