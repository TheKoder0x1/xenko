﻿// <auto-generated>
// Do not edit this file yourself!
//
// This code was generated by Xenko Shader Mixin Code Generator.
// To generate it yourself, please install SiliconStudio.Xenko.VisualStudio.Package .vsix
// and re-save the associated .xkfx.
// </auto-generated>

using System;
using SiliconStudio.Core;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Shaders;
using SiliconStudio.Core.Mathematics;
using Buffer = SiliconStudio.Xenko.Graphics.Buffer;

namespace SiliconStudio.Xenko.Rendering
{
    public static partial class MaterialFrontBackBlendShaderKeys
    {
        public static readonly ValueParameterKey<Color3> ColorFront = ParameterKeys.NewValue<Color3>();
        public static readonly ValueParameterKey<float> ColorBlend = ParameterKeys.NewValue<float>();
        public static readonly ValueParameterKey<Color3> ColorBack = ParameterKeys.NewValue<Color3>();
        public static readonly ValueParameterKey<float> AlphaBlend = ParameterKeys.NewValue<float>();
    }
}
