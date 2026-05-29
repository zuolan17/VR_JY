using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

/// <summary>
/// 从InputManager读取输入，计算末端目标位姿，发布到ROS2
/// </summary>
public class ArmController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 0.2f;
    [SerializeField] float _rotateSpeed = 20f;
    [SerializeField] string _poseTopic = "/target_pose";
    [SerializeField] Transform _baseLink;
    [SerializeField] Transform _toolLink;

    ROSConnection _ros;
    Vector3 _targetPos;   // ROS坐标系（X=前 Y=左 Z=上）
    Vector3 _targetEuler;
    Vector3 _homePos;     // 场景初始位姿（归零目标）
    Vector3 _homeEuler;
    bool _dirty;

    // ====== 测试代码 — 在Scene中显示末端位姿变化 ======
    // [SerializeField] bool _enableTest = true;
    // [SerializeField] GameObject _robotRoot;
    // Transform _endEffector;
    // Vector3 _initEndPos;
    // Quaternion _initEndRot;
    // ====== 测试代码结束 ======

    void Start()
    {
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<PoseMsg>(_poseTopic);

        // 从场景读取末端初始位姿（相对于base_link）
        if (_baseLink != null && _toolLink != null)
        {
            Vector3 uPos = _baseLink.InverseTransformPoint(_toolLink.position);
            _targetPos = new Vector3(uPos.z, -uPos.x, uPos.y);

            Quaternion uRot = Quaternion.Inverse(_baseLink.rotation) * _toolLink.rotation;
            Vector3 uEuler = uRot.eulerAngles;
            _targetEuler = new Vector3(uEuler.z, -uEuler.x, uEuler.y);

            _homePos = _targetPos;
            _homeEuler = _targetEuler;

            Debug.Log($"初始位姿已从场景读取 — 位置:{_targetPos:F3}  旋转:{_targetEuler:F1}°");
        }
        else
        {
            Debug.LogError("ArmController: 未赋值 _baseLink 或 _toolLink，请在Inspector中拖入");
        }
    }

    void Update()
    {
        if (InputManager.Instance == null) return;
        var inp = InputManager.Instance.Adapter;

        if (inp.GetEmergencyStop()) { Debug.LogWarning("急停!"); return; }
        if (inp.GetHome()) { _targetPos = _homePos; _targetEuler = _homeEuler; Debug.Log("归零"); }

        float dt = Time.deltaTime;
        // Unity坐标增量 → ROS坐标增量：Unity(z, -x, y) → ROS(x, y, z)
        Vector3 uMove = inp.GetMovement() * _moveSpeed * dt;
        _targetPos += new Vector3(uMove.z, -uMove.x, uMove.y);
        Vector3 uRot = inp.GetRotation() * _rotateSpeed * dt;
        _targetEuler += new Vector3(uRot.z, -uRot.x, uRot.y);

        // 只在用户有操作时才发布
        bool hasInput = uMove.sqrMagnitude > 0.0001f || uRot.sqrMagnitude > 0.0001f;
        if (hasInput || _dirty)
        {
            _dirty = hasInput;
            PublishPose();
        }

        if (Time.frameCount % 30 == 0)
            Debug.Log($"目标位姿 — 位置:{_targetPos:F3}m  旋转:{_targetEuler:F1}°");
    }

    void PublishPose()
    {
        // _targetPos/_targetEuler 已是ROS坐标系，直接发送
        var pos = new PointMsg(_targetPos.x, _targetPos.y, _targetPos.z);

        // ROS欧拉 → Unity欧拉，再生成四元数
        Quaternion q = Quaternion.Euler(-_targetEuler.y, _targetEuler.z, _targetEuler.x);
        // Unity四元数 → ROS四元数（URDF导入器标准）
        var orient = new QuaternionMsg(-q.x, -q.z, -q.y, q.w);

        _ros.Publish(_poseTopic, new PoseMsg(pos, orient));
    }
}
