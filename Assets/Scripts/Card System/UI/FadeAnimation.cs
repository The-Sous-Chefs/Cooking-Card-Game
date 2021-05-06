using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private IEnumerator FadeIn()
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            Color c = GetComponent<Renderer>().material.color;
            c.a = f;
            GetComponent<Renderer>().material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void startFading()
    {
        Debug.Log("faded");
        StartCoroutine("FadeIn");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
