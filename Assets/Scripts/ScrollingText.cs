using UnityEngine;
using System.Collections;

public class ScrollingText : MonoBehaviour
{
    public string Text
    {
        get { return guiText.text; }
        set
        {
            guiText.text = value;

            GUIText[] children = GetComponentsInChildren<GUIText>();
            foreach (GUIText child in children)
            {
                child.text = value;
            }
        }
    }

    public Color TextColor
    {
        get { return guiText.color; }
        set
        {
            guiText.color = value;
            m_Alpha = value.a;
        }
    }

    public float m_ScrollSpeed = 0.1f; //Public for editor
    public float m_Duration = 1.5f; 
    private float m_Alpha = 1.0f;

    private void Start()
    {}

    private void Update()
    {
        if (m_Alpha > 0)
        {
            transform.Translate(0.0f, m_ScrollSpeed * Time.deltaTime, 0.0f);
            m_Alpha -= Time.deltaTime / m_Duration;

            Color col = guiText.color;
            guiText.color = new Color(col.r, col.g, col.b, m_Alpha);
        } 
        else
        {
            Destroy(gameObject); // text vanished - destroy itself
        }
    }
}
