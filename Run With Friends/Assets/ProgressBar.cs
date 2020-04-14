using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    RectTransform rectTransform;

    float leftX;
    float rightX;

    public RectTransform filledBar;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        leftX = rectTransform.position.x - (rectTransform.rect.width / 2);
        rightX = rectTransform.position.x + (rectTransform.rect.width / 2);
    }

    //Update the visuals on the progress bar given map percentage traveled (as a float between 0 and 1)
    public void UpdateProgress(float fractionComplete)
    {
        //set the center of the completed portion of the bar
        filledBar.position = new Vector3(((rightX - leftX) * fractionComplete) / 2f + leftX, rectTransform.position.y, 0);

        //set the width of the completed portion of the bar
        filledBar.sizeDelta = new Vector2((rightX - leftX) * fractionComplete, filledBar.sizeDelta.y);
    }
}
