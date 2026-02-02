// MaterialToTexture_BakerEditor.csC:\Users\DimaS\Desktop\FeebleSnowOriginal-master\Assets\Dima Serebrennikov\Nested shader execution example\MaterialToTexture_BakerEditor.csMaterialToTexture_BakerEditor.cs
using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Serebrennikov {
    [CustomEditor(typeof(MaterialToTexture_Baker))]
    public sealed class MaterialToTexture_BakerEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            EditorGUILayout.Space(8);
            MaterialToTexture_Baker baker = (MaterialToTexture_Baker)target;
            using (new EditorGUI.DisabledScope(!CanBake(baker))) {
                if (GUILayout.Button("Bake Texture", GUILayout.Height(32))) {
                    Bake(baker);
                }
            }
        }
        static bool CanBake(MaterialToTexture_Baker baker) {
            SerializedObject serialized = new(baker);
            SerializedProperty material = serialized.FindProperty("_sourceMaterial");
            return material.objectReferenceValue != null;
        }
        static void Bake(MaterialToTexture_Baker baker) {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Baked Texture",
                baker.name + "_Baked",
                "png",
                "Choose where to save the baked texture"
                );
            if (string.IsNullOrEmpty(path)) {
                return;
            }
            Texture2D texture = baker.BakeToTexture();
            byte[] data = texture.EncodeToPNG();
            File.WriteAllBytes(path, data);
            DestroyImmediate(texture);
            AssetDatabase.Refresh();
        }
    }
}

#endif