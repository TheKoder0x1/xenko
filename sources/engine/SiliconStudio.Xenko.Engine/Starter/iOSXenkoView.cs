// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
#if SILICONSTUDIO_PLATFORM_IOS
using System;
using System.Drawing;
using System.Reflection;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using OpenGLES;
using OpenTK;
using OpenTK.Graphics.ES30;
using OpenTK.Platform.iPhoneOS;
using SiliconStudio.Xenko.Games;
using UIKit;

namespace SiliconStudio.Xenko.Starter
{
    // note: for more information on iOS application life cycle, 
    // see http://docs.xamarin.com/guides/cross-platform/application_fundamentals/backgrounding/part_1_introduction_to_backgrounding_in_ios
    [Register("iOSXenkoView")]
    public sealed class iOSXenkoView : iPhoneOSGameView, IAnimatedGameView
    {
        CADisplayLink displayLink;
        private bool isRunning;

        private uint depthRenderBuffer;
        private int renderBuffer;
        private int frameBuffer;

        public iOSXenkoView(System.Drawing.RectangleF frame)
            : base(frame)
        {
        }

        protected override void CreateFrameBuffer()
        {
            base.CreateFrameBuffer();

            renderBuffer = Renderbuffer;
            frameBuffer = Framebuffer;
        }

        public override void LayoutSubviews()
        {
            // ISSUE: reference to a compiler-generated method
            base.LayoutSubviews();
            if (GraphicsContext == null)
                return;
            var bounds = Bounds;
            if (Math.Round(bounds.Width) == Size.Width && Math.Round(bounds.Height) == Size.Height)
                return;

            if (!GraphicsContext.IsCurrent)
                MakeCurrent();

            Size = new Size((int)bounds.Width, (int)bounds.Height);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBuffer);
            if (!EAGLContext.RenderBufferStorage((uint)RenderbufferTarget.Renderbuffer, (CAEAGLLayer)Layer))
            {
                throw new InvalidOperationException("Error with RenderbufferStorage()!");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, frameBuffer);
        }

        [Export("layerClass")]
        public static Class LayerClass()
        {
            return GetLayerClass();
        }

        public void StartAnimating()
        {
            if (isRunning)
                return;

            CreateFrameBuffer();

            var link = UIScreen.MainScreen.CreateDisplayLink(this, new Selector("drawFrame"));
            link.FrameInterval = 0;
            link.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);
            displayLink = link;

            isRunning = true;
        }

        public void StopAnimating()
        {
            if (!isRunning)
                return;

            displayLink.Invalidate();
            displayLink = null;

            DestroyFrameBuffer();

            isRunning = false;
        }

        [Export("drawFrame")]
        void DrawFrame()
        {
            OnRenderFrame(new FrameEventArgs());
        }
    }
}
#endif
