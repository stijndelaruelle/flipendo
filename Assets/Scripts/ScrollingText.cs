using UnityEngine;
using System.Collections;

public class ScrollingText : MonoBehaviour
{
    public Color color = new Color(0.8f, 0.8f, 0.0f, 1.0f);
    public float scroll = 0.05f; // scrolling velocity
    public float duration = 1.5f; // time to die
    public float alpha;

    private void Start()
    {
        //guiText.material.color = color; // set text color
        alpha = 1;
    }

    private void Update()
    {
        if (alpha > 0)
        {
            transform.Translate(0.0f, scroll * Time.deltaTime, 0.0f);
            alpha -= Time.deltaTime / duration;
            //guiText.material.color.a = alpha;
        } 
        else
        {
            Destroy(gameObject); // text vanished - destroy itself
        }
    }
}
