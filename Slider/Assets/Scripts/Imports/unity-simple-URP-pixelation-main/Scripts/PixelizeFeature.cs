

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizeFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int screenHeight = 144;
        public bool enabled = false;
    }

    [SerializeField] public CustomPassSettings settings;
    private PixelizePass customPass;

    public override void Create()
    {
        customPass = new PixelizePass(settings);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        if(settings.enabled)
            renderer.EnqueuePass(customPass);
    }
}


