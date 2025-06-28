using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Camera Pose", menuName = "Camera/Camera Pose")]
public class CameraPose : ScriptableObject
{
    [Header("Camera Transform")]
    public Vector3 position;
    public Quaternion rotation;
    
    [Header("Metadata")]
    public string poseName = "New Pose";
    [TextArea(2, 4)]
    public string description = "";
    public Color poseColor = Color.white;
    
    [Header("Transition Settings")]
    [Range(0.1f, 5f)]
    public float transitionDuration = 1f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Preview")]
    public bool showPreview = true;
    [SerializeField] private string screenshotPath = "";
    
    // Runtime property to get the screenshot texture
    public Texture2D screenshotPreview
    {
        get
        {
            if (string.IsNullOrEmpty(screenshotPath))
                return null;
            
#if UNITY_EDITOR
            return Resources.Load<Texture2D>(screenshotPath) ?? 
                   AssetDatabase.LoadAssetAtPath<Texture2D>(screenshotPath);
#else
            return Resources.Load<Texture2D>(screenshotPath);
#endif
        }
    }
    
    public void CaptureFromCamera(Camera camera)
    {
        if (camera != null)
        {
            position = camera.transform.position;
            rotation = camera.transform.rotation;
        }
    }
    
    public void ApplyToCamera(Camera camera)
    {
        if (camera != null)
        {
            camera.transform.position = position;
            camera.transform.rotation = rotation;
        }
    }
    
    public void CaptureScreenshot(Camera camera, int width = 256, int height = 144)
    {
        if (camera == null) return;
        
        // Store original camera settings
        var originalRenderTexture = camera.targetTexture;
        var originalClearFlags = camera.clearFlags;
        var originalBackgroundColor = camera.backgroundColor;
        
        // Create render texture for screenshot
        var renderTexture = new RenderTexture(width, height, 24);
        camera.targetTexture = renderTexture;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        
        // Render the camera view
        camera.Render();
        
        // Create texture and read pixels
        var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        var previousActive = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        RenderTexture.active = previousActive;
        
        // Clean up
        camera.targetTexture = originalRenderTexture;
        camera.clearFlags = originalClearFlags;
        camera.backgroundColor = originalBackgroundColor;
        RenderTexture.DestroyImmediate(renderTexture);
        
#if UNITY_EDITOR
        // Save the texture as an asset file
        SaveScreenshotAsAsset(texture);
        
        // Clean up the temporary texture
        DestroyImmediate(texture);
#else
        // In builds, we can't save assets, so we'll just store the path
        // The screenshot should be pre-captured in the editor
        Debug.Log("Screenshot capture is only available in the Unity Editor");
#endif
    }
    
#if UNITY_EDITOR
    private void SaveScreenshotAsAsset(Texture2D texture)
    {
        // Get the path of this pose asset
        string posePath = AssetDatabase.GetAssetPath(this);
        string poseDirectory = System.IO.Path.GetDirectoryName(posePath);
        string screenshotFileName = $"{poseName}_Screenshot.png";
        string screenshotPath = System.IO.Path.Combine(poseDirectory, screenshotFileName);
        
        // Convert texture to PNG bytes
        byte[] pngData = texture.EncodeToPNG();
        
        // Write to file
        System.IO.File.WriteAllBytes(screenshotPath, pngData);
        
        // Import the asset
        AssetDatabase.ImportAsset(screenshotPath);
        
        // Store the path
        this.screenshotPath = screenshotPath;
        
        // Mark this asset as dirty
        EditorUtility.SetDirty(this);
    }
    
    public void ClearScreenshot()
    {
        if (!string.IsNullOrEmpty(screenshotPath))
        {
            // Delete the asset file
            AssetDatabase.DeleteAsset(screenshotPath);
            screenshotPath = "";
            EditorUtility.SetDirty(this);
        }
    }
#endif
} 