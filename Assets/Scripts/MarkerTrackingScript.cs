using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.XR.MagicLeap.MLMarkerTracker;
using Logger = LearnXR.Core.Logger;


public class MarkerTracker : MonoBehaviour
{
    // Create a ditionary for all markers detected.
    public Dictionary<string, (Vector3 markerPosition, Quaternion markerRotation)> markerPoses = new();
    // Initialize am empty string for id of marker in filed of view
    public string marker_in_fov = "";

    // Marker settings
    public float QRCodeSize = .1f;
    public float ArucoMarkerSize = .1f;
    public MarkerType MarkerTypes = MarkerType.Aruco_April;
    public ArucoDictionaryName ArucoDicitonary = ArucoDictionaryName.DICT_APRILTAG_25h9;
    // The Marker Tracker Profile to use
    public Profile trackerProfile = Profile.Custom;
    // Custom Profile Settings
    FPSHint fpsHint = FPSHint.High;
    ResolutionHint resolutionHint = ResolutionHint.Medium;
    public CameraHint cameraHint = CameraHint.World;
    FullAnalysisIntervalHint analysisInterval = FullAnalysisIntervalHint.Medium;
    CornerRefineMethod refineMethod = CornerRefineMethod.Contour;
    bool useEdgeRefinement = false;
    bool EnableMarkerScanning = true;


    //#if !UNITY_EDITOR
    private void Start()
    {
        // Create Custom Settings only if tarckerProfile is set to custom, otherwise, use default.
        TrackerSettings.CustomProfile customProfile = trackerProfile == Profile.Custom
            ? TrackerSettings.CustomProfile.Create(fpsHint, resolutionHint, cameraHint, analysisInterval, refineMethod, useEdgeRefinement)
            : default;

        //Create and set the marker tracker settings.
        TrackerSettings markerSettings = TrackerSettings.Create(EnableMarkerScanning, MarkerTypes, QRCodeSize, ArucoDicitonary, ArucoMarkerSize, trackerProfile, customProfile);
        _ = SetSettingsAsync(markerSettings);
    }
    private void OnEnable()
    {
         OnMLMarkerTrackerResultsFound += OnTrackerResultsFound;
    }

    private void OnDisable()
    {
        OnMLMarkerTrackerResultsFound -= OnTrackerResultsFound;
        _ = StopScanningAsync();
    }
    
    private void OnTrackerResultsFound(MarkerData data)
    {
        string id = "";
        if (data.Type == MarkerType.Aruco_April)
        {
            id = data.ArucoData.Id.ToString();
        }
        else
        {
            Debug.Log("Not a valid AptilTag");
        }
        marker_in_fov = id;

        if (!string.IsNullOrEmpty(id))
        {
            if (markerPoses.ContainsKey(id))
            {
                markerPoses[id] = (data.Pose.position, data.Pose.rotation);    // 'data.Pose': The world-space position and rotation of the marker.
                //Print id of markers in field of view to logger
                //Logger.Instance.LogInfo($"IN FOV: {id} Position: {data.Pose.position}");
            }
            else
            {
                markerPoses.Add(id, (data.Pose.position, data.Pose.rotation));
                //Print id of markers in field of view to logger
                //Logger.Instance.LogInfo($"IN FOV: {id} Position: {data.Pose.position}");
            }
        }

    }

}
