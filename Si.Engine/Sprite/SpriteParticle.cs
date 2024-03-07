﻿using SharpDX;
using SharpDX.Direct2D1;
using Si.Engine;
using Si.GameEngine.Sprite._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite
{
    public class SpriteParticle : SpriteParticleBase
    {
        /// <summary>
        /// The max travel distance from the creation x,y before the sprite is automatically deleted.
        /// This is ignored unless the CleanupModeOption is Distance.
        /// </summary>
        public float MaxDistance { get; set; } = 1000;

        /// <summary>
        /// The amount of brightness to reduce the color by each time the particle is rendered.
        /// This is ignored unless the CleanupModeOption is FadeToBlack.
        /// This should be expressed as a number between 0-1 with 0 being no reduxtion per frame and 1 being 100% reduction per frame.
        /// </summary>
        public float FadeToBlackReductionAmount { get; set; } = 0.01f;

        public ParticleColorType ColorType { get; set; } = ParticleColorType.SingleColor;
        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.Native;
        public ParticleShape Shape { get; set; } = ParticleShape.FilledEllipse;
        public ParticleCleanupMode CleanupMode { get; set; } = ParticleCleanupMode.None;
        public float RotationSpeed { get; set; } = 0;
        public SiRelativeDirection RotationDirection { get; set; } = SiRelativeDirection.None;

        /// <summary>
        /// The color of the particle when ColorType == Color;
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// The color of the particle when ColorType == Graident;
        /// </summary>
        public Color4 GradientStartColor { get; set; }
        /// <summary>
        /// The color of the particle when ColorType == Graident;
        /// </summary>
        public Color4 GradientEndColor { get; set; }
        public SiAngle TravelAngle { get; set; } = new SiAngle();

        public SpriteParticle(EngineCore engine, SiPoint location, Size size, Color4? color = null)
            : base(engine)
        {
            Initialize(size);

            Location = location.Clone();

            Color = color ?? engine.Rendering.Materials.Colors.White;
            RotationSpeed = SiRandom.Between(1, 100) / 20.0f;
            RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
            TravelAngle.Degrees = SiRandom.Between(0, 359);

            Velocity.ForwardMomentium = 100;
            Velocity.MaximumSpeed = SiRandom.Between(1.0f, 4.0f);

            _engine = engine;
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            if (RotationDirection == SiRelativeDirection.Right)
            {
                Velocity.Angle.Degrees += RotationSpeed;
            }
            else if (RotationDirection == SiRelativeDirection.Left)
            {
                Velocity.Angle.Degrees -= RotationSpeed;
            }

            if (VectorType == ParticleVectorType.Independent)
            {
                //We use a seperate angle for the travel direction because the base ApplyMotion()
                //  moves the object in the the direction of the Velocity.Angle.
                Location += TravelAngle * (Velocity.MaximumSpeed * Velocity.ForwardMomentium) * epoch;
            }
            else if (VectorType == ParticleVectorType.Native)
            {
                base.ApplyMotion(epoch, displacementVector);
            }

            if (CleanupMode == ParticleCleanupMode.FadeToBlack)
            {
                if (ColorType == ParticleColorType.SingleColor)
                {
                    Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
                else if (ColorType == ParticleColorType.Graident)
                {
                    GradientStartColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.
                    GradientEndColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (GradientStartColor.Red < 0.5f && GradientStartColor.Green < 0.5f && GradientStartColor.Blue < 0.5f
                        || GradientEndColor.Red < 0.5f && GradientEndColor.Green < 0.5f && GradientEndColor.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
            }
            else if (CleanupMode == ParticleCleanupMode.DistanceOffScreen)
            {
                if (_engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visable)
            {
                switch (Shape)
                {
                    case ParticleShape.FilledEllipse:
                        if (ColorType == ParticleColorType.SingleColor)
                        {
                            _engine.Rendering.FillEllipseAt(renderTarget,
                                RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, (float)Velocity.Angle.Degrees);
                        }
                        else if (ColorType == ParticleColorType.Graident)
                        {
                            _engine.Rendering.FillEllipseAt(renderTarget, RenderLocation.X, RenderLocation.Y,
                                Size.Width, Size.Height, GradientStartColor, GradientEndColor, (float)Velocity.Angle.Degrees);
                        }
                        break;
                    case ParticleShape.HollowEllipse:
                        _engine.Rendering.HollowEllipseAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Velocity.Angle.Degrees);
                        break;
                    case ParticleShape.Triangle:
                        _engine.Rendering.HollowTriangleAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Velocity.Angle.Degrees);
                        break;
                }

                if (IsHighlighted)
                {
                    _engine.Rendering.DrawRectangleAt(renderTarget, RawRenderBounds, Velocity.Angle.Radians, _engine.Rendering.Materials.Colors.Red, 0, 1);
                }
            }
        }
    }
}