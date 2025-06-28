using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraPose))]
public class CameraPoseGizmoDrawer : Editor
{
    private void OnSceneGUI()
    {
        CameraPose pose = (CameraPose)target;
        
        if (!pose.showPreview) return;
        
        // Draw camera position
        Handles.color = pose.poseColor;
        Handles.SphereHandleCap(0, pose.position, pose.rotation, 0.3f, EventType.Repaint);
        
        // Draw camera forward direction
        Vector3 forward = pose.rotation * Vector3.forward;
        Handles.DrawLine(pose.position, pose.position + forward * 2f);
        
        // Draw camera up direction
        Vector3 up = pose.rotation * Vector3.up;
        Handles.color = Color.green;
        Handles.DrawLine(pose.position, pose.position + up * 1f);
        
        // Draw camera right direction
        Vector3 right = pose.rotation * Vector3.right;
        Handles.color = Color.red;
        Handles.DrawLine(pose.position, pose.position + right * 1f);
        
        // Draw camera frustum preview
        DrawCameraFrustum(pose);
        
        // Draw pose name
        Handles.color = pose.poseColor;
        Handles.Label(pose.position + Vector3.up * 1.5f, pose.poseName);
        
        // Draw transition duration info
        if (pose.transitionDuration > 0)
        {
            Handles.Label(pose.position + Vector3.up * 1.3f, $"Duration: {pose.transitionDuration:F1}s");
        }
    }
    
    private void DrawCameraFrustum(CameraPose pose)
    {
        // Create a simple camera frustum visualization
        Vector3 forward = pose.rotation * Vector3.forward;
        Vector3 up = pose.rotation * Vector3.up;
        Vector3 right = pose.rotation * Vector3.right;
        
        float nearDistance = 1f;
        float farDistance = 5f;
        float nearHeight = 0.5f;
        float nearWidth = 0.7f;
        
        Vector3 nearCenter = pose.position + forward * nearDistance;
        Vector3 farCenter = pose.position + forward * farDistance;
        
        // Near plane corners
        Vector3 nearTopLeft = nearCenter + up * nearHeight - right * nearWidth;
        Vector3 nearTopRight = nearCenter + up * nearHeight + right * nearWidth;
        Vector3 nearBottomLeft = nearCenter - up * nearHeight - right * nearWidth;
        Vector3 nearBottomRight = nearCenter - up * nearHeight + right * nearWidth;
        
        // Far plane corners
        Vector3 farTopLeft = farCenter + up * nearHeight * 2 - right * nearWidth * 2;
        Vector3 farTopRight = farCenter + up * nearHeight * 2 + right * nearWidth * 2;
        Vector3 farBottomLeft = farCenter - up * nearHeight * 2 - right * nearWidth * 2;
        Vector3 farBottomRight = farCenter - up * nearHeight * 2 + right * nearWidth * 2;
        
        // Draw near plane
        Handles.color = new Color(pose.poseColor.r, pose.poseColor.g, pose.poseColor.b, 0.3f);
        Handles.DrawLine(nearTopLeft, nearTopRight);
        Handles.DrawLine(nearTopRight, nearBottomRight);
        Handles.DrawLine(nearBottomRight, nearBottomLeft);
        Handles.DrawLine(nearBottomLeft, nearTopLeft);
        
        // Draw far plane
        Handles.DrawLine(farTopLeft, farTopRight);
        Handles.DrawLine(farTopRight, farBottomRight);
        Handles.DrawLine(farBottomRight, farBottomLeft);
        Handles.DrawLine(farBottomLeft, farTopLeft);
        
        // Draw connecting lines
        Handles.DrawLine(nearTopLeft, farTopLeft);
        Handles.DrawLine(nearTopRight, farTopRight);
        Handles.DrawLine(nearBottomLeft, farBottomLeft);
        Handles.DrawLine(nearBottomRight, farBottomRight);
    }
} 