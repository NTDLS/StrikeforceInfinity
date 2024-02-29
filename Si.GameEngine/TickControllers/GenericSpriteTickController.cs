﻿using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.TickControllers
{
    /// <summary>
    /// These are just generic bitmap sprites.
    /// </summary>
    public class GenericSpriteTickController : SpriteTickControllerBase<SpriteGeneric>
    {
        public GenericSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
            }
        }

        public SpriteGeneric CreateAt(SpriteBase sprite, SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGeneric(GameEngine, sprite.Location, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteGeneric Create(SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGeneric(GameEngine, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
