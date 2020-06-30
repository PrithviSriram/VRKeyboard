using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandCanvasController : MonoBehaviour
{
    //public bool isHeld;
    //public GameObject m_HeldUI;

    public TextMeshProUGUI DefaultRecordText;
    public Slider RecordSliderIU;
    public GameObject RecordingText;

    /*
    public GameObject DefaultDeleteText;
    public Slider DeleteSliderUI;
    public GameObject DeletingText;
    */

    private float TimeThreshold;

    private void Update() {
        /*
        if (isHeld) {
            m_HeldUI.SetActive(true);
        }
        else {
            m_HeldUI.SetActive(false);
        }
        */
    }

    public void InitializeRecordSlider(float threshold) {
        TimeThreshold = threshold;
        RecordSliderIU.maxValue = threshold * 100f;
        DeactivateRecordSlider();
    }
    /*
    public void InitializeDeleteSlider(float threshold) {
        TimeThreshold = threshold;
        DeleteSliderUI.maxValue = threshold * 100f;
    }
    */

    public void StartRecordSlider() {
        RecordSliderIU.value = 0;
        DefaultRecordText.text = "3";
        this.gameObject.SetActive(true);
    }
    public void SetRecordSlider(float curTime) {
        RecordSliderIU.value = (curTime < TimeThreshold) ? curTime*100f : TimeThreshold*100f;
        int TimeRemaining = (curTime < TimeThreshold) ? (int)Mathf.Ceil(TimeThreshold - curTime) : 0;
        if (curTime >= TimeThreshold) {
            DefaultRecordText.text = "Recording";
        } else {
            DefaultRecordText.text = TimeRemaining.ToString();
        }
    }
    public void DeactivateRecordSlider() {
        this.gameObject.SetActive(false);
        RecordSliderIU.value = 0;
        DefaultRecordText.text = TimeThreshold.ToString();
    }
    /*
    public void StartDeleting() {
        DeleteSliderUI.gameObject.SetActive(true);
        DeleteSliderUI.value = 0;
    }
    public void SetDeleteSlider(float curTime) {
        DeleteSliderUI.value = (curTime < TimeThreshold) ? curTime * 100f : TimeThreshold*100f;
        if (curTime >= TimeThreshold) DeletingText.SetActive(true);
    }
    public void DeactivateDeleteSlider() {
        DeleteSliderUI.gameObject.SetActive(false);
    }
    */
}
