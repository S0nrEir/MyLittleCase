using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 地形扫描render feature
/// </summary>
//RenderFreature负责将render pass添加到render pass中，Render Freature可以在渲染管线的某个时间点增加一个或多个pass
public class TerrainScanRenderFeature : ScriptableRendererFeature
{
    #region render setting
    /// <summary>
    /// 渲染设置
    /// </summary>
    [System.Serializable]
    public class TerrainScanRenderSetting
    {
        public RenderPassEvent _passEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material _mat;
        public int _matPassIdx = -1;

        /// <summary>
        /// 过滤模式
        /// </summary>
        public FilterMode _filterMode = FilterMode.Bilinear;
    }
    #endregion
    //END CLASS

    #region TerrainScanRenderkPass
    /// <summary>
    /// 自定义的renderPass类则负责了实际的渲染工作，其有一个重要字段renderPassEvent决定了pass的执行时间点（继承自SRP类）
    /// </summary>
    public class TerrainScanRenderPass : ScriptableRenderPass
    {
        /// <summary>
        /// SRP中，可以在constructor指定renderEvent的执行时间点，默认是AfterRenderingOpaque，及不透明物体之后
        /// </summary>
        public TerrainScanRenderPass (RenderPassEvent passEvent, Material material, int passint, string tag)
        {
            _passMat = material;
            _passMatInt = passint;
            _passTag = tag;
            renderPassEvent = passEvent;
        }

        /// <summary>
        /// 等待接受render feature给过来的图
        /// </summary>
        public void Setup ( RenderTargetIdentifier source,TerrainScanRenderSetting setting)
        {
            _passSource = source;
            _setting = setting;
        }

        /// <summary>
        /// Configure于渲染过程执行前调用，可以在该过程配置render target,初始化状态，创建临时的渲染纹理
        /// </summary>
        public override void Configure ( CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor ) => base.Configure( cmd, cameraTextureDescriptor );

        /// <summary>
        /// Excute是SRP的核心函数，在这里定义pass的规则，基本相当于内置管线的OnRenderImage()函数，所以后处理，图像混合在这里做 
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Comp != null)
            {
                //设置pass参数值
                _passMat.SetFloat( "_Distance", Comp._distance.value );
                _passMat.SetFloat( "_Width", Comp._widht.value );
                _passMat.SetColor( "_ScanLineColor", Comp._color.value );
            }

            //从命令缓存池中获取一个GL命令
            //CommandBuffer类主要用于收集一系列GL指令，然后执行
            CommandBuffer cmd = CommandBufferPool.Get(_passTag); 
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            //申请一个临时图像
            cmd.GetTemporaryRT
                (
                    _passTemplateTex.id,
                    descriptor, 
                    _setting == null ? FilterMode.Bilinear : _setting._filterMode 
                );

            //将source输入到对应材质的pass里进行处理，并将处理结果保存在_passTemplateTex
            Blit( cmd, _passSource, _passTemplateTex.Identifier(), _passMat, _passMatInt );
            //然后把结果放回source
            Blit( cmd, _passTemplateTex.Identifier(), _passSource );
            //执行命令缓冲区的该命令
            context.ExecuteCommandBuffer( cmd );
            //释放该命令
            CommandBufferPool.Release( cmd );
            //释放临时图像
            cmd.ReleaseTemporaryRT( _passTemplateTex.id );
        }

        /// <summary>
        /// 该函数用于释放这次渲染流程创建的分配资源，它在完成渲染相机后调用
        /// </summary>
        /// <param name="cmd"></param>
        public override void FrameCleanup ( CommandBuffer cmd ) => base.FrameCleanup( cmd );

        /// <summary>
        /// tag名称
        /// </summary>
        private string _passTag;

        /// <summary>
        /// 临时计算图像
        /// </summary>
        private RenderTargetHandle _passTemplateTex;

        /// <summary>
        /// 源图像
        /// </summary>
        private RenderTargetIdentifier _passSource;

        private TerrainScanRenderSetting _setting;

        /// <summary>
        /// 过滤模式
        /// </summary>
        //public FilterMode _filterMode = FilterMode.Bilinear;

        public int _passMatInt = 0;

        public Material _passMat = null;

        private TerrainScan_VolumComponent _comp = null;

        /// <summary>
        /// volum组件
        /// </summary>
        private TerrainScan_VolumComponent Comp
        {
            get
            {
                if(_comp == null)
                    _comp = VolumeManager.instance.stack.GetComponent<TerrainScan_VolumComponent>();

                return _comp;
            }
        }
    }
    #endregion
    //END CLASS

    /// <summary>
    /// render pass，暂时不知道干啥的
    /// </summary>
    private TerrainScanRenderPass _renderPass = null;

    /// <summary>
    /// 渲染设置
    /// </summary>
    public TerrainScanRenderSetting _setting = new TerrainScanRenderSetting();

    /// <summary>
    /// AddRenderPass函数会在render中插入ScriptableRenderPass，该render对每个摄像机都设置一次
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        _renderPass.Setup( src ,_setting);
        renderer.EnqueuePass( _renderPass );
    }

    /// <summary>
    /// Create函数会初始化该freature的资源
    /// </summary>
    public override void Create()
    {
        int passInt = _setting == null || _setting._mat == null ? -1 : _setting._mat.passCount;
        //将renderPass的id限制在-1到材质的最大pass数
        _setting._matPassIdx = Mathf.Clamp( _setting._matPassIdx, -1, passInt );
        //实例化并传参，name就是tag
        _renderPass = new TerrainScanRenderPass( _setting._passEvent, _setting._mat, _setting._matPassIdx, name );
    }
}
