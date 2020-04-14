using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float globalStartTime;
    Text UIText;

    // Start is called before the first frame update
    void Start()
    {
        globalStartTime = Time.time;
        UIText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int timeAsInt = (int) Time.time;
        UIText.text = timeAsInt.ToString();
    }
}
