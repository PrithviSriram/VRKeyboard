using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OculusSampleFramework;
using TMPro;

public class New_Custom_Controller : MonoBehaviour
{
    #region References
    public Game m_Game;                                 // Ref to main game object
    public HandCanvasController m_HandCanvasController; // Ref to canvas
    private New_Pointer raycastPointer;                 // Ref to pointer
    #endregion

    #region OVR Inputs
    public OVRInput.Controller m_Controller;            
    public OVRInput.RawAxis1D RecordAndPointButton;
    public OVRInput.RawButton PlayButton;
    public OVRInput.RawButton CreateDeleteButton;
    public OVRInput.RawAxis1D GripButton;
    private Vector3 m_anchorOffsetPosition;
    private Quaternion m_anchorOffsetRotation;
    private OVRGrabber m_OVRGrabber;
    #endregion

    #region Public Variables
    public float HoldTimeThreshold;
    public float forceStrength;
    #endregion

    #region Private Variables
    private Star m_GrabbedObject = null;      // The object this controller is currently grabbing
    Transform PointerTransform = null;              // The pointer's transformation reference 
    private bool RecordAndPoint_TimeStarted = false;    // has the recording buffer been started?
    private float RecordAndPoint_StartTime = 0.0f;      // the time of the recording buffer
    #endregion

    /*
    private bool CurrentlyDeleting = false;
    private float CurrentlyDeleting_StartTime = 0.0f;
    private OVRGrabbable HeldObject;
    private NewOrb HeldObject_NewOrb;
    private NewOrb HoveredObject;
    private bool isPulled;
    */

    private void Awake()
    {
        m_OVRGrabber = this.GetComponent<OVRGrabber>();
        //m_Controller = m_OVRGrabber.GetController();
        m_HandCanvasController.InitializeRecordSlider(HoldTimeThreshold);   // Initialize the record slider with the hold time threshold
        raycastPointer = this.GetComponent<New_Pointer>();                      // get the pointer

        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;
        //m_HandCanvasController.InitializeDeleteSlider(HoldTimeThreshold);
    }

    private void Update()
    {
        // Check mic status - if no mic attached, end here
        if (!m_Game.GetMicStatus()) {
            return;
        }

        // Update gripTrans position
        PointerTransform = raycastPointer.GetRefTransform();

        // Check if this controller is holding something
        // Based on this, the behavior of the inputs will change
        if (m_GrabbedObject != null) {
            HeldInputs();
        }
        else {
            UnheldInputs();
        }

        /*
        // Check if the hand is currently holding something
        // This is entirely controlled by DistanceGrabber - we won't do the work for that
        CheckHolding();
        Transform RPTransform = raycastPointer.GetReturnPosition();

        // Depending on if it's holding something, the behavior of the hand changes
        if (HeldObject != null) {

            HeldObject_NewOrb = HeldObject.GetComponent<NewOrb>();

            // If an object is being held, there are several things to do:
            // 1) If RecordAndPoint (default = Index Trigger) clicked AND held for some time, then a recording starts or begins to overwrite
            // 2) If RecordAndPoint (default = Index Trigger) is let go, then the recording stops
            // 3) If PlayButton (default = A/X button) is clicked AND if there's an audio clip inside, it is toggled on/off
            // 4) If CreateDeletetButton (default = B/Y button) is clicked and held for some time, then the star itself is deleted

            // 1)
            if (OVRInput.Get(RecordAndPointButton, m_Controller) > 0.1f) {      RecordAndPoint_Down();  }

            // 2)
            if (OVRInput.Get(RecordAndPointButton, m_Controller) <= 0.1f) {     RecordAndPoint_Up();    }

            // 3) 
            if (OVRInput.GetDown(PlayButton, m_Controller) && HeldObject_NewOrb.GetAudioClip() != null) {   HeldObject_NewOrb.ToggleAudio();   }

            // 4) 
            /*
            if (OVRInput.Get(CreateDeleteButton, m_Controller)) {   DeleteOrb();    }
            else {  StopDelete();   }
            */
        /*
        } 
        else {
            // If an object is NOT being held, then there are several things to do:
            // 1) If RecordAndPoint (default = Index Trigger) is clicked and held, a raycast is sent out for easier detection
            // 2) If grip is held and there's a raycast hit detected from pointer...
            // 3) If CreateDeleteButton (default = B/Y button) is clicked and held for some time, then a new star is instantiated in front of the player.
            // NOTE: PlayButton doesn't do anything

            // 1)
            /*
            if (OVRInput.GetDown(PlayButton, m_Controller) && !raycastPointer.GetStatus()) {   raycastPointer.TurnOn();  }
            if (OVRInput.GetUp(PlayButton, m_Controller) && raycastPointer.GetStatus()) {   raycastPointer.TurnOff();   }
            */
        /*
            // 2) 
            if (OVRInput.Get(GripButton, m_Controller) > 0.1f && raycastPointer.GetHit() != null) {
                HoveredObject = raycastPointer.GetHit();
                float dist = Vector3.Distance(RPTransform.position + RPTransform.forward*0.2f, HoveredObject.transform.position);
                if (dist >= 0.1f) {
                //if (!isPulled) {
                //    StartCoroutine(PullBack());
                //}
                    //HoveredObject.gameObject.transform.position = Vector3.Lerp(HoveredObject.gameObject.transform.position, raycastPointer.GetReturnPosition().position, Time.deltaTime * 3f);
                    //m_DistanceGrabber.SetTarget(HoveredObject.GetComponent<DistanceGrabbable>());
                    //Vector3 smoothedDelta = Vector3.MoveTowards(HoveredObject.gameObject.transform.position, RPTransform.position + RPTransform.forward*0.2f, Time.deltaTime * 3f);
                    //HoveredObject.GetComponent<Rigidbody>().MovePosition(smoothedDelta);
                //}
            }

            // 3)
            if (OVRInput.GetDown(CreateDeleteButton,m_Controller)) {
                StartCoroutine(m_Game.CreateStarAtPosition(RPTransform.position + RPTransform.forward*0.2f));
            }
        }
        */
    }
    
    // 
    private void HeldInputs() {
        // Right now, the controller is holding an object

        // Upon the grip immediately being let go, we remove the grab object and returns
        if (OVRInput.Get(GripButton, m_Controller) <= 0.1f) {
            GrabEnd();
            return;
        }

        // If an object is being held, there are several things to do:
        // 1) If RecordAndPoint (default = Index Trigger) clicked AND held for some time, then a recording starts or begins to overwrite
        // 2) If RecordAndPoint (default = Index Trigger) is let go, then the recording stops
        // 3) If PlayButton (default = A/X button) is clicked AND if there's an audio clip inside, it is toggled on/off
        // 4) If CreateDeletetButton (default = B/Y button) is clicked and held for some time, then the star itself is deleted

        // 1)
        if (OVRInput.Get(RecordAndPointButton, m_Controller) > 0.1f) {      RecordAndPoint_Down();  }

        // 2)
        if (OVRInput.Get(RecordAndPointButton, m_Controller) <= 0.1f) {     RecordAndPoint_Up();    }

        // 3) 
        if (OVRInput.GetDown(PlayButton, m_Controller) && m_GrabbedObject.GetAudioClip() != null) {   m_GrabbedObject.ToggleAudio();   }



    }

    private void UnheldInputs() {
        // Right now, the controller doesn't have any objects its holding
        // To that end, the way to go is to do the following:
        // 1) Check if the pointer is colliding with an object.
        if ((OVRGrabbable)m_OVRGrabber.grabbedObject != null) {
            GrabBegin(m_OVRGrabber.grabbedObject.GetComponent<Star>());
            return;
        }
        Star possibleCollision = raycastPointer.GetHit();

        // 2) If something is colliding, check if the OVRInput grip is being pressed
        //      If pressed, set "m_GrabbedObject" to object of collision and return
        if (OVRInput.Get(GripButton, m_Controller) > 0.1f && possibleCollision != null) {
            GrabBegin(possibleCollision);
            return;
        }

        // 3) Check if the star creation button is held
        //      If pressed, generate a star in front of the controller
        if (OVRInput.GetDown(CreateDeleteButton,m_Controller)) {
            StartCoroutine(m_Game.CreateStarAtPosition(PointerTransform.position + PointerTransform.forward*0.2f));
        }

    }

    private void GrabBegin(Star col) {
        m_GrabbedObject = col;
        raycastPointer.SetTarget(m_GrabbedObject);
        m_GrabbedObject.GrabBegin(this.GetComponent<New_Custom_Controller>());
    }
    private void GrabEnd() {
        raycastPointer.ResetTarget();

        OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_Controller), orientation = OVRInput.GetLocalControllerRotation(m_Controller) };
        OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
        localPose = localPose * offsetPose;

		OVRPose trackingSpace = this.transform.ToOVRPose() * localPose.Inverse();
		Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_Controller);
		Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_Controller);

        m_GrabbedObject.GrabEnd(linearVelocity, angularVelocity);
        m_GrabbedObject = null;
    }

    public Vector3 GetTargetRef() {
        Transform rt = raycastPointer.GetRefTransform();
        //Vector3 tr = rt.position + rt.forward*0.2f;
        Vector3 tr = rt.position;
        return tr;
    }
    public OVRInput.Controller GetController() {
        return m_Controller;
    }
    public float GetForceStrength() {
        return forceStrength;
    }
    public void ActivateHover() {
        raycastPointer.ActivateHover();
    }
    public void DeactivateHover() {
        raycastPointer.DeactivateHover();
    }
    
    private void RecordAndPoint_Down() {
        if (!m_GrabbedObject.CheckRecordingStatus()) {
            if (m_Game.CanRecord().Key == OVRInput.Controller.None) {
                // we've got the go-ahead, start recording
                if (!RecordAndPoint_TimeStarted) {
                    RecordAndPoint_StartTime = Time.time;
                    RecordAndPoint_TimeStarted = true;
                    m_HandCanvasController.StartRecordSlider();
                    return;
                }
                float TimeDiff = Time.time - RecordAndPoint_StartTime;
                m_HandCanvasController.SetRecordSlider(TimeDiff);
                if (RecordAndPoint_TimeStarted && TimeDiff >= HoldTimeThreshold) {
                    m_GrabbedObject.StartRecording();
                }
            } else {
                m_HandCanvasController.DeactivateRecordSlider();
                RecordAndPoint_TimeStarted = false;
            }
        }
    }
    private void RecordAndPoint_Up() {
        RecordAndPoint_StartTime = 0.0f;
        RecordAndPoint_TimeStarted = false;
        m_HandCanvasController.DeactivateRecordSlider();
        if (m_GrabbedObject.CheckRecordingStatus()) {
            m_GrabbedObject.EndRecording();
            m_Game.AddStar(m_GrabbedObject.gameObject);
        }
    }
    /*

    private IEnumerator PullBack() {
        isPulled = true;
        HoveredObject.GetComponent<Rigidbody>().AddForce((raycastPointer.GetReturnPosition().position - HoveredObject.transform.position).normalized * 10f * Time.smoothDeltaTime);
        yield return new WaitForSeconds(1f);
        isPulled = false;
    }
    */
}
