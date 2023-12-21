﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Sprites.BasesAndInterfaces;

namespace StrikeforceInfinity.Game.Sprites.Player
{
    internal class SpriteSerpentPlayerDrone : SpriteSerpentPlayer, ISpriteDrone
    {
        public SpriteSerpentPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public void ApplyMultiPlayVector(SiSpriteVector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            ThrustAnimation.Visable = vector.ThrottlePercentage > 0;
            BoostAnimation.Visable = vector.BoostPercentage > 0;
        }

    }
}
