﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites.Enemies.BasesAndInterfaces;
using StrikeforceInfinity.Game.Utility;
using System;
using System.Drawing;

namespace StrikeforceInfinity.Game.Sprites.Enemies.Peons.BasesAndInterfaces
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyPeonBase : SpriteEnemyBase
    {
        public SpriteAnimation ThrustAnimation { get; internal set; }
        public SpriteAnimation BoostAnimation { get; internal set; }

        public SpriteEnemyPeonBase(EngineCore gameCore, int hullHealth, int bountyMultiplier)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new SpriteAnimation.PlayMode()
            {
                Replay = HgAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrustAnimation = new SpriteAnimation(_gameCore, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode);
            ThrustAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(ThrustAnimation, this);

            BoostAnimation = new SpriteAnimation(_gameCore, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode);
            BoostAnimation.Reset();
            _gameCore.Sprites.Animations.InsertAt(BoostAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void PositionChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }
            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointRight = HgMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(20, 20));
                BoostAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                BoostAnimation.X = X + pointRight.X;
                BoostAnimation.Y = Y + pointRight.Y;
            }
        }

        private void EnemyBase_OnVisibilityChanged(SpriteBase sender)
        {
            if (ThrustAnimation != null)
            {
                ThrustAnimation.Visable = false;
            }
            if (BoostAnimation != null)
            {
                BoostAnimation.Visable = false;
            }
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            if (_gameCore.Multiplay.PlayMode == HgPlayMode.MutiPlayerClient)
            {
                return;
            }

            base.ApplyMotion(displacementVector);

            if (ThrustAnimation != null)
            {
                ThrustAnimation.Visable = Velocity.ThrottlePercentage > 0;
            }
            if (BoostAnimation != null)
            {
                BoostAnimation.Visable = Velocity.BoostPercentage > 0;
            }
        }

        public override void Cleanup()
        {
            ThrustAnimation?.QueueForDelete();
            BoostAnimation?.QueueForDelete();

            base.Cleanup();
        }
    }
}