using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrabber : MonoBehaviour
{

    #region External References
    public Transform ControllerAnchor;
    public Transform m_gripTrans;
    public CustomGrabber_GrabVolume grabVol;
    public CustomOVRControllerHelper m_OVRControllerHelper;
    public CustomPointer m_CustomPointer;
    #endregion

    #region Public Variables
    public OVRInput.Controller m_controller;
    public bool shouldSnap = true;
    public bool debugToggle = false;
    [SerializeField]
    private bool m_DistanceGrabToggle = false;
    public bool distanceGrabToggle {
        get {   return m_DistanceGrabToggle;    }
        set {   m_DistanceGrabToggle = value;   }
    }
    #endregion

    #region Private Variables
    private CustomGrabbable m_grabbedObject;
    private List<GameObject> inRange = new List<GameObject>();
    #endregion

    #region Prefab References
    [SerializeField]
    private CustomPointer m_CustomPointerPrefab;
    #endregion

    private bool m_isActive = true;
    public bool isActive {
        get {   return m_isActive;  }
        set {   m_isActive = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_gripTrans == null) m_gripTrans = ControllerAnchor;
        if (debugToggle && m_gripTrans != ControllerAnchor) {
            m_gripTrans.gameObject.SetActive(true);
        }
        if (m_DistanceGrabToggle && m_CustomPointer == null) {
            m_CustomPointer = Instantiate(m_CustomPointerPrefab);
            m_CustomPointer.transform.parent = this.transform;    
        }
        if (m_CustomPointer != null) {  m_CustomPointer.Activate(); }
        m_OVRControllerHelper.m_controller = m_controller;
        StartCoroutine(CustomUpdate());
    }

    public OVRInput.Controller GetController() {
        return m_controller;
    }

    private IEnumerator CustomUpdate() {
        while(true) {
            
            if (!m_isActive) {
                yield return null;
                continue;
            }

            // Turn line on or off depending on if custom pointer is present
            if (m_CustomPointer != null) {
                if (m_grabbedObject != null) {
                    m_CustomPointer.LineOff();
                } else {
                    m_CustomPointer.LineSet(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > 0.1f);
                }
            }

            // Check for inputs
            CheckGrabbables();

            yield return null;
        }
    }

    private void CheckGrabbables() {
        //grabVol.GetComponent<Renderer>().enabled = (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) > 0.1f);
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) > 0.1f) {
            if (debugToggle) grabVol.GetComponent<Renderer>().enabled = true;
            // If the grip is being held down
            if (m_grabbedObject == null) {
                GameObject closest = null;
                if (m_DistanceGrabToggle == true) {
                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > 0.1f) {
                        if (m_CustomPointer.raycastTarget.GetComponent<CustomGrabbable>() && m_CustomPointer.raycastTarget.GetComponent<CustomGrabbable>().CanBeGrabbed()) {
                            grabVol.GetComponent<Renderer>().enabled = false;
                            closest = m_CustomPointer.raycastTarget;
                        } else {
                            grabVol.GetComponent<Renderer>().enabled = true;
                        }
                    } else {
                        // Check if any objects are in range
                        inRange = grabVol.GetInRange();
                        // Find Closest
                        closest = GetClosestInRange();
                    }
                } else {
                    // Check if any objects are in range
                    inRange = grabVol.GetInRange();
                    // Find Closest
                    closest = GetClosestInRange();
                }
                // If there is a closest, then we initialize the grab
                if (closest != null) {
                    GrabBegin(closest.GetComponent<CustomGrabbable>());
                }
            }
        } else {
            if (debugToggle) grabVol.GetComponent<Renderer>().enabled = false;
            if (m_grabbedObject != null) {
                GrabEnd();
            }
        }
    }

    private GameObject GetClosestInRange() {
        GameObject closest = null;
        float closestDistance = 0f;
        float d = 0f;
        foreach(GameObject cg in inRange) {
            if (!cg.GetComponent<CustomGrabbable>().CanBeGrabbed()) {   continue;   }
            d = Vector3.Distance(m_gripTrans.position, cg.transform.position);
            if (closest == null || d < closestDistance) {
                closest = cg;
                closestDistance = d;
            }
        }
        return closest;
    }

    private void GrabBegin(CustomGrabbable c) {
        c.GrabBegin(this);
        m_grabbedObject = c;
    }

    private void GrabEnd() {        
        // Imported from Oculus Implementations
        OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };

		OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
		Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
		Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);
        angularVelocity *= -1;
        
        m_grabbedObject.GrabEnd(this, linearVelocity, angularVelocity);
        m_grabbedObject = null;
    }
}
