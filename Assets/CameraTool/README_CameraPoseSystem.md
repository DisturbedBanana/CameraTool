# Camera Pose System

A powerful Unity editor tool for capturing, managing, and transitioning between camera positions and rotations. Perfect for creating smooth menu transitions and cinematic camera movements.

## Features

### 🎯 **Core Functionality**
- **Visual Camera Pose Capture**: Capture camera positions directly from the Scene View
- **Smooth Transitions**: Blend between poses with customizable duration and curves
- **Pose Collections**: Organize poses into collections for different scenes/menus
- **Real-time Preview**: Visual gizmos and previews in the Scene View

### 🎨 **Editor Tools**
- **Camera Pose Editor Window**: Comprehensive editor for managing poses
- **Visual Gizmos**: See camera poses in the Scene View with frustum previews
- **Search & Filter**: Find poses quickly with search and category filtering
- **Metadata Support**: Add descriptions, colors, and custom transition settings

### ⚡ **Runtime System**
- **Smooth Blending**: Position and rotation interpolation with custom curves
- **Transition Control**: Start, stop, and manage ongoing transitions
- **Event Integration**: Easy integration with UI buttons and game events

## Quick Start

### 1. Create a Camera Pose Collection
1. Right-click in the Project window
2. Select `Create > Camera > Camera Pose Collection`
3. Name your collection (e.g., "MenuCameraPoses")

### 2. Open the Camera Pose Editor
1. Go to `Window > Camera Pose Editor`
2. Select your collection in the toolbar
3. Click "Capture Current View" to create your first pose

### 3. Set Up Runtime Components
1. Add `CameraPoseController` to your camera GameObject
2. Assign your pose collection to the controller
3. Use `TransitionToPose("PoseName")` to move between poses

## Detailed Usage

### Editor Workflow

#### Capturing Camera Poses
1. **Position your camera** in the Scene View where you want a pose
2. **Open the Camera Pose Editor** (`Window > Camera Pose Editor`)
3. **Select your collection** in the toolbar
4. **Click "Capture Current View"** to create a new pose
5. **Rename and customize** the pose in the details panel

#### Managing Poses
- **Select poses** from the list to edit their properties
- **Update poses** from the current Scene View position
- **Delete poses** with the X button
- **Search and filter** poses using the search bar
- **Toggle preview** to show/hide gizmos in Scene View

#### Pose Properties
- **Name**: Identifier for the pose
- **Description**: Notes about the pose
- **Color**: Visual color for gizmos and UI
- **Position**: Camera world position
- **Rotation**: Camera world rotation (Euler angles)
- **Transition Duration**: How long transitions take
- **Transition Curve**: Animation curve for smoothness

### Runtime Integration

#### Basic Setup
```csharp
// Add to your camera GameObject
CameraPoseController controller = camera.gameObject.AddComponent<CameraPoseController>();
controller.poseCollection = yourPoseCollection;
```

#### Transition Between Poses
```csharp
// Smooth transition
controller.TransitionToPose("MainMenu");

// Instant snap
controller.SnapToPose("Options");

// Check if transitioning
if (!controller.IsTransitioning)
{
    // Safe to start new transition
}
```

#### UI Integration
```csharp
// Connect to UI buttons
public void OnOptionsButtonClick()
{
    cameraController.TransitionToPose("Options");
}

public void OnBackButtonClick()
{
    cameraController.TransitionToPose("MainMenu");
}
```

### Advanced Features

#### Custom Transition Curves
- Use Unity's Animation Curve editor to create custom easing
- Smooth in/out, bounce, elastic, and more
- Different curves for different poses

#### Pose Categories
- Organize poses by category (Menu, Gameplay, Cutscene, etc.)
- Filter poses in the editor
- Use naming conventions for automatic categorization

#### Scene View Integration
- Visual gizmos show camera positions and orientations
- Camera frustum previews
- Click to focus Scene View on poses
- Real-time updates when editing poses

## File Structure

```
Assets/
├── Scripts/
│   ├── CameraPose.cs                    # Individual pose data
│   ├── CameraPoseCollection.cs          # Collection of poses
│   ├── CameraPoseController.cs          # Runtime transition controller
│   └── CameraPoseMenuExample.cs         # Example usage
├── Editor/
│   ├── CameraPoseEditorWindow.cs        # Main editor window
│   └── CameraPoseGizmoDrawer.cs         # Scene View gizmos
└── README_CameraPoseSystem.md           # This file
```

## Tips & Best Practices

### Naming Conventions
- Use descriptive names: "MainMenu", "OptionsMenu", "SettingsView"
- Include context: "MainMenu_Front", "MainMenu_Side"
- Use consistent naming for easy searching

### Transition Settings
- **Fast transitions** (0.5-1s) for responsive UI
- **Medium transitions** (1-2s) for menu changes
- **Slow transitions** (2-3s) for cinematic effects
- **Custom curves** for unique feels

### Performance
- Limit active gizmos in large scenes
- Use pose collections to organize by scene
- Disable preview for poses not in current view

### Workflow Tips
1. **Capture poses first**, then refine positions
2. **Use Scene View focus** to quickly jump between poses
3. **Test transitions** in Play mode frequently
4. **Backup collections** before major changes
5. **Use colors** to visually distinguish pose types

## Troubleshooting

### Common Issues

**Poses not showing in Scene View**
- Check "Gizmos" toggle in editor window
- Ensure pose.showPreview is enabled
- Verify Scene View gizmos are enabled

**Transitions not working**
- Check if CameraPoseController is attached
- Verify pose collection is assigned
- Ensure pose names match exactly

**Editor window not updating**
- Try refreshing the window
- Check for compilation errors
- Restart Unity if needed

### Performance Issues
- Reduce gizmo complexity in large scenes
- Limit number of visible poses
- Use pose collections to organize content

## Extending the System

### Custom Pose Types
You can extend `CameraPose` to include additional data:
```csharp
public class ExtendedCameraPose : CameraPose
{
    public float fieldOfView;
    public float nearClipPlane;
    public float farClipPlane;
    // ... additional properties
}
```

### Custom Transition Effects
Extend `CameraPoseController` for special effects:
```csharp
public class CinematicCameraController : CameraPoseController
{
    public void TransitionWithShake(string poseName, float intensity)
    {
        // Custom transition with camera shake
    }
}
```

### Integration with Other Systems
- **Timeline**: Use poses in Unity Timeline
- **Animation**: Blend with animation curves
- **Events**: Trigger events during transitions
- **Audio**: Sync camera moves with audio cues

## Support

For issues, questions, or feature requests:
1. Check this README first
2. Review the example scripts
3. Test with the provided example scene
4. Check Unity Console for errors

---

**Happy camera posing! 🎬** 