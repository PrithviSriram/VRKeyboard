using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_Pointer : MonoBehaviour
{
    public GameObject m_HoverObjectPrefab;
    public bool DebugToggle;
    public Transform refTransform;
    private LineRenderer m_LineRenderer;
    private Vector3[] positions = new Vector3[2];

    private int layerMask = 1 << 8;
    private RaycastHit hit;
    private Star hitObj = null;
    private Star targetRef = null;
    public GameObject TestRaycast;
    private Vector3 fromPos, toPos;
    private GameObject m_HoverObject = null;

    private void Awake() {
        m_LineRenderer = this.GetComponent<LineRenderer>();
        if (!DebugToggle) {
            TestRaycast.SetActive(false);
        } else {
            TestRaycast.SetActive(true);
        }
        SetPositions(Vector3.zero, Vector3.zero);
        StartCoroutine(GenerateLine());
    }

    // Coroutine to print the line - operates outside the update function
    private IEnumerator GenerateLine() {
        while(true) {
            m_LineRenderer.SetPositions(positions);
            yield return null;
        }
    }

    public void SetPositions(Vector3 from, Vector3 to) {
        positions[0] = from;
        positions[1] = to;
    }
    public void SetPosition(Vector3 to) {
        positions[0] = refTransform.transform.position;
        positions[1] = to;
    }

    private void Update() {
        // If a target isn't set, 1) calculate the positions of the line renderer/raycast, and 2) perform the raycast
        // else, just set the positions of the line and raycast to the ref and target
        if (targetRef == null) {
            CalculatePositions();
            PerformRaycast();
            SetPositions(fromPos, toPos);
        } else {
            SetPosition(targetRef.transform.position);
        }
    }

    private void CalculatePositions() {
        fromPos = refTransform.transform.position;
        toPos = fromPos + refTransform.transform.forward * 100f;
    }

    private void PerformRaycast() {
         Vector3 direction = toPos - fromPos;
        if (Physics.Raycast(fromPos, direction, out hit, layerMask)) {
            if (hit.collider.CompareTag("Star")) {
                if (DebugToggle) TestRaycast.SetActive(false);
                hitObj = hit.collider.GetComponent<Star>();
                if (m_HoverObject == null) {
                    m_HoverObject = Instantiate(m_HoverObjectPrefab, hit.collider.transform.position, Quaternion.identity, hit.collider.transform);
                    List<string> timeData = hitObj.GetDateOfCreation();
                    if (timeData != null) {
                        m_HoverObject.GetComponent<HoverObject>().SetText(timeData[0] + ' ' + timeData[1]);
                    }
                }
                toPos = hit.collider.transform.position;
            } else {
                if (DebugToggle) TestRaycast.SetActive(true);
                if (m_HoverObject != null) Destroy(m_HoverObject);
                hitObj = null;
            }
        } else {
            if (DebugToggle) TestRaycast.SetActive(true);
            if (m_HoverObject != null) Destroy(m_HoverObject);
            hitObj = null;
        }
    }

    public void TurnOn() {
        m_LineRenderer.enabled = true;
    }
    public void TurnOff() {
        m_LineRenderer.enabled = false;
    }
    public bool GetStatus() {
        return m_LineRenderer.enabled;
    }
    public Star GetHit() {
        return hitObj;
    }
    public Transform GetRefTransform() {
        return refTransform;
    }
    public void SetTarget(Star target) {
        targetRef = target;
    }
    public void ResetTarget() {
        targetRef = null;
    }
    public void DeactivateHover() {
        if (m_HoverObject != null) m_HoverObject.SetActive(false);
    }
    public void ActivateHover() {
        if (m_HoverObject != null) m_HoverObject.SetActive(true);
    }
}
