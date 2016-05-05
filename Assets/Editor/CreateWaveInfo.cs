using UnityEditor;
using UnityEngine;
using System.Collections;

public class CreateWaveInfo
{
    [MenuItem("Assets/Create/Wave Preset")]
    public static void CreateMyAsset()
    {
        WaveInfo asset = ScriptableObject.CreateInstance<WaveInfo>();

        AssetDatabase.CreateAsset(asset, "Assets/Wave Presets/UntitledWavePreset.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
