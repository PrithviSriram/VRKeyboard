   
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    private bool micConnected = false;      //A boolean that flags whether there's a connected microphone 
    private int minFreq, maxFreq;    //The minimum and maximum available recording frequencies  
    public Camera centerEyeCamera;
    public GameObject frontOfAvatar;
    public GameObject orbPrefab;
    public List<GameObject> orbs;
    private bool isTriggered;
	private AudioSource goAudioSource;  //A handle to the attached AudioSource

    public GameObject curOrb;
    private Vector3 curOrbVelocity;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.RoomScale);

        orbs = new List<GameObject>();
        goAudioSource = this.GetComponent<AudioSource>();

        curOrbVelocity = Vector3.zero;
        
        micConnected = Microphone.devices.Length > 0;
        if (micConnected) {
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);   //Get the default microphone recording capabilities 
            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            //...meaning 44100 Hz can be used as the recording sampling rate 
            if(minFreq == 0 && maxFreq == 0) maxFreq = 44100; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One)) {
            //GameObject newOrb = Instantiate(orbPrefab, centerEyeCamera.transform.position + Vector3.forward * 0.2f, Quaternion.identity);
            //orbs.Add(newOrb);
            curOrb.transform.position = Vector3.SmoothDamp(curOrb.transform.position, frontOfAvatar.transform.position + Vector3.forward * 0.2f + Vector3.up * -0.2f, ref curOrbVelocity, 1f);
            curOrb.GetComponent<Rigidbody>().velocity = curOrbVelocity;
        }
        
        /*
        isTriggered = false;
        foreach(GameObject orb in orbs) {
            if (orb.GetComponent<Orb>().GetTriggered()) {
                isTriggered = true;
            }
        }
        if (isTriggered && !Microphone.IsRecording(null)) {
			goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);
			//orb.SetColor(Color.blue);
		}
		else if (!isTriggered && Microphone.IsRecording(null)) {
			Microphone.End(null); //Stop the audio recording
			goAudioSource.Play(); //Playback the recorded audio
			//orb.SetColor(Color.green);
		} else {
			//orb.SetColor(Color.yellow);
		}
        */

    }

    public bool GetMicStatus() {
        return micConnected;
    }
    public int GetMinFreq() {
        return minFreq;
    }
    public int GetMaxFreq() {
        return maxFreq;
    }
}
