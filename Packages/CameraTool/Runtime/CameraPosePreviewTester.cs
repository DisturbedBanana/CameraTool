using UnityEngine;

public class CameraPosePreviewTester : MonoBehaviour
{
    [Header("References")]
    public CameraPoseController cameraController;
    
    [Header("Pose Names")]
    public string pose1Name = "Left";
    public string pose2Name = "Middle";
    public string pose3Name = "Right";
    public string pose4Name = "Top";
    
    [Header("Controls")]
    public KeyCode pose1Key = KeyCode.Alpha1;
    public KeyCode pose2Key = KeyCode.Alpha2;
    public KeyCode pose3Key = KeyCode.Alpha3;
    public KeyCode pose4Key = KeyCode.Alpha4;
    public KeyCode snapKey = KeyCode.LeftShift;
    
    private void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraPoseController>();
        }
    }
    
    private void Update()
    {
        if (cameraController == null) return;
        
        bool snap = Input.GetKey(snapKey);
        
        if (Input.GetKeyDown(pose1Key))
        {
            if (snap)
                cameraController.SnapToPose(pose1Name);
            else
                cameraController.TransitionToPose(pose1Name);
        }
        
        if (Input.GetKeyDown(pose2Key))
        {
            if (snap)
                cameraController.SnapToPose(pose2Name);
            else
                cameraController.TransitionToPose(pose2Name);
        }
        
        if (Input.GetKeyDown(pose3Key))
        {
            if (snap)
                cameraController.SnapToPose(pose3Name);
            else
                cameraController.TransitionToPose(pose3Name);
        }

        if (Input.GetKeyDown(pose4Key))
        {
            if (snap)
                cameraController.SnapToPose(pose4Name);
            else
                cameraController.TransitionToPose(pose4Name);
        }
    }
    
    private void OnGUI()
    {
        if (cameraController == null) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("Camera Pose Preview Controls", GUI.skin.box);
        GUILayout.Label($"Press {pose1Key} to go to {pose1Name}");
        GUILayout.Label($"Press {pose2Key} to go to {pose2Name}");
        GUILayout.Label($"Press {pose3Key} to go to {pose3Name}");
        GUILayout.Label($"Hold {snapKey} + pose key for instant snap");
        GUILayout.Label($"Current Pose: {cameraController.CurrentPose?.poseName ?? "None"}");
        GUILayout.Label($"Transitioning: {cameraController.IsTransitioning}");
        GUILayout.EndArea();
    }
} 