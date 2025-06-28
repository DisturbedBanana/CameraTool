using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Camera Pose Collection", menuName = "Camera/Camera Pose Collection")]
public class CameraPoseCollection : ScriptableObject
{
    [Header("Collection Settings")]
    public string collectionName = "Menu Camera Poses";
    [TextArea(2, 4)]
    public string description = "Collection of camera poses for menu transitions";
    
    [Header("Poses")]
    public List<CameraPose> poses = new List<CameraPose>();
    
    [Header("Default Settings")]
    [Range(0.1f, 5f)]
    public float defaultTransitionDuration = 1f;
    public AnimationCurve defaultTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    public CameraPose GetPoseByName(string poseName)
    {
        return poses.Find(pose => pose.poseName == poseName);
    }
    
    public void AddPose(CameraPose pose)
    {
        if (pose != null && !poses.Contains(pose))
        {
            poses.Add(pose);
        }
    }
    
    public void RemovePose(CameraPose pose)
    {
        if (pose != null)
        {
            poses.Remove(pose);
        }
    }
    
    public void RemovePoseAt(int index)
    {
        if (index >= 0 && index < poses.Count)
        {
            poses.RemoveAt(index);
        }
    }
} 