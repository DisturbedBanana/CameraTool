using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEditor.EditorTools;
using System.IO;

public class CameraPoseEditorWindow : EditorWindow
{
    private CameraPoseCollection currentCollection;
    private Camera sceneCamera;
    private Vector2 scrollPosition;
    private int selectedPoseIndex = -1;
    private bool showPreview = true;
    private bool showTransitionSettings = true;
    private bool showMetadata = true;
    
    // Preview settings
    private float previewSize = 100f;
    private bool showPoseGizmos = true;
    private bool showTransitionPaths = true;
    private bool applyToGameCamera = true;
    private bool showScreenshotPreviews = true;
    private int screenshotWidth = 256;
    private int screenshotHeight = 144;
    
    // Search and filter
    private string searchFilter = "";
    private string[] categoryFilters = { "All", "Menu", "Gameplay", "Cutscene", "Debug" };
    private int selectedCategoryFilter = 0;
    
    // Enhanced UI features
    private bool showAdvancedOptions = false;
    private bool showBulkOperations = false;
    private Vector2 dragStartPosition;
    private int draggedPoseIndex = -1;
    private bool isDragging = false;
    private List<int> selectedPoseIndices = new List<int>();
    private bool multiSelectMode = false;
    
    private bool previewWithGameCam = false;
    
    [MenuItem("Window/Camera Pose Editor")]
    public static void OpenWindow()
    {
        GetWindow<CameraPoseEditorWindow>("Camera Pose Editor");
    }
    
    private void OnEnable()
    {
        // Find scene camera
        if (SceneView.lastActiveSceneView != null)
        {
            sceneCamera = SceneView.lastActiveSceneView.camera;
        }
        
        // Load last used collection
        string lastCollectionPath = EditorPrefs.GetString("CameraPoseEditor_LastCollection", "");
        if (!string.IsNullOrEmpty(lastCollectionPath))
        {
            currentCollection = AssetDatabase.LoadAssetAtPath<CameraPoseCollection>(lastCollectionPath);
            
            // Refresh missing screenshots for the loaded collection
            if (currentCollection != null)
            {
                RefreshMissingScreenshots();
            }
        }
    }
    
    private void OnGUI()
    {
        DrawToolbar();
        DrawMainContent();
        DrawPreviewPanel();
    }
    
    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        // Collection selector
        GUILayout.Label(new GUIContent("Collection:", "Select the camera pose collection to edit."), EditorStyles.toolbarButton, GUILayout.Width(70));
        CameraPoseCollection newCollection = (CameraPoseCollection)EditorGUILayout.ObjectField(
            currentCollection, typeof(CameraPoseCollection), false, GUILayout.Width(200));
        
        if (newCollection != currentCollection)
        {
            currentCollection = newCollection;
            if (currentCollection != null)
            {
                EditorPrefs.SetString("CameraPoseEditor_LastCollection", AssetDatabase.GetAssetPath(currentCollection));
            }
        }
        
        GUILayout.Space(10);
        
        // Create new collection button
        if (GUILayout.Button(new GUIContent("New Collection", "Create a new camera pose collection asset."), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            CreateNewCollection();
        }
        
        // Capture all screenshots button
        if (showScreenshotPreviews && currentCollection != null && currentCollection.poses.Count > 0)
        {
            if (GUILayout.Button(new GUIContent("Capture All Screenshots", "Capture screenshot previews for all poses in the collection."), EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                CaptureScreenshotForAllPoses();
            }
            
            if (GUILayout.Button(new GUIContent("Refresh Screenshots", "Refresh missing screenshot previews for poses in the collection."), EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                RefreshMissingScreenshots();
            }
        }
        
        // Enhanced UI buttons
        showAdvancedOptions = GUILayout.Toggle(showAdvancedOptions, new GUIContent("Advanced", "Show advanced features and quick actions."), EditorStyles.toolbarButton, GUILayout.Width(70));
        showBulkOperations = GUILayout.Toggle(showBulkOperations, new GUIContent("Bulk", "Show bulk operations for multi-pose actions."), EditorStyles.toolbarButton, GUILayout.Width(50));
        multiSelectMode = GUILayout.Toggle(multiSelectMode, new GUIContent("Multi", "Enable multi-select mode (Ctrl+Click to select multiple poses)."), EditorStyles.toolbarButton, GUILayout.Width(50));
        showPreview = GUILayout.Toggle(showPreview, new GUIContent("Preview", "Show/hide the preview panel at the bottom."), EditorStyles.toolbarButton, GUILayout.Width(60));
        showPoseGizmos = GUILayout.Toggle(showPoseGizmos, new GUIContent("Gizmos", "Show/hide pose gizmos in the Scene View."), EditorStyles.toolbarButton, GUILayout.Width(60));
        applyToGameCamera = GUILayout.Toggle(applyToGameCamera, new GUIContent("Game Cam", "Apply/capture poses using the main game camera instead of the Scene View camera."), EditorStyles.toolbarButton, GUILayout.Width(70));
        showScreenshotPreviews = GUILayout.Toggle(showScreenshotPreviews, new GUIContent("Screenshots", "Show/hide screenshot previews for each pose."), EditorStyles.toolbarButton, GUILayout.Width(80));
        
        GUILayout.FlexibleSpace();
        
        GUILayout.EndHorizontal();
    }
    
    private void DrawMainContent()
    {
        if (currentCollection == null)
        {
            DrawNoCollectionMessage();
            return;
        }
        
        GUILayout.BeginHorizontal();
        
        // Left panel - Pose list
        GUILayout.BeginVertical(GUILayout.Width(300));
        DrawPoseList();
        GUILayout.EndVertical();
        
        // Right panel - Pose details
        GUILayout.BeginVertical();
        DrawPoseDetails();
        GUILayout.EndVertical();
        
        GUILayout.EndHorizontal();
    }
    
    private void DrawNoCollectionMessage()
    {
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        
        GUILayout.Label("No Camera Pose Collection Selected", EditorStyles.boldLabel);
        GUILayout.Label("Create a new collection or load an existing one to get started.", EditorStyles.helpBox);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create New Collection", GUILayout.Width(150)))
        {
            CreateNewCollection();
        }
        
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }
    
    private void DrawPoseList()
    {
        GUILayout.Label("Camera Poses", EditorStyles.boldLabel);
        
        // Enhanced search and filter section
        GUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Search bar with clear button
        GUILayout.BeginHorizontal();
        GUILayout.Label("🔍", GUILayout.Width(20));
        searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);
        if (GUILayout.Button("✕", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            searchFilter = "";
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();
        
        // Category filter with better styling
        GUILayout.BeginHorizontal();
        GUILayout.Label("Category:", GUILayout.Width(60));
        selectedCategoryFilter = EditorGUILayout.Popup(selectedCategoryFilter, categoryFilters, EditorStyles.toolbarPopup);
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        
        // Bulk operations panel
        if (showBulkOperations && currentCollection != null && currentCollection.poses.Count > 0)
        {
            DrawBulkOperationsPanel();
        }
        
        GUILayout.Space(5);
        
        // Enhanced capture and apply buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("📷 Capture Current View", GUILayout.Height(30)))
        {
            CaptureCurrentView();
        }
        if (GUILayout.Button("🎯 Apply Selected", GUILayout.Height(30)))
        {
            ApplySelectedPose();
        }
        if (showAdvancedOptions && GUILayout.Button("⚡ Quick Preview", GUILayout.Height(30)))
        {
            QuickPreviewSelectedPose();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // Pose list with enhanced features
        DrawEnhancedPoseList();
    }
    
    private void DrawPoseDetails()
    {
        if (selectedPoseIndex < 0 || selectedPoseIndex >= currentCollection.poses.Count)
        {
            GUILayout.Label("Select a pose to edit its properties", EditorStyles.helpBox);
            return;
        }
        
        var pose = currentCollection.poses[selectedPoseIndex];
        
        GUILayout.Label($"Editing: {pose.poseName}", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        
        // Metadata section
        showMetadata = EditorGUILayout.Foldout(showMetadata, "Metadata", true);
        if (showMetadata)
        {
            EditorGUI.indentLevel++;
            pose.poseName = EditorGUILayout.TextField("Name", pose.poseName);
            pose.description = EditorGUILayout.TextArea(pose.description, GUILayout.Height(60));
            pose.poseColor = EditorGUILayout.ColorField("Color", pose.poseColor);
            EditorGUI.indentLevel--;
        }
        
        GUILayout.Space(5);
        
        // Screenshot preview section
        if (showScreenshotPreviews)
        {
            GUILayout.Label("Screenshot Preview", EditorStyles.boldLabel);
            
            // Screenshot display
            if (pose.screenshotPreview != null)
            {
                GUILayout.Label(pose.screenshotPreview, GUILayout.Width(200), GUILayout.Height(112));
            }
            else
            {
                GUILayout.Label("No screenshot available", EditorStyles.helpBox, GUILayout.Width(200), GUILayout.Height(112));
            }
            
            // Screenshot controls
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Capture Screenshot"))
            {
                CaptureScreenshotForPose(pose);
            }
            if (GUILayout.Button("Refresh"))
            {
                RefreshScreenshotForPose(pose);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
        }
        
        // Transform section
        GUILayout.Label("Transform", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        
        pose.position = EditorGUILayout.Vector3Field("Position", pose.position);
        
        Vector3 eulerAngles = pose.rotation.eulerAngles;
        eulerAngles = EditorGUILayout.Vector3Field("Rotation (Euler)", eulerAngles);
        pose.rotation = Quaternion.Euler(eulerAngles);
        
        EditorGUI.indentLevel--;
        
        GUILayout.Space(5);
        
        // Transition settings section
        showTransitionSettings = EditorGUILayout.Foldout(showTransitionSettings, "Transition Settings", true);
        if (showTransitionSettings)
        {
            EditorGUI.indentLevel++;
            pose.transitionDuration = EditorGUILayout.Slider("Duration", pose.transitionDuration, 0.1f, 5f);
            pose.transitionCurve = EditorGUILayout.CurveField("Curve", pose.transitionCurve);
            EditorGUI.indentLevel--;
        }
        
        GUILayout.Space(10);
        
        // Action buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update from Scene View"))
        {
            UpdatePoseFromSceneView(pose);
        }
        if (GUILayout.Button("Focus Scene View"))
        {
            FocusSceneViewOnPose(pose);
        }
        GUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(pose);
            EditorUtility.SetDirty(currentCollection);
        }
    }
    
    private void DrawPreviewPanel()
    {
        if (!showPreview || currentCollection == null) return;
        
        GUILayout.Space(10);
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        
        // Preview settings
        GUILayout.BeginHorizontal();
        previewSize = EditorGUILayout.Slider("Preview Size", previewSize, 50f, 200f);
        showTransitionPaths = GUILayout.Toggle(showTransitionPaths, "Show Paths");
        GUILayout.EndHorizontal();
        
        // Preview area
        Rect previewRect = GUILayoutUtility.GetRect(position.width, previewSize);
        DrawPreviewArea(previewRect);
    }
    
    private void DrawPreviewArea(Rect rect)
    {
        if (currentCollection == null || currentCollection.poses.Count == 0)
        {
            EditorGUI.LabelField(rect, "No poses to preview", EditorStyles.centeredGreyMiniLabel);
            return;
        }
        
        var filteredPoses = GetFilteredPoses();
        if (filteredPoses.Count == 0)
        {
            EditorGUI.LabelField(rect, "No poses match current filter", EditorStyles.centeredGreyMiniLabel);
            return;
        }
        
        int columns = Mathf.Max(1, Mathf.FloorToInt(rect.width / (previewSize + 10)));
        int rows = Mathf.CeilToInt((float)filteredPoses.Count / columns);
        
        float totalHeight = rows * (previewSize + 20);
        scrollPosition = GUI.BeginScrollView(rect, scrollPosition, new Rect(0, 0, rect.width - 20, totalHeight));
        
        for (int i = 0; i < filteredPoses.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float x = col * (previewSize + 10);
            float y = row * (previewSize + 20);
            
            var pose = filteredPoses[i];
            var poseRect = new Rect(x, y, previewSize, previewSize);
            
            // Draw pose name
            var nameRect = new Rect(x, y + previewSize + 2, previewSize, 16);
            EditorGUI.LabelField(nameRect, pose.poseName, EditorStyles.centeredGreyMiniLabel);
            
            // Draw screenshot or placeholder
            if (pose.screenshotPreview != null)
            {
                GUI.DrawTexture(poseRect, pose.screenshotPreview, ScaleMode.ScaleToFit);
            }
            else
            {
                // Draw placeholder and refresh button
                EditorGUI.DrawRect(poseRect, new Color(0.2f, 0.2f, 0.2f));
                EditorGUI.LabelField(poseRect, "No Screenshot", EditorStyles.centeredGreyMiniLabel);
                
                // Add a small refresh button
                var refreshRect = new Rect(x + previewSize - 20, y + 5, 15, 15);
                if (GUI.Button(refreshRect, "↻", EditorStyles.miniButton))
                {
                    RefreshScreenshotForPose(pose);
                }
            }
            
            // Handle click to select pose
            if (Event.current.type == EventType.MouseDown && poseRect.Contains(Event.current.mousePosition))
            {
                int originalIndex = currentCollection.poses.IndexOf(pose);
                if (originalIndex >= 0)
                {
                    selectedPoseIndex = originalIndex;
                    GUI.FocusControl(null);
                    Event.current.Use();
                }
            }
        }
        
        GUI.EndScrollView();
    }
    
    private List<CameraPose> GetFilteredPoses()
    {
        if (currentCollection == null) return new List<CameraPose>();
        
        var poses = currentCollection.poses;
        
        // Apply search filter
        if (!string.IsNullOrEmpty(searchFilter))
        {
            poses = poses.Where(p => p.poseName.ToLower().Contains(searchFilter.ToLower()) ||
                                   p.description.ToLower().Contains(searchFilter.ToLower())).ToList();
        }
        
        // Apply category filter (simplified - you could add category field to CameraPose)
        if (selectedCategoryFilter > 0)
        {
            // For now, just return all poses
            // You could implement category filtering based on pose name patterns
        }
        
        return poses;
    }
    
    private void CreateNewCollection()
    {
        string path = EditorUtility.SaveFilePanelInProject("Create Camera Pose Collection", 
            "NewCameraPoseCollection", "asset", "Create Camera Pose Collection");
        
        if (!string.IsNullOrEmpty(path))
        {
            var collection = CreateInstance<CameraPoseCollection>();
            AssetDatabase.CreateAsset(collection, path);
            AssetDatabase.SaveAssets();
            
            currentCollection = collection;
            EditorPrefs.SetString("CameraPoseEditor_LastCollection", path);
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = collection;
        }
    }
    
    private void CaptureCurrentView()
    {
        if (currentCollection == null) return;
        
        Camera sourceCamera = null;
        
        if (applyToGameCamera)
        {
            // Capture from game camera
            sourceCamera = Camera.main;
            if (sourceCamera == null)
            {
                sourceCamera = FindFirstObjectByType<Camera>();
            }
        }
        else
        {
            // Capture from Scene View camera
            if (SceneView.lastActiveSceneView != null)
            {
                sourceCamera = SceneView.lastActiveSceneView.camera;
            }
        }
        
        if (sourceCamera == null)
        {
            Debug.LogWarning("Could not find camera to capture pose from");
            return;
        }
        
        var newPose = CreateInstance<CameraPose>();
        newPose.CaptureFromCamera(sourceCamera);
        newPose.poseName = $"Pose_{currentCollection.poses.Count + 1}";
        
        currentCollection.AddPose(newPose);
        
        // Save the pose asset
        string collectionPath = AssetDatabase.GetAssetPath(currentCollection);
        string collectionDir = System.IO.Path.GetDirectoryName(collectionPath);
        string posePath = $"{collectionDir}/{newPose.poseName}.asset";
        AssetDatabase.CreateAsset(newPose, posePath);
        
        EditorUtility.SetDirty(currentCollection);
        AssetDatabase.SaveAssets();
        
        selectedPoseIndex = currentCollection.poses.Count - 1;
        Debug.Log($"Captured new pose '{newPose.poseName}' from {(applyToGameCamera ? "game camera" : "Scene View camera")}: {sourceCamera.name}");
        
        // Automatically capture screenshot for new poses
        if (showScreenshotPreviews)
        {
            CaptureScreenshotForPose(newPose);
        }
    }
    
    private void UpdatePoseFromSceneView(CameraPose pose)
    {
        if (applyToGameCamera)
        {
            // Update from game camera
            Camera gameCamera = Camera.main;
            if (gameCamera == null)
            {
                gameCamera = FindFirstObjectByType<Camera>();
            }
            
            if (gameCamera != null)
            {
                pose.CaptureFromCamera(gameCamera);
                EditorUtility.SetDirty(pose);
                Debug.Log($"Updated pose '{pose.poseName}' from game camera: {gameCamera.name}");
            }
            else
            {
                Debug.LogWarning("Could not find game camera to update pose from");
            }
        }
        else
        {
            // Update from Scene View camera
            Camera sceneCam = null;
            if (SceneView.lastActiveSceneView != null)
            {
                sceneCam = SceneView.lastActiveSceneView.camera;
            }
            
            if (sceneCam != null)
            {
                pose.CaptureFromCamera(sceneCam);
        EditorUtility.SetDirty(pose);
                Debug.Log($"Updated pose '{pose.poseName}' from Scene View camera");
            }
            else
            {
                Debug.LogWarning("Could not find Scene View camera to update pose from");
            }
        }
    }
    
    private void ApplySelectedPose()
    {
        if (selectedPoseIndex < 0 || selectedPoseIndex >= currentCollection.poses.Count) return;
        
        var pose = currentCollection.poses[selectedPoseIndex];
        
        if (applyToGameCamera)
        {
            // Apply to game camera
            Camera gameCamera = Camera.main;
            if (gameCamera == null)
            {
                // Try to find any camera in the scene
                gameCamera = FindFirstObjectByType<Camera>();
            }
            
            if (gameCamera != null)
            {
                pose.ApplyToCamera(gameCamera);
                Debug.Log($"Applied pose '{pose.poseName}' to game camera: {gameCamera.name}");
            }
            else
            {
                Debug.LogWarning("Could not find game camera to apply pose to");
            }
        }
        else
        {
            // Apply to Scene View camera
            Camera sceneCam = null;
            if (SceneView.lastActiveSceneView != null)
            {
                sceneCam = SceneView.lastActiveSceneView.camera;
            }
            
            if (sceneCam != null)
            {
                pose.ApplyToCamera(sceneCam);
        SceneView.RepaintAll();
                Debug.Log($"Applied pose '{pose.poseName}' to Scene View camera");
            }
            else
            {
                Debug.LogWarning("Could not find Scene View camera to apply pose to");
            }
        }
    }
    
    private void FocusSceneViewOnPose(CameraPose pose)
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.pivot = pose.position;
            SceneView.lastActiveSceneView.rotation = pose.rotation;
            SceneView.lastActiveSceneView.Repaint();
        }
    }
    
    private void OnSceneGUI()
    {
        if (!showPoseGizmos || currentCollection == null) return;
        // Draw pose gizmos as before
        foreach (var pose in currentCollection.poses)
        {
            if (!pose.showPreview) continue;
            Handles.color = pose.poseColor;
            Handles.SphereHandleCap(0, pose.position, pose.rotation, 0.5f, EventType.Repaint);
            Vector3 forward = pose.rotation * Vector3.forward;
            Handles.DrawLine(pose.position, pose.position + forward * 2f);
            Handles.Label(pose.position + Vector3.up * 0.7f, pose.poseName);
        }
    }
    
    private void RefreshScreenshotForPose(CameraPose pose)
    {
        CaptureScreenshotForPose(pose);
    }
    
    private void CaptureScreenshotForAllPoses()
    {
        if (currentCollection == null) return;
        
        foreach (var pose in currentCollection.poses)
        {
            CaptureScreenshotForPose(pose);
        }
        
        Debug.Log($"Captured screenshots for all {currentCollection.poses.Count} poses");
    }
    
    private void DrawEnhancedPoseList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        var filteredPoses = GetFilteredPoses();
        for (int i = 0; i < filteredPoses.Count; i++)
        {
            var pose = filteredPoses[i];
            int originalIndex = currentCollection.poses.IndexOf(pose);
            bool isSelected = selectedPoseIndex == originalIndex;
            bool isMultiSelected = selectedPoseIndices.Contains(originalIndex);

            // Save background color
            Color prevBg = GUI.backgroundColor;
            if (isSelected)
                GUI.backgroundColor = Color.cyan;
            else if (isMultiSelected)
                GUI.backgroundColor = new Color(0.8f, 0.8f, 1f);
            else
                GUI.backgroundColor = Color.white;

            // Start horizontal row
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Drag handle
            GUILayout.Space(2);
            GUILayout.Button("⋮⋮", GUILayout.Width(20));

            // Screenshot preview
            if (showScreenshotPreviews && pose.screenshotPreview != null)
                GUILayout.Label(pose.screenshotPreview, GUILayout.Width(80), GUILayout.Height(45));
            else if (showScreenshotPreviews)
                GUILayout.Label("No Preview", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(80), GUILayout.Height(45));

            // Pose color indicator
            GUI.color = pose.poseColor;
            GUILayout.Label("■", GUILayout.Width(20));
            GUI.color = Color.white;

            // Pose name and info
            GUILayout.BeginVertical();
            GUILayout.Label(pose.poseName, EditorStyles.label, GUILayout.ExpandWidth(true));
            GUILayout.Label($"Pos: {pose.position.ToString("F1")}", EditorStyles.miniLabel);
            GUILayout.EndVertical();

            // Action buttons
            GUILayout.BeginVertical();
            // Preview toggle
            if (GUILayout.Button("👁", GUILayout.Width(25)))
                pose.showPreview = !pose.showPreview;
            // Quick actions
            if (showAdvancedOptions)
            {
                if (GUILayout.Button("📷", GUILayout.Width(25)))
                    CaptureScreenshotForPose(pose);
                if (GUILayout.Button("🎯", GUILayout.Width(25)))
                {
                    selectedPoseIndex = originalIndex;
                    ApplySelectedPose();
                }
            }
            // Delete button
            if (GUILayout.Button("🗑", GUILayout.Width(25)))
            {
                if (EditorUtility.DisplayDialog("Delete Pose", $"Are you sure you want to delete '{pose.poseName}'?", "Delete", "Cancel"))
                {
                    currentCollection.RemovePose(pose);
                    if (selectedPoseIndex == originalIndex)
                        selectedPoseIndex = -1;
                    else if (selectedPoseIndex > originalIndex)
                        selectedPoseIndex--;
                    selectedPoseIndices.Remove(originalIndex);
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // Now get the rect for the just-drawn row
            Rect rowRect = GUILayoutUtility.GetLastRect();
            GUI.backgroundColor = prevBg;

            // Overlay a transparent button for row selection (except on action buttons)
            if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
            {
                // Check if mouse is over action buttons (right side of row)
                float actionWidth = 25f * (showAdvancedOptions ? 4 : 2); // 4 buttons if advanced, 2 otherwise
                if (Event.current.mousePosition.x < rowRect.width - actionWidth)
                {
                    if (!multiSelectMode || !Event.current.control)
                    {
                        selectedPoseIndex = originalIndex;
                        selectedPoseIndices.Clear();
                        selectedPoseIndices.Add(originalIndex);
                    }
                    else
                    {
                        if (selectedPoseIndices.Contains(originalIndex))
                            selectedPoseIndices.Remove(originalIndex);
                        else
                            selectedPoseIndices.Add(originalIndex);
                    }
                    Event.current.Use();
                }
            }
        }
        GUILayout.EndScrollView();
    }
    
    private void DrawBulkOperationsPanel()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Bulk Operations", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Select All"))
        {
            selectedPoseIndices.Clear();
            for (int i = 0; i < currentCollection.poses.Count; i++)
            {
                selectedPoseIndices.Add(i);
            }
        }
        
        if (GUILayout.Button("Clear Selection"))
        {
            selectedPoseIndices.Clear();
        }
        
        if (GUILayout.Button("Invert Selection"))
        {
            var newSelection = new List<int>();
            for (int i = 0; i < currentCollection.poses.Count; i++)
            {
                if (!selectedPoseIndices.Contains(i))
                    newSelection.Add(i);
            }
            selectedPoseIndices = newSelection;
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Capture Screenshots") && selectedPoseIndices.Count > 0)
        {
            foreach (int index in selectedPoseIndices)
            {
                if (index < currentCollection.poses.Count)
                {
                    CaptureScreenshotForPose(currentCollection.poses[index]);
                }
            }
        }
        
        if (GUILayout.Button("Delete Selected") && selectedPoseIndices.Count > 0)
        {
            if (EditorUtility.DisplayDialog("Delete Multiple Poses", 
                $"Are you sure you want to delete {selectedPoseIndices.Count} poses?", "Delete", "Cancel"))
            {
                // Sort indices in descending order to avoid index shifting
                selectedPoseIndices.Sort();
                for (int i = selectedPoseIndices.Count - 1; i >= 0; i--)
                {
                    int index = selectedPoseIndices[i];
                    if (index < currentCollection.poses.Count)
                    {
                        currentCollection.RemovePose(currentCollection.poses[index]);
                    }
                }
                selectedPoseIndices.Clear();
                selectedPoseIndex = -1;
            }
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
    }
    
    private void HandleDragAndDrop(Rect rect, int poseIndex)
    {
        var evt = Event.current;
        
        switch (evt.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(evt.mousePosition))
                {
                    dragStartPosition = evt.mousePosition;
                    draggedPoseIndex = poseIndex;
                    isDragging = false;
                    evt.Use();
                }
                break;
                
            case EventType.MouseDrag:
                if (draggedPoseIndex == poseIndex && !isDragging)
                {
                    if (Vector2.Distance(evt.mousePosition, dragStartPosition) > 10f)
                    {
                        isDragging = true;
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("PoseIndex", poseIndex);
                        DragAndDrop.StartDrag($"Moving {currentCollection.poses[poseIndex].poseName}");
                    }
                }
                break;
                
            case EventType.DragUpdated:
                if (rect.Contains(evt.mousePosition) && DragAndDrop.GetGenericData("PoseIndex") != null)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.Use();
                }
                break;
                
            case EventType.DragPerform:
                if (rect.Contains(evt.mousePosition) && DragAndDrop.GetGenericData("PoseIndex") != null)
                {
                    int sourceIndex = (int)DragAndDrop.GetGenericData("PoseIndex");
                    if (sourceIndex != poseIndex)
                    {
                        MovePose(sourceIndex, poseIndex);
                    }
                    DragAndDrop.AcceptDrag();
                    evt.Use();
                }
                break;
        }
    }
    
    private void MovePose(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= currentCollection.poses.Count ||
            toIndex < 0 || toIndex >= currentCollection.poses.Count)
            return;
        
        var pose = currentCollection.poses[fromIndex];
        currentCollection.poses.RemoveAt(fromIndex);
        currentCollection.poses.Insert(toIndex, pose);
        
        // Update selection indices
        if (selectedPoseIndex == fromIndex)
            selectedPoseIndex = toIndex;
        else if (selectedPoseIndex > fromIndex && selectedPoseIndex <= toIndex)
            selectedPoseIndex--;
        else if (selectedPoseIndex < fromIndex && selectedPoseIndex >= toIndex)
            selectedPoseIndex++;
        
        EditorUtility.SetDirty(currentCollection);
        Debug.Log($"Moved pose '{pose.poseName}' from index {fromIndex} to {toIndex}");
    }
    
    private void QuickPreviewSelectedPose()
    {
        if (selectedPoseIndex >= 0 && selectedPoseIndex < currentCollection.poses.Count)
        {
            var pose = currentCollection.poses[selectedPoseIndex];
            
            // Apply pose temporarily
            Camera sourceCamera = null;
            if (applyToGameCamera)
            {
                sourceCamera = Camera.main ?? FindFirstObjectByType<Camera>();
            }
            else
            {
                if (SceneView.lastActiveSceneView != null)
                    sourceCamera = SceneView.lastActiveSceneView.camera;
            }
            
            if (sourceCamera != null)
            {
                var originalPosition = sourceCamera.transform.position;
                var originalRotation = sourceCamera.transform.rotation;
                
                pose.ApplyToCamera(sourceCamera);
                
                // Start a coroutine to restore position after a short delay
                EditorApplication.delayCall += () =>
                {
                    if (sourceCamera != null)
                    {
                        sourceCamera.transform.position = originalPosition;
                        sourceCamera.transform.rotation = originalRotation;
                    }
                };
                
                Debug.Log($"Quick preview of pose '{pose.poseName}'");
            }
        }
    }
    
    private void CaptureScreenshotForPose(CameraPose pose)
    {
        Camera sourceCamera = null;
        if (applyToGameCamera)
        {
            sourceCamera = Camera.main ?? FindFirstObjectByType<Camera>();
        }
        else if (SceneView.lastActiveSceneView != null)
        {
            sourceCamera = SceneView.lastActiveSceneView.camera;
        }
        if (sourceCamera == null)
        {
            Debug.LogWarning("Could not find camera to capture screenshot from");
            return;
        }
        var originalPosition = sourceCamera.transform.position;
        var originalRotation = sourceCamera.transform.rotation;
        pose.ApplyToCamera(sourceCamera);
        pose.CaptureScreenshot(sourceCamera, screenshotWidth, screenshotHeight);
        sourceCamera.transform.position = originalPosition;
        sourceCamera.transform.rotation = originalRotation;
        EditorUtility.SetDirty(pose);
        Debug.Log($"Captured screenshot for pose '{pose.poseName}'");
    }
    
    private void RefreshMissingScreenshots()
    {
        if (currentCollection == null) return;
        
        int missingCount = 0;
        foreach (var pose in currentCollection.poses)
        {
            if (pose.screenshotPreview == null)
            {
                missingCount++;
            }
        }
        
        if (missingCount > 0)
        {
            if (EditorUtility.DisplayDialog("Refresh Missing Screenshots", 
                $"Found {missingCount} poses without screenshots. Would you like to capture them now?", 
                "Capture All", "Cancel"))
            {
                CaptureScreenshotForAllPoses();
            }
        }
        else
        {
            Debug.Log("All camera poses have screenshots!");
        }
    }
}

