using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardKeyVer2 : MonoBehaviour
{
    public List<string> characters = new List<string>();
    public Image m_background;
    public TextMeshProUGUI m_text;

    [SerializeField]
    private Color m_defaultColor = new Color32(255, 255, 255, 255);
    [SerializeField]
    private Color m_highlightColor = new Color32(255, 255, 255, 230);
    [SerializeField]
    private Color m_selectedColor = new Color32(255, 255, 255, 200);

    private Collider m_Collider;
    private bool keyStatus = false;
    private int charactersIndex = -1;
    private string curCharacter = "";

    private void Awake() {
        m_Collider = this.GetComponent<Collider>();
        PrintOverlay();
        StartCoroutine(LoopThroughCharacters());
    }
    private IEnumerator LoopThroughCharacters() {
        while(true) {
            if (charactersIndex > -1) {
                curCharacter = characters[charactersIndex];
            }
            else {
                curCharacter = "";
            }
            yield return true;
        }
    }

    public void CollisionToggle(bool c) {
        if (m_Collider != null) {   m_Collider.enabled = c; }
        return;
    }
    public void ResetKey() {
        /*
        highlightObject.SetActive(false);
        selectedObject.SetActive(false);
        */
        m_background.color = m_defaultColor;
        ResetCharactersIndex();
        return;
    }
    public void HighlightKey() {
        /*
        highlightObject.SetActive(true);
        selectedObject.SetActive(false);
        */
        m_background.color = m_highlightColor;
        return;
    }
    public void SelectKey() {
        /*
        highlightObject.SetActive(false);
        selectedObject.SetActive(true);
        */
        m_background.color = m_selectedColor;
        return;
    }
    public void NextCharactersIndex() {
        if (charactersIndex == -1) charactersIndex = 0;
        else charactersIndex = (charactersIndex == characters.Count - 1) ? 0 : charactersIndex + 1;
        return;
    }
    public void ResetCharactersIndex() {
        charactersIndex = -1;
        return;
    }
    public string GetCurrentCharacter() {
        if (charactersIndex > -1) curCharacter = characters[charactersIndex];
        else curCharacter = "";
        return curCharacter;
    }
    public List<string> GetAllCharacters() {
        return characters;
    }
    public void SetCharactersIndex(int i) {
        charactersIndex = (i >= characters.Count) ? characters.Count - 1 : (i < 0) ? 0 : i;
        return;
    }
    public void PrintOverlay() {
        string ov = "";
        if (characters.Count == 1 && characters[0] == " ") {    ov = "space";   }
        else {
            foreach(string str in characters) {
                ov += str;
            }
        }
        m_text.text = ov;
        return;
    }
    public int GetKeyIndex() {
        return charactersIndex;
    }

    private void OnCollisionEnter(Collider collision) {
        return;
    }
}
