using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnlyRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            renderer.enabled = false;
        }
    }
}
