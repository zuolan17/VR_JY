using UnityEngine;
using UnityEditor;

/// <summary>
/// 编辑器脚本：批量给niryo_one各部件创建并分配材质
/// </summary>
public class AssignRobotMaterials : MonoBehaviour
{
    [MenuItem("Tools/Assign Niryo One Materials")]
    public static void Assign()
    {
        var root = GameObject.Find("niryo_one");
        if (root == null)
        {
            Debug.LogError("未找到 niryo_one");
            return;
        }

        // 材质颜色定义
        var colorMap = new System.Collections.Generic.Dictionary<string, Color>
        {
            { "base_link", new Color(0.18f, 0.18f, 0.18f) },       // 深灰底座
            { "shoulder_link", new Color(0.29f, 0.44f, 0.63f) },    // 蓝色
            { "arm_link", new Color(0.29f, 0.44f, 0.63f) },
            { "elbow_link", new Color(0.29f, 0.44f, 0.63f) },
            { "forearm_link", new Color(0.29f, 0.44f, 0.63f) },
            { "wrist_link", new Color(0.18f, 0.18f, 0.18f) },      // 深灰手腕
            { "hand_link", new Color(0.55f, 0.55f, 0.55f) },        // 浅灰连接
            { "tool_link", new Color(0.25f, 0.25f, 0.25f) },        // 深灰工具
            { "gripper_base", new Color(0.18f, 0.18f, 0.18f) },
            { "servo_head", new Color(0.55f, 0.55f, 0.55f) },
        };

        var matFolder = "Assets/URDF/niryo_one/Materials";
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        int count = 0;

        foreach (var r in renderers)
        {
            var parts = r.name.Split('_');
            var id = parts.Length >= 2 ? parts[0] + "_" + parts[1] : r.name;

            if (colorMap.TryGetValue(id, out var color) ||
                colorMap.TryGetValue(r.name, out color))
            {
                var matPath = $"{matFolder}/{id}.mat";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat == null)
                {
                    mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.color = color;
                    AssetDatabase.CreateAsset(mat, matPath);
                }
                r.sharedMaterial = mat;
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"已分配 {count} 个部件材质");
    }
}
