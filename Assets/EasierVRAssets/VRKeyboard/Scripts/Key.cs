using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class Key : MonoBehaviour
{

    private KeyboardController2 m_KeyboardController = null;
    private IEnumerator m_CustomUpdate;
    private bool updateRunning = false;
    private int m_keyIndex = -1;
    public int keyIndex
    {
        get {   return m_keyIndex;  }
        set {   m_keyIndex = value; }
    }

    #region Characters
    [SerializeField]
    private List<char> m_characters = new List<char>();
    public List<char> characters 
    {
        get {   return m_characters;    }
        set {   m_characters = value;   }
    }
    private string m_characterString = "";
    public string characterString 
    {
        get { return m_characterString; }
    }
    private int characterIndex = -1;
    public string currentCharacter
    {
        get {   return (characterIndex == -1) ? null : m_characters[characterIndex].ToString();    }
    }
    #endregion
    
    #region Key State
    private int m_state = 0;
    public int state
    {
        get {   return m_state;     }
        set {   m_state = value;    }
    }
    public bool selectedStateRegistered = false;
    #endregion

    #region Colliders
    private Collider touchCollider;
    #endregion

    #region Appearance
    [SerializeField]
    private Color m_neutralColor = new Color(0.23f,0.23f,0.23f);
    [SerializeField]
    private Color m_highlightColor = new Color(0.4f, 0.4f, 0.4f);
    [SerializeField]
    private Color m_selectedColor = new Color(0.64f, 0.64f, 0.64f);
    public Color neutralColor
    {
        set {   m_neutralColor = value;     }
    }
    public Color highlightColor
    {
        set {   m_highlightColor = value;   }
    }
    public Color selectedColor
    {
        set {   m_selectedColor = value;    }
    }
    [SerializeField]
    private Renderer m_renderer;
    [SerializeField]
    private TextMeshProUGUI m_text;
    #endregion
    
    private void Start() {
        touchCollider = this.GetComponent<Collider>();
        m_CustomUpdate = CustomUpdate();
    }
    private IEnumerator CustomUpdate() {
        while(true) {
            updateRunning = true;
            m_characterString = new string(characters.ToArray());
            m_text.text = m_characterString;
            touchCollider.enabled = m_KeyboardController.allowCollision;
            switch(m_state) {
                case 0: // not selected or highlighted
                    characterIndex = -1;
                    selectedStateRegistered = false;
                    if (m_renderer.material.color != m_neutralColor) {  m_renderer.material.SetColor("_Color", m_neutralColor); }
                    break;
                case 1: // highlighted
                    // character index not changed - the person may want to click this while still highlighting it
                    selectedStateRegistered = false;
                    m_KeyboardController.currentKeyIndex = m_keyIndex;
                    if (m_renderer.material.color != m_highlightColor) {    m_renderer.material.SetColor("_Color", m_highlightColor);   }
                    break;
                case 2: // selected
                    m_KeyboardController.currentKeyIndex = m_keyIndex;
                    if (!selectedStateRegistered) {
                        characterIndex = (characterIndex == characters.Count-1) ? 0 : characterIndex + 1;
                        selectedStateRegistered = true;
                    }
                    if (m_renderer.material.color != m_selectedColor)  {   m_renderer.material.SetColor("_Color", m_selectedColor);    }
                    break;
                default:    // m_state = 0
                    characterIndex = -1;
                    selectedStateRegistered = false;
                    if (m_renderer.material.color != m_neutralColor) {  m_renderer.material.SetColor("_Color", m_neutralColor); }
                    break;
            }
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider collider) {
        m_state = 2;
    }
    private void OnTriggerExit(Collider collider) {
        m_state = 0;
    }

    public void Initialize(KeyboardController2 c, int i) {
        m_KeyboardController = c;
        m_keyIndex = i;
        StartCoroutine(m_CustomUpdate);
    }
    public void ResetKey(int i) {
        keyIndex = i;
        characterIndex = -1;
        selectedStateRegistered = false;
        m_state = 0;
        if (!updateRunning) StartCoroutine(m_CustomUpdate);
    }
    public void ResetKey() {
        characterIndex = -1;
        selectedStateRegistered = false;
        m_state = 0;
        if (!updateRunning) StartCoroutine(m_CustomUpdate);
    }
}
