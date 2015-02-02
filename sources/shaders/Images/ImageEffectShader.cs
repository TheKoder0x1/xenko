﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;

using SiliconStudio.Core;
using SiliconStudio.Paradox.Graphics;

namespace SiliconStudio.Paradox.Effects.Images
{
    /// <summary>
    /// Post effect using an <see cref="Effect"/> (either pdxfx or pdxsl).
    /// </summary>
    public class ImageEffectShader : ImageEffect
    {
        /// <summary>
        /// The current effect instance.
        /// </summary>
        protected readonly DefaultEffectInstance EffectInstance;

        private readonly DynamicEffectCompiler effectCompiler;

        private readonly List<ParameterCollection> parameterCollections;

        private readonly List<ParameterCollection> appliedParameterCollections;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEffectShader" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="effectName">Name of the shader.</param>
        /// <exception cref="System.ArgumentNullException">effectName</exception>
        public ImageEffectShader(DrawEffectContext context, string effectName)
            : this(context, effectName, (ParameterCollection[])null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEffectShader" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="effectName">Name of the shader.</param>
        /// <param name="sharedParameterCollections">The shared parameters.</param>
        /// <exception cref="System.ArgumentNullException">effectName</exception>
        public ImageEffectShader(DrawEffectContext context, string effectName, params ParameterCollection[] sharedParameterCollections)
            : base(context)
        {
            if (effectName == null) throw new ArgumentNullException("effectName");

            parameterCollections = new List<ParameterCollection> { context.Parameters };
            if (sharedParameterCollections != null)
            {
                parameterCollections.AddRange(sharedParameterCollections);
            }
            parameterCollections.Add(Parameters);
            appliedParameterCollections = new List<ParameterCollection>();

            // Setup the effect compiler
            EffectInstance = new DefaultEffectInstance(parameterCollections);
            effectCompiler = new DynamicEffectCompiler(context.Services, effectName);

            SetDefaultParameters();
        }

        /// <summary>
        /// Gets the parameter collections used by this effect.
        /// </summary>
        /// <value>The parameter collections.</value>
        public List<ParameterCollection> ParameterCollections
        {
            get
            {
                return parameterCollections;
            }
        }

        /// <summary>
        /// Sets the default parameters (called at constructor time and if <see cref="Reset"/> is called)
        /// </summary>
        protected override void SetDefaultParameters()
        {
            Parameters.Set(TexturingKeys.Sampler, GraphicsDevice.SamplerStates.LinearClamp);
        }

        protected override void PreDrawCore(string name)
        {
            base.PreDrawCore(name);

            // Default handler for parameters
            UpdateParameters();
        }

        /// <summary>
        /// Updates the effect <see cref="ImageEffectShader.Parameters" /> from properties defined in this instance. See remarks.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Expecting less than 10 textures in input</exception>
        /// <remarks>By default, all the input textures will be remapped to <see cref="TexturingKeys.Texture0" />...etc.</remarks>
        protected virtual void UpdateParameters()
        {
            // By default, we are copying all input textures to TexturingKeys.Texture#
            var count = InputCount;
            for (int i = 0; i < count; i++)
            {
                var texture = GetInput(i);
                if (i < TexturingKeys.DefaultTextures.Count)
                {
                    var texturingKeys = texture.Dimension == TextureDimension.TextureCube ? TexturingKeys.TextureCubes : TexturingKeys.DefaultTextures;
                    Parameters.Set(texturingKeys[i], texture);
                }
                else
                {
                    throw new InvalidOperationException("Expecting less than {0} textures in input".ToFormat(TexturingKeys.DefaultTextures.Count));
                }
            }
        }

        protected void UpdateEffect()
        {
            // Dynamically update/compile the effect based on the current parameters.
            effectCompiler.Update(EffectInstance, null);
        }

        protected override void DrawCore(ParameterCollection contextParameters)
        {
            UpdateEffect();

            // Update parameters
            appliedParameterCollections.Clear();
            if (contextParameters != null)
            {
                appliedParameterCollections.Add(contextParameters);
            }
            appliedParameterCollections.AddRange(parameterCollections);

            // Draw a full screen quad
            GraphicsDevice.DrawQuad(EffectInstance.Effect, appliedParameterCollections);
        }
    }
}