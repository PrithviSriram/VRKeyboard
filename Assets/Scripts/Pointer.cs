using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public bool DebugToggle;
    public Transform returnPosition;
    private LineRenderer m_LineRenderer;
    private Vector3[] positions = new Vector3[2];
    private bool isOn = false;

    private int layerMask = 1 << 8;
    private RaycastHit hit;
    private NewOrb hitObj;

    public GameObject TestRaycast;

    private Vector3 fromPos, toPos;

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
        positions[0] = returnPosition.transform.position;
        positions[1] = to;
    }

    private void Update() {

        CalculatePositions();
        PerformRaycast();
        SetPositions(fromPos, toPos);
        
    }

    private void CalculatePositions() {
        fromPos = returnPosition.transform.position;
        toPos = returnPosition.transform.forward * 100f;
    }

    private void PerformRaycast() {
         Vector3 direction = toPos - fromPos;
        if (Physics.Raycast(fromPos, direction, out hit, layerMask)) {
            if (hit.collider.CompareTag("Star")) {
                if (DebugToggle) TestRaycast.SetActive(false);
                hitObj = hit.collider.GetComponent<NewOrb>();
                toPos = hit.collider.transform.position;
            } else {
                if (DebugToggle) TestRaycast.SetActive(true);
                hitObj = null;
            }
        } else {
            if (DebugToggle) TestRaycast.SetActive(true);
            hitObj = null;
        }
    }

    public void TurnOn() {
        //isOn = true;
        m_LineRenderer.enabled = true;
    }
    public void TurnOff() {
        //isOn = false;
        m_LineRenderer.enabled = false;
    }
    public bool GetStatus() {
        //return isOn;
        return m_LineRenderer.enabled;
    }
    public NewOrb GetHit() {
        return hitObj;
    }
    public Transform GetReturnPosition() {
        return returnPosition;
    }
}
