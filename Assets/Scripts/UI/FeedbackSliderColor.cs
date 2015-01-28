using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeedbackSliderColor : MonoBehaviour {

    public Slider feedbackSlider;

    public void UpdateSliderColor()
    {
        Color newcolor = new Color( 0f + feedbackSlider.value, 1 - (feedbackSlider.value), 0f);
        feedbackSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponentInChildren<Image>().color = newcolor;
    }
}
