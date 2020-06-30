using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Show off all the Debug UI components.
public class DebugUISample2 : MonoBehaviour
{

    GameObject testingSuite;
    bool inMenu;
    private Text sliderText;
    public bool first = true;
    public bool practice = true;
    public bool practice2 = true;
    public bool test = false;
    public bool test2 = false;
    public bool cheat = false;
    public bool cheatNext = false;
    public bool secondSession = false;
    bool menuToggle = false;
    public float timeBetweenSessions = 60f*5; //should be 60f*7 (7 minutes for practice session). Can change to a smaller ammount for debugging.
    float timeLeft = 60f*7; 
    float timeAfterSession1 = 2f; //don't change this
    float timeLeft2 = 100000f; //don't change this

    void Start()
    {
        timeLeft = timeBetweenSessions;
        testingSuite = GameObject.Find("TestingSuite");

        DebugUIBuilder.instance.AddLabel("Welcome to VRKeyboard");
        DebugUIBuilder.instance.AddLabel("You have entered a practice session that will last 5 minutes. Press the start btn on the left controller to continue");
        DebugUIBuilder.instance.Show();
        inMenu = true;
    }

    public void TogglePressed(Toggle t)
    {
        Debug.Log("Toggle pressed. Is on? " + t.isOn);
    }
    public void RadioPressed(string radioLabel, string group, Toggle t)
    {
        Debug.Log("Radio value changed: " + radioLabel + ", from group " + group + ". New value: " + t.isOn);
    }

    public void SliderPressed(float f)
    {
        Debug.Log("Slider: " + f);
        sliderText.text = f.ToString();
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        timeLeft2 -= Time.deltaTime;
        cheat = testingSuite.GetComponent<TestingSuite>().cheatTime;
        if (cheat == true && cheatNext == false)
        {
            timeAfterSession1 -= Time.deltaTime;
            float WPM = testingSuite.GetComponent<TestingSuite>().init_WPM;
            float CPM = testingSuite.GetComponent<TestingSuite>().init_CPM;
            float errorRate = testingSuite.GetComponent<TestingSuite>().init_errorRate;

            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.AddLabel("Test Statistics");
            DebugUIBuilder.instance.AddDivider(0);
            DebugUIBuilder.instance.AddLabel("WPM: " + WPM.ToString());
            DebugUIBuilder.instance.AddLabel("CPM: " + CPM.ToString());
            DebugUIBuilder.instance.AddLabel("#Errors: " + errorRate.ToString());

            if(test2 == true)
            {
                DebugUIBuilder.instance.AddLabel("You are done. Take off the headset.");
            }
            else
            {
                DebugUIBuilder.instance.AddLabel("Press the start btn to continue.");
            }
            
            DebugUIBuilder.instance.Show();

            if (timeAfterSession1 < 0)
            {
                cheatNext = true;
                timeAfterSession1 = 100000;
            }
            
        }


        if (timeLeft < 0)
        {
            practice = false;
        }
        if (timeLeft2 < 0)
        {
            practice2 = false;
        }
        if (OVRInput.GetDown(OVRInput.Button.Start)  && first == false && practice == true)
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }

        if (OVRInput.GetDown(OVRInput.Button.Start) && first == true && practice == true)
        {
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.AddLabel("VRKeyboard Instructions");
            DebugUIBuilder.instance.AddDivider(0);
            DebugUIBuilder.instance.AddLabel("Left joystick : change letter\nRight A : select letter  ");
            DebugUIBuilder.instance.AddLabel("Right B : backspace\nLeft Start btn : toggle instructions");

            DebugUIBuilder.instance.AddLabel("How to select a letter");
            DebugUIBuilder.instance.AddLabel("Move the left joystick to a group of characters. Press 'A' until you achieve the desired letter. Moving the joystick will lock it in place.");

            DebugUIBuilder.instance.Show();
            first = false;
        }
        if(practice == false && test == false)
        {
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.AddLabel("Practice Session Complete");
            DebugUIBuilder.instance.AddLabel("You will enter the testing session immediately after pressing the start btn.");
            DebugUIBuilder.instance.AddLabel("Testing session");
            DebugUIBuilder.instance.AddLabel("The testing session will have 10 different phrases. Type each phrase and press the start btn to move onto the next.");
            DebugUIBuilder.instance.AddLabel("The phrases will be located in the maroon box where 'practice session' was.");
            DebugUIBuilder.instance.Show();
            first = false;
        }
        if(practice == false && cheatNext == false && cheat == false && OVRInput.GetDown(OVRInput.Button.Start))
        {
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.Hide();
            test = true;
        }


      
        if (OVRInput.GetDown(OVRInput.Button.Start) && menuToggle == true)
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }

        if (OVRInput.GetDown(OVRInput.Button.Start) && secondSession == true && menuToggle == false)
        {
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.AddLabel("VRKeyboard Instructions");
            DebugUIBuilder.instance.AddDivider(0);
            DebugUIBuilder.instance.AddLabel("You now have access to a 'cheat layer' with some shortcuts.");
            DebugUIBuilder.instance.AddDivider(0);
            DebugUIBuilder.instance.AddLabel("Left index trigger : access cheat layer");
            DebugUIBuilder.instance.AddLabel("Left joystick : change letter\nRight A : select letter  ");
            DebugUIBuilder.instance.AddLabel("Right B : backspace\nLeft Start btn : toggle instructions");

            DebugUIBuilder.instance.AddLabel("How to select a letter");
            DebugUIBuilder.instance.AddLabel("Move the left joystick to a group of characters. Press 'A' until you achieve the desired letter. Moving the joystick will lock it in place.");

            DebugUIBuilder.instance.Show();
            menuToggle = true;
        }
        if (OVRInput.GetDown(OVRInput.Button.Start) && cheatNext == true && secondSession == false)
        {
            timeLeft2 = timeBetweenSessions;
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.AddLabel("Welcome to the second session");
            DebugUIBuilder.instance.AddLabel("You have entered another practice session that will last 5 minutes. Press the start btn on the left controller to continue");
            secondSession = true;
        }

        if (practice2 == false && test2 == false)
        {
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.AddLabel("Practice Session Complete");
            DebugUIBuilder.instance.AddLabel("You will enter the testing session immediately after pressing the start btn.");
            DebugUIBuilder.instance.AddDivider(0);
            DebugUIBuilder.instance.AddLabel("Testing session");
            DebugUIBuilder.instance.AddLabel("The testing session will have 10 different phrases. Type each phrase and press the start btn to move onto the next.");
            DebugUIBuilder.instance.AddLabel("The phrases will be located in the maroon box where 'practice session' was.");
            DebugUIBuilder.instance.AddDivider(0);
            DebugUIBuilder.instance.AddLabel("Use the cheat keyboard as much or as little as you would like.");
            DebugUIBuilder.instance.Show();

            cheatNext = false;
        }

        if (practice2 == false && OVRInput.GetDown(OVRInput.Button.Start))
        {
            DebugUIBuilder.instance.clearCanvas();
            DebugUIBuilder.instance.Hide();
            test2 = true;
        }
    }

    void LogButtonPressed()
    {
        Debug.Log("Button pressed");
    }
}
