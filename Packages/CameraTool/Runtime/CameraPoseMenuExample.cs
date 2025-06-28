using UnityEngine;
using UnityEngine.UI;

public class CameraPoseMenuExample : MonoBehaviour
{
    [Header("References")]
    public CameraPoseController cameraController;
    public CameraPoseCollection menuPoses;
    
    [Header("UI Buttons")]
    public Button mainMenuButton;
    public Button optionsButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button backButton;
    
    [Header("Menu States")]
    public string mainMenuPoseName = "MainMenu";
    public string optionsPoseName = "Options";
    public string settingsPoseName = "Settings";
    public string creditsPoseName = "Credits";
    
    private void Start()
    {
        // Set up the camera controller
        if (cameraController != null && menuPoses != null)
        {
            cameraController.SetPoseCollection(menuPoses);
        }
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Start at main menu
        TransitionToMainMenu();
    }
    
    private void SetupButtonListeners()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(TransitionToMainMenu);
            
        if (optionsButton != null)
            optionsButton.onClick.AddListener(TransitionToOptions);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(TransitionToSettings);
            
        if (creditsButton != null)
            creditsButton.onClick.AddListener(TransitionToCredits);
            
        if (backButton != null)
            backButton.onClick.AddListener(TransitionToMainMenu);
    }
    
    public void TransitionToMainMenu()
    {
        if (cameraController != null)
        {
            cameraController.TransitionToPose(mainMenuPoseName);
        }
    }
    
    public void TransitionToOptions()
    {
        if (cameraController != null)
        {
            cameraController.TransitionToPose(optionsPoseName);
        }
    }
    
    public void TransitionToSettings()
    {
        if (cameraController != null)
        {
            cameraController.TransitionToPose(settingsPoseName);
        }
    }
    
    public void TransitionToCredits()
    {
        if (cameraController != null)
        {
            cameraController.TransitionToPose(creditsPoseName);
        }
    }
    
    // Runtime pose switching (useful for testing)
    public void TransitionToPose(string poseName)
    {
        if (cameraController != null)
        {
            cameraController.TransitionToPose(poseName);
        }
    }
    
    public void SnapToPose(string poseName)
    {
        if (cameraController != null)
        {
            cameraController.SnapToPose(poseName);
        }
    }
    
    // Check if currently transitioning
    public bool IsTransitioning()
    {
        return cameraController != null && cameraController.IsTransitioning;
    }
    
    // Get current pose
    public CameraPose GetCurrentPose()
    {
        return cameraController != null ? cameraController.CurrentPose : null;
    }
} 