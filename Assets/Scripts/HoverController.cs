using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{

    private float crosshairTargetedScale;    // default = 2
    private float crosshairCurrentTargetScale;   // somewhere between m_OriginalScale and m_TargetedScale;
    private float crosshairOriginalScale = 0;    // default = 1;
    public float transformScale;

    // Start is called before the first frame update
    void Start()
    {
        SetScale(0);
    }

    // Update is called once per frame
    void Update()
    {
         this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.one * crosshairTargetedScale, Time.deltaTime * transformScale);
    }

    public void SetScale(float scale) {
        crosshairTargetedScale = scale;
    }
}
