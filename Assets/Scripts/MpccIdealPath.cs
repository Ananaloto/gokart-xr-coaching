using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.Visualization;
using Logger = LearnXR.Core.Logger;


public class MpccIdealPath : MonoBehaviour
{
    public MarkerTracker mt;
    public RobotInfoSubscriber ri;
    public Tag_Baselink_RobotFrame_Measure tbrm;
    public Material CustomMaterial;

    public int N_idea_path_points = 31;
    public List<GameObject> idea_path_points = new List<GameObject>();

    void Start()
    {
        if (ri == null | mt == null | tbrm == null)
        {
            Debug.Log("RobotInfoSubscriber or MarkerTracker or Tag_Baselink_RobotFrame_Measure is not initiated. Please check again.");
            return;
        }
        // Initialize the GameObjects for points
        for (int j = 0; j < N_idea_path_points; j++)
        {
            idea_path_points.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
            idea_path_points[j].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            idea_path_points[j].GetComponent<Renderer>().material = CustomMaterial;
        }
        Debug.Log("point number: " + idea_path_points.Count);

        // Get mpcc info from ROS msg
        ROSConnection.GetOrCreateInstance().Subscribe<MarkerArrayMsg>("bumblebee/MpccController/mpcc_horizon", ReceiveMpccIdealPath);
    }


    void ReceiveMpccIdealPath(MarkerArrayMsg mpccMessage)
    {
        PointMsg[] points_msg = mpccMessage.markers[0].points;
        ColorRGBAMsg[] point_colors_msg = mpccMessage.markers[0].colors;

        // Get robot baselink pose from ROS msg (in MapFrame)
        Vector3 baselink_pos_MapFrame = ri.baselink_map_pos;
        Quaternion baselink_rot_MapFrame = ri.baselink_map_rot;

        // Marker in field of view
        string marker_in_fov = mt.marker_in_fov;
        if (marker_in_fov == "")
        {
            Debug.Log("No valid marker in field of view.");
            Logger.Instance.LogInfo($"No valid marker in field of view.");
            return;
        }

        // Update pose and color for each point
        for (int i = 0; i < N_idea_path_points; i++) 
        {
            // Pose ===========================================================================================
            // Get pose of each point from ROS msg in MapFrame: point_MapFrame
            (float point_x, float point_y, float point_z) = (Convert.ToSingle(points_msg[i].x), Convert.ToSingle(points_msg[i].y), Convert.ToSingle(points_msg[i].z));
            Vector3 point_pos_MapFrame = new Vector3(point_x, point_y, point_z);

            // Move points forarwd for 1m along x axis in RobotFrame  -> calculate placement in MapFrame for each point
            double theta_in_rad = Convert.ToDouble(ri.baselink_map_theta_rad);
            float displacement_len_RobotFrame_X = 1f; //m
            float displacement_x = Convert.ToSingle(displacement_len_RobotFrame_X * Math.Cos(theta_in_rad));
            float displacement_y = Convert.ToSingle(displacement_len_RobotFrame_X * Math.Sin(theta_in_rad));
            Vector3 point_displacement_MapFrame = new Vector3(displacement_x, displacement_y, 0f);
            point_pos_MapFrame = point_pos_MapFrame + point_displacement_MapFrame;

            // Marker_baselink in RobotFrame
            Vector3 marker_baselink_pos_RobotFrame = tbrm.marker_baselink[marker_in_fov].marker_baselink_pos;

            // Rotation matrix of robot->map
            Matrix4x4 HomogeneousMatrix_MapFrame_RobotFrame = Matrix4x4.TRS(baselink_pos_MapFrame, baselink_rot_MapFrame, Vector3.one);

            // Convert marker pose from RobotFrame into MapFrame: marker_map_MapFrame
            Vector3 marker_pos_MapFrame = HomogeneousTransformation(marker_baselink_pos_RobotFrame, HomogeneousMatrix_MapFrame_RobotFrame);
            Debug.Log("Tag_MapFrame: " + marker_pos_MapFrame);

            // point_marker_MapFrame
            Vector3 point_marker_pos_MapFrame = point_pos_MapFrame - marker_pos_MapFrame;
            Debug.Log("Point_tag_MapFrame: " + point_marker_pos_MapFrame);

            // point_marker_UnityFrame
            Vector3 point_marker_pos_UnityFrame = Ros2Unity_pos(point_marker_pos_MapFrame);

            // marker_UnityFrame
            Vector3 marker_pos_UnityFrame = mt.markerPoses[marker_in_fov].markerPosition;

            // Calculate point_UnityFrame
            Vector3 point_pos_UnityFrame = point_marker_pos_UnityFrame + marker_pos_UnityFrame;

            // Change pose of corresponding GameObject of current point
            idea_path_points[i].transform.position = point_pos_UnityFrame;


            // Color ===========================================================================================
            Color point_color = new Color(point_colors_msg[i].r, point_colors_msg[i].g, point_colors_msg[i].b, point_colors_msg[i].a);
            idea_path_points[i].GetComponent<Renderer>().material.color = point_color;
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
