using System.Collections;
using UnityEngine;

public class CameraPoseController : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public CameraPoseCollection poseCollection;
    
    [Header("Runtime State")]
    [SerializeField] private CameraPose currentPose;
    [SerializeField] private bool isTransitioning = false;
    
    private Coroutine transitionCoroutine;
    
    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }
    
    public void TransitionToPose(string poseName)
    {
        if (poseCollection == null) return;
        
        CameraPose targetPose = poseCollection.GetPoseByName(poseName);
        if (targetPose != null)
        {
            TransitionToPose(targetPose);
        }
        else
        {
            Debug.LogWarning($"Camera pose '{poseName}' not found in collection!");
        }
    }
    
    public void TransitionToPose(CameraPose targetPose)
    {
        if (targetPose == null || targetCamera == null) return;
        
        // Stop any ongoing transition
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        transitionCoroutine = StartCoroutine(TransitionCoroutine(targetPose));
    }
    
    public void SnapToPose(string poseName)
    {
        if (poseCollection == null) return;
        
        CameraPose targetPose = poseCollection.GetPoseByName(poseName);
        if (targetPose != null)
        {
            SnapToPose(targetPose);
        }
    }
    
    public void SnapToPose(CameraPose targetPose)
    {
        if (targetPose == null || targetCamera == null) return;
        
        // Stop any ongoing transition
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
        
        targetPose.ApplyToCamera(targetCamera);
        currentPose = targetPose;
        isTransitioning = false;
    }
    
    private IEnumerator TransitionCoroutine(CameraPose targetPose)
    {
        isTransitioning = true;
        
        Vector3 startPosition = targetCamera.transform.position;
        Quaternion startRotation = targetCamera.transform.rotation;
        
        Vector3 endPosition = targetPose.position;
        Quaternion endRotation = targetPose.rotation;
        
        float duration = targetPose.transitionDuration;
        AnimationCurve curve = targetPose.transitionCurve;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = curve.Evaluate(t);
            
            targetCamera.transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            targetCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            
            yield return null;
        }
        
        // Ensure we end up exactly at the target
        targetCamera.transform.position = endPosition;
        targetCamera.transform.rotation = endRotation;
        
        currentPose = targetPose;
        isTransitioning = false;
        transitionCoroutine = null;
    }
    
    public bool IsTransitioning => isTransitioning;
    public CameraPose CurrentPose => currentPose;
    
    // Editor helper methods
    public void SetPoseCollection(CameraPoseCollection collection)
    {
        poseCollection = collection;
    }
} 