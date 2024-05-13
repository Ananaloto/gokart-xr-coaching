using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Logger = LearnXR.Core.Logger;
using GokartState = RosMessageTypes.Gokart.GokartStateMsg;


public class RobotInfoSubscriber : MonoBehaviour
{
    // NOTICE: We calculate all the pose in Unity frame (X axis points right, Y up, and Z forward).
    // Use ".From<FLU>()" to transform the pose received from ROS msg from FLU frame to Unity frame if needed.

    public Vector3 baselink_map_pos;
    public Quaternion baselink_map_rot;
    public float baselink_map_theta_rad;

    public float velocity_x;
    public float velocity_y;
    public float velocity_theta;

    void Start()
    {
        // Connect to ROS and subscribe topics for robot pose and velocity
        ROSConnection.GetOrCreateInstance().Subscribe<GokartState>("bumblebee/se2_localization/gokart_state", ReceiveRobotPoseVelocity);
        // Test
        //ROSConnection.GetOrCreateInstance().Subscribe<RosMessageTypes.Std.StringMsg>("/test_msg", TestReceiveMsg);
    }


    void ReceiveRobotPoseVelocity(GokartState gokartStateMessage) 
    {
        // Pose in map frame
        Pose2DMsg pose2d_msg = gokartStateMessage.pose2d;
        float baselink_map_x = Convert.ToSingle(pose2d_msg.x);
        float baselink_map_y = Convert.ToSingle(pose2d_msg.y);
        baselink_map_theta_rad = Convert.ToSingle(pose2d_msg.theta);  // in radian [-pi,pi]
        float baselink_map_theta_degree = Convert.ToSingle(pose2d_msg.theta * (180.0 / Math.PI));  // in degrees [-180,180]
        // Convert into Vector3 and Quaternion
        baselink_map_pos = new Vector3(baselink_map_x, baselink_map_y, 0f);
        baselink_map_rot = Quaternion.Euler(0, 0, baselink_map_theta_degree);

        // Velocity in map frame
        Pose2DMsg pose2d_dot_msg = gokartStateMessage.pose2d_dot;
        velocity_x = Convert.ToSingle(pose2d_dot_msg.x);
        velocity_y = Convert.ToSingle(pose2d_dot_msg.y);
        velocity_theta = Convert.ToSingle(pose2d_dot_msg.theta);

        // Print
        Debug.Log("robot_cog_map_x: " + baselink_map_x + "   robot_cog_map_y: " + baselink_map_y + "  robot_cog_map_theta" + baselink_map_theta_degree);
        // Visualize on ML
        Logger.Instance.LogInfo($"robot_x: {baselink_map_x} robot_y: {baselink_map_y} robot_theta: {baselink_map_theta_degree}");
        //Logger.Instance.LogInfo($"velocity_x: {velocity_x}");
    }




    //void TestReceiveMsg(RosMessageTypes.Std.StringMsg stringMessage)
    //{
    //    Debug.Log(stringMessage);
    //    // Visualize on ML
    //    Logger.Instance.LogInfo($"test: {stringMessage}");
    //}



    //    void ReceiveImuTf(RosTf imuTfMessage)
    //    {
    //        Debug.Log("imuTfMessage");
    //        Debug.Log(imuTfMessage);

    //        // Test
    //        string frameId = imuTfMessage.transforms[0].header.frame_id;        // Is [0] correct?
    //        string chideFrameId = imuTfMessage.transforms[0].child_frame_id;
    //        Debug.Log("Frame_id: " + frameId + "ChildframeId: " + chideFrameId);    // Should be "bumblebee/base_link" and "bumblebee/imu"

    //        // Get imu_baselink TF info from message
    //        TransformMsg imu_baselink_transform = imuTfMessage.transforms[0].transform;
    //        imu_baselink_position = imu_baselink_transform.translation.From<FLU>();
    //        imu_baselink_rotation = imu_baselink_transform.rotation.From<FLU>();

    //        // Print
    //        Debug.Log("Imu_baselink_position: " + imu_baselink_position + "Imu_baselink_rotation: " + imu_baselink_rotation);

    //        // Visualize on ML
    //        //Logger.Instance.LogInfo($"Imu_baselink_position: {imu_baselink_position} Imu_baselink_rotation: {imu_baselink_rotation}");
    //    }

}
