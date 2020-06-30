using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class Star : MonoBehaviour
{

    #region References
    private Game m_Game;
    private New_Custom_Controller m_GrabbedBy = null;
    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.MainModule m_ParticlesMain;
    private SphereCollider m_SphereCollider;
    private AudioSource m_AudioSource;
    private Rigidbody m_Rigidbody;
    public GameObject m_EmptyIndicator;
    public GameObject m_Crosshairs;
    public bool isRecording = false;
    public int numPositions;
    #endregion

    #region Private Variables
    private Color m_Color = Color.white;
    private bool isDeactivated = false;
    private int minFreq, maxFreq;
    private List<Vector3> positions = new List<Vector3>();
    private Vector3 prevPos;
    private Vector3 linearVelocity;
    private string theTime, theDate;
    #endregion

    /*
    #region Publics
    public GameObject m_PointLight;
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
    #endregion
    */

    private void Awake() {
        /*
        m_DistanceGrabbable = this.GetComponent<DistanceGrabbable>();
        m_Light = m_PointLight.GetComponent<Light>();
        */
        m_Game = GameObject.Find("LocalAvatarWithGrab").GetComponent<Game>();
        m_ParticleSystem = this.GetComponent<ParticleSystem>();
        m_ParticlesMain = m_ParticleSystem.main;
        m_AudioSource = this.GetComponent<AudioSource>();
        m_Rigidbody = this.GetComponent<Rigidbody>();
        m_SphereCollider = this.GetComponent<SphereCollider>();
        prevPos = transform.position;

        StartCoroutine(SetAppearance());
    }

    private void Update() {

        if (isDeactivated)  return;

        linearVelocity = transform.position - prevPos;
        prevPos = transform.position;
        
        // check if currently grabbed - if not grabbed, return and set color to default white
        if (m_GrabbedBy == null) {
            m_Color = Color.white;
            return;
        }

        // Move the star to the controller's ref position
        MoveStar();

        // Set color to something else to indicate being grabbed - if recording, blue, otherwise, yellow
        m_Color = (isRecording) ? Color.blue : Color.white;

        /*
        // If vibrations haven't started, start them
        if (!vibrationStarted) {
            StartCoroutine(SetVibration());
        }
        */

        // Check mic status - if no mic attached, end here
        if (!m_Game.GetMicStatus()) {
            return;
        }
        // If mice is attached, update frequencies
        minFreq = m_Game.GetMinFrequency();
        maxFreq = m_Game.GetMinFrequency();
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

    /*
    private IEnumerator SetVibration() {
        vibrationStarted = true;
        VibrationManager.singleton.TriggerVibration(5000,2,255,m_GrabbedBy);
        yield return new WaitForSeconds(5);
        vibrationStarted = false;
    }
    */

    private void AddPosition() {
        Vector3 curPos = this.transform.position;
        if (positions.Count > numPositions) positions.RemoveAt(0);
        positions.Add(curPos);
        return;
    }
    private void MoveStar() {
        float dist = Vector3.Distance(m_GrabbedBy.GetTargetRef(),this.transform.position);
        if (dist > 0.05) {
            Vector3 dir = m_GrabbedBy.GetTargetRef() - this.transform.position;
            this.transform.position = this.transform.position + dir.normalized * m_GrabbedBy.GetForceStrength() * Time.deltaTime;
        } else {
            this.transform.position = m_GrabbedBy.GetTargetRef();
            m_GrabbedBy.DeactivateHover();
        }
        m_Rigidbody.velocity = linearVelocity;
    }
    public void GrabBegin(New_Custom_Controller contr) {
        m_GrabbedBy = contr;
        m_SphereCollider.enabled = false;
    }
    public void GrabEnd(Vector3 linVel, Vector3 angVel) {
        m_GrabbedBy.ActivateHover();
        m_GrabbedBy = null;
        m_Rigidbody.velocity = linVel;
        m_Rigidbody.angularVelocity = angVel;
        m_SphereCollider.enabled = true;
    }

    public void StartRecording() {
        isRecording = true;
        minFreq = m_Game.GetMinFrequency();
        maxFreq = m_Game.GetMinFrequency();
        m_Game.SetControllerStatus(m_GrabbedBy.GetController(), true);
        m_AudioSource.clip = Microphone.Start(null, true, 20, maxFreq);
    }
    public void EndRecording() {
        isRecording = false;
        m_Game.SetControllerStatus(m_GrabbedBy.GetController(), false);
        Microphone.End(null); //Stop the audio recording
        theTime = System.DateTime.Now.ToString("hh:mm:ss"); 
        theDate = System.DateTime.Now.ToString("MM/dd/yyyy");
		m_AudioSource.Play(); //Playback the recorded audio
    }
    public bool CheckRecordingStatus() {
        return isRecording;
    }
    public void SetColor(Color c) {
        //m_Light.color = c;
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
    public List<string> GetDateOfCreation() {
        return (theDate != null && theTime != null) ? new List<string>(){ theDate, theTime } : null;
    }

    public void Deactivate(){ 
        //m_Light.enabled = false;
        m_Crosshairs.GetComponent<GrabbableCrosshair>().enabled = false;
        m_EmptyIndicator.GetComponent<MeshRenderer>().enabled = false;
        isDeactivated = true;
    }
    public bool GetIsDeactivated() {
        return isDeactivated;
    }
}
