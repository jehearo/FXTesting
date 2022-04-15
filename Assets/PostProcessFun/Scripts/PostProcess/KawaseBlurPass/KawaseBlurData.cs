using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable, VolumeComponentMenu("Post Process/Kawase Blur Feature")]
public class KawaseBlurData : VolumeComponent
{
    [Tooltip("Downsample Amount")]
    public ClampedIntParameter downsample = new ClampedIntParameter(1, 1,8);
    [Tooltip("Blur Passes")]
    public ClampedIntParameter passes = new ClampedIntParameter(1, 1,8);
    [Tooltip("Offset Amount")]
    public ClampedFloatParameter offset = new ClampedFloatParameter(0.0f, 0.0f, 2.0f);
}
