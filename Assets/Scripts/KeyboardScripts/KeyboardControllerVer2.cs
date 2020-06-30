using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

[RequireComponent(typeof(CustomGrabbable))]
public class KeyboardControllerVer2 : MonoBehaviour
{

    #region External References
    // NOT SERIALIZED
    [Tooltip("Reference to this object's Custom Grabbable component")]
    private CustomGrabbable m_CustomGrabbable;
    // NOT SERIALIZED
    [Tooltip("Reference to this object's KeyboardTypeControllerVer2 component")]
    private KeyboardTypeControllerVer2 m_KeyboardTypeController;
    // NOT SERIALIZED
    [Tooltip("List of keys attached to this keyboard - retrieved from m_KeyboardTypeController upon Awake")]
    //private KeyboardKeyVer2[] m_Keys;
    private List<KeyboardKeyVer2> m_Keys;
    // TECHNICALLY PUBLIC, BUT WILL BE MADE INTO A SERIALIZED PRIVATE FIELD LATER
    [Tooltip("Reference to the Input script attached to a child of this keyboard")]
    public KeyboardInputVer2 m_Input;
    // TECHNICALLY PUBLIC, BUT WILL BE MADE INTO A SERIALIZED PRIVATE FIELD LATER
    [Tooltip("Reference to the Overlay script attached to a child of this keyboard")]
    public List<KeyboardOverlayVer2> m_OverlayList;
    // TECHNICALLY PUBLIC, BUT WILL BE MADE INTO A SERIALIZED PRIVATE FIELD LATER
    [Tooltip("A debug script - MAY DELETE")]
    public DebugKeyboard m_debugConsole;
    [SerializeField]
    [Tooltip("Reference to a Custom Pointer, which is usually a child of this keyboard")]
    private CustomPointer m_CustomPointer;
    [SerializeField]
    [Tooltip("Reference to a KeyboardTextbox object out there")]
    private KeyboardTextbox m_targetText = null;
    public KeyboardTextbox targetText {
        get {   return m_targetText;    }
        set {
            if (value != null) {
                ActivateKeyboard(value.gameObject);
                m_targetSet = true;
            }
            else {
                DeactivateKeyboard();
                m_targetSet = false;
            }
        }
    }
    // NOT SERIALIZED
    [Tooltip("Reference to cursor of thumbstick")]
    private RectTransform m_cursor;
    // NOT SERIALIZED
    [Tooltip("Boolean check for if a target textbox has been pre-defined")]
    private bool m_targetSet = false;
    // NOT SERIALIZED
    [Tooltip("Reference to the KeyboardWordPreview script attached to a child of this keyboard")]
    private KeyboardWordPreviewVer2 m_Preview;
    #endregion

    #region Controllers
    [SerializeField]
    [Tooltip("Reference to the Custom hand holding the keyboard. If initialized as NONE, then it will require to be grabbed to operate.")]
    private CustomGrabber m_customGrabber = null;
    // NOT SERIALIZED
    [Tooltip("Reference to if the keyboard must be grabbed or not")]
    private bool m_mustBeGrabbed = true;
    // NOT SERIALIZED
    [Tooltip("Reference to the OVR controller holding the keyboard")]
    private OVRInput.Controller m_GrabbedBy = OVRInput.Controller.None;
    // NOT SERIALIZED
    [Tooltip("Reference to the OVR controller NOT holding the keyboard")]
    private OVRInput.Controller m_ButtonController = OVRInput.Controller.None;
    #endregion

    #region Public Variables
    [SerializeField]
    [Tooltip("The font size of text in the preview window")]
    private float m_fontSize = 0.025f;
    public float fontSize {
        get {   return m_fontSize;  }
        set {   m_fontSize = value; }
    }
    [SerializeField]
    [Tooltip("List of keyboards attached to this keyboard")]
    private List<KeyboardTypeControllerVer2> m_KeyboardTypeList = new List<KeyboardTypeControllerVer2>();
    // NOT SERIALIZED
    private enum KeyboardTypeDropdown {
        Wheel,
        Wheel_With_Cheat
    }
    [SerializeField]
    [Tooltip("For determining which kind of keyboard this keyboard should use")]
    private KeyboardTypeDropdown m_KeyboardType;
    [SerializeField]
    [Tooltip("The threshold time for adding a new character")]
    private float m_AddCharacterThresholdTime = 0.4f;
    public float AddCharacterThresholdTime {
        get {   return m_AddCharacterThresholdTime;     }
        set {   m_AddCharacterThresholdTime = value;    }
    }
    [SerializeField]
    [Tooltip("For determining the threshold where a button should be determined to be 'held down'")]
    private float m_HoldButtonThresholdTime = 0.3f;
    public float HoldButtonThresholdTime {
        get {   return m_HoldButtonThresholdTime;   }
        set {   m_HoldButtonThresholdTime = value;  }
    }

    // I WILL UPDATE THESE LATER. FOR NOW, THESE ARE USELESS BUT STILL IMPORTANT TO THE FUNCTIONING OF THIS SCRIPT
    public bool allowCollision = false;
    public bool debugToggle = false;
    #endregion

    #region Private Variables 
    // NOT SERIALIZED
    [Tooltip("Stores the index number of which key is active")]
    private int currentKeyIndex = -1;
    public string text {
        get {   return (m_Input != null) ? m_Input.GetText() : "";  }
        set {   m_Input.SetText(value);                             }
    }

    GameObject instructionCanvas;
    GameObject testCanvas;
    GameObject keyboardCanvas;
    GameObject sampleSupport;
    GameObject camera;
    private bool test = true;
    private bool clearWords = false;
    private bool session2 = false;
    private bool switchKey = false;
    #endregion

    #region Private Non-Grabber Variables
    // NOT SERIALIZED
    [Tooltip("Stores a boolean value determining whether the button thumb was registered or not")]
    private bool buttonThumbRegistered = false;
    // NOT SERIALIZED
    [Tooltip("Holds a reference to timings related to the select button")]
    private float selectButtonHeldTime = 0f;
    // NOT SERIALIZED
    [Tooltip("Holds a reference to timings related to the delete button")]
    private float deleteButtonHeldTime = 0f;
    // NOT SERIALIZED
    [Tooltip("Tracks of keys were changed by the grabber's thumbstick or not")]
    private bool changedKeys = false;
    #endregion


    // NOT SERIALIZED
    [Tooltip("A dictionary of all keys available")]
    private Dictionary<string, float> m_inputTimes = new Dictionary<string, float>();
    // NOT SERIALIZED
    [Tooltip("A dictionary of all key presses")]
    private Dictionary<string, float> m_inputDowns = new Dictionary<string, float>();
    // NOT SERIALIZED
    [Tooltip("Storing thumbstick position of both grabber and non-grabber")]
    private Dictionary<string, Vector2> m_thumbDirections = new Dictionary<string, Vector2>();
    // NOT SERIALIZED
    [Tooltip("Storing the angle and distance from center for both thumbsticks")]
    private Dictionary<string, Vector2> m_thumbAngles = new Dictionary<string, Vector2>();
    // NOT SERIALIZED
    [Tooltip("Used for sorting buttons we want a held property to detect")]
    private List<string> m_shouldTrackForHold = new List<string>();
    // NOT SERIALIZED
    [Tooltip("Tracks if the index trigger of the grabbed hand has changed or not")]
    private float m_grabbedTriggerPrevTime = -1f;
    // NOT SERIALIZED
    [Tooltip("Tracks the last key pressed, at least out of those indicated in m_shouldTrackForHold")]
    private string m_latest = "NONE";
    // NOT SERIALIZED
    [Tooltip("Stores the time of last action - default = 0")]
    private float lastActionTime = 0f;
    // NOT SERIALIZED
    [Tooltip("Reference to HoldButton coroutine")]
    private IEnumerator m_holdCoroutine;

    private int curKeyboardID;
	
	[SerializeField]
    [Tooltip("Determines whether a line should be rendered or not when attaching the keyboard to a target")]
    private bool m_useLineRenderer = true;

    // Start is called before the first frame update
    void Start()
    {
        sampleSupport = GameObject.Find("SampleSupport");
        instructionCanvas = GameObject.Find("InstructionCanvas");
        testCanvas = GameObject.Find("TestCanvas");
        keyboardCanvas = GameObject.Find("KeyboardCanvas");
        camera = GameObject.Find("CenterEyeAnchor");
        // Grab the reference to this keyboard's custom grabbable script
        m_CustomGrabbable = this.GetComponent<CustomGrabbable>();

        // Initializes the Input script, which controls everything when it comes to printing text in the VR world
        m_Input.Initialize(this, m_fontSize);

        // The inspector allows us to select what kind of keyboard format we want.
        // We first need to make all keyboard formats saved so that they are inactive in the hierarchy
        foreach (KeyboardTypeControllerVer2 k in m_KeyboardTypeList) {
            k.gameObject.SetActive(false);
        }
        // Now, we make only the selected keyboard format viewable
        switch(m_KeyboardType) {
	        case(KeyboardTypeDropdown.Wheel):
	            curKeyboardID = 0;
	            break;
            case(KeyboardTypeDropdown.Wheel_With_Cheat):
	            curKeyboardID = 1;
	            break;
	        default:
	            curKeyboardID = 0;
                break;
	    }
        m_KeyboardTypeController = m_KeyboardTypeList[curKeyboardID];
        m_KeyboardTypeController.gameObject.SetActive(true);
        
        // The chosen keyboard type controller has some other children of its own
        // We need to initialize Preview, which shows us the preview text
        m_Preview = m_KeyboardTypeController.preview;
        m_cursor = m_KeyboardTypeController.cursor;
        m_Preview.Initialize(this, m_fontSize);

        // Our keyboard can technically have multiple overlays, but for now we save the chosen keyboard format's overlay as a reference
        if (m_KeyboardTypeController.overlay != null) {
            m_OverlayList.Add(m_KeyboardTypeController.overlay);
        }
        // For each overlay we have, we initialize them too
        foreach (KeyboardOverlayVer2 overlay in m_OverlayList) {
            overlay.Initialize(this);
        }

        // If this keyboard comes with a custom pointer, we prep it for our keyboard
        if (m_CustomPointer != null) {  
            m_CustomPointer.LineOff();
            m_CustomPointer.Activate(); 
        }

        // Check if this thing must be grabbed in order for it to work.
        if (m_customGrabber == null) {
            m_CustomGrabbable.enabled = true;
            m_mustBeGrabbed = true;
        } else {
            GrabBegin(m_customGrabber.m_controller);
            m_CustomGrabbable.enabled = false;
            m_mustBeGrabbed = false;
        }
        if (m_targetText != null) {
            ActivateKeyboard(m_targetText.gameObject);
            m_targetSet = true;
        } else {
            m_targetSet = false;
        }

        // Update our dictionaries
        // Grabbing hand
        m_inputTimes.Add("grab_index",-1f);
        m_inputTimes.Add("grab_grip",-1f);
        m_inputTimes.Add("grab_one",-1f);
        m_inputTimes.Add("grab_two",-1f);
        m_inputTimes.Add("grab_thumbDir",-1f);
        m_inputTimes.Add("grab_thumbPress",-1f);

        m_inputDowns.Add("grab_index",-1f);
        m_inputDowns.Add("grab_grip",-1f);
        m_inputDowns.Add("grab_one",-1f);
        m_inputDowns.Add("grab_two",-1f);
        m_inputDowns.Add("grab_thumbNorth",-1f);
        m_inputDowns.Add("grab_thumbSouth",-1f);
        m_inputDowns.Add("grab_thumbEast",-1f);
        m_inputDowns.Add("grab_thumbWest",-1f);
        m_inputDowns.Add("grab_thumbPress",-1f);

        m_thumbDirections.Add("grab", Vector2.zero);
        m_thumbAngles.Add("grab", Vector2.zero);

        // Non-grabbing hand
        m_inputTimes.Add("non_index",-1f);
        m_inputTimes.Add("non_grip",-1f);
        m_inputTimes.Add("non_one",-1f);
        m_inputTimes.Add("non_two",-1f);
        m_inputTimes.Add("non_thumbDir",-1f);
        m_inputTimes.Add("non_thumbPress",-1f);

        m_inputDowns.Add("non_index",-1f);
        m_inputDowns.Add("non_grip",-1f);
        m_inputDowns.Add("non_one",-1f);
        m_inputDowns.Add("non_two",-1f);
        m_inputDowns.Add("non_thumbNorth",-1f);
        m_inputDowns.Add("non_thumbSouth",-1f);
        m_inputDowns.Add("non_thumbEast",-1f);
        m_inputDowns.Add("non_thumbWest",-1f);
        m_inputDowns.Add("non_thumbPress",-1f);

        m_thumbDirections.Add("non", Vector2.zero);
        m_thumbAngles.Add("non", Vector2.zero);

        m_shouldTrackForHold.Add("grab_thumbPress");
        m_shouldTrackForHold.Add("non_one");
        m_shouldTrackForHold.Add("non_two");

        // Store a reference to HoldCoroutine... and then start it
        m_holdCoroutine = HoldButton();
        StartCoroutine(m_holdCoroutine);

        //Finally, we just update our keyboard (more is explained in the function)
        UpdateKeyboard(true);

        // Boom
        return;
    }

    private void UpdateKeyboard(bool forceUpdate = true) {

        // So sometimes, a keyboard type has a list of subset of keys.
        // Sometimes keyboards have only one subset of keys - sometimes they may have two.
        // regardless of how many key subsets a keyboard type has, we need to make sure that the current key subset we're using is up to date
        // We also need to check if the key subset has been changed or not, according to the index trigger
        if (forceUpdate || m_inputTimes["grab_index"] != m_grabbedTriggerPrevTime) {
            m_KeyboardTypeController.keysContainerCurrent = (m_inputTimes["grab_index"] > -1f) ? 1 : 0;
            // The chosen keyboard has its own keys saved under it. We need to grab them and save them as a reference
            m_Keys = m_KeyboardTypeController.keys;
            // For each key, we alter collision (default set to FALSE) + reset the key so that they're ready for the keyboard
            for (int i = 0; i < m_Keys.Count; i++) {
                m_Keys[i].CollisionToggle(allowCollision);
                m_Keys[i].ResetKey();
            }
        }
        m_grabbedTriggerPrevTime = m_inputTimes["grab_index"];
        return;
    }

    private void Update()
    {
        
        //update instruction canvas position
        //instructionCanvas.transform.position = new Vector3(0, 0, 2) + camera.transform.position;
        //instructionCanvas.transform.position = new Vector3(0, 3, 2) + camera.transform.position;

        // Check if the object is being grabbed or not
        if (m_mustBeGrabbed) CheckGrabbedBy();
        // Prevent any actions unless it's being held by a hand
        if (m_GrabbedBy == OVRInput.Controller.None) return;

        // Check if any inputs have been detected
        CheckInputs();

        // DEBUG - change keyboard type by button
	    if (m_inputDowns["grab_two"] != -1f) {
	        SwitchKeyboard();
            return;
	    }

        // Before we process any inputs, we have to check if we're actually hitting an object... or if we're actually holding the trigger
        if (!m_targetSet) CheckTarget();
        // If no target was selected, we stop here
        if (m_targetText == null) return;


        clearWords = GameObject.Find("TestingSuite").GetComponent<TestingSuite>().clearWords;
        if (clearWords)
        {
            m_Input.text = "";
            m_Preview.text = "";

        }

        // With inputs checked, we can now do things related to those inputs
        // The first step is to update the keyboard in case we updated which keyboard we wanted to use - more explained in the function itself
        UpdateKeyboard(false);
    
        // Process any Inputs
        ProcessInputs();

        // Add scaffold if enough time has passed
        /*
        if (lastActionTime != 0f && Time.time - lastActionTime > m_AddCharacterThresholdTime) {
            AddScaffold(true);
        }
        */

        //m_Input.SetText(m_thumbDirections["grab"].ToString() + " | "  + m_thumbAngles["grab"].ToString() + " | " + m_inputTimes["grab_index"].ToString() + " | " + m_inputTimes["grab_grip"].ToString() + " | " + m_inputTimes["grab_one"].ToString() + " | " + m_inputTimes["grab_two"].ToString() + " | " + m_inputTimes["grab_thumbDir"].ToString() + " | " + m_inputTimes["grab_thumbPress"].ToString());
        //m_Input.SetText(m_thumbDirections["non"].ToString() + " | "  + m_thumbAngles["non"].ToString() + " | " + m_inputTimes["non_index"].ToString() + " | " + m_inputTimes["non_grip"].ToString() + " | " + m_inputTimes["non_one"].ToString() + " | " + m_inputTimes["non_two"].ToString() + " | " + m_inputTimes["non_thumbDir"].ToString() + " | " + m_inputTimes["non_thumbPress"].ToString());
        //m_Input.SetText(m_cursor.localPosition.ToString());

        // Reset some bools
        changedKeys = false;

        session2 = GameObject.Find("SampleSupport").GetComponent<DebugUISample2>().secondSession;

        if(session2 && switchKey == false)
        {
            SwitchKeyboard();
            switchKey = true;
        }
    }

    private void CheckGrabbedBy() {
        // Checks the CustomGrabbable.cs script to see if a grabber is registered.
        // If there is, then we initialize the GrabBegin() for this particular script
        // Else, we initialize the GrabEnd() for this particular script
        OVRInput.Controller gb = m_CustomGrabbable.GetGrabber();
        if (gb != OVRInput.Controller.None)     GrabBegin(gb);
        else                                    GrabEnd();
        return;
    }
    public void GrabBegin(OVRInput.Controller g) {
        // This GrabBegin() must do several things:
        // 1) Set the GrabbedBy to what was provided in the argument
        // 2) Determine which controller is the grabber and which is the button presser
        m_GrabbedBy = g;
        switch(g) {
            case(OVRInput.Controller.LTouch):
                m_ButtonController = OVRInput.Controller.RTouch;
                m_Input.transform.localPosition = new Vector3(0.15f, 0f, 0f);
                break;
            case(OVRInput.Controller.RTouch):
                m_ButtonController = OVRInput.Controller.LTouch;
                m_Input.transform.localPosition = new Vector3(-0.15f, 0f, 0f);
                break;
            case(OVRInput.Controller.LTrackedRemote):
                m_ButtonController = OVRInput.Controller.RTrackedRemote;
                m_Input.transform.localPosition = new Vector3(0.15f, 0f, 0f);
                break;
            case(OVRInput.Controller.RTrackedRemote):
                m_ButtonController = OVRInput.Controller.LTrackedRemote;
                m_Input.transform.localPosition = new Vector3(-0.15f, 0f, 0f);
                break;
            default:
                m_ButtonController = g;
                m_Input.transform.localPosition = new Vector3(0.15f, 0f, 0f);
                break;
        }
        return;
    }
    public void GrabEnd() {
        // This GrabEnd() must do several things:
        // 1) reset the GrabbedBy to None
        // 2) Reset the keyboard itself, which means setting all keys to their default state and deactivating the keyboard
        m_GrabbedBy = OVRInput.Controller.None;
        AddScaffold(true);
        DeactivateKeyboard();
        if (currentKeyIndex != -1) {
            m_Keys[currentKeyIndex].ResetKey();
            currentKeyIndex = -1;
        }
        return;
    }
    public KeyboardKeyVer2 GetCurrentKey() {
        // Gets the current key thats activated by the grabber's thumbstick
        if (currentKeyIndex != -1) return m_Keys[currentKeyIndex];
        return null;
    }

    private void CheckInputs() {

        #region Inputs For Grabber
        // Check inputs for grabber. Inputs for grabber include:
        // Thumbstick Direction: Numpad Select
        // Next Button: shortcut to Space, press down on thumbstick
        // Index Trigger: Textbox selection or release
        m_inputTimes["grab_index"] =    (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_GrabbedBy)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_GrabbedBy)) ? -1f : m_inputTimes["grab_index"];
        m_inputTimes["grab_grip"] =     (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, m_GrabbedBy)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, m_GrabbedBy)) ? -1f : m_inputTimes["grab_grip"];
        m_inputTimes["grab_one"] =      (OVRInput.GetDown(OVRInput.Button.One, m_GrabbedBy)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.One, m_GrabbedBy)) ? -1f : m_inputTimes["grab_one"];
        m_inputTimes["grab_two"] =      (OVRInput.GetDown(OVRInput.Button.Two, m_GrabbedBy)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.Two, m_GrabbedBy)) ? -1f : m_inputTimes["grab_two"];
        m_inputTimes["grab_thumbDir"] = (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_GrabbedBy) != Vector2.zero) ? (m_inputTimes["grab_thumbDir"] == -1f) ? Time.time : m_inputTimes["grab_thumbDir"] : -1f;
        m_inputTimes["grab_thumbPress"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, m_GrabbedBy)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, m_GrabbedBy)) ? -1f : m_inputTimes["grab_thumbPress"];
        
        m_inputDowns["grab_index"] = (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_grip"] = (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_one"] = (OVRInput.GetDown(OVRInput.Button.One, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_two"] = (OVRInput.GetDown(OVRInput.Button.Two, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_thumbNorth"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_thumbSouth"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_thumbEast"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_thumbWest"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, m_GrabbedBy)) ? 1f : -1f;
        m_inputDowns["grab_thumbPress"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, m_GrabbedBy)) ? 1f : -1f;

        m_thumbDirections["grab"] =     Vector2.ClampMagnitude(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_GrabbedBy), 0.4f);
        m_thumbAngles["grab"] = new Vector2(GetAngleFromVector2(m_thumbDirections["grab"],m_GrabbedBy), Vector2.Distance(Vector2.zero, m_thumbDirections["grab"]));
        m_cursor.localPosition = new Vector3(Mathf.Cos(m_thumbAngles["grab"].x * Mathf.Deg2Rad), Mathf.Sin(m_thumbAngles["grab"].x * Mathf.Deg2Rad), 0f) * m_thumbAngles["grab"].y * -1f;
        
        #endregion

        #region Inputs For Non-Grabber
        // Check inputs for non-grabber. Inputs for non-grabber include:
        // Thumstick Right - Next Character, Space
        // A/X: Select Numpad
        // B/Y: Delete
        m_inputTimes["non_index"] =    (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_ButtonController)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, m_ButtonController)) ? -1f : m_inputTimes["non_index"];
        m_inputTimes["non_grip"] =     (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, m_ButtonController)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, m_ButtonController)) ? -1f : m_inputTimes["non_grip"];
        m_inputTimes["non_one"] =      (OVRInput.GetDown(OVRInput.Button.One, m_ButtonController)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.One, m_ButtonController)) ? -1f : m_inputTimes["non_one"];
        m_inputTimes["non_two"] =      (OVRInput.GetDown(OVRInput.Button.Two, m_ButtonController)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.Two, m_ButtonController)) ? -1f : m_inputTimes["non_two"];
        m_inputTimes["non_thumbDir"] = (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_ButtonController) != Vector2.zero) ? (m_inputTimes["non_thumbDir"] == -1f) ? Time.time : m_inputTimes["non_thumbDir"] : -1f;
        m_inputTimes["non_thumbPress"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, m_ButtonController)) ? Time.time : (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, m_ButtonController)) ? -1f : m_inputTimes["non_thumbPress"];
        
        m_inputDowns["non_index"] = (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_grip"] = (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_one"] = (OVRInput.GetDown(OVRInput.Button.One, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_two"] = (OVRInput.GetDown(OVRInput.Button.Two, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_thumbNorth"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_thumbSouth"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_thumbEast"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_thumbWest"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, m_ButtonController)) ? 1f : -1f;
        m_inputDowns["non_thumbPress"] = (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, m_ButtonController)) ? 1f : -1f;
        
        m_thumbDirections["non"] =     Vector2.ClampMagnitude(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, m_ButtonController), 0.4f);
        m_thumbAngles["non"] = new Vector2(GetAngleFromVector2(m_thumbDirections["non"], m_ButtonController), Vector2.Distance(Vector2.zero, m_thumbDirections["non"]));
        
        #endregion

        return;
    }

    private float GetAngleFromVector2(Vector2 original, OVRInput.Controller source) {
        // Derive angle from y and x
        float angle = Mathf.Atan2(original.y, original.x) * Mathf.Rad2Deg + 180f;
        // We need to do some offsettting becuase for some inane reason the thumbsticks have a ~5-degree offset
        switch(source) {
            case(OVRInput.Controller.LTouch):
                // need to add 5 degrees
                angle += 5f;
                break;
            case(OVRInput.Controller.RTouch):
                // Need to subtract 5 degrees
                angle -= 5f;
                break;
        }
        // We need to recenter the angle so that it's between 0 and 360, not 5 and 365
        angle = (angle > 360f) ? angle - 360 : angle;
        // Return
        return angle;
    }

    private void CheckTarget() {

        // The custom pointer is set so that it detects items even when the laser is not working.
        // We want to do some hickery pockery so that the line renders when it hits something
        GameObject t = m_CustomPointer.raycastTarget;
        if (m_targetText == null)   m_CustomPointer.LineSet(t != null);
        else                        m_CustomPointer.LineOn();

        // Next, we check if the trigger button was actually pressed down
        if (m_inputTimes["grab_index"] != -1f) {
            // In this case, we have two scenarios: if we're tracking a textbox already or not
            if (m_targetText == null) {
                // We haven't hit a target yet. We need to verify if this is an accceptable target or not
                if (t!= null && t.GetComponent<KeyboardTextbox>() != null && t.GetComponent<KeyboardTextbox>().keyboardController == null && (m_targetText == null || !GameObject.ReferenceEquals(m_targetText.gameObject,t.GetComponent<KeyboardTextbox>().gameObject))) {
                    ActivateKeyboard(t);
                }
            } else {
                // We have a target. We need to disassociate it with our keyboard
                DeactivateKeyboard();
            }
        }
        return;
    }

    private void ActivateKeyboard(GameObject targetTextbox) {
        // A function that activates the keyboard for input
        // usually initialized when the keyboard's custom pointer detects a textbox and we press the index trigger to select it.
        m_targetText = targetTextbox.GetComponent<KeyboardTextbox>();
        m_targetText.keyboardController = this;
        m_Input.Activate();
        m_Preview.Activate();
        m_CustomPointer.SetPointerType("SetTarget",targetTextbox);
        if (m_useLineRenderer) m_CustomPointer.LineOn();
        m_CustomPointer.lineColor = Color.magenta;
        foreach (KeyboardOverlayVer2 overlay in m_OverlayList) {
            overlay.Activate();
        }
        m_Input.SetText(m_targetText.text);
    }
    private void DeactivateKeyboard(bool resetTarget = true) {
        // A function that deactivates the keyboard
        // Usually called when the index trigger is pressed while a textbox is being edited by the keyboard
        if (resetTarget) m_targetText = null;
        m_Input.Deactivate();
        m_Preview.Deactivate();
        m_CustomPointer.SetPointerType("Target");
        if (m_useLineRenderer) m_CustomPointer.LineOff();
        m_CustomPointer.lineColor = Color.yellow;
        foreach (KeyboardOverlayVer2 overlay in m_OverlayList) {
            overlay.Deactivate();
        }
    }

    private bool BetweenValues(float val, float min, float max, bool inclusive) {
        // Function that converts a Vector2 between (1,1) and (-1,-1) to angles (<-- == 0 degrees, --> == 180 degrees)
        if (inclusive)  return (val-min)*(max-val)>=0;
        else            return (val-min)*(max-val)>0;
    }
    private bool BetweenValues(float val, float min, float max, float inclusive) {
        // Function that converts a Vector2 between (1,1) and (-1,-1) to angles (<-- == 0 degrees, --> == 180 degrees)
        if (inclusive == 1f)    return (val-min)*(max-val)>=0;
        else                    return (val-min)*(max-val)>0;
    }
    private void ProcessInputs() {

        // Firstly, I want to check which key was last pressed
        string curLatest = m_inputTimes.Where(kvp => m_shouldTrackForHold.Contains(kvp.Key)).ToDictionary(it => it.Key, it => it.Value).Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        // We need to compare it with the previous key - if they are the same key, then we simply add the time difference, else we restart the timer
        if (m_inputTimes[curLatest] == -1f) curLatest = "NONE";

        if (curLatest == "NONE") lastActionTime = 0f;
        else lastActionTime = Time.time - m_inputTimes[curLatest];
        m_latest = curLatest;

        // Secondly, gotta process the grabber inputs - namely the joystick
        // Process thumbstick input
        int index = -1;
        if (m_thumbAngles["grab"].y < 0.15f) index = m_KeyboardTypeController.defaultIndex;  
        else {
            foreach(Vector4 m in m_KeyboardTypeController.thumbstickAngleMapping) {
                if ( BetweenValues(m_thumbAngles["grab"].x, m.x, m.y, m.z) ) {
                    index = (int)m.w;
                    break;
                }
            }
        }
        if (index == -1 && currentKeyIndex != -1) {
            m_Keys[currentKeyIndex].ResetKey();
            currentKeyIndex = -1;
        }
        if (currentKeyIndex == -1) currentKeyIndex = index;
        else if (index != currentKeyIndex) {
            m_Keys[currentKeyIndex].ResetKey();
            currentKeyIndex = index;
            m_Keys[currentKeyIndex].HighlightKey();
            changedKeys = true;
        }
        if (m_inputDowns["grab_thumbPress"] == 1f) {
            AddScaffold(true);
            AddSpace();
        }
        if (m_inputDowns["grab_index"] == 1f) AddScaffold(true);
        if (changedKeys) AddScaffold(true);

        // Thirdly, gotta process the non-grabber inputs - namely Button.One, Button.Two, and ThumbPress
        // Dead space zone
        if (m_thumbAngles["non"].y < 0.15f) buttonThumbRegistered = false;
        else if (!buttonThumbRegistered) {
            if (m_Input.GetScaffold() != "") AddScaffold(true);
            //else AddSpace();
            m_Keys[currentKeyIndex].ResetCharactersIndex();
            buttonThumbRegistered = true;
        }
        // Select (A) Button
        if (m_inputTimes["non_one"] != -1f) m_Keys[currentKeyIndex].SelectKey();
        else                        m_Keys[currentKeyIndex].HighlightKey();
        // If we changed keys in the interim, we add the current scaffold first
        if (m_inputDowns["non_one"] == 1f) {
            // The button has been pushed down
            // we select the next character for that key, if that key has more than 1 character
            if (m_Keys[currentKeyIndex].characters.Count > 1f ) {
                 m_Keys[currentKeyIndex].NextCharactersIndex();
                // Add the currently selected key as scaffolding
                UpdateScaffold(m_Keys[currentKeyIndex].GetCurrentCharacter());
            } 
            // If there is only one character, we add that character and then reset
            else {
                m_Keys[currentKeyIndex].SetCharactersIndex(0);
                UpdateScaffold(m_Keys[currentKeyIndex].GetCurrentCharacter());
                AddScaffold(true);
            }
        }
        // Index trigger - automatically add scaffold
        if (m_inputDowns["non_index"] == 1f) AddScaffold(true);
        // Delete button
        if (m_inputDowns["non_two"] == 1f) {
            // The button has been pushed down now

            // For now, we'l have the joystick control the next character in text stuff.
            // it'll just be that for now, if the button has been pressed down, we select the next character for that key
            if (m_Input.GetScaffold() == "") DeleteCharacter();
            else ResetScaffold();
        }

        // Fourthly, miscellaneous reset some variables
        changedKeys = false;

        // Boom
        return;
    }

    private IEnumerator HoldButton() {
        while(true) {
            // If our last action is holding and is taking up more than 1 second of holding, we do the hold
            if (m_latest != "NONE" && lastActionTime > 0.5f) {
                switch(m_latest) {
                    case("grab_thumbPress"):
                        AddSpace();
                        break;
                    case("non_one"):
                        AddScaffold(false, true);
                        break;
                    case("non_two"):
                        DeleteCharacter();
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(0.075f);
            } else {
                yield return null;
            }
        }
    }

    private void AddScaffold(bool reset, bool lastWasHold = false) {
        m_Input.AddScaffold(reset, lastWasHold);
        m_Preview.AddScaffold(reset, lastWasHold);
        //lastActionTime = 0f;
    }
    private void AddSpace() {
        m_Input.AddSpace();
        m_Preview.AddSpace();
        //lastActionTime = 0f;
    }
    private void UpdateScaffold(string s) {
        m_Input.UpdateScaffold(s);
        m_Preview.UpdateScaffold(s);
        //lastActionTime = 0f;
    }
    private void ResetScaffold() {
        m_Input.ResetScaffold();
        m_Preview.ResetScaffold();
        //lastActionTime = 0f;
    }
    private void DeleteCharacter() {
        m_Input.DeleteCharacter();
        m_Preview.DeleteCharacter();
        //lastActionTime = 0f;
    }

    public void SwitchKeyboard() {
	    curKeyboardID = (curKeyboardID == 1) ? 0 : 1;
	    DeactivateKeyboard(false);
	    m_KeyboardTypeController.gameObject.SetActive(false);
	    m_KeyboardTypeController = m_KeyboardTypeList[curKeyboardID];
        m_KeyboardTypeController.gameObject.SetActive(true);
        m_OverlayList = new List<KeyboardOverlayVer2>();
	    // The chosen keyboard type controller has some other children of its own
	    // We need to initialize Preview, which shows us the preview text
	    m_Preview = m_KeyboardTypeController.preview;
	    m_cursor = m_KeyboardTypeController.cursor;
	    m_Preview.Initialize(this, m_fontSize);
	    // Our keyboard can technically have multiple overlays, but for now we save the chosen keyboard format's overlay as a reference
	    if (m_KeyboardTypeController.overlay != null) {
	        m_OverlayList.Add(m_KeyboardTypeController.overlay);
	    }
	    // For each overlay we have, we initialize them too
	    foreach (KeyboardOverlayVer2 overlay in m_OverlayList) {
	        overlay.Initialize(this);
	    }
	    if (m_targetText != null) {
	        ActivateKeyboard(m_targetText.gameObject);
	        m_targetSet = true;
	    } else {
	        m_targetSet = false;
	    }
	    UpdateKeyboard(true);
	}
}
