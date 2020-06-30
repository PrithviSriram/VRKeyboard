using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BezierCurves))]
public class CustomPointer: MonoBehaviour
{   
    // NOT SERIALIZED
    [Tooltip("Reference to LineRenderer")]
    private LineRenderer m_LineRenderer;
    public LineRenderer lineRenderer {
        get {   return m_LineRenderer;  }
        set {   m_LineRenderer = value; }
    }
    // NOT SERIALIZED
    [Tooltip("Reference to bezier curves")]
    private BezierCurves m_bezierCurves;
    public BezierCurves bezierCurves {
        get {   return m_bezierCurves;  }
        set {   m_bezierCurves = value; }
    }

    private enum pointerType {
        Target,
        Teleport,
        Set_Target
    }
    [SerializeField]
    [Tooltip("The type of pointer you wish to work with")]
    private pointerType m_pointerType;
    
    private enum laserType {
        Laser,
        Parabolic
    }
    [SerializeField]
    [Tooltip("The type of laser to use for locomotion")]
    private laserType m_laserType;

    // NOT SERIALIZED
    [Tooltip("storage of raycast target, if any")]
    private GameObject m_raycastTarget = null;
    public GameObject raycastTarget {
        get {   return m_raycastTarget;     }
        set {   m_raycastTarget = value;    }
    }

    [SerializeField]
    [Tooltip("The origin point from where the raycast should be performed")]
    private Transform m_raycastOrigin;
    public Transform raycastOrigin {
        get {   return m_raycastOrigin;     }
        set {   m_raycastOrigin = value;    }
    }

    [SerializeField]
    [Tooltip("The distance the raycast is permitted to travel - defaul = 100 meters")]
    private float m_raycastDistance = 100f;
    public float raycastDistance {
        get {   return m_raycastDistance;   }
        set {   m_raycastDistance = value;  }
    }

    // NOT SERIALIZED
    [Tooltip("the list of points the line renderer will render - must be converted into Vector3[] to use properly")]
    private List<Vector3> m_linePoints = new List<Vector3>();

    [SerializeField]
    [Tooltip("The color the line must be rendered with")]
    private Color m_lineColor = new Color32(255, 255, 0, 255);
    public Color lineColor {
        get {   return m_lineColor;     }
        set {   m_lineColor = value;    }
    }

    [SerializeField]
    [Tooltip("The detection of a raycast target can only be performed when the pointer is activated")]
    private bool m_detectOnlyWhenActivated = false;
    public bool detectOnlyWhenActivated {
        get {   return m_detectOnlyWhenActivated;   }
        set {   m_detectOnlyWhenActivated = value;  }
    }

    [SerializeField]
    [Tooltip("The number of points the line renderer will use when printing the line")]
    private int m_numPoints = 30;
    public int numPoints {
        get {   return m_numPoints; }
        set {   m_numPoints = value;}
    }

    // NOT SERIALIZED
    [Tooltip("The location where the player must teleport to - only important to locomotion")]
    private Vector3 m_locomotionPosition = Vector3.zero;
    public Vector3 locomotionPosition {
        get {   return m_locomotionPosition;    }
        set {   m_locomotionPosition = value;   }
    }

    //[SerializeField]
    //private bool m_showTransparentLine = false;
    // Here's the rub:
    // The idea is that when "m_detectOnlyWhenActivated" is set to TRUE...
    //  ... then the custompointer will report the target as whatever's colliding with it...
    //  ... when the trigger is pressed
    // When "m_showTransparentLine" is set to TRUE...
    //  ... then the line's material will change
    // There are some things to keep in mind:
    // 1) the line renderer will ALWAYS be enabled
    // 2) the line renderer's appearance will change depending on the "m_showTransparentLine" parameter
    // 3) the line renderer's public target (not private target) will be set to NULL depending on the "m_detectOnlyWhenActivated" parameter

    [SerializeField]
    [Tooltip("For debugging purposes")]
    private bool m_debugMode = false;
    public bool debugMode {
        get {   return m_debugMode;     }
        set {   m_debugMode = value;    }
    }

    [SerializeField]
    [Tooltip("FOR DEBUGGING")]
    private GameObject m_XYZ;

    // NOT SERIALIZED
    private bool m_isActive = false;
    public bool isActive {
        get {   return m_isActive;  }
        set {   m_isActive = value; }
    }
    
    // NOT SERIALIZED
    private int layerMask;

    private void Awake()
    {
        m_LineRenderer = this.GetComponent<LineRenderer>();
        m_bezierCurves = this.GetComponent<BezierCurves>();
        if (m_raycastOrigin == null) {  m_raycastOrigin = this.transform;   }
        
        layerMask = 1 << 11;
        layerMask = ~layerMask;
        m_LineRenderer.positionCount = m_numPoints;

        m_linePoints.Add(Vector3.zero);
        m_linePoints.Add(Vector3.zero);
    }

    private void Update() {
        if (!m_isActive) return;
        // (if in debug mode, turn on XYZ)
        m_XYZ.SetActive(m_debugMode);
        // Update start position

        m_linePoints = GetPoints();

        if (m_detectOnlyWhenActivated) {
            m_raycastTarget = (m_LineRenderer.enabled) ? m_raycastTarget : null;
        }

        if (m_LineRenderer.enabled) {   
            m_LineRenderer.material.SetColor("_Color", m_lineColor);
            m_LineRenderer.positionCount = m_numPoints;
            Vector3[] points = m_linePoints.ToArray();
            m_LineRenderer.SetPositions(points); 
        }
    }

    private List<Vector3> GetPoints() {
        List<Vector3> points = new List<Vector3>();
        Vector3 pointerDest, destination;

        switch(m_pointerType) {
            case(pointerType.Target):
                pointerDest = CheckRaycastForward();
                destination = CheckRaycastDownward(pointerDest);
                switch(m_laserType) {
                    case(laserType.Laser):
                        points = m_bezierCurves.DetermineLinearCurve(m_numPoints,m_raycastOrigin.position,pointerDest,0);
                        break;
                    case(laserType.Parabolic):
                        points = m_bezierCurves.DetermineQuadraticCurve(m_numPoints,m_raycastOrigin.position,pointerDest,destination,0);
                        break;
                }
                break;
            case(pointerType.Teleport):
                pointerDest = CheckRaycastForward();
                destination = CheckRaycastDownward(pointerDest);
                switch(m_laserType) {
                    case(laserType.Laser):
                        points = m_bezierCurves.DetermineLinearCurve(m_numPoints,m_raycastOrigin.position,pointerDest,0);
                        if (m_LineRenderer.enabled) m_locomotionPosition = pointerDest;
                        break;
                    case(laserType.Parabolic):
                        points = m_bezierCurves.DetermineQuadraticCurve(m_numPoints,m_raycastOrigin.position,pointerDest,destination,0);
                        if (m_LineRenderer.enabled) m_locomotionPosition = destination;
                        break;
                }
                break;
            case(pointerType.Set_Target):
                destination = (m_raycastTarget != null) ? m_raycastTarget.transform.position : m_raycastOrigin.position;
                points = m_bezierCurves.DetermineLinearCurve(m_numPoints,m_raycastOrigin.position,destination,0);
                break;
        }

        return points;
    }
 
    private Vector3 CheckRaycastForward() {
        RaycastHit rayHit;
        Vector3 returnPoint = m_raycastOrigin.position + m_raycastOrigin.forward * m_raycastDistance;
        if (Physics.Raycast(m_raycastOrigin.position, m_raycastOrigin.TransformDirection(Vector3.forward), out rayHit, m_raycastDistance, layerMask)) {
            // Something in front of it
            m_raycastTarget = rayHit.transform.gameObject;
            returnPoint = rayHit.point;
        } else {
            m_raycastTarget = null;
        }
        return returnPoint;
    }

    private Vector3 CheckRaycastDownward(Vector3 origin) {
        RaycastHit rayHit;
        Vector3 destination = origin;
        if (Physics.Raycast(origin, -Vector3.up, out rayHit, Mathf.Infinity, layerMask)) {
            // Floor is hit
            destination = new Vector3(origin.x, rayHit.point.y, origin.z);
        }
        return destination;
    }

    public bool isTargetType() {
        return m_pointerType == pointerType.Target;
    }
    public bool isTeleportType() {
        return m_pointerType == pointerType.Teleport;
    }
    public bool isSetTargetType() {
        return m_pointerType == pointerType.Set_Target;
    }
    public void SetPointerType(string t, GameObject targetForSet = null) {
        switch(t) {
            case("Target"):
                m_pointerType = pointerType.Target;
                break;
            case("Teleport"):
                m_pointerType = pointerType.Teleport;
                break;
            case("SetTarget"):
                m_pointerType = pointerType.Set_Target;
                m_raycastTarget = targetForSet;
                break;
        }
        return;
    }
    public void SetTarget(GameObject targetForSet = null) {
        m_raycastTarget = targetForSet;
    }

    public void Activate() {
        m_isActive = true;
    }
    public void Deactivate() {
        m_isActive = false;
    }
    public bool LineIsEnabled() {
        return m_LineRenderer.enabled;
    }
    public void LineSet(bool s) {
        m_LineRenderer.enabled = (!m_debugMode) ? s : true;
    }
    public void LineToggle() {
        m_LineRenderer.enabled = (!m_debugMode) ? !m_LineRenderer.enabled : true;
    }
    public void LineOff() {
        m_LineRenderer.enabled = (!m_debugMode) ? false : true;
    }
    public void LineOn() {
        m_LineRenderer.enabled = true;
    }

}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class CustomPointer : MonoBehaviour
{

    private LineRenderer m_LineRenderer;
    private IEnumerator m_CustomUpdate;
    private GameObject m_raycastTarget = null;
    public GameObject raycastTarget {
        get {   return m_raycastTarget;     }
        set {   m_raycastTarget = value;    }
    }
    [SerializeField]
    private Transform m_raycastOrigin;
    public Transform raycastOrigin {
        get {   return m_raycastOrigin;     }
        set {   m_raycastOrigin = value;    }
    }
    [SerializeField]
    private float m_raycastDistance = 100f;
    public float raycastDistance {
        get {   return m_raycastDistance;   }
        set {   m_raycastDistance = value;  }
    }
    private Vector3[] linePoints = new Vector3[2];
    [SerializeField]
    private Color m_lineColor = new Color32(255, 255, 0, 255);
    public Color lineColor {
        get {   return m_lineColor;     }
        set {   m_lineColor = value;    }
    }

    [SerializeField]
    private bool m_debugMode = false;
    public bool debugMode {
        get {   return m_debugMode;     }
        set {   m_debugMode = value;    }
    }

    [SerializeField]
    private bool m_isGraphicRaycaster = false;
    public bool isGraphicRaycaster {
        get {   return m_isGraphicRaycaster;    }
        set {   m_isGraphicRaycaster = value;   }
    }

    [SerializeField]
    private GameObject m_XYZ;

    private void Awake()
    {
        m_LineRenderer = this.GetComponent<LineRenderer>();
        m_CustomUpdate = CustomUpdate();
        if (m_raycastOrigin == null) {  m_raycastOrigin = this.transform;   }
        linePoints[0] = Vector3.zero;
        linePoints[1] = Vector3.zero;
    }

    private IEnumerator CustomUpdate() {
        while(true) {
            // (if in debug mode, turn on XYZ)
            m_XYZ.SetActive(m_debugMode);
            // Update start position
            linePoints[0] = m_raycastOrigin.position;
            linePoints[1] = CheckRaycast();
            if (m_LineRenderer.enabled) {   
                m_LineRenderer.material.SetColor("_Color", m_lineColor);
                m_LineRenderer.SetPositions(linePoints); 
            }
            yield return null;
        }
    }
    private Vector3 CheckRaycast() {
        RaycastHit rayHit;
        Vector3 returnPoint = m_raycastOrigin.position + m_raycastOrigin.forward * m_raycastDistance;
        if (Physics.Raycast(m_raycastOrigin.position, m_raycastOrigin.TransformDirection(Vector3.forward), out rayHit, m_raycastDistance)) {
            // Something in front of it
            m_raycastTarget = rayHit.transform.gameObject;
            returnPoint = rayHit.point;
        } else {
            m_raycastTarget = null;
        }
        return returnPoint;
    }
    
    public void Activate() {
        StartCoroutine(m_CustomUpdate);
    }
    public void Deactivate() {
        StartCoroutine(m_CustomUpdate);
    }
    public void LineSet(bool s) {
        m_LineRenderer.enabled = (!m_debugMode) ? s : true;
    }
    public void LineToggle() {
        m_LineRenderer.enabled = (!m_debugMode) ? !m_LineRenderer.enabled : true;
    }
    public void LineOff() {
        m_LineRenderer.enabled = (!m_debugMode) ? false : true;
    }
    public void LineOn() {
        m_LineRenderer.enabled = true;
    }
}
*/