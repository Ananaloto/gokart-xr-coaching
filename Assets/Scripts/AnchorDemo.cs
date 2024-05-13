using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = LearnXR.Core.Logger;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine.Rendering.Universal;


public class AnchorDemo : MonoBehaviour
{
    public RobotInfoSubscriber ri;
    public MarkerTracker mt;
    public Tag_Baselink_RobotFrame_Measure tbrm;
    public Material CustomMaterial;

    private bool anchor_created = false;
    public GameObject anchor_prefab;

    // Set anchor pose in MapFrame (to choose): anchor_map_MapFrame
    public Vector3 anchor_map_pos_MapFrame = new Vector3(32.3f, 22.3f, 0.5f);   //################### CHANGE FOR NEED ######################
    public Quaternion anchor_map_rot_MapFrame = Quaternion.Euler(0, 0, 0);


    void Start()
    {
        if (ri == null | mt == null | tbrm == null)
        {
            Debug.Log("RobotInfoSubscriber or MarkerTracker or Tag_Baselink_RobotFrame_Measure is not initiated. Please check again.");
            return;
        }
        LocalizeAnchor();
    }

    void Update()
    {
        LocalizeAnchor();
    }

    void LocalizeAnchor()
    {
        string marker_in_fov = mt.marker_in_fov;
        if (marker_in_fov == "")
        {
            Debug.Log("No valid marker in field of view.");
            Logger.Instance.LogInfo($"No valid marker in field of view.");
            return;
        }

        // Calculate tag pose wrt baselink in RobotFrame with measurement: marker_baselink pose (Fixed)
        Vector3 marker_baselink_RobotFrame = tbrm.marker_baselink[marker_in_fov].marker_baselink_pos;


        // ------------------------------------ MapFrame ------------------------------------------------------------------------
        // Get robot baselink pose from ROS msg (in MapFrame)
        Vector3 baselink_map_pos = ri.baselink_map_pos;
        Quaternion baselink_map_rot = ri.baselink_map_rot;
        Debug.Log("ROBOT: " + baselink_map_pos);

        // Convert marker pose from RobotFrame into MapFrame: marker_map_MapFrame
        Matrix4x4 HomogeneousMatrix_RobotFrame_MapFrame = Matrix4x4.TRS(baselink_map_pos, baselink_map_rot, Vector3.one);     // Rotation matrix of robot->map
        Vector3 marker_map_pos_MapFrame = HomogeneousTransformation(marker_baselink_RobotFrame, HomogeneousMatrix_RobotFrame_MapFrame);
        Debug.Log("marker_map_MapFrame: " + marker_map_pos_MapFrame);

        // Calculate relative pose of anchor to marker in MapFrame: anchor_marker_MapFrame
        Vector3 anchor_marker_pos_MapFrame = anchor_map_pos_MapFrame - marker_map_pos_MapFrame;
        Debug.Log("anchor_marker_pos_MapFrame: " + anchor_marker_pos_MapFrame);


        // ------------------------------------ UnityFrame ----------------------------------------------------------------------
        // Convert anchor_marker_MapFrame from MapFrame into UnityFrame: anchor_marker_UnityFrame
        Vector3 anchor_marker_pos_UnityFrame = Ros2Unity_pos(anchor_marker_pos_MapFrame);
        Debug.Log("anchor_marker_pos_UnityFrame: " + anchor_marker_pos_UnityFrame);

        // Get marker pose in UnityFrame from MarkerTraker Script
        Vector3 marker_pos_UnityFrame = mt.markerPoses[marker_in_fov].markerPosition;
        Debug.Log("marker_UnityFrame: " + marker_pos_UnityFrame);

        // Calculate anchor pose in UnityFrame
        Vector3 anchor_pos_UnityFrame = anchor_marker_pos_UnityFrame + marker_pos_UnityFrame;
        Debug.Log("anchor_pos_UnityFrame: " + anchor_pos_UnityFrame);


        // ------------------------------------- Visualization ------------------------------------------------------------------
        if (anchor_created == false)
        {
            // Create a new Gameobject to instantiate the prefab for anchor
            GameObject anchor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            anchor.name = "anchor";
            // Material
            anchor.GetComponent<Renderer>().material = CustomMaterial;
            // Color
            anchor.GetComponent<Renderer>().material.color = Color.red;
            // Scale (size)
            anchor.transform.localScale = new Vector3(0.81f, 0.27f, 0.81f);
            // Position and rotation
            anchor.transform.position = anchor_pos_UnityFrame;
            anchor.transform.rotation = anchor_map_rot_MapFrame;
       
            anchor_created = true;
        }
        else
        {
            // Update anchor position
            GameObject anchor = GameObject.Find("anchor");
            anchor.transform.position = anchor_pos_UnityFrame;
        }
    }


    static Vector3 HomogeneousTransformation(Vector3 vector, Matrix4x4 HomogeneousMatrix)
    {
        // Convert Vector3 to homogeneous coordinates (x, y, z, 1)
        Vector4 vectorHomogeneous = new Vector4(vector.x, vector.y, vector.z, 1.0f);
        // Multiply the vector by the rotation matrix
        Vector4 resultHomogeneous = HomogeneousMatrix * vectorHomogeneous;
        // Extract translation from the last column of the matrix
        Vector3 resultVector = new Vector3(resultHomogeneous.x, resultHomogeneous.y, resultHomogeneous.z);
        return resultVector;
    }


    static Vector3 Ros2Unity_pos(Vector3 vector3)
    {
        return new Vector3(-vector3.y, vector3.z, vector3.x);
    }

}
