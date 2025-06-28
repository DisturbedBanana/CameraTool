# 🎥 CameraTool - Unity Camera Pose System

A powerful Unity editor tool for creating smooth camera transitions and cinematic movements. Perfect for menu systems, cutscenes, and dynamic camera work in Unity games and applications.

![Unity Version](https://img.shields.io/badge/Unity-6000.0.45f1-blue.svg)
![HDRP](https://img.shields.io/badge/Render%20Pipeline-HDRP-orange.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## ✨ What is CameraTool?

CameraTool is a comprehensive Unity system that allows you to:
- **Capture camera positions** directly from the Scene View
- **Create smooth transitions** between camera poses with customizable curves
- **Organize poses** into collections for different scenes or menus
- **Preview camera movements** with visual gizmos in the editor
- **Integrate seamlessly** with UI systems and game events

## 🚀 Quick Start

### Prerequisites
- Unity 6000.0.45f1 or later
- HDRP (High Definition Render Pipeline) - already configured in this project

### Installation
1. **Clone or download** this repository
2. **Open the project** in Unity
3. **Open the sample scene**: `Assets/OutdoorsScene.unity`

### First Steps (5 minutes)
1. **Open the Camera Pose Editor**: `Window > Camera Pose Editor`
2. **Create a pose collection**: Right-click in Project → `Create > Camera > Camera Pose Collection`
3. **Capture your first pose**: Position camera in Scene View → Click "Capture Current View"
4. **Add the controller**: Select your camera → Add `CameraPoseController` component
5. **Test transitions**: Enter Play Mode → Use the UI buttons or keyboard shortcuts

## 🎯 Key Features

### 🎨 Visual Editor Tools
- **Camera Pose Editor Window**: Complete pose management interface
- **Scene View Gizmos**: Visual preview of camera positions and orientations
- **Real-time Updates**: See changes instantly in the Scene View
- **Search & Filter**: Quickly find poses in large collections

### ⚡ Runtime System
- **Smooth Transitions**: Position and rotation interpolation with custom curves
- **Transition Control**: Start, stop, and manage ongoing camera movements
- **Event Integration**: Easy connection to UI buttons and game events
- **Performance Optimized**: Efficient interpolation and memory management

### 📁 Organization
- **Pose Collections**: Group poses by scene, menu, or functionality
- **Metadata Support**: Add descriptions, colors, and custom settings
- **Category Filtering**: Organize poses with tags and categories

## 📖 How to Use

### Creating Camera Poses
1. **Position your camera** in the Scene View where you want a pose
2. **Open Camera Pose Editor** (`Window > Camera Pose Editor`)
3. **Select your collection** in the toolbar
4. **Click "Capture Current View"** to create a new pose
5. **Customize the pose**: Name, description, transition settings

### Setting Up Runtime
```csharp
// Add to your camera GameObject
CameraPoseController controller = camera.gameObject.AddComponent<CameraPoseController>();
controller.poseCollection = yourPoseCollection;

// Transition between poses
controller.TransitionToPose("MainMenu");     // Smooth transition
controller.SnapToPose("Options");            // Instant snap
```

### UI Integration
```csharp
// Connect to UI buttons
public void OnMenuButtonClick()
{
    cameraController.TransitionToPose("MainMenu");
}
```

## 🎮 Sample Scene

The project includes a complete sample scene (`OutdoorsScene.unity`) with:
- **5 pre-configured camera poses** (Left, Middle, Right, Top views)
- **UI buttons** for testing transitions
- **Keyboard shortcuts** for quick testing
- **Visual gizmos** showing camera positions

### Testing the Sample
1. Open `Assets/OutdoorsScene.unity`
2. Enter Play Mode
3. Use the UI buttons or keyboard shortcuts:
   - **1, 2, 3, 4, 5** - Smooth transitions
   - **Shift + 1, 2, 3, 4, 5** - Instant snaps

## 📁 Project Structure

```
Assets/
├── CameraTool/                    # Main tool folder
│   ├── Scripts/                   # Runtime scripts
│   │   ├── CameraPose.cs         # Individual pose data
│   │   ├── CameraPoseCollection.cs # Collection management
│   │   ├── CameraPoseController.cs # Runtime controller
│   │   ├── CameraPoseUIExample.cs # UI integration example
│   │   └── CameraPosePreviewTester.cs # Keyboard testing
│   ├── EditorScripts/             # Editor tools
│   │   ├── CameraPoseEditorWindow.cs # Main editor window
│   │   └── CameraPoseGizmoDrawer.cs # Scene View gizmos
│   ├── README_CameraPoseSystem.md # Detailed documentation
│   └── UI_Setup_Guide.md         # UI integration guide
├── OutdoorsScene.unity           # Sample scene
├── Pose_1.asset to Pose_5.asset  # Sample pose assets
└── kenney_*                      # Sample 3D assets
```

## 🔧 Advanced Features

### Custom Transition Curves
- Use Unity's Animation Curve editor for custom easing
- Create smooth in/out, bounce, elastic effects
- Different curves for different poses

### Performance Optimization
- Efficient interpolation algorithms
- Gizmo culling for large scenes
- Memory-efficient pose storage

### Extensibility
- Extend `CameraPose` class for additional data
- Custom transition types
- Integration with other Unity systems

## 🛠️ Troubleshooting

### Common Issues

**Poses not showing in Scene View**
- Check "Gizmos" toggle in editor window
- Ensure Scene View gizmos are enabled
- Verify pose.showPreview is enabled

**Transitions not working**
- Check if CameraPoseController is attached to camera
- Verify pose collection is assigned
- Ensure pose names match exactly (case-sensitive)

**Editor window not updating**
- Try refreshing the window
- Check for compilation errors
- Restart Unity if needed

## 📚 Documentation

- **[Camera Pose System Guide](Assets/CameraTool/README_CameraPoseSystem.md)** - Complete system documentation
- **[UI Setup Guide](Assets/CameraTool/UI_Setup_Guide.md)** - UI integration instructions
- **Sample Scene** - Working example with all features

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built for Unity 6000.0.45f1 with HDRP
- Sample 3D assets from Kenney (kenney.nl)
- Designed for smooth camera workflows in Unity

---

**Ready to create amazing camera movements?** Open the project in Unity and start with the sample scene!