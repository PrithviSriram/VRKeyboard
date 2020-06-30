using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    #region Globals
    public GameObject starPrefab;
    private bool isCreatingStar = false;
    public Transform StarBirthLocation;
    public bool m_InputActive = true;   // PUBLIC - either enables or disables all inputs
    private bool micConnected;          // tracks if mic is connected to Oculus or not
    private int minFreq, maxFreq;       // the minimum and maximum frequency of the current microphone
    private List<GameObject> m_Stars = new List<GameObject>();  // Tracks all stars in the world
    private GameObject emptyStar = null;
    #endregion

    #region Inputs
    // Keep track of controllers and their orbs
    private Dictionary<OVRInput.Controller, bool> m_ControllerSets = new Dictionary<OVRInput.Controller, bool>();
    // Component for audio recording - useless at the moment, but probably important to keep
    private AudioSource m_AudioSource;
    #endregion

    private void Awake() {
        // Set the tracking space to roomscale
        UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.RoomScale);
        // get the AudioSource component
        m_AudioSource = this.GetComponent<AudioSource>();
        // Genreate controller sets
        m_ControllerSets = CreateControllerSets();
    }

    private void Update() {
        
        // Block if inputs should be disabled
        if (!m_InputActive) return;
        
        // Check the status of any microphone attached to our oculus
        CheckMicStatus();
    }

    private Dictionary<OVRInput.Controller, bool> CreateControllerSets() {
        Dictionary<OVRInput.Controller, bool> newSets = new Dictionary<OVRInput.Controller, bool>()
        {
            { OVRInput.Controller.LTouch, false },
            { OVRInput.Controller.RTouch, false }
        };
        return newSets;
    }

    private void CheckMicStatus() {
        // Check if there is any mic connected to Oculus
        micConnected = Microphone.devices.Length > 0;
        if (micConnected) {
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);   //Get the default microphone recording capabilities 
            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            //...meaning 44100 Hz can be used as the recording sampling rate 
            if(minFreq == 0 && maxFreq == 0) maxFreq = 44100; 
        }
    }

    public bool GetMicStatus() {
        return micConnected;
    }
    public List<int> GetFrequencies() {
        List<int> frequencies = new List<int>();
        frequencies.Add(minFreq);
        frequencies.Add(maxFreq);
        return frequencies;
    }
    public int GetMinFrequency() {
        return minFreq;
    }
    public int GetMaxFrequency() {
        return maxFreq;
    }
    public void AddStar(GameObject star) {
        if (m_Stars.Contains(star)) return;
        m_Stars.Add(star);
        if (emptyStar == star) emptyStar = null;
    }
    public void RemoveStar(GameObject star) {
        if (!m_Stars.Contains(star)) return;
        m_Stars.Remove(star);
    }
    public void SetControllerStatus(OVRInput.Controller controller, bool status) {
        m_ControllerSets[controller] = status;
    }

    public KeyValuePair<OVRInput.Controller, bool> CanRecord() {

        KeyValuePair<OVRInput.Controller, bool> toReturn = new KeyValuePair<OVRInput.Controller, bool>(OVRInput.Controller.None, false);

        foreach(KeyValuePair<OVRInput.Controller, bool> pair in m_ControllerSets) {
            //Use pair.Key to get the key
            //Use pair.Value for value
            if (pair.Value == true) {
                toReturn = pair;
                break;
            }
        }

        return toReturn;

    }

    public IEnumerator CreateStar() {
        if (emptyStar == null) {
            emptyStar = Instantiate(starPrefab, StarBirthLocation.position, Quaternion.identity);
        }
        // else {}
        yield return null;
    }

    public IEnumerator CreateStarAtPosition(Vector3 target) {
        if (emptyStar == null) {
            emptyStar = Instantiate(starPrefab, target, Quaternion.identity);
        }
        // else {}
        yield return null;
    }

    public void ToggleRecording() {
        return;
    }

}
