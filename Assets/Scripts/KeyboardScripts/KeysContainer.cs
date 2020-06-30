using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysContainer : MonoBehaviour
{
    [SerializeField]
    private List<KeyboardKeyVer2> m_keys = new List<KeyboardKeyVer2>();
    public List<KeyboardKeyVer2> keys 
    {
        get {   return m_keys;  }
        set {   m_keys = value; }
    }
    [SerializeField]
    private List<Vector4> m_thumbstickAngleMapping = new List<Vector4>();
    public List<Vector4> thumbstickAngleMapping
    {
        get {   return m_thumbstickAngleMapping;    }
        set {   m_thumbstickAngleMapping = value;   }
    }
    [SerializeField]
    private int m_defaultIndex = 0;
    public int defaultIndex 
    {
        get {   return m_defaultIndex;  }
        set {   m_defaultIndex = value; }
    }
}
