﻿using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.Library.Mathematics.Geometry;
using System;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyPeonBase : SpriteEnemyBase
    {
        public SpriteAnimation ThrustAnimation { get; internal set; }
        public SpriteAnimation BoostAnimation { get; internal set; }

        public SpriteEnemyPeonBase(GameEngineCore gameEngine, int hullHealth, int bountyMultiplier)
            : base(gameEngine, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            OnVisibilityChanged += EnemyBase_OnVisibilityChanged;

            var playMode = new SpriteAnimation.PlayMode()
            {
                Replay = SiAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            ThrustAnimation = new SpriteAnimation(_gameEngine, @"Graphics\Animation\ThrustStandard32x32.png", new Size(32, 32), 10, playMode)
            {
                OwnerUID = UID
            };
            ThrustAnimation.Reset();
            _gameEngine.Sprites.Animations.AddAt(ThrustAnimation, this);

            BoostAnimation = new SpriteAnimation(_gameEngine, @"Graphics\Animation\ThrustBoost32x32.png", new Size(32, 32), 10, playMode)
            {
                OwnerUID = UID
            };
            BoostAnimation.Reset();
            _gameEngine.Sprites.Animations.AddAt(BoostAnimation, this);

            UpdateThrustAnimationPositions();
        }

        public override void LocationChanged() => UpdateThrustAnimationPositions();

        private void UpdateThrustAnimationPositions()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointBehind = SiVector.PointFromAngleAtDistance360(Velocity.Angle + SiVector.DegreesToRadians(180), new SiVector(20, 20));
                ThrustAnimation.Velocity.Angle = Velocity.Angle;
                ThrustAnimation.Location = Location + pointBehind;
            }
            if (BoostAnimation != null && BoostAnimation.Visable)
            {
                var pointBehind = SiVector.PointFromAngleAtDistance360(Velocity.Angle + SiVector.DegreesToRadians(180), new SiVector(20, 20));
                BoostAnimation.Velocity.Angle = Velocity.Angle;
                BoostAnimation.Location = Location + pointBehind;
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

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            base.ApplyMotion(epoch, displacementVector);

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
