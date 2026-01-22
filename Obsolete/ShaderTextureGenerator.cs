using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
public sealed class ShaderTextureGenerator : MonoBehaviour {
    [SerializeField] Shader generatorShader;
    [SerializeField] Material targetMaterial;
    [SerializeField] string targetTextureProperty = "_BaseMap";
    [SerializeField] int resolution = 512;
    RenderTexture generatedTexture;
    Material generatorMaterial;
    void Awake() {
        if (generatorShader == null || targetMaterial == null) {
            return;
        }
        generatedTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32) {
            useMipMap = false,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        generatedTexture.Create();
        generatorMaterial = new Material(generatorShader);
        Generate();
        targetMaterial.SetTexture(targetTextureProperty, generatedTexture);
    }
    void OnDestroy() {
        if (generatedTexture != null) {
            generatedTexture.Release();
        }
        Destroy(generatorMaterial);
    }
    void Generate() {
        CommandBuffer cmd = CommandBufferPool.Get("Generate Texture");
        cmd.SetRenderTarget(generatedTexture);
        cmd.ClearRenderTarget(false, true, Color.clear);

        // Valid ONLY if shader is procedural (no _MainTex sampling)
        cmd.Blit(Texture2D.blackTexture, generatedTexture, generatorMaterial);
        Graphics.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
