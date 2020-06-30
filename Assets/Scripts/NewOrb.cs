using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class NewOrb : MonoBehaviour
{
    #region Publics
    public GameObject m_PointLight;
    public GameObject m_EmptyIndicator;
    public GameObject m_Crosshairs;
    public OVRInput.RawAxis1D m_StartAudioButton;
    public OVRInput.RawButton m_PlayAudioButton;
    #endregion

    #region Privates
    private OVRInput.Controller m_GrabbedBy = OVRInput.Controller.None;
    private DistanceGrabbable m_DistanceGrabbable;
    private Light m_Light;
    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.MainModule m_ParticlesMain;
    private AudioSource m_AudioSource;
    private bool vibrationStarted = false;
    private int minFreq, maxFreq;
    public bool isRecording = false;
    private bool isDeactivated = false;
    private Color m_Color;

    private Game m_Game;
    #endregion

    private void Awake() {
        m_DistanceGrabbable = this.GetComponent<DistanceGrabbable>();
        m_Light = m_PointLight.GetComponent<Light>();
        m_ParticleSystem = this.GetComponent<ParticleSystem>();
        m_ParticlesMain = m_ParticleSystem.main;
        m_AudioSource = this.GetComponent<AudioSource>();

        m_Game = GameObject.Find("OVRPlayerController").GetComponent<Game>();

        StartCoroutine(SetAppearance());
    }

    private void Update() {

        if (isDeactivated)  return;

        // check if currently grabbed - if not grabbed, return and set color to default white
        CheckGrabbed();

        if (m_GrabbedBy == OVRInput.Controller.None) {
            m_Color = Color.white;
            return;
        }

        // Set color to something else to indicate being grabbed - if recording, blue, otherwise, yellow
        m_Color = (isRecording) ? Color.blue : Color.white;

        /*
        // If vibrations haven't started, start them
        if (!vibrationStarted) {
            StartCoroutine(SetVibration());
        }

        // Check mic status - if no mic attached, end here
        if (!m_Game.GetMicStatus()) {
            return;
        }
        // If mice is attached, update frequencies
        minFreq = m_Game.GetMinFrequency();
        maxFreq = m_Game.GetMinFrequency();

        // If a downpress on the trigger was detected by the controller holding the star, and we're not recording, initialize recording
        if (OVRInput.Get(m_StartAudioButton, m_GrabbedBy) > 0.1f && !isRecording) {
            // Check if we can even start recording - check with Game if the other hand is recording
            KeyValuePair<OVRInput.Controller, bool> checkIfRecording = m_Game.CanRecord();
            if (checkIfRecording.Key == OVRInput.Controller.None) {
                // we've got the go-ahead, start recording
                StartRecording();
            }
        }
        // If a letting go of the trigger was detected by the controller holding the star, and we're currently recording, stop recording
        if (OVRInput.Get(m_StartAudioButton, m_GrabbedBy) <= 0.1f && isRecording) {
            EndRecording();
        }

        // If the player presses "A" or "X" on their respective controller, and there's an audio clip that exists inside the orb, we toggle it
        if (OVRInput.GetDown(m_PlayAudioButton, m_GrabbedBy) && m_AudioSource.clip != null) {
            ToggleAudio();
        }
        */
    }

    // Alter the appearance of the orb - checks for presence of audio clip and color of star
    private IEnumerator SetAppearance() {
        // While loop
        while(true) {
            // Change appearance based on if there is an audio clip
            if (m_AudioSource.clip == null) {
                m_EmptyIndicator.transform.localScale = Vector3.Lerp(m_EmptyIndicator.transform.localScale, Vector3.one, Time.deltaTime*3);
                m_ParticlesMain.startSize = new ParticleSystem.MinMaxCurve(0.1f,1f);
            }
            else {
                m_EmptyIndicator.transform.localScale = Vector3.Lerp(m_EmptyIndicator.transform.localScale, Vector3.zero, Time.deltaTime*3);
                m_ParticlesMain.startSize = new ParticleSystem.MinMaxCurve(1f,2f);
            }

            // Set the color of the star
            SetColor(m_Color);
            
            // Yield return null to restart loop
            yield return null;
        }
    }

    private void CheckGrabbed() {
        if (m_DistanceGrabbable.grabbedBy) {
            m_GrabbedBy = m_DistanceGrabbable.grabbedBy.GetController();
        }
        else {
            m_GrabbedBy = OVRInput.Controller.None;
        }
    }

    private IEnumerator SetVibration() {
        vibrationStarted = true;
        VibrationManager.singleton.TriggerVibration(5000,2,255,m_GrabbedBy);
        yield return new WaitForSeconds(5);
        vibrationStarted = false;
    }

    public void StartRecording() {
        isRecording = true;
        minFreq = m_Game.GetMinFrequency();
        maxFreq = m_Game.GetMinFrequency();
        m_Game.SetControllerStatus(m_GrabbedBy, true);
        m_AudioSource.clip = Microphone.Start(null, true, 20, maxFreq);
    }
    public void EndRecording() {
        isRecording = false;
        m_Game.SetControllerStatus(m_GrabbedBy, false);
        Microphone.End(null); //Stop the audio recording
		m_AudioSource.Play(); //Playback the recorded audio
    }
    public bool CheckRecordingStatus() {
        return isRecording;
    }
    public void SetColor(Color c) {
        m_Light.color = c;
        m_ParticlesMain.startColor = c;
    }
    public void SetAudioClip(AudioClip clip) {
        m_AudioSource.clip = clip;
    }
    public void ToggleAudio() {
        if (m_AudioSource.isPlaying) {  m_AudioSource.Stop();   } 
        else {  m_AudioSource.Play();   }
    }
    public AudioClip GetAudioClip() {
        return m_AudioSource.clip;
    }
    public void Deactivate(){ 
        m_Light.enabled = false;
        m_Crosshairs.GetComponent<GrabbableCrosshair>().enabled = false;
        m_EmptyIndicator.GetComponent<MeshRenderer>().enabled = false;
        isDeactivated = true;
    }
    public bool GetIsDeactivated() {
        return isDeactivated;
    }
}
