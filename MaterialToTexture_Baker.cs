// MaterialToTexture_Baker.csC:\Users\DimaS\Desktop\FeebleSnowOriginal-master\Assets\Dima Serebrennikov\Nested shader execution example\MaterialToTexture_Baker.csMaterialToTexture_Baker.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
namespace Serebrennikov {
    public sealed class MaterialToTexture_Baker : MonoBehaviour {
        [SerializeField] Material _sourceMaterial;
        [SerializeField] int _resolution = 1024;
        const RenderTextureFormat Format = RenderTextureFormat.ARGB32;
#if UNITY_EDITOR
        public Texture2D BakeToTexture() {
            RenderTexture renderTarget = new(_resolution, _resolution, 0, Format) {
                useMipMap = false,
                autoGenerateMips = false,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            renderTarget.Create();
            Render(renderTarget, _sourceMaterial);
            Texture2D texture = Read(renderTarget);
            renderTarget.Release();
            DestroyImmediate(renderTarget);
            return texture;
        }
        static void Render(RenderTexture target, Material material) {
            CommandBuffer cmd = CommandBufferPool.Get("Material → Texture (Editor)");
            cmd.SetRenderTarget(target);
            cmd.ClearRenderTarget(false, true, Color.clear);
            CoreUtils.DrawFullScreen(cmd, material);
            Graphics.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        static Texture2D Read(RenderTexture source) {
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = source;
            Texture2D texture = new(
                source.width,
                source.height,
                TextureFormat.RGBA32,
                false,
                false
                );
            texture.ReadPixels(
                new Rect(0, 0, source.width, source.height),
                0,
                0
                );
            texture.Apply();
            RenderTexture.active = previous;
            return texture;
        }
#endif
    }
}
