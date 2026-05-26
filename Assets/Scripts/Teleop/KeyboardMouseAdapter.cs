using UnityEngine;

/// <summary>
/// 键鼠操控适配器
/// WASD=前后左右 QE=升降 UI=俯仰 JK=偏航 OL=自转 左键=夹爪 空格=急停 Home=归零
/// </summary>
public class KeyboardMouseAdapter : IControlCommand
{
    public Vector3 GetMovement()
    {
        float fwd   = (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0);
        float up    = (Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? -1 : 0);
        float right = (Input.GetKey(KeyCode.D) ? 1 : 0) + (Input.GetKey(KeyCode.A) ? -1 : 0);
        return new Vector3(right, up, fwd);
    }

    public Vector3 GetRotation()
    {
        float pitch = (Input.GetKey(KeyCode.U) ? 1 : 0) + (Input.GetKey(KeyCode.I) ? -1 : 0);
        float yaw   = (Input.GetKey(KeyCode.J) ? 1 : 0) + (Input.GetKey(KeyCode.K) ? -1 : 0);
        float roll  = (Input.GetKey(KeyCode.O) ? 1 : 0) + (Input.GetKey(KeyCode.L) ? -1 : 0);
        return new Vector3(pitch, yaw, roll);
    }

    public Vector3 GetBaseMovement()
    {
        float fwd   = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
        float up    = (Input.GetKey(KeyCode.RightShift) ? 1 : 0) + (Input.GetKey(KeyCode.RightControl) ? -1 : 0);
        float right = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
        return new Vector3(right, up, fwd);
    }

    public float GetGripper() => Input.GetMouseButton(0) ? 1f : 0f;

    public bool GetEmergencyStop() => Input.GetKey(KeyCode.Space);
    public bool GetHome() => Input.GetKey(KeyCode.Home);
}
