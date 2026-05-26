using UnityEngine;

/// <summary>
/// 操控指令统一接口 — 键鼠/VR/触屏均实现此接口，业务层只调接口，不直接读硬件
/// </summary>
public interface IControlCommand
{
    Vector3 GetMovement();     // 末端位移：X=前后, Y=升降, Z=左右
    Vector3 GetRotation();     // 末端旋转：X=俯仰, Y=偏航, Z=自转
    Vector3 GetBaseMovement(); // 底座位移：X=前后, Y=升降, Z=左右
    float GetGripper();        // 夹爪开合（预留）
    bool GetEmergencyStop();   // 急停
    bool GetHome();            // 归零
}
