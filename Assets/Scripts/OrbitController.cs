using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{

    #region External References and Scripts
    public Game m_Game;
    public Transform m_GripTrans;
    public Transform m_StarOverControllerTrans;
    public GameObject m_ControllerModel;
    public GameObject InputDummy;
    private OVRGrabber m_OVRGrabber;
    private OrbitPointer m_OrbitPointer;
    #endregion

    #region Public Variables
    public OVRInput.Controller m_controller;
    public OVRInput.RawAxis1D m_grip;
    public OVRInput.RawAxis1D m_trigger;
    public OVRInput.RawButton m_top;
    public OVRInput.RawButton m_lower;
    public OVRInput.RawAxis2D m_joystick;
    public bool isGrabber;
    public bool debugMode;
    #endregion

    #region Private Variables
    private Star m_GrabbedStar = null;
    private bool gripActive = false, triggerActive = false, topActive = false, lowerActive = false;
    // NEED INPUT FOR JOYSTICK
    private bool isPulling = false, isGrabbing = false;
    #endregion

    private void Awake()
    {
        m_OVRGrabber = this.GetComponent<OVRGrabber>();
        m_OrbitPointer = this.GetComponent<OrbitPointer>();
        m_OrbitPointer.TurnOff();

        /*
        if (!isGrabber) {
            m_OVRGrabber.m_isActive = false;       
            return;
        }

        m_OVRGrabber.m_isActive = true;
        m_OrbitPointer.SetStartPoint(m_GripTrans);
        if (debugMode) m_OrbitPointer.SetInputDummy(InputDummy);
        */
    }

    // Update is called once per frame
    void Update()
    {
        // Check mic status - if no mic attached, end here
        if (!m_Game.GetMicStatus()) {
            return;
        }
        if (!isGrabber) {
            return;
        }

        CheckInputs();
        ProcessInputs();
        return;

        /*
        // Check if this controller is holding something
        // Based on this, the behavior of the inputs will change
        if (m_GrabbedStar != null) {
            HeldInputs();
        }
        else {
            UnheldInputs();
        }
        */
    }

    // Check which inputs are pressed or not
    private void CheckInputs() {
        //gripActive = OVRInput.Get(m_grip, m_controller) > 0.1f;
        triggerActive = OVRInput.Get(m_trigger, m_controller) > 0.1f;
        //topActive = OVRInput.GetDown(m_top, m_controller);
        lowerActive = OVRInput.GetDown(m_lower, m_controller);
        return;
    }
    private void ProcessInputs() {
        // Trigger - activates pointer
        if (triggerActive) {
            m_OrbitPointer.TurnOn();
            if (debugMode) InputDummy.SetActive(true);
        } else {
            m_OrbitPointer.TurnOff();
            if(debugMode) InputDummy.SetActive(false);
        }

        // Lower - Creates recording if not grabbing, play recording when grabbing
        if (lowerActive) {
            if (isGrabbing) {

            } else {
                m_Game.ToggleRecording();    
            }
        }
        /*
        else {
            // Top - edit title or create star
            if (topActive) {
                if (m_GrabbedStar == null) {
                    StartCoroutine(m_Game.CreateStarAtPosition(PointerTransform.position + PointerTransform.forward*0.2f));
                }
            }
        }
        */
    }
}
