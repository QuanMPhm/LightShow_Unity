using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashLineController : MonoBehaviour
{
    public SpriteRenderer renderer;
    public float fadeSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        for (float ft = 1f; ft >= 0; ft -= fadeSpeed)
        {
            Color c = renderer.material.color;
            c.a = ft;
            renderer.material.color = c;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (renderer.material.color.a < fadeSpeed) Destroy(gameObject);
    }
}
