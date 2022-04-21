using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 地形扫描后处理组件
/// </summary>
[VolumeComponentMenu("Custom Post-Process/TerrainScanner")]
public class TerrainScan_VolumComponent : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter _distance = new ClampedFloatParameter( 0.005f, 0f, 0.01f );

    public ClampedFloatParameter _widht = new ClampedFloatParameter( 0.0001f, 0.0001f, 0.0117f );

    public ColorParameter _color = new ColorParameter( Color.white );

    public bool IsActive () => active;
    public bool IsTileCompatible () => false;
}
