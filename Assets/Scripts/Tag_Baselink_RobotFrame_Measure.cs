using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag_Baselink_RobotFrame_Measure : MonoBehaviour
{       
    // ################  Measurement with Vicon system (26/02/2024)  ################################
    // Left tag in WorldFrame (different to MapFrame)
    public Vector3 left_tag_pos_WorldFrame = new Vector3(16.46f, 1.73f, 0.43f);
    public Quaternion left_tag_rot_WorldFrame = new Quaternion(0.51f, 0.48f, 0.53f, -0.47f);
    // Right tag in WorldFrame
    public Vector3 right_tag_pos_WorldFrame = new Vector3(15.58f, 2.33f, 0.43f);
    public Quaternion right_tag_rot_WorldFrame = new Quaternion(0.70f, 0.15f, 0.68f, -0.18f);
    // IMU in WorldFrame
    //public Vector3 imu_pos_WorldFrame = new Vector3(16.52f, 2.28f, 0.75f);
    public Vector3 imu_pos_WorldFrame = new Vector3(16.52f, 2.28f, 0.1f);
    public Quaternion imu_rot_WorldFrame = new Quaternion(0f, 0f, 0.89f, -0.46f);
    // ##############################################################################################


    // Tag poses wrt cog in RobotFrame with corresponding marker id
    public Dictionary<string, (Vector3 marker_baselink_pos, Quaternion marker_baselink_rot)> marker_baselink = new();


    void Start()
    {
        imu_rot_WorldFrame.Normalize();

        // Calculate tag poses wrt IMU in ImuFrame
        // Homogeneous matrix to WorldFrame From ImuFrame
        Matrix4x4 HomogeneousMatrix_WorldFrame_ImuFrame = Matrix4x4.TRS(imu_pos_WorldFrame, imu_rot_WorldFrame, Vector3.one);
        // Homogeneous matrix from WorldFrame to ImuFrame
        Matrix4x4 HomogeneousMatrix_ImuFrame_WolrdFrame = HomogeneousMatrix_WorldFrame_ImuFrame.inverse;
        // Left
        Vector3 left_tag_imu_pos_ImuFrame = HomogeneousTransformation(left_tag_pos_WorldFrame, HomogeneousMatrix_ImuFrame_WolrdFrame);
        Quaternion left_tag_imu_rot_ImuFrame = (Matrix4x4.Rotate(left_tag_rot_WorldFrame) * HomogeneousMatrix_ImuFrame_WolrdFrame).rotation;
        Debug.Log("left_tag_imu_ImuFrame: " + left_tag_imu_pos_ImuFrame);
        // Right
        Vector3 right_tag_imu_pos_ImuFrame = HomogeneousTransformation(right_tag_pos_WorldFrame, HomogeneousMatrix_ImuFrame_WolrdFrame);
        Quaternion right_tag_imu_rot_RobotFrame = (Matrix4x4.Rotate(right_tag_rot_WorldFrame) * HomogeneousMatrix_ImuFrame_WolrdFrame).rotation;
        Debug.Log("right_tag_imu_ImuFrame: " + right_tag_imu_pos_ImuFrame);

        // Get imu position wrt baselink in RobotFrame from ROS TF msg         ******************* CHANGE LATER TO READ FROM ROS MSG
        Vector3 imu_baselink_pos_RobotFrame = new Vector3(0.78f, 0.26f, 0.08f);
        
        // Calculate tag poses wrt cog in RobotFrame (No change in orientation because ImuFrame and RobotFrame have the same orientation)
        // Left
        Vector3 tag_left_baselink_pos_RobotFrame = left_tag_imu_pos_ImuFrame + imu_baselink_pos_RobotFrame;
        Quaternion tag_left_baselink_rot_RobotFrame = left_tag_imu_rot_ImuFrame;
        // Right
        Vector3 tag_right_baselink_pos_RobotFrame = right_tag_imu_pos_ImuFrame + imu_baselink_pos_RobotFrame;
        Quaternion tag_right_baselink_rot_RobotFrame = right_tag_imu_rot_RobotFrame;

        // Store tag poses wrt cog in RobotFrame with corresponding marker id
        // Marker on the left
        marker_baselink.Add("7", (tag_left_baselink_pos_RobotFrame, tag_left_baselink_rot_RobotFrame));
        Debug.Log("tag_left_baselink_pos_RobotFrame" + tag_left_baselink_rot_RobotFrame);
        // Marker on the right
        marker_baselink.Add("22", (tag_right_baselink_pos_RobotFrame, tag_right_baselink_rot_RobotFrame));
        Debug.Log("tag_right_baselink_pos_RobotFrame" + tag_right_baselink_rot_RobotFrame);
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
}
