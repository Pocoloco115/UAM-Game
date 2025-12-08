using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private RectTransform canvas;
    private float imageWidth;
    private float canvasWidth;
    private RectTransform rt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
        imageWidth = rt.rect.width;
        canvasWidth = canvas.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition += Vector2.left * speed * Time.deltaTime;
        if(rt.anchoredPosition.x <= -canvasWidth/2f)
        {
            rt.anchoredPosition = new Vector2(canvasWidth / 2f, rt.anchoredPosition.y);
        }
    }
}
