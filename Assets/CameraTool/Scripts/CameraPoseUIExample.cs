using UnityEngine;
using UnityEngine.UI;

public class CameraPoseUIExample : MonoBehaviour
{
    [Header("References")]
    public CameraPoseController cameraController;
    
    [Header("UI Buttons")]
    public Button pose1Button;
    public Button pose2Button;
    public Button pose3Button;
    public Button snapPose1Button;
    public Button snapPose2Button;
    public Button snapPose3Button;
    
    [Header("Pose Names")]
    public string pose1Name = "Pose_1";
    public string pose2Name = "Pose_2";
    public string pose3Name = "Pose_3";
    
    [Header("UI Settings")]
    public bool showTransitionStatus = true;
    public Text statusText;
    
    private void Start()
    {
        // Auto-find camera controller if not assigned
        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraPoseController>();
        }
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Update status text
        UpdateStatusText();
    }
    
    private void SetupButtonListeners()
    {
        // Smooth transition buttons
        if (pose1Button != null)
            pose1Button.onClick.AddListener(() => TransitionToPose(pose1Name));
        
        if (pose2Button != null)
            pose2Button.onClick.AddListener(() => TransitionToPose(pose2Name));
        
        if (pose3Button != null)
            pose3Button.onClick.AddListener(() => TransitionToPose(pose3Name));
        
        // Instant snap buttons
        if (snapPose1Button != null)
            snapPose1Button.onClick.AddListener(() => SnapToPose(pose1Name));
        
        if (snapPose2Button != null)
            snapPose2Button.onClick.AddListener(() => SnapToPose(pose2Name));
        
        if (snapPose3Button != null)
            snapPose3Button.onClick.AddListener(() => SnapToPose(pose3Name));
    }
    
    private void TransitionToPose(string poseName)
    {
        if (cameraController != null)
        {
            cameraController.TransitionToPose(poseName);
            Debug.Log($"Transitioning to pose: {poseName}");
            UpdateStatusText();
        }
        else
        {
            Debug.LogError("CameraPoseController not found! Make sure it's attached to a camera.");
        }
    }
    
    private void SnapToPose(string poseName)
    {
        if (cameraController != null)
        {
            cameraController.SnapToPose(poseName);
            Debug.Log($"Snapped to pose: {poseName}");
            UpdateStatusText();
        }
        else
        {
            Debug.LogError("CameraPoseController not found! Make sure it's attached to a camera.");
        }
    }
    
    private void Update()
    {
        // Update status text every frame if needed
        if (showTransitionStatus && statusText != null)
        {
            UpdateStatusText();
        }
    }
    
    private void UpdateStatusText()
    {
        if (statusText == null || cameraController == null) return;
        
        string currentPose = cameraController.CurrentPose?.poseName ?? "None";
        string transitioning = cameraController.IsTransitioning ? " (Transitioning...)" : "";
        
        statusText.text = $"Current Pose: {currentPose}{transitioning}";
    }
    
    // Public methods for external calls (e.g., from other scripts)
    public void OnPose1ButtonClick() => TransitionToPose(pose1Name);
    public void OnPose2ButtonClick() => TransitionToPose(pose2Name);
    public void OnPose3ButtonClick() => TransitionToPose(pose3Name);
    public void OnSnapPose1ButtonClick() => SnapToPose(pose1Name);
    public void OnSnapPose2ButtonClick() => SnapToPose(pose2Name);
    public void OnSnapPose3ButtonClick() => SnapToPose(pose3Name);
} 