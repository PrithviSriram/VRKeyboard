using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTypeControllerVer2 : MonoBehaviour
{

    private KeyboardControllerVer2 m_KeyboardController = null;

    [SerializeField]
    private List<KeysContainer> m_keysContainers = new List<KeysContainer>();
    public List<KeysContainer> keysContainers {
        get {   return m_keysContainers; }
    }
    [SerializeField]
    private int m_keysContainerCurrent = 0;
    public int keysContainerCurrent {
        get {   return m_keysContainerCurrent;  }
        set {   m_keysContainerCurrent = (value > m_keysContainers.Count-1) ? m_keysContainers.Count-1 : value; }
    }

    // NOT SERIALIZED
    public List<KeyboardKeyVer2> keys {
        get {   return m_keysContainers[m_keysContainerCurrent].keys;    }
    }

    // NOT SERIALIZED
    public int defaultIndex {
        get {   return m_keysContainers[m_keysContainerCurrent].defaultIndex;   }
    }

    // NOT SERIALIZED
    public List<Vector4> thumbstickAngleMapping
    {
        get {   return m_keysContainers[m_keysContainerCurrent].thumbstickAngleMapping; }
    }

    [SerializeField]
    private KeyboardWordPreviewVer2 m_Preview;
    public KeyboardWordPreviewVer2 preview {
        get {   return m_Preview;   }
        set {   m_Preview = value;  }
    }
    [SerializeField]
    private KeyboardOverlayVer2 m_Overlay = null;
    public KeyboardOverlayVer2 overlay {
        get {   return m_Overlay;   }
        set {   m_Overlay = value;  }
    }
    [SerializeField]
    private RectTransform m_cursor;
    public RectTransform cursor {
        get {   return m_cursor;    }
    }

    private void Update() {
        //m_keysContainers[m_keysContainerCurrent].GetComponent<CanvasGroup>().alpha = 1f;
        for (int i = 0; i < m_keysContainers.Count; i++) {
            if (i == m_keysContainerCurrent) continue;
            //m_keysContainers[i].GetComponent<CanvasGroup>().alpha = (i != 0) ? 0f : 0.25f;
            foreach(KeyboardKeyVer2 k in m_keysContainers[i].keys) {
                k.ResetKey();
            }
        }
    }

}
