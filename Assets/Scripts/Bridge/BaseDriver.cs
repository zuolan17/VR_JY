using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

/// <summary>
/// 底座驱动器：订阅/base_pose，更新底座Transform
/// </summary>
public class BaseDriver : MonoBehaviour
{
    [SerializeField] Transform _baseLink;

    ROSConnection _ros;
    Vector3 _targetUnityPos;

    void Start()
    {
        if (_baseLink == null)
        {
            Debug.LogError("BaseDriver: 未赋值 _baseLink");
            return;
        }
        _targetUnityPos = _baseLink.localPosition;
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.Subscribe<PoseMsg>("/base_pose", OnBasePose);
    }

    void OnBasePose(PoseMsg msg)
    {
        // ROS坐标 → Unity坐标
        _targetUnityPos = new Vector3(-(float)msg.position.y, (float)msg.position.z, (float)msg.position.x);
    }

    void Update()
    {
        if (_baseLink != null)
            _baseLink.localPosition = _targetUnityPos;
    }
}
