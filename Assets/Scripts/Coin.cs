using UnityEngine;

public class Coin : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().coinCounter++;
            GameData.Instance.AddCoins(1);
            boxCollider2D.enabled = false;
            anim.SetTrigger("wasCollected");
        }
    }
    public void DestroyCoin()
    {
        Destroy(gameObject);
    }
}
