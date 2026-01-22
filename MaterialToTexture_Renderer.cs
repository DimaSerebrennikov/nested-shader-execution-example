// MaterialToTexture_Renderer.csC:\Users\DimaS\Desktop\FeebleSnowOriginal-master\Assets\Dima Serebrennikov\Debug shaders\MaterialToTexture_Renderer.csMaterialToTexture_Renderer.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
class MaterialToTexture_Renderer : MonoBehaviour {
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Material _targetToGenerate;
    [SerializeField] string _textureName;
    RenderTexture generatedTexture;
    [SerializeField] int resolution = 1024;
    const RenderTextureFormat format = RenderTextureFormat.ARGB32;
    void Awake() {
        CreateTexture();
        Generate();
        Material targetMaterial = Application.isPlaying ? _meshRenderer.material : _meshRenderer.sharedMaterial;
        targetMaterial.SetTexture(_textureName, generatedTexture);
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
        CoreUtils.DrawFullScreen(cmd, _targetToGenerate);
        Graphics.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
