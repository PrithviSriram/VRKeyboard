using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflectionManager : MonoBehaviour
{
    
    Camera m_ReflectionCamera;
    Camera m_mainCamera;

    public GameObject m_ReflectionPlane;

    public Material m_FloorMaterial;

    RenderTexture m_RenderTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject reflectionCameraGo = new GameObject("ReflectionCamera");
        m_ReflectionCamera = reflectionCameraGo.AddComponent<Camera>();
        m_ReflectionCamera.enabled = false;
        m_mainCamera = Camera.main;

        m_RenderTarget = new RenderTexture(Screen.width, Screen.height, 24);
    }

    void RenderReflection() {
        m_ReflectionCamera.CopyFrom(m_mainCamera);

        Vector3 cameraDirectionWorldSpace = m_mainCamera.transform.forward;
        Vector3 cameraUpWorldSpace = m_mainCamera.transform.up;
        Vector3 cameraPositionWorldSpace = m_mainCamera.transform.position;

        // Transform the vectors to the floor's space
        Vector3 cameraDirectionPlaneSpace = m_ReflectionPlane.transform.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = m_ReflectionPlane.transform.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = m_ReflectionPlane.transform.InverseTransformPoint(cameraPositionWorldSpace);

        // Mirror the Vectors
        cameraDirectionPlaneSpace.y *= -1.0f;
        cameraUpPlaneSpace.y *= -1.0f;
        cameraPositionPlaneSpace.y *= -1.0f;

        // Transform Vectors back to world space
        cameraDirectionWorldSpace = m_ReflectionPlane.transform.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = m_ReflectionPlane.transform.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = m_ReflectionPlane.transform.TransformPoint(cameraPositionPlaneSpace);

        // Set reflection camera's position and rotation
        m_ReflectionCamera.transform.position = cameraPositionWorldSpace;
        m_ReflectionCamera.transform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);

        // Set Render Target for the Reflection Camera
        m_ReflectionCamera.targetTexture = m_RenderTarget;

        // Render the reflection camera
        m_ReflectionCamera.Render();

        // Draw full screen quad
        DrawQuad();
    }

    void DrawQuad() {
        GL.PushMatrix();

        // Use ground material to draw quad
        m_FloorMaterial.SetPass(0);
        m_FloorMaterial.SetTexture("_ReflectionTex", m_RenderTarget);
         
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.0f);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.0f);
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.0f);
        GL.End();

        GL.PopMatrix();
    }

    private void onPostRender() {
        RenderReflection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
