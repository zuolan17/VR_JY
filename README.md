# VRJY — Unity + ROS2 遥操作 VR 机器人控制系统

基于 **Unity + OpenXR** 的机器人遥操作方案，通过 **ROS-TCP-Connector** 与 ROS2 端通信，支持 **VR 手柄** 与 **键鼠** 双模式操控 Niryo One 六轴机械臂。

## 架构概览

```
┌─────────────────────┐         ROS2 TCP          ┌──────────────────┐
│   Unity (OpenXR)     │ ◄──────────────────────► │   ROS2 机器人端    │
│                      │                           │                  │
│  输入适配层            │    /target_pose ──────►  │  逆运动学解算      │
│  ├─ VRAdapter        │    /base_target_pose ──►  │  底座移动         │
│  └─ KeyboardMouse    │                           │                  │
│                      │    /joint_states ◄──────  │  关节状态发布      │
│  驱动层               │    /base_pose ◄─────────  │  底座位姿          │
│  ├─ ArmController    │    /scan ◄──────────────  │  激光雷达          │
│  ├─ BaseController   │    /camera/image_raw ◄───  │  相机图像          │
│  ├─ JointDriver      │                           │                  │
│  └─ BaseDriver       │                           │                  │
└─────────────────────┘                           └──────────────────┘
```

## 功能特性

- **6-DOF 机械臂末端位姿控制** — 发送目标位姿给 ROS2 端逆运动学解算
- **底座移动控制** — 独立控制移动底盘
- **关节状态实时回传** — 订阅 `/joint_states` 驱动 URDF 模型可视化
- **传感器数据接收** — 支持激光雷达 (`/scan`)、相机图像 (`/camera/image_raw`)
- **TF 广播维护** — 传感器静态 TF + 关节状态周期性更新
- **急停 & 归零** — 紧急情况立即停止，一键恢复初始位姿
- **输入自动切换** — 检测 VR 头显，自动切换 VR 手柄 / 键鼠操控

## 环境依赖

| 组件 | 版本 |
|------|------|
| Unity | 2022.3 LTS+ |
| Render Pipeline | URP |
| XR Plugin | XR Plugin Management + OpenXR |
| ROS2 | Humble / Iron |
| ROS-TCP-Connector | Unity Robotics Hub |
| 机器人模型 | Niryo One (URDF) |

## 输入操作

### 键鼠模式

| 操作 | 按键 |
|------|------|
| 末端前后/左右/升降 | WASD / QE |
| 末端俯仰/偏航/自转 | UI / JK / OL |
| 底座前后/左右/升降 | ↑↓←→ / RShift RCtrl |
| 夹爪 | 鼠标左键 |
| 急停 | Space |
| 归零 | Home |

### VR 模式（Meta Quest / HTC Vive / Pico）

| 操作 | 手柄按键 |
|------|---------|
| 末端前后/左右 | 右摇杆 |
| 末端偏航 | 右摇杆 X 轴 |
| 末端俯仰+自转 | 右扳机 + 右摇杆 |
| 底座移动 | 左摇杆 |
| 夹爪 | 左扳机 |
| 急停 | 左右握把键同时按下 |
| 归零 | 菜单键 |

## ROS2 话题

| 话题 | 方向 | 类型 | 说明 |
|------|------|------|------|
| /target_pose | 发布 -> | geometry_msgs/Pose | 机械臂末端目标位姿 |
| /base_target_pose | 发布 -> | geometry_msgs/PoseStamped | 底座目标位姿 |
| /joint_states | <- 订阅 | sensor_msgs/JointState | 关节角度反馈 |
| /base_pose | <- 订阅 | geometry_msgs/PoseStamped | 底座位姿反馈 |
| /scan | <- 订阅 | sensor_msgs/LaserScan | 激光雷达数据 |
| /camera/image_raw | <- 订阅 | sensor_msgs/Image | 相机图像 |
| /test_msg | <- 订阅 | std_msgs/String | 通信链路测试 |

## 项目结构

```
Assets/
├── Scripts/
│   ├── Bridge/              # ROS2 通信驱动层
│   │   ├── RosConnectionTest.cs    # 通信链路测试
│   │   ├── JointDriver.cs          # 关节状态 -> URDF 模型驱动
│   │   ├── BaseDriver.cs           # 底座位姿 -> Transform
│   │   └── SensorDataTest.cs       # 多传感器数据接收测试
│   └── Teleop/              # 遥操作控制层
│       ├── IControlCommand.cs      # 控制命令统一接口
│       ├── InputManager.cs         # 输入管理器（自动切换VR/键鼠）
│       ├── VRAdapter.cs            # VR 手柄适配器
│       ├── KeyboardMouseAdapter.cs # 键鼠适配器
│       ├── ArmController.cs        # 机械臂控制器
│       └── BaseController.cs       # 底座控制器
├── URDF/                    # Niryo One 机器人 URDF 模型
├── Editor/                  # 编辑器工具脚本
├── Prefabs/                 # 预制体
├── Scenes/                  # 场景文件
└── Settings/                # URP / XR 配置
```

## 快速开始

1. 确保 ROS2 端已启动 ros_tcp_endpoint，话题正常发布
2. 在 Unity 中配置 ROSConnection 的 ROS IP 地址和端口（Inspector 面板）
3. 打开场景，点击 Play
4. 如连接 VR 头显，自动使用 VR 操控；否则使用键鼠

## 版本记录

| 版本 | 内容 |
|------|------|
| v5 | 降低机械臂控制灵敏度 |
| v4 | 接入 XR Plugin Management + OpenXR |
| v3 | TF 广播完成 — 传感器静态 TF + 关节状态周期性维护 |
| v2 | 底座移动链路 |
| v1 | Unity <-> ROS2 遥操作链路打通 |
