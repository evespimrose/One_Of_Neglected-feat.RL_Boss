using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererTarget : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private void Awake()
    {
        if (m_spriteRenderer == null)
            m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        try
        {
            GameManager.Instance.allSpriteRenderer.Add(m_spriteRenderer);

        }
        catch
        {
            //Debug.Log(gameObject.name);

        }
    }
    private void OnDisable()
    {
        GameManager.Instance.allSpriteRenderer.Remove(m_spriteRenderer);
    }
}
