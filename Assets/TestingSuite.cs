using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class TestingSuite : MonoBehaviour
{
    public bool test = false;
    public bool test2 = false;
    GameObject sampleSupport;
    public GameObject testCanvasText;
    public GameObject keyboardCanvasText;
    public GameObject numCanvasText;
    public GameObject testMiniCanvasText;

    public int count = 0;

    public float time2 = 0;

    int totalPhrases = 10;
    int totalWords = 0;
    int totalCharacters = 0;
    public int init_errors = 0;
    public float init_WPM = 0;
    public float init_CPM = 0;
    public float init_errorRate = 0;

    public int cheat_errors = 0;
    public float cheat_WPM = 0;
    public float cheat_CPM = 0;
    public float cheat_errorRate = 0;

    public bool cheatTime = false;

    //6 different phrase sets, randomly chosen
    string[] phraseset1;
    string[] phraseset2;
    string[] phraseset3;
    string[] phraseset4;
    string[] phraseset5;
    string[] phraseset6;
    string[][] phrases;
    int[][] wcpm;
    public bool clearWords = false;

    bool testWait = true;

    int phraseIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        sampleSupport = GameObject.Find("SampleSupport");
        phraseset1 = new string[]{
"my watch fell in the water","prevailing wind from the east",
"never too rich and never too thin","breathing is difficult",
"I can see the rings on Saturn","physics and chemistry are hard",
"my bank account is overdrawn","elections bring out the best",
"we are having spaghetti","time to go shopping" };

        phraseset2 = new string[]{
"a problem with the engine","elephants are afraid of mice",
"my favorite place to visit","three two one zero blast off",
"my favorite subject is psychology","circumstances are unacceptable",
"watch out for low flying objects","if at first you do not succeed",
"please provide your date of birth","we run the risk of failure"};

        phraseset3 = new string[]{
"prayer in schools offends some","he is just like everyone else",
"great disturbance in the force","love means many things",
"you must be getting old","the world is a stage",
"can I skate with sister today","neither a borrower nor a lender be",
"one heck of a question","question that must be answered" };

        phraseset4 = new string[]{
"beware the ides of March","double double toil and trouble",
"the power of denial","I agree with you",
"do not say anything","play it again Sam",
"the force is with you","you are not a jedi yet",
"an offer you cannot refuse","are you talking to me" };

        phraseset5 = new string[] {
 "yes you are very smart","all work and no play",
"hair gel is very greasy","Valium in the economy size",
"the facts get in the way","the dreamers of dreams",
"did you have a good time","space is a high priority",
"you are a wonderful example","do not squander your time"};

    phraseset6 = new string[] {
"do not drink too much","take a coffee break",
"popularity is desired by all","the music is better than it sounds",
"starlight and dewdrop","the living is easy",
"fish are jumping","the cotton is high",
"drove my chevy to the levee","but the levee was dry"};

        int[] wcpm1 = { 51, 267 };
        int[] wcpm2 = { 54, 291 };
        int[] wcpm3 = { 53, 269 };
        int[] wcpm4 = { 47, 215 };
        int[] wcpm5 = { 51, 237 };
        int[] wcpm6 = { 46, 223 };
        wcpm = new int[][] {wcpm1,wcpm2,wcpm3,wcpm4,wcpm5,wcpm6};

        phrases = new string[][] { phraseset1, phraseset2, phraseset3, phraseset4, phraseset5, phraseset6 };
        System.Random rnd = new System.Random();
        phraseIndex = rnd.Next(0, 3); //0,1,2
    }

    // Update is called once per frame
    void Update()
    {
        testMiniCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text = testCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text;

        clearWords = false;
        test = sampleSupport.GetComponent<DebugUISample2>().test;
        test2 = sampleSupport.GetComponent<DebugUISample2>().test2;

        if(test2==true && testWait == true)
        {
           test = true;
           cheatTime = false;
           count = 0;
           testWait = false;
           init_errors = 0;
           phraseIndex = phraseIndex + 3;
        }
        if (test==true && cheatTime == false)
        {
            time2 += Time.deltaTime;

            if ((count == 0 || OVRInput.GetDown(OVRInput.Button.Start)) && count< totalPhrases + 1)
            {
                clearWords = true;
                count += 1;

                if(count <= totalPhrases)
                {
                    testCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text = phrases[phraseIndex][count-1].ToString();
                    keyboardCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                    numCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text = count.ToString() + "/10";
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.Start) && count == totalPhrases + 1)
            {
                float min = time2 / 60f;
                totalWords = wcpm[phraseIndex][0];
                totalCharacters = wcpm[phraseIndex][1];

                init_WPM = totalWords / min;
                init_CPM = totalCharacters / min;
                init_errorRate = init_errors;
                cheatTime = true;
                testCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text = "try typing this phrase"; 
                numCanvasText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }

            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                init_errors += 1;
            }

        }
    }



}
