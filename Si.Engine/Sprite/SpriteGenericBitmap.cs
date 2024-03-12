﻿using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteGenericBitmap : SpriteBase
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

        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.Native;
        public SiAngle TravelAngle { get; set; } = new SiAngle();
        public ParticleCleanupMode CleanupMode { get; set; } = ParticleCleanupMode.None;
        public float RotationSpeed { get; set; } = 0;
        public SiRelativeDirection RotationDirection { get; set; } = SiRelativeDirection.None;


        public SpriteGenericBitmap(EngineCore engine, SiPoint location, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine)
        {
            Initialize(bitmap);
            Location = location.Clone();
            Velocity = new SiVelocity();

            RotationSpeed = SiRandom.Between(1, 100) / 20.0f;
            RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
            TravelAngle.Degrees = SiRandom.Between(0, 359);

            Velocity.ForwardVelocity = 100;
            Velocity.MaximumSpeed = SiRandom.Between(1.0f, 4.0f);
        }

        public SpriteGenericBitmap(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine)
        {
            Initialize(bitmap);
            Velocity = new SiVelocity();
        }

        public SpriteGenericBitmap(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImage(imagePath);
            Velocity = new SiVelocity();
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            if (RotationDirection == SiRelativeDirection.Right)
            {
                Velocity.ForwardAngle.Degrees += RotationSpeed;
            }
            else if (RotationDirection == SiRelativeDirection.Left)
            {
                Velocity.ForwardAngle.Degrees -= RotationSpeed;
            }

            if (VectorType == ParticleVectorType.Independent)
            {
                //We use a seperate angle for the travel direction because the base ApplyMotion()
                //  moves the object in the the direction of the Velocity.Angle.
                Location += TravelAngle * (Velocity.MaximumSpeed * Velocity.ForwardVelocity) * epoch;
            }
            else if (VectorType == ParticleVectorType.Native)
            {
                base.ApplyMotion(epoch, displacementVector);
            }

            if (CleanupMode == ParticleCleanupMode.FadeToBlack)
            {
                throw new NotImplementedException();
                /*
                Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                // Check if the particle color is below a certain threshold and remove it.
                if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                {
                    QueueForDelete();
                }
                */
            }
            else if (CleanupMode == ParticleCleanupMode.DistanceOffScreen)
            {
                if (_engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }
    }
}