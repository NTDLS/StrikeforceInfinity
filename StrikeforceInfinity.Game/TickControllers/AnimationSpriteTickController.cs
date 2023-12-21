﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.TickControllers.BasesAndInterfaces;
using System.Drawing;

namespace StrikeforceInfinity.Game.Controller
{
    internal class AnimationSpriteTickController : SpriteTickControllerBase<SpriteAnimation>
    {
        public AnimationSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var animation in Visible())
            {
                animation.ApplyMotion(displacementVector);
                animation.AdvanceImage();
            }
        }

        /// <summary>
        /// Creates an animation on top of another sprite.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="defaultPosition"></param>
        public void InsertAt(SpriteAnimation animation, SpriteBase defaultPosition)
        {
            lock (SpriteManager.Collection)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = HgRotationMode.Rotate;
                SpriteManager.Collection.Add(animation);
            }
        }

        public SpriteAnimation Create(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, SpriteAnimation.PlayMode playMode = null)
        {
            lock (SpriteManager.Collection)
            {
                SpriteAnimation obj = new SpriteAnimation(GameCore, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
