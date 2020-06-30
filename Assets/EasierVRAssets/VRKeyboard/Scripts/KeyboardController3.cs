using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController3 : MonoBehaviour
{

    private Dictionary<float, Vector2> actionList = new Dictionary<float, Vector2>();

    private void Awake() {
        return;
    }

    private void Update() {
        return;
    }

    /*
    #region Text and Characters
    [SerializeField]
    [TextArea]
    private string m_text;
    public string text
    {
        get {   return m_text;  }
        set {   m_text = value; }
    }
    [SerializeField]
    private float m_fontSize = 0.025f;
    public float fontSize 
    {
        get {   return m_fontSize;  }
        set {   m_fontSize = value; }
    }
    [SerializeField]
    private KeyboardInput m_KeyboardInput;
    public KeyboardInput keyIn
    {
        get {   return m_KeyboardInput;     }
        set {   m_KeyboardInput = value;    }
    }
    [SerializeField]
    private float AddCharacterThresholdTime = 0.5f;
    [SerializeField]
    private float HoldButtonThresholdTime = 0.3f;

    private string m_scaffold = "";
    public string scaffold
    {
        get {   return m_scaffold;  }
        set {   m_scaffold = value; }
    }
    #endregion

    #region Keys
    [SerializeField]
    private List<Key> m_Keys = new List<Key>();
    public List<Key> keys 
    {
        get {   return m_Keys;  }
        set {   m_Keys = value; }
    }
    private int m_currentKeyIndex = -1;
    public int currentKeyIndex
    {
        get {   return m_currentKeyIndex;   }
        set {   if (m_Keys[m_currentKeyIndex].state != 2) { m_currentKeyIndex = value; }    }
    }
    public Key currentKey
    {
        get {   return m_Keys[m_currentKeyIndex];    }
    }
    #endregion

    #region Keyboard
    [SerializeField]
    private KeyboardTypeController2 m_KeyboardTypeController;
    private List<Vector4> m_thumbstickAngleMapping;
    public KeyboardTypeController2 keyTypeCon
    {
        get {   return m_KeyboardTypeController;    }
        set 
        {
            m_KeyboardTypeController = value;
            m_Keys = m_KeyboardTypeController.keys;
            m_thumbstickAngleMapping = m_KeyboardTypeController.thumbstickAngleMapping;
        }
    }
    #endregion

    #region CustomGrab Stuff
    [SerializeField]
    private bool m_shouldBeGrabbed = true;
    private CustomGrabbable m_CustomGrabbable;
    private OVRInput.Controller m_grabbedBy = OVRInput.Controller.None, m_notGrabbedBy = OVRInput.Controller.None;
    [SerializeField]
    private bool m_allowCollision = true;
    public bool allowCollision
    {
        get {   return m_allowCollision;    }
        set {   m_allowCollision = value;   }
    }
    private Vector2 grabberThumb = new Vector2(0f, 0f);
    private Vector2 grabberThumbProcessed = new Vector2(0f, 0f);
    private float lastActionTime = 0f;
    private IEnumerator holdCoroutine;

    private Vector2 buttonThumb = new Vector2(0f, 0f);
    private Vector2 buttonThumbProcessed = new Vector2(0f, 0f);
    private bool buttonThumbRegistered = false;

    private bool selectButton = false;
    private float selectButtonTime = 0f, selectButtonHeldTime = 0f;

    private bool deleteButton = false;
    private float deleteButtonTime = 0f, deleteButtonHeldTime = 0f;

    private bool triggerButton = false;
    private bool changedKeys = false;
    #endregion

    */

    /*
    private void Start() {
        if (m_shouldBeGrabbed) {    m_CustomGrabbable = this.GetComponent<CustomGrabbable>();   }
        else {  m_allowCollision = true;    }   // We force the keys of this keyboard to be collision-friendly - otherwise, we cannot interact with it.
        m_KeyboardTypeController.Initialize(this);
        m_Keys = m_KeyboardTypeController.keys;
    }
    */

    /*
    private IEnumerator CustomUpdate() {
        while(true) {
            // Keys usually update by themselves, but if we need for the keyboard to be grabbed then we need to do the following
            if (m_shouldBeGrabbed) {
                
                // Check if the object is being grabbed or not
                CheckGrabbedBy();

                // Prevent any actions unless it's being held by a hand
                if (m_grabbedBy == OVRInput.Controller.None) {
                    yield return null;
                    continue;
                }

                // Check if any inputs have been detected
                CheckInputs();
                // Process any Inputs
                ProcessInputs();

                // Reset some bools
                changedKeys = false;
            }
            else if (!m_KeyboardTypeController.isActive) {  m_KeyboardTypeController.Activate();    }

            // Get current character and scaffold
            GetScaffold();

            // Add scaffold if enough time has passed
            if (lastActionTime != 0f && Time.time - lastActionTime > AddCharacterThresholdTime) {
                AddScaffold(true);
            }
            yield return null;
        }
    }
    */

    /*
    private void CheckGrabbedBy() {
        // Checks the CustomGrabbable.cs script to see if a grabber is registered.
        // If there is, then we initialize the GrabBegin() for this particular script
        // Else, we initialize the GrabEnd() for this particular script
        OVRInput.Controller gb = m_CustomGrabbable.GetGrabber();
        if (gb != OVRInput.Controller.None) {   GrabBegin(gb);  }
        else {  GrabEnd();  }
        return;
    }
    */

    /*
    public void GrabBegin(OVRInput.Controller g) {
        // This GrabBegin() must do several things:
        // 1) Set the GrabbedBy to what was provided in the argument
        // 2) Determine which controller is the grabber and which is the button presser
        m_grabbedBy = g;
        if (!m_KeyboardTypeController.isActive) {   m_KeyboardTypeController.Activate();    }
        //m_KeyboardInput.Activate();
        switch(g) {
            case(OVRInput.Controller.LTouch):
                m_notGrabbedBy = OVRInput.Controller.RTouch;
                break;
            case(OVRInput.Controller.RTouch):
                m_notGrabbedBy = OVRInput.Controller.LTouch;
                break;
            case(OVRInput.Controller.LTrackedRemote):
                m_notGrabbedBy = OVRInput.Controller.RTrackedRemote;
                break;
            case(OVRInput.Controller.RTrackedRemote):
                m_notGrabbedBy = OVRInput.Controller.LTrackedRemote;
                break;
            default:
                m_notGrabbedBy = g;
                break;
        }
        return;
    }
    */
    /*
    public void GrabEnd() {
        // This GrabEnd() must do several things:
        // 1) reset the GrabbedBy to None
        // 2) Reset the keyboard itself, which means setting all keys to their default state.
        m_grabbedBy = OVRInput.Controller.None;
        if (m_KeyboardTypeController.isActive) { m_KeyboardTypeController.Deactivate();  }
        currentKeyIndex = -1;
        //AddScaffold(true);
        //m_KeyboardInput.Deactivate();
        //if (currentKeyIndex != -1) {
        //    m_Keys[currentKeyIndex].ResetKey();
        //    currentKeyIndex = -1;
        //}
        return;
    }
    */

    /*
    private void CheckInputs() {
        #region Inputs For Grabber
        // Check inputs for grabber. Inputs for grabber include:
        // Thumbstick Direction: Numpad Select
        grabberThumb = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_grabbedBy);
        #endregion

        #region Inputs For Buttons
        // Check inputs for non-grabber. Inputs for non-grabber include:
        // Thumstick Right - Next Character, Space
        // A/X: Select Numpad
        // B/Y: Delete
        buttonThumb = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_notGrabbedBy);
        if (OVRInput.GetDown(OVRInput.Button.One, m_notGrabbedBy)) {
            selectButton = true;
            selectButtonTime = Time.time;
        } else {
            selectButton = false;
        }
        if (OVRInput.GetUp(OVRInput.Button.One, m_notGrabbedBy)) {
            selectButtonTime = 0f;
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, m_notGrabbedBy)) {
            // Registering delete keypress - delete last inputted character
            deleteButton = true;
            deleteButtonTime = Time.time;
        } else {
            deleteButton = false;
        }
        if (OVRInput.GetUp(OVRInput.Button.Two, m_notGrabbedBy)) {
            // Registering delete keyup
            deleteButtonTime = 0f;
        }
        #endregion

        return;
    }
    */
    /*
    private bool BetweenValues(float val, float min, float max, bool inclusive) {
        if (inclusive) {    return (val-min)*(max-val)>=0;  } 
        else {  return (val-min)*(max-val)>0;   }
    }
    private bool BetweenValues(float val, float min, float max, float inclusive) {
        if (inclusive == 1f) {  return (val-min)*(max-val)>=0;  } 
        else {  return (val-min)*(max-val)>0;   }
    }
    */

    /*
    private void ProcessInputs() {
        #region Process Grabber Inputs
        // Process thumbstick input
        float thumbX = grabberThumb.x, thumbY = grabberThumb.y;
        float angle = Mathf.Atan2(thumbY, thumbX) * Mathf.Rad2Deg + 180f;
        int joystickIndex = -1;
        if ( BetweenValues(thumbX,-0.35f,0.35f,true) && BetweenValues(thumbY,-0.35f,0.35f,true) ) { joystickIndex = m_KeyboardTypeController.defaultIndex;  }
        else {
            foreach(Vector4 m in m_KeyboardTypeController.thumbstickAngleMapping) {
                if ( BetweenValues(angle, m.x, m.y, m.z) ) {
                    joystickIndex = (int)m.w;
                    break;
                }
            }
        }
        if (joystickIndex != currentKeyIndex) {
            // If the joystick index is not equal to the current key index, then we do some stuff
            // Firstly, is the joystick index even a valid character?
            if (joystickIndex != -1) {  
                // joystick index is a valid character, but what if it isn't the same as teh current key index?
                if (joystickIndex != currentKeyIndex) {
                    m_Keys[joystickIndex].state = 1;
                    // Okay, so since the joystick index is not the same as the current key index, we set the joystick's key to be state = 1
                    // The Key itself attached to this particular idnex will attempt to update the KeyboardController's currentKeyIndex by itself
                    // However, if the key attached to currentKeyIndex is already set to state = 2 (ie. being touched by finger or if it's already selected), then this won't change anything
                    // We theoretically don't need to change the currentKeyIndex here directly, because the updated key will be attempting to set it itself via script
                }
                // If the joystick index and the currentKeyIndex are the same... then nothing needs to be done, I think
            }
            // If the joystick index is == -1, then that means the joystick either isn't working (probably) or (more likely) the joystick is hitting an angle that isn't mapped
            // In this scenario, we will need to change the last highlighted key to neutral (state = 0)
            else {
                // We need to consider the possibility that the joystick isn't hitting anything but the touch detection is.
                if (m_Keys[currentKeyIndex].state != 2) {
                    m_Keys[currentKeyIndex].state = 0;
                    currentKeyIndex = -1;
                    // this will only work if the touch detection is off or no keys are being touched by the finger
                }
            }
        }
    
        // else if ( BetweenValues(angle,0f,22.5f,false) || BetweenValues(angle,337.5f,360f,false) ) { index = 3;  } 
        // else if ( BetweenValues(angle,22.5f,67.5f,true) ) { index = 6;  }
        // else if ( BetweenValues(angle,67.5f,112.5f,false) ) {   index = 7;  }
        // else if ( BetweenValues(angle,112.5f,157.5f,true) ) {   index = 8;  }
        // else if ( BetweenValues(angle,157.5f,202.5f,false) ) {  index = 5;  }
        // else if ( BetweenValues(angle,202.5f,247.5f,true) ) {   index = 2;  }
        // else if ( BetweenValues(angle,247.5f,292.5f,false) ) {  index = 1;  }
        // else if ( BetweenValues(angle,292.5f,337.5f,true) ) {   index = 0;  }
    
        // if (index == -1) {
            // If there is no highlighted key and, for some reason, the current key index is not -1, then we need to reset that key
        //     if (currentKeyIndex != -1) {    m_Keys[currentKeyIndex].ResetKey(); }
        //    currentKeyIndex = -1;
        //}
        //if (index != -1 && currentKeyIndex == -1) {
        //    m_Keys[currentKeyIndex].state = 1;
        //    currentKeyIndex = index;
        //}
        //else if (index != currentKeyIndex) {
        //    changedKeys = true;
        //    m_Keys[currentKeyIndex].ResetKey();
        //    currentKeyIndex = index;
        //    m_Keys[currentKeyIndex].state = 2;
        //}

        //m_Input.SetText(angle.ToString() + " | " + index.ToString());
        //m_Input.SetText(m_Keys[index].gameObject.name);
        //m_Keys[currentKeyIndex].HighlightKey();
        #endregion

        #region Process Non-Grabber Inputs
        // Select (A) Button Held
        if (selectButtonTime != 0f) {   m_Keys[currentKeyIndex].state = 2;  } 
        else if (joystickIndex != currentKeyIndex) {  m_Keys[currentKeyIndex].state = 1;  }
        // Select (A) Button
        
        //if (selectButtonTime != 0f && Time.time - selectButtonTime > HoldButtonThresholdTime) {
        //    if (holdCoroutine != null) StopCoroutine(holdCoroutine);
        //    holdCoroutine = HoldButton("Select");
        //    StartCoroutine(holdCoroutine);
        //} else {
        //    StopCoroutine(holdCoroutine);
        //    holdCoroutine = null;
        //}

        // If we changed keys in the interim, we add the current scaffold first
        //if (changedKeys) {
        //    AddScaffold(true);
        //}
        
        //if (selectButton) {
            // The button has been pushed down
            
            // we select the next character for that key
        //    m_Keys[currentKeyIndex].NextCharactersIndex();

            // Add the currently selected key as scaffolding
        //    UpdateScaffold(m_Keys[currentKeyIndex].GetCurrentCharacter());
           
                                        //else if (changedKeys) {
                                        //    m_Keys[currentKeyIndex].NextCharactersIndex();
                                        //    changedKeys = false;
                                        //}

        //} else {
            // The button has been released or if it was switched
            
                                        //if (m_Keys[currentKeyIndex].isSpace) {
                                        //    m_Input.AddSpace();
                                        //}
            //if (changedKeys) {
            //    m_Input.AddScaffold();
            //}
            //else {
            //    m_Keys[currentKeyIndex].HighlightKey();
            //}
                                    //selectButtonRegistered = false;
        //}

        
        //if (deleteButtonTime != 0f && Time.time - deleteButtonTime > HoldButtonThresholdTime) {
        //    if (holdCoroutine != null) StopCoroutine(holdCoroutine);
        //    holdCoroutine = HoldButton("Delete");
        //    StartCoroutine(holdCoroutine);
        //} else {
        //    StopCoroutine(holdCoroutine);
        //    holdCoroutine = null;
        //}

        //if (deleteButton) {
            // The button has been pushed down now

            // For now, we'l have the joystick control the next character in text stuff.
            // it'll just be that for now, if the button has been pressed down, we select the next character for that key
        //    if (m_Input.GetScaffold() == "") {
        //        DeleteCharacter();
        //    } else {
        //        ResetScaffold();
        //    }
                                //deleteButtonRegistered = true;
        //} else {
                             //deleteButtonRegistered = false;
        //}

        float buttonThumbX = buttonThumb.x, buttonThumbY = buttonThumb.y;
        float buttonAngle = Mathf.Atan2(buttonThumbY, buttonThumbX) * Mathf.Rad2Deg + 180f;
        if ( BetweenValues(buttonThumbX,-0.35f,0.35f,true)&&BetweenValues(buttonThumbY,-0.35f,0.35f,true) ) {
            buttonThumbRegistered = false;
        }
        else if (!buttonThumbRegistered) {
            if (m_Input.GetScaffold() != "") {
                AddScaffold(true);
            } else {
                AddSpace();
            }
            m_Keys[currentKeyIndex].ResetCharactersIndex();
            buttonThumbRegistered = true;
        }
        
        //else if ( BetweenValues(buttonAngle,157.5f,202.5f,false) && !buttonThumbRegistered ) {
        //    if (m_Keys[currentKeyIndex].GetCurrentCharacter() != "") {
        //        AddScaffold(true);
        //    }
        //    m_Input.AddSpace();
        //    m_Keys[currentKeyIndex].ResetCharactersIndex();
        //    buttonThumbRegistered = true;
        //}
        #endregion

        #region Miscellaneous Reset Some Variables
        //changedKeys = false;
        #endregion
    }
    */

    /*
    private IEnumerator HoldButton(string type) {
        while(true) {
            switch(type) {
                case("Select"):
                    AddScaffold(false);
                    break;
                case("Delete"):
                    DeleteCharacter();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    */

    /*
    private void AddScaffold(bool reset) {
        //m_Input.AddScaffold(reset);
        lastActionTime = 0f;
    }
    private void AddSpace() {
        //m_Input.AddSpace();
        lastActionTime = 0f;
    }
    private void UpdateScaffold(string s) {
        //m_Input.UpdateScaffold(s);
        lastActionTime = 0f;
    }
    private void ResetScaffold() {
        //m_Input.ResetScaffold();
        lastActionTime = 0f;
    }
    private void DeleteCharacter() {
        //m_Input.DeleteCharacter();
        lastActionTime = 0f;
    }

    private void GetScaffold() {
        m_scaffold = (currentKeyIndex != -1) ? m_Keys[currentKeyIndex].currentCharacter : "";
    }
    */
}
