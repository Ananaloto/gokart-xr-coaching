//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Gokart
{
    [Serializable]
    public class GokartStateMsg : Message
    {
        public const string k_RosMessageName = "gokart_msgs/GokartState";
        public override string RosMessageName => k_RosMessageName;

        //  This message contains the pose estimate for the gokart
        public Std.HeaderMsg header;
        public Geometry.Pose2DMsg pose2d;
        public Geometry.Pose2DMsg pose2d_dot;

        public GokartStateMsg()
        {
            this.header = new Std.HeaderMsg();
            this.pose2d = new Geometry.Pose2DMsg();
            this.pose2d_dot = new Geometry.Pose2DMsg();
        }

        public GokartStateMsg(Std.HeaderMsg header, Geometry.Pose2DMsg pose2d, Geometry.Pose2DMsg pose2d_dot)
        {
            this.header = header;
            this.pose2d = pose2d;
            this.pose2d_dot = pose2d_dot;
        }

        public static GokartStateMsg Deserialize(MessageDeserializer deserializer) => new GokartStateMsg(deserializer);

        private GokartStateMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            this.pose2d = Geometry.Pose2DMsg.Deserialize(deserializer);
            this.pose2d_dot = Geometry.Pose2DMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.pose2d);
            serializer.Write(this.pose2d_dot);
        }

        public override string ToString()
        {
            return "GokartStateMsg: " +
            "\nheader: " + header.ToString() +
            "\npose2d: " + pose2d.ToString() +
            "\npose2d_dot: " + pose2d_dot.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
