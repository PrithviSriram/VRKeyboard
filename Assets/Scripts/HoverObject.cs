using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverObject : MonoBehaviour
{
    private Transform m_centerEyeAnchor;
    public TextMeshProUGUI m_TextMesh;
    Vector3 startScale;
    Vector3 textStartScale;
    //float textXDistance;
    //private bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
        startScale = transform.localScale;
        textStartScale = m_TextMesh.transform.localScale;
        //textXDistance = m_TextMesh.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(m_centerEyeAnchor);
        float dist = Vector3.Distance(m_centerEyeAnchor.position, this.transform.position);
        Vector3 newScale = startScale + Vector3.one * 0.1f * dist;
        Vector3 newTextScale = textStartScale + Vector3.one * 0.1f * dist;
        //float textDistMultiple = newTextScale.x / textStartScale.x;
        transform.localScale = newScale;
        m_TextMesh.transform.localScale = newTextScale;
        //m_TextMesh.transform.position = new Vector3(textXDistance * textDistMultiple, m_TextMesh.transform.position.y, m_TextMesh.transform.position.z);
    }

    public void SetText(string t) {
        m_TextMesh.text = t;
    }

}
