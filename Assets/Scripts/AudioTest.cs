using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : OVRGrabbable
{ 
    private Orb orbRef;
    private AudioSource goAudioSource;  //A handle to the attached AudioSource  

    /// <summary>
	/// Notifies the object that it has been grabbed.
	/// </summary>
	public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        m_grabbedBy = hand;
        m_grabbedCollider = grabPoint;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
	/// Notifies the object that it has been released.
	/// </summary>
    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic;
        rb.velocity = linearVelocity;
        rb.angularVelocity = angularVelocity;
        m_grabbedBy = null;
        m_grabbedCollider = null;
    }

    /*
    void Start()
    {
        //Check if there is at least one microphone connected  
        //Throw a warning message at the console if there isn't 
        if(Microphone.devices.Length <= 0)  Debug.LogWarning("Microphone not connected!");
        
        //At least one microphone is present 
        else {  
            micConnected = true;    //Set 'micConnected' to true    
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);   //Get the default microphone recording capabilities 
            
            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            //...meaning 44100 Hz can be used as the recording sampling rate 
            if(minFreq == 0 && maxFreq == 0)    maxFreq = 44100; 
  
            goAudioSource = this.GetComponent<AudioSource>();  //Get the attached AudioSource component  
        }  
    }

    void OnGUI()   
    {  
        //If there is a microphone  
        if(micConnected)  
        {  
            //If the audio from any microphone isn't being captured  
            if(!Microphone.IsRecording(null))  
            {  
                //Case the 'Record' button gets pressed  
                if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Record"))  
                {  
                    //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
                    goAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);  
                }  
            }  
            else //Recording is in progress  
            {  
                //Case the 'Stop and Play' button gets pressed  
                if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Stop and Play!"))  
                {  
                    Microphone.End(null); //Stop the audio recording  
                    goAudioSource.Play(); //Playback the recorded audio  
                }  
  
                GUI.Label(new Rect(Screen.width/2-100, Screen.height/2+25, 200, 50), "Recording in progress...");  
            }  
        }  
        else // No microphone  
        {  
            //Print a red "Microphone not connected!" message at the center of the screen  
            GUI.contentColor = Color.red;  
            GUI.Label(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Microphone not connected!");  
        }  
  
    }  
    */
}
