using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardInputVer2 : MonoBehaviour
{

    #region External References
    public TextMeshProUGUI input, caret;
    public GameObject background;
    private KeyboardControllerVer2 m_KeyboardController;
    #endregion

    #region Public Variables
    [TextArea]
    public string text = "Deactivated";
    public float blinkRate = 0.3f;
    #endregion

    #region Private Variables
    private bool active = false;
    private float fontSize = 0.025f;
    private string textBackup;
    private string defaultScaffoldCharacter = "_";
    private string scaffoldCharacter = "";
    private string lastCharacter = "";
    private IEnumerator m_Blink;
    #endregion

    private void Awake()
    {
        m_Blink = Blink();
        input.text = text + scaffoldCharacter;
        textBackup = input.text;
    }
    private void Update() {
        input.text = text + scaffoldCharacter;
        input.fontSize = fontSize;
        caret.fontSize = fontSize;
        if (!active) return;
        if (scaffoldCharacter.Length == 0 || scaffoldCharacter == " ") {
            caret.text = text + "<mark=#FF00FF>" + defaultScaffoldCharacter + "</mark>";
        } else {
            caret.text = text + "<mark=#FF00FF>" + scaffoldCharacter + "</mark>";
        }
    }

    private IEnumerator Blink() {
        while(true) {
            caret.enabled = !caret.enabled;
            yield return new WaitForSeconds(blinkRate);
        }
    }

    public void Initialize(KeyboardControllerVer2 parent, float size) {
        m_KeyboardController = parent;
        fontSize = size;
        SetText("");
    }

    public void Activate() {
        if (!active) {
            ResetScaffold();
            input.gameObject.SetActive(true);
            caret.gameObject.SetActive(true);
            background.SetActive(true);
            StartCoroutine(m_Blink);
            active = true;
        }
    }
    public void Deactivate() {
        if (active) {
            ResetScaffold();
            StopCoroutine(m_Blink);
            input.gameObject.SetActive(false);
            caret.gameObject.SetActive(false);
            background.SetActive(false);
            //caret.enabled = false;
            active = false;
        }
    }
    public void UpdateScaffold(string s) {
        scaffoldCharacter = s;
    }
    public void ResetScaffold() {
        scaffoldCharacter = "";
    }
    public void AddScaffold(bool reset, bool lastActionWasHold = false) {
        if (scaffoldCharacter != "") {
            text += scaffoldCharacter;
            lastCharacter = scaffoldCharacter;
        } else if (lastCharacter != "" && lastActionWasHold) {
            text += lastCharacter;
        }
        if (reset) ResetScaffold();
    }
    public void AddSpace() {
        text += " ";
        //text += "\u00A0";
        ResetScaffold();
    }
    public string GetScaffold() {
        return scaffoldCharacter;
    }
    public void DeleteCharacter() {
        text = (text.Length > 0) ? text.Substring(0,text.Length-1) : "";
        ResetScaffold();
    }
    public void SetText(string t) {
        text = t;
        ResetScaffold();
    }
    public string GetText() {
        return text;
    }
}
