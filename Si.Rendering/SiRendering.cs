﻿using NTDLS.Semaphore;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using Si.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Si.Rendering
{
    public class SiRendering : IDisposable
    {
        private struct ScreenShake
        {
            public float Intensity = 0;
            public double Duration = 0;
            public Stopwatch Timer = new();
            public ScreenShake() { }
        }

        public PessimisticCriticalResource<SiCriticalRenderTargets> RenderTargets { get; private set; } = new();
        public SiPrecreatedMaterials Materials { get; private set; }
        public SiPrecreatedTextFormats TextFormats { get; private set; }

        private readonly List<ScreenShake> _screenShakes = new();
        private readonly SharpDX.Direct2D1.Factory _direct2dFactory = new(FactoryType.SingleThreaded);
        private readonly SharpDX.DirectWrite.Factory _directWriteFactory = new();
        private readonly ImagingFactory _wicFactory = new();
        private Size _totalCanvasSize;
        private Size _drawingSurfaceSize;

        public SiRendering(SiEngineSettings settings, Control drawingSurface, Size totalCanvasSize)
        {
            _drawingSurfaceSize = drawingSurface.Size;
            _totalCanvasSize = totalCanvasSize;

            var presentOptions = PresentOptions.Immediately;
            var antialiasMode = AntialiasMode.Aliased;

            if (settings.VerticalSync == true)
            {
                presentOptions = PresentOptions.None;
            }

            if (settings.AntiAliasing == true)
            {
                antialiasMode = AntialiasMode.PerPrimitive;
            }

            var windowRenderProperties = new HwndRenderTargetProperties()
            {
                PresentOptions = presentOptions,
                Hwnd = drawingSurface.Handle,
                PixelSize = new Size2(_drawingSurfaceSize.Width, _drawingSurfaceSize.Height)
                //PixelSize = new Size2(engine.Display.NatrualScreenSize.Width, engine.Display.NatrualScreenSize.Height)
            };

            var renderTargetProperties = new RenderTargetProperties
            {
                PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                //MinLevel = FeatureLevel.Level_10,
                Type = RenderTargetType.Hardware
            };

            //The intermediate render target is much larger than the redner target window. We create this
            //  larger render target so that we can zoom-out when we want to see more of the universe.
            var intermediateRenderTargetSize = new Size2F(_totalCanvasSize.Width, _totalCanvasSize.Height);

            var renderTargets = new SiCriticalRenderTargets()
            {
                ScreenRenderTarget = new WindowRenderTarget(_direct2dFactory, renderTargetProperties, windowRenderProperties)
                {
                    AntialiasMode = antialiasMode
                }
            };

            renderTargets.IntermediateRenderTarget = new BitmapRenderTarget(
                renderTargets.ScreenRenderTarget, CompatibleRenderTargetOptions.None, intermediateRenderTargetSize)
            {
                AntialiasMode = antialiasMode
            };

            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget = renderTargets.ScreenRenderTarget;
                o.IntermediateRenderTarget = renderTargets.IntermediateRenderTarget;
            });

            Materials = new SiPrecreatedMaterials(renderTargets.ScreenRenderTarget);
            TextFormats = new SiPrecreatedTextFormats(_directWriteFactory);

            SiTransforms.RegisterRenderTarget(renderTargets.ScreenRenderTarget);
            SiTransforms.RegisterRenderTarget(renderTargets.IntermediateRenderTarget);
        }

        public void Dispose()
        {
            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget?.Dispose();
                o.ScreenRenderTarget?.Dispose();
            });
        }

        public void AddScreenShake(float intensity, float duration)
        {
            var screenShake = new ScreenShake
            {
                Intensity = intensity,
                Duration = duration,
            };

            screenShake.Timer.Start();
            _screenShakes.Add(screenShake);
        }

        public void TransferWithZoom(BitmapRenderTarget intermediateRenderTarget, RenderTarget screenRenderTarget, float scale)
        {
            var sourceRect = SiRenderingUtility.CalculateCenterCopyRectangle(intermediateRenderTarget.Size, scale);
            var destRect = new RawRectangleF(0, 0, _drawingSurfaceSize.Width, _drawingSurfaceSize.Height);

            var appliedScreenShakes = new List<ScreenShake>();

            foreach (var screenShake in _screenShakes)
            {
                var totalElapsedScreenshakeTime = ((double)screenShake.Timer.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0;

                var percentComplete = (float)(totalElapsedScreenshakeTime / screenShake.Duration);
                if (percentComplete >= 1)
                {
                    screenShake.Timer.Stop();
                }

                var intensity = screenShake.Intensity * (1 - percentComplete);

                var offsetX = (float)(SiRandom.NextFloat() * intensity * 2 - intensity);
                var offsetY = (float)(SiRandom.NextFloat() * intensity * 2 - intensity);

                SiTransforms.PushTransform(screenRenderTarget, Matrix3x2.Translation(offsetX, offsetY));

                appliedScreenShakes.Add(screenShake);
            }

            screenRenderTarget.DrawBitmap(intermediateRenderTarget.Bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);

            foreach (var screenShake in appliedScreenShakes)
            {
                if (screenShake.Timer.IsRunning == false)
                {
                    _screenShakes.Remove(screenShake);
                }

                SiTransforms.PopTransform(screenRenderTarget);
            }
        }

        public SharpDX.Direct2D1.Bitmap BitmapStreamToD2DBitmap(Stream stream)
        {
            using var decoder = new BitmapDecoder(_wicFactory, stream, DecodeOptions.CacheOnLoad);
            using var frame = decoder.GetFrame(0);
            using var converter = new FormatConverter(_wicFactory);

            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA);

            return RenderTargets.Use(o => SharpDX.Direct2D1.Bitmap.FromWicBitmap(o.ScreenRenderTarget, converter));
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public void DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angleRadians)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF(x, y, (x + bitmap.PixelSize.Width), (y + bitmap.PixelSize.Height));
            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            SiTransforms.PopTransform(renderTarget);
        }

        /// Draws a bitmap from a specified location of a given size, to the the specified location.
        public void DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap,
            float x, float y, float angleRadians, RawRectangleF sourceRect, Size2F destSize)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF(x, y, (x + destSize.Width), (y + destSize.Height));
            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
            SiTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public void DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y)
        {
            var destRect = new RawRectangleF(x, y, (x + bitmap.PixelSize.Width), (y + bitmap.PixelSize.Height));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        }

        public RawRectangleF GetTextRect(float x, float y, string text, SharpDX.DirectWrite.TextFormat format)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new RawRectangleF(x, y, (x + textLayout.Metrics.Width), (y + textLayout.Metrics.Height));
        }

        public SizeF GetTextSize(string text, SharpDX.DirectWrite.TextFormat format)
        {
            //We have to check the size with some ending characters becuase TextLayout() seems to want to trim the text before calculating the metrics.
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, $"[{text}]", format, float.MaxValue, float.MaxValue);
            using var spacerLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, "[]", format, float.MaxValue, float.MaxValue);
            return new SizeF(textLayout.Metrics.Width - spacerLayout.Metrics.Width, textLayout.Metrics.Height);
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public void DrawTextAt(RenderTarget renderTarget,
            float x, float y, float angleRadians, string text, SharpDX.DirectWrite.TextFormat format, SolidColorBrush brush)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var destRect = new RawRectangleF(x, y, (x + textWidth), (y + textHeight));

            //DrawRectangleAt(renderTarget, textRectangle, 0, Materials.Raw.Blue, 0, 1);

            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawText(text, format, destRect, brush);
            SiTransforms.PopTransform(renderTarget);
        }

        public void DrawLine(RenderTarget renderTarget,
            float startPointX, float startPointY, float endPointX, float endPointY,
            SolidColorBrush brush, float strokeWidth = 1)
        {
            var startPoint = new RawVector2(startPointX, startPointY);
            var endPoint = new RawVector2(endPointX, endPointY);

            renderTarget.DrawLine(startPoint, endPoint, brush, strokeWidth);
        }

        /// <summary>
        /// Draws a color filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void FillEllipseAt(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 color, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));
            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.FillEllipse(ellipse, brush);

            SiTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a color gradient filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void FillEllipseAt(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 startColor, Color4 endColor, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));
            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));

            // Define gradient stops
            using var gradientStops = new GradientStopCollection(renderTarget, new GradientStop[]
            {
                new GradientStop() { Position = 0.0f, Color = startColor },
                new GradientStop() { Position = 1.0f, Color = endColor }
            });

            // Create linear gradient brush
            using var linearGradientBrush = new LinearGradientBrush(renderTarget,
                new LinearGradientBrushProperties()
                {
                    StartPoint = new RawVector2(x - radiusX, y),
                    EndPoint = new RawVector2(x + radiusX, y)
                }, gradientStops);

            // Fill ellipse with gradient brush
            renderTarget.FillEllipse(ellipse, linearGradientBrush);

            SiTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a hollow ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void HollowEllipseAt(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));

            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawEllipse(ellipse, brush, strokeWidth);

            SiTransforms.PopTransform(renderTarget);
        }

        public void HollowTriangleAt(RenderTarget renderTarget, float x, float y,
            float height, float width, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            // Define the points for the triangle
            var trianglePoints = new RawVector2[]
            {
                new RawVector2(0, height),           // Vertex 1 (bottom-left)
                new RawVector2(width, height), // Vertex 2 (bottom-right)
                new RawVector2((width / 2.0f), 0)      // Vertex 3 (top-center)
            };

            // Create a PathGeometry and add the triangle to it
            var triangleGeometry = new PathGeometry(_direct2dFactory);
            using (GeometrySink sink = triangleGeometry.Open())
            {
                sink.BeginFigure(trianglePoints[0], FigureBegin.Filled);
                sink.AddLines(trianglePoints);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            // Calculate the center of the triangle
            float centerX = (trianglePoints[0].X + trianglePoints[1].X + trianglePoints[2].X) / 3;
            float centerY = (trianglePoints[0].Y + trianglePoints[1].Y + trianglePoints[2].Y) / 3;

            // Calculate the adjustment needed to center the triangle at the desired position
            x -= centerX;
            y -= centerY;

            // Create a translation transform to move the triangle to the desired position
            var destRect = new RawRectangleF(x, y, (x + width), (y + height));

            SiTransforms.PushTransform(renderTarget,
                Matrix3x2.Multiply(SiTransforms.CreateOffsetTransform(x, y), SiTransforms.CreateAngleTransform(destRect, angleRadians)));

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawGeometry(triangleGeometry, brush, strokeWidth);

            SiTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangleAt(RenderTarget renderTarget, RawRectangleF destRect,
            float angleRadians, RawColor4 color, float expand = 0, float strokeWidth = 1)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            SiTransforms.PushTransform(renderTarget, SiTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawRectangle(destRect, brush, strokeWidth);
            SiTransforms.PopTransform(renderTarget);

            return destRect;
        }

        public List<SharpDX.Direct2D1.Bitmap> GenerateIrregularFragments(SharpDX.Direct2D1.Bitmap originalBitmap, int countOfFragments, int countOfVertices = 3)
            => SiBitmapFragmenter.GenerateIrregularFragments(this, originalBitmap, countOfFragments, countOfVertices);

        public RawMatrix3x2 GetScalingMatrix(float zoomFactor)
        {
            // Calculate the new center point (assuming dimensions are known)
            float centerX = _totalCanvasSize.Width / 2.0f;
            float centerY = _totalCanvasSize.Height / 2.0f;

            // Calculate the scaling transformation matrix
            var scalingMatrix = new RawMatrix3x2(
                zoomFactor, 0,
                0, zoomFactor,
                (centerX * (1.0f - zoomFactor)),
                (centerY * (1.0f - zoomFactor))
            );

            return scalingMatrix;
        }
    }
}