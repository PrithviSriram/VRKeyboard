using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ReflectionObject : MonoBehaviour
{
    public GameObject m_ReflectionSurface;
    private GameObject m_ReflectionReference;
    private Vector3 m_ReflectionPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_ReflectionReference == null) return;
        m_ReflectionPosition = RetrieveReflectionPosition();
        SetPos();
    }

    public void SetReflectionReference(GameObject r) {
        m_ReflectionReference = r;
    }

    private Vector3 RetrieveReflectionPosition() {
        return new Vector3(m_ReflectionReference.transform.position.x, 0, m_ReflectionReference.transform.position.z);
    }
    private void SetPos() {
        this.transform.position = m_ReflectionPosition;
    }
}
