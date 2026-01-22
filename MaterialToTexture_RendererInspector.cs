// MaterialToTexture_RendererInspector.csC:\Users\DimaS\Desktop\FeebleSnowOriginal-master\Assets\Dima Serebrennikov\Debug shaders\MaterialToTexture_RendererInspector.csMaterialToTexture_RendererInspector.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MaterialToTexture_Renderer))]
public class MaterialToTexture_RendererInspector : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        EditorGUILayout.Space(8);
        MaterialToTexture_Renderer targetScript = (MaterialToTexture_Renderer)target;
        using (new EditorGUI.DisabledScope(!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(target))) {
            if (GUILayout.Button("Generate Texture")) {
                Generate(targetScript);
            }
        }
    }
    static void Generate(MaterialToTexture_Renderer script) {
        if (script == null) return;
        Undo.RegisterCompleteObjectUndo(script, "Generate Material Texture");
        MethodInfo awake = script.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
        if (!Application.isPlaying && awake != null) {
            awake.Invoke(script, null);
        } else {
            script.Generate();
        }
        EditorUtility.SetDirty(script);
        SceneView.RepaintAll();
    }
}
