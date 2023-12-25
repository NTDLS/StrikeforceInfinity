﻿using Si.GameEngine.Engine;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprites
{
    public class SpriteDebug : _SpriteShipBase
    {
        public SpriteDebug(EngineCore gameCore)
            : base(gameCore)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new SiVelocity();
        }

        public SpriteDebug(EngineCore gameCore, double x, double y)
            : base(gameCore)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public SpriteDebug(EngineCore gameCore, double x, double y, string imagePath)
            : base(gameCore)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo360(_gameCore.Player.Sprite);
            base.ApplyMotion(displacementVector);
        }
    }
}