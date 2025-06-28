# Camera Pose UI Setup Guide

This guide shows you how to set up UI buttons to test your camera pose functionality.

## Quick Setup (Recommended)

### Step 1: Set Up Camera Controller
1. Select your main camera in the scene
2. Add the `CameraPoseController` component
3. Assign your `CameraPoseCollection` to the "Pose Collection" field

### Step 2: Create UI Canvas
1. Right-click in Hierarchy → UI → Canvas
2. This creates a Canvas with an EventSystem

### Step 3: Add UI Buttons
1. Right-click on Canvas → UI → Button
2. Create 6 buttons total:
   - **Pose 1** (smooth transition)
   - **Pose 2** (smooth transition) 
   - **Pose 3** (smooth transition)
   - **Snap Pose 1** (instant)
   - **Snap Pose 2** (instant)
   - **Snap Pose 3** (instant)

### Step 4: Add Status Text
1. Right-click on Canvas → UI → Text
2. Position it at the top of the screen
3. Set text to "Current Pose: None"

### Step 5: Add the UI Script
1. Create an empty GameObject called "CameraPoseUI"
2. Add the `CameraPoseUIExample` script
3. Assign all the buttons and text in the inspector

## Manual Button Setup (Alternative)

If you prefer to set up buttons manually without the script:

### Step 1: Create Button
1. Create a UI Button
2. In the Button's Inspector, find "On Click ()" section
3. Click the "+" to add an event

### Step 2: Connect to Camera Controller
1. Drag your camera (with CameraPoseController) to the event field
2. Select `CameraPoseController → TransitionToPose(string)`
3. Type the pose name (e.g., "Pose_1") in the parameter field

### Step 3: Repeat for All Poses
- For smooth transitions: Use `TransitionToPose(string)`
- For instant snaps: Use `SnapToPose(string)`

## Testing Your Setup

### Method 1: Using the UI Script
1. Enter Play Mode
2. Click the buttons to test transitions
3. Watch the status text update
4. Check Console for debug messages

### Method 2: Using Keyboard Controls
1. Add the `CameraPosePreviewTester` script to any GameObject
2. Use keyboard shortcuts:
   - **1, 2, 3** - Smooth transitions
   - **Shift + 1, 2, 3** - Instant snaps

## Troubleshooting

### "CameraPoseController not found" Error
- Make sure CameraPoseController is attached to a camera
- Check that the camera is active in the scene

### Buttons Don't Work
- Verify pose names match exactly (case-sensitive)
- Check that the CameraPoseCollection is assigned
- Ensure you're in Play Mode

### No Transitions
- Check that poses exist in your collection
- Verify the camera has the CameraPoseController component
- Look for errors in the Console

## Advanced Usage

### Custom Button Layout
You can arrange buttons however you like:
- Horizontal layout for menu-style
- Vertical layout for sidebar
- Grid layout for dashboard-style

### Dynamic Button Creation
You can also create buttons programmatically:
```csharp
// Create button at runtime
Button newButton = Instantiate(buttonPrefab, canvas.transform);
newButton.onClick.AddListener(() => cameraController.TransitionToPose("Pose_1"));
```

### Integration with Other Systems
- Connect to game events
- Use with input systems
- Integrate with UI frameworks like UI Toolkit 