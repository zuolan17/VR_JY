using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

/// <summary>
/// 传感器数据接收测试：订阅 /joint_states、/camera/image_raw、/scan
/// 收到数据后在Console打印摘要
/// </summary>
public class SensorDataTest : MonoBehaviour
{
    ROSConnection ros;
    int jointCount, imageCount, scanCount;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointStateMsg>("/joint_states", OnJointState);
        ros.Subscribe<ImageMsg>("/camera/image_raw", OnImage);
        ros.Subscribe<LaserScanMsg>("/scan", OnLaserScan);
        Debug.Log("已订阅传感器话题：/joint_states, /camera/image_raw, /scan");
    }

    void OnJointState(JointStateMsg msg)
    {
        jointCount++;
        if (jointCount % 10 == 1) // 每10帧打印一次，避免刷屏
            Debug.Log($"收到JointState [{jointCount}]: {msg.name.Length}个关节");
    }

    void OnImage(ImageMsg msg)
    {
        imageCount++;
        if (imageCount % 10 == 1)
            Debug.Log($"收到Image [{imageCount}]: {msg.width}x{msg.height}, 编码={msg.encoding}, 数据量={msg.data.Length}字节");
    }

    void OnLaserScan(LaserScanMsg msg)
    {
        scanCount++;
        if (scanCount % 10 == 1)
            Debug.Log($"收到LaserScan [{scanCount}]: {msg.ranges.Length}个点, range=[{msg.range_min}~{msg.range_max}]");
    }
}
