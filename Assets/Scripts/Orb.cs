using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent (typeof (AudioSource))]  

public class Orb : MonoBehaviour
{
    private SceneController SC;

    private OVRGrabbable grabbableRef;
    private Renderer rendererRef;
    public GameObject buttonCanvas;
    public GameObject pointLight;
    
    private bool recording;
    private AudioSource aud;
    private Light lightComp;
    private ParticleSystem particles;
    private ParticleSystem.MainModule particlesMain;
    public OVRInput.Button shootingButton;
    private bool isTriggered;
    private bool isGrabbed;
    public float frequency, amplitude;

    //private IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        SC = GameObject.Find("SceneController").GetComponent<SceneController>();
        grabbableRef = this.GetComponent<OVRGrabbable>();
        rendererRef = this.GetComponent<Renderer>();
        aud = this.GetComponent<AudioSource>();
        lightComp = pointLight.GetComponent<Light>();
        particles = this.GetComponent<ParticleSystem>();
        particlesMain = particles.main;

        recording = false;
        isTriggered = false;
        isGrabbed = false;
        SetColor(Color.yellow);
    }

    // Update is called once per frame
    void Update()
    {
        isGrabbed = grabbableRef.isGrabbed;
        if (OVRInput.GetDown(shootingButton,grabbableRef.grabbedBy.GetController())) isTriggered = true;
        if (OVRInput.GetUp(shootingButton,grabbableRef.grabbedBy.GetController())) isTriggered = false;
        
        if (isGrabbed) {
            buttonCanvas.SetActive(false);
            OVRInput.SetControllerVibration (frequency,amplitude,grabbableRef.grabbedBy.GetController());
        } else {
            OVRInput.SetControllerVibration (0,0,grabbableRef.grabbedBy.GetController());
        }
         
        /*
        if (OVRInput.GetDown(shootingButton,grabbableRef.grabbedBy.GetController()) && !isTriggered) isTriggered = true;
        if (OVRInput.GetUp(shootingButton,grabbableRef.grabbedBy.GetController()) && isTriggered) isTriggered = false;
        */
        
        /*
        if (grabbableRef.isGrabbed) {
            buttonCanvas.SetActive(true);
            if (isTriggered) {
                aud.mute = false;
                SetColor(Color.blue);
            } else {
                aud.mute = true;
                SetColor(Color.green);
            }
        }
        else {
            EndRecording();
            SetColor(Color.yellow);
            aud.mute = true;
            buttonCanvas.SetActive(false);
        }
        */
    }
    /*
    private IEnumerator GetMicCount() {
        while(true) {

            yield return new WaitForSeconds(0.1f);
        }
    }
    */

    public SceneController GetSceneController() {
        return SC;
    }
    public void SetColor(Color c) {
        //rendererRef.material.SetColor("_Color", c);
        lightComp.color = c;
        particlesMain.startColor = c;
    }
    public bool GetTriggered() {
        return isTriggered;
    }
    public bool GetGrabbed() {
        return isGrabbed;
    }
}
