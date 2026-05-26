using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

/// <summary>
/// 从InputManager读取方向键输入，计算底座目标位姿，发布到ROS2
/// </summary>
public class BaseController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 0.5f;
    [SerializeField] string _targetTopic = "/base_target_pose";

    ROSConnection _ros;
    Vector3 _targetPos;

    void Start()
    {
        _targetPos = Vector3.zero;
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<PoseStampedMsg>(_targetTopic);
    }

    void Update()
    {
        if (InputManager.Instance == null) return;
        var inp = InputManager.Instance.Adapter;

        if (inp.GetEmergencyStop()) return;

        // Unity坐标增量 → ROS坐标增量
        Vector3 uMove = inp.GetBaseMovement() * _moveSpeed * Time.deltaTime;
        _targetPos += new Vector3(uMove.z, -uMove.x, uMove.y);

        bool hasInput = uMove.sqrMagnitude > 0.0001f;
        if (hasInput)
        {
            var msg = new PoseStampedMsg
            {
                header = new HeaderMsg { frame_id = "world" },
                pose = new PoseMsg(
                    new PointMsg(_targetPos.x, _targetPos.y, _targetPos.z),
                    new QuaternionMsg(0, 0, 0, 1))
            };
            _ros.Publish(_targetTopic, msg);
        }

        // if (Time.frameCount % 30 == 0)
        //     Debug.Log($"底座目标位姿 — 位置(ROS):{_targetPos:F3}m");
    }
}
