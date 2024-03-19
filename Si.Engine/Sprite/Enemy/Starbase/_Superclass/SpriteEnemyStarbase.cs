﻿using Si.Engine.Sprite.Enemy._Superclass;

namespace Si.Engine.Sprite.Enemy.Starbase._Superclass
{
    /// <summary>
    /// Base class for "Peon" enemies. These guys are basically all the same in theit functionality and animations.
    /// </summary>
    internal class SpriteEnemyStarbase : SpriteEnemyBase
    {
        public SpriteEnemyStarbase(EngineCore engine)
            : base(engine)
        {
            Velocity.ForwardVelocity = 1;
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
