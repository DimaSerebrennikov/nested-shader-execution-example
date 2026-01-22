// MaterialToTexture.csC:\Users\DimaS\Desktop\FeebleSnowOriginal-master\Assets\Dima Serebrennikov\Debug shaders\MaterialToTexture.csMaterialToTexture.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
public sealed class MaterialToTexture : MonoBehaviour {
    [Header("Source")] [SerializeField] Material sourceMaterial;
    [Header("Target")] [SerializeField] Material targetMaterial;
    [SerializeField] string targetTextureProperty = "_BaseMap";
    [Header("Texture Settings")] [SerializeField] int resolution = 512;
    [SerializeField] RenderTextureFormat format = RenderTextureFormat.ARGB32;
    RenderTexture generatedTexture;
    void Awake() {
        if (sourceMaterial == null || targetMaterial == null) {
            return;
        }
        CreateTexture();
        Generate();
        targetMaterial.SetTexture(targetTextureProperty, generatedTexture);
    }
    void OnDestroy() {
        if (generatedTexture != null) {
            generatedTexture.Release();
            Destroy(generatedTexture);
        }
    }
    void CreateTexture() {
        generatedTexture = new RenderTexture(resolution, resolution, 0, format) {
            useMipMap = false,
            autoGenerateMips = false,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        generatedTexture.Create();
    }
    public void Generate() {
        CommandBuffer cmd = CommandBufferPool.Get("Material → Texture");
        cmd.SetRenderTarget(generatedTexture);
        cmd.ClearRenderTarget(false, true, Color.clear);
        CoreUtils.DrawFullScreen(cmd, sourceMaterial); /*Pipeline-safe fullscreen draw*/
        Graphics.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
