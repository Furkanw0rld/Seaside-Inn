using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalculateFPS : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float msec = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        msec = deltaTime * 1000.0f;
        text.text = string.Format("{0:0.} fps ({1:0.0} ms)", fps, msec);
    }
}
