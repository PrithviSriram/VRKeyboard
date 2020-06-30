using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTypeController2 : MonoBehaviour
{
    private KeyboardController2 m_KeyboardController = null;
    [SerializeField]
    private List<Key> m_keys = new List<Key>();
    public List<Key> keys 
    {
        get {   return m_keys;  }
        set {   m_keys = value; }
    }
    [SerializeField]
    private int m_defaultIndex = 0;
    public int defaultIndex 
    {
        get {   return m_defaultIndex;  }
        set {   m_defaultIndex = value; }
    }
    [SerializeField]
    private List<Vector4> m_thumbstickAngleMapping = new List<Vector4>();
    public List<Vector4> thumbstickAngleMapping
    {
        get {   return m_thumbstickAngleMapping;    }
        set {   m_thumbstickAngleMapping = value;   }
    }
    [SerializeField]
    private bool m_isActive = false;
    public bool isActive
    {
        get {   return m_isActive;  }
        set {   m_isActive = value; }
    }

    public void Initialize(KeyboardController2 c) {
        m_KeyboardController = c;
        for (int i = 0; i < m_keys.Count; i++) {
            m_keys[i].gameObject.SetActive(true);
            m_keys[i].Initialize(c, i);
        }
    }
    public void Activate() {
        m_isActive = true;
        for (int i = 0; i < m_keys.Count; i++) {
            m_keys[i].gameObject.SetActive(true);
            m_keys[i].ResetKey(i);
        }
    }
    public void Deactivate() {
        m_isActive = false;
        for (int i = 0; i < m_keys.Count; i++) {
            m_keys[i].gameObject.SetActive(false);
        }
    }
}
