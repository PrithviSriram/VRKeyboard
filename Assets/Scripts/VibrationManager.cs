using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager singleton;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton && singleton != this) {
            Destroy(this);
        }
        else {
            singleton = this;
        }
    }

    public void TriggerVibration(int iteration, int frequency, int amplitude, OVRInput.Controller toVibrate) {

        OVRHapticsClip clip = new OVRHapticsClip();

        for (int i = 0; i < iteration; i++) {
            clip.WriteSample(i%frequency == 0 ? (byte)amplitude : (byte)0);
        }

        // Vibrate left controller
        if (toVibrate == OVRInput.Controller.LTouch) {
            OVRHaptics.LeftChannel.Preempt(clip);
        }

        // Vibrate right controller
        else if (toVibrate == OVRInput.Controller.RTouch) {
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }
}
