using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable, VolumeComponentMenu("Post Process/Game Boy Feature")]
public class GameBoyData : VolumeComponent
{
    [Tooltip("Bit Amount")]
    public ClampedIntParameter bit = new ClampedIntParameter(256, 8,1024);
    [Tooltip("Thickness Outline")]
    public ClampedFloatParameter thickness = new ClampedFloatParameter(1.0f, 0.1f, 4.0f);
    [Tooltip("Colour Outline")]
    public ColorParameter edgecolor = new ColorParameter(Color.black);
    [Tooltip("Thickness Outline")]
    public ClampedFloatParameter sobelStep = new ClampedFloatParameter(0.001f, 0.0000001f, 0.001f);
    [Tooltip("Add to Luma")]
    public ClampedFloatParameter lumaAdd = new ClampedFloatParameter(0.0f, 0.0f, 0.5f);
    [Tooltip("Enable Gameboy Colour")]
    public BoolParameter gameBoyFilter = new BoolParameter(false);
}
