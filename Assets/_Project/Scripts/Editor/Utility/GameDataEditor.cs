using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class GameDataEditor : OdinMenuEditorWindow
{
    [MenuItem("Monster Farm/Open Data Manager", priority = -1)]
    private static void OpenWindow()
    {
        GetWindow<GameDataEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Config.DrawSearchToolbar = true;
        tree.AddAllAssetsAtPath("World", "Assets/_Project/ScriptableObjects/World", typeof(ScriptableObject));
        tree.AddAllAssetsAtPath("Player", "Assets/_Project/ScriptableObjects/Player", typeof(ScriptableObject), true);
        tree.AddAllAssetsAtPath("Enemy", "Assets/_Project/ScriptableObjects/Enemy", typeof(ScriptableObject), true);

        return tree;
    }

    protected override void DrawMenu()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(25);

        if (GUILayout.Button("Reset Game Data", GUILayout.Height(40), GUILayout.Width(130)))
        {
            ResetGameData();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        base.DrawMenu();
    }

    private void ResetGameData()
    {
        var playerStatsGuid = AssetDatabase.FindAssets($"t:{typeof(PlayerStatsSO)}", null)[0];
        var worldStatsGuid = AssetDatabase.FindAssets($"t:{typeof(WorldStatsSO)}", null)[0];
        var playerStats =
            AssetDatabase.LoadAssetAtPath<PlayerStatsSO>(AssetDatabase.GUIDToAssetPath(playerStatsGuid));
        var worldStats = AssetDatabase.LoadAssetAtPath<WorldStatsSO>(AssetDatabase.GUIDToAssetPath(worldStatsGuid));

        playerStats.Reset();
        worldStats.Reset();
    }
}