using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

/// <summary>
/// 通信链路测试：订阅ROS2的/test_msg话题，收到消息打印到Console
/// </summary>
public class RosConnectionTest : MonoBehaviour
{
    ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<StringMsg>("/test_msg", OnTestMsg);
        Debug.Log("等待ROS2连接，已订阅 /test_msg ...");
    }

    void OnTestMsg(StringMsg msg)
    {
        Debug.Log($"收到ROS2消息: {msg.data}");
    }
}
