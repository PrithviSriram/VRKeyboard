using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class KeyboardTextbox : MonoBehaviour
{
    [SerializeField]
    private KeyboardControllerVer2 m_KeyboardController;
    public KeyboardControllerVer2 keyboardController {
        get {   return m_KeyboardController;    }
        set {   m_KeyboardController = value;   }
    }
    [SerializeField]
    private TextMeshProUGUI m_thisText;
    public string text {
        get {   return m_thisText.text;     }
        set {   m_thisText.text = value;    }
    }

    /*
    private void Awake() {
        StartCoroutine(CustomUpdate());
    }
    private IEnumerator CustomUpdate() {
        while(true) {
            if (m_KeyboardController == null) {
                yield return null;
                continue;
            }
            if (m_KeyboardController.targetText == null) {
                m_KeyboardController = null;
                yield return null;
                continue;
            }
            m_thisText.text = m_KeyboardController.text;
            yield return null;
        }
    }
    */
    private void Update() {
        if (m_KeyboardController == null) return;
        if (m_KeyboardController.targetText == null) {
            m_KeyboardController = null;
            return;
        }
        m_thisText.text = m_KeyboardController.text;
    }
}
