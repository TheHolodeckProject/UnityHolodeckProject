using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeedbackSliderColor : MonoBehaviour {

    public Slider feedbackSlider;
    public Image highAccPanel;

    public void UpdateSliderColor()
    {
        Color newcolor = new Color( 0f + feedbackSlider.value, 1 - (feedbackSlider.value), 0f);
        feedbackSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponentInChildren<Image>().color = newcolor;
    }

    public void AccuracyMarkers()
    {
        if (feedbackSlider.value > .25f)
        {
            highAccPanel.color = new Color(.25f, .75f, 0f);
        }

    }
}
