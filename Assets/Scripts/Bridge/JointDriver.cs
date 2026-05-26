using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

/// <summary>
/// 关节驱动器：订阅/joint_states，将角度映射到URDF关节的localRotation
/// </summary>
public class JointDriver : MonoBehaviour
{
    ROSConnection ros;

    // 关节名 → 子连杆Transform
    Dictionary<string, Transform> jointMap = new();
    // 关节名 → 初始localRotation（零位）
    Dictionary<string, Quaternion> initialRot = new();
    // 关节名 → ROS2最新角度（弧度）
    Dictionary<string, float> targetAngles = new();

    // 6个被驱动连杆的完整路径（相对于niryo_one根）
    static readonly string[] LinkPaths = {
        "world/base_link/shoulder_link",                              // joint_1
        "world/base_link/shoulder_link/arm_link",                     // joint_2
        "world/base_link/shoulder_link/arm_link/elbow_link",          // joint_3
        "world/base_link/shoulder_link/arm_link/elbow_link/forearm_link", // joint_4
        "world/base_link/shoulder_link/arm_link/elbow_link/forearm_link/wrist_link", // joint_5
        "world/base_link/shoulder_link/arm_link/elbow_link/forearm_link/wrist_link/hand_link", // joint_6
    };

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointStateMsg>("/joint_states", OnJointState);

        // 移除URDF-Importer自动添加的ArticulationBody，ROS2驱动不需要Unity物理
        foreach (var ab in GetComponentsInChildren<ArticulationBody>(true))
            ab.enabled = false;

        for (int i = 0; i < 6; i++)
        {
            var name = $"joint_{i + 1}";
            var t = transform.Find(LinkPaths[i]);
            if (t == null)
            {
                Debug.LogError($"未找到连杆: {LinkPaths[i]}");
                continue;
            }
            jointMap[name] = t;
            initialRot[name] = t.localRotation;
            targetAngles[name] = 0f;
        }

        Debug.Log($"关节驱动器就绪，已映射 {jointMap.Count} 个关节");
    }

    int _logCounter;

    void OnJointState(JointStateMsg msg)
    {
        for (int i = 0; i < msg.name.Length; i++)
            targetAngles[msg.name[i]] = (float)msg.position[i];

        // 每 30 帧打印一次接收到的关节角度
        if (++_logCounter % 30 == 0)
        {
            var parts = new System.Text.StringBuilder();
            for (int i = 0; i < msg.name.Length; i++)
                parts.Append($"{msg.name[i]}={(float)msg.position[i] * Mathf.Rad2Deg:F1}° ");
            Debug.Log($"收到关节角度: {parts}");
        }
    }

    void Update()
    {
        foreach (var kv in jointMap)
        {
            var t = kv.Value;
            if (t == null) continue;
            var angleDeg = targetAngles[kv.Key] * Mathf.Rad2Deg;
            // URDF axis=(0,0,1) → Unity 本地Y轴
            t.localRotation = initialRot[kv.Key] * Quaternion.Euler(0, angleDeg, 0);
        }
    }
}
