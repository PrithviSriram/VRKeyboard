using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitUIController : MonoBehaviour
{

    #region External References
    public GameObject RecordingUI;
    #endregion

    public void ToggleRecordingUI(bool toToggle) {
        RecordingUI.SetActive(toToggle);
        return;
    }
}
