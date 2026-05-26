using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// VR手柄适配器 — 兼容Meta Quest/HTC Vive/Valve Index/Pico
/// 左摇杆=位移 右摇杆=偏航+升降 右扳机+右摇杆=俯仰+自转
/// </summary>
public class VRAdapter : IControlCommand
{
    InputDevice _left, _right;
    bool _inited;

    void EnsureDevices()
    {
        if (_inited && _left.isValid && _right.isValid) return;

        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
            devices);
        foreach (var d in devices)
        {
            if ((d.characteristics & InputDeviceCharacteristics.Left) != 0) _left = d;
            if ((d.characteristics & InputDeviceCharacteristics.Right) != 0) _right = d;
        }
        _inited = _left.isValid && _right.isValid;
    }

    public Vector3 GetMovement()
    {
        EnsureDevices();
        if (!_inited) return Vector3.zero;

        _right.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigger);
        _right.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightStick);

        // 扳机按住时由GetRotation接管右摇杆，此处不返回位移
        if (rightTrigger > 0.5f) return Vector3.zero;
        return new Vector3(rightStick.x, 0, rightStick.y); // (right, up, fwd)
    }

    public Vector3 GetRotation()
    {
        EnsureDevices();
        if (!_inited) return Vector3.zero;

        _right.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigger);
        _right.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightStick);

        if (rightTrigger > 0.5f) // 扳机按住 → 俯仰+自转
            return new Vector3(rightStick.y, 0, rightStick.x);
        else                     // 默认 → 偏航
            return new Vector3(0, rightStick.x, 0);
    }

    public Vector3 GetBaseMovement()
    {
        EnsureDevices();
        if (!_inited) return Vector3.zero;
        _left.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftStick);
        return new Vector3(leftStick.x, 0, leftStick.y); // (right, up, fwd)
    }

    public float GetGripper()
    {
        EnsureDevices();
        if (!_inited) return 0f;
        _left.TryGetFeatureValue(CommonUsages.trigger, out float val);
        return val > 0.3f ? val : 0f;
    }

    public bool GetEmergencyStop()
    {
        EnsureDevices();
        if (!_inited) return false;
        _left.TryGetFeatureValue(CommonUsages.gripButton, out bool gripL);
        _right.TryGetFeatureValue(CommonUsages.gripButton, out bool gripR);
        return gripL && gripR;
    }

    public bool GetHome()
    {
        EnsureDevices();
        if (!_inited) return false;
        _left.TryGetFeatureValue(CommonUsages.menuButton, out bool menu);
        if (!menu) _right.TryGetFeatureValue(CommonUsages.menuButton, out menu);
        return menu;
    }
}
