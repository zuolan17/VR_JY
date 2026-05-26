using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 输入管理器单例 — 检测VR头显自动切换键鼠/VR适配器
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public IControlCommand Adapter { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var hmd = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, hmd);
        bool hasVR = hmd.Count > 0 && hmd[0].isValid;

        Adapter = hasVR ? new VRAdapter() : new KeyboardMouseAdapter();
        Debug.Log($"输入管理器就绪：{Adapter.GetType().Name}");
    }
}
