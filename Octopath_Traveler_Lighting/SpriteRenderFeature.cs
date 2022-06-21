using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SpriteRender
{
    public class SpriteRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class SpriteRenderSetting
        {
            public RenderPassEvent _passEvent = RenderPassEvent.AfterRenderingTransparents;
            public Material _mat;
            public int matPassIdx = -1;

            public FilterMode _filterMode = FilterMode.Bilinear;
        }

        public class SpriteRenderPass : ScriptableRenderPass
        {
            public SpriteRenderPass ( RenderPassEvent passEvent, Material mat, int passInt, string tag )
            {
                
            }

            public override void Execute ( ScriptableRenderContext context, ref RenderingData renderingData )
            {
            }
        }

        public override void AddRenderPasses ( ScriptableRenderer renderer, ref RenderingData renderingData )
        {
        }

        public override void Create ()
        {
        }
    }

}
