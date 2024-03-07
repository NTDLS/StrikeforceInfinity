﻿using Si.Engine;
using Si.GameEngine.Sprite._Superclass;
using Si.GameEngine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite
{
    public class SpriteAttachment : SpriteShipBase
    {
        public bool TakesDamage { get; set; }

        public SpriteAttachment(EngineCore engine, string imagePath)
            : base(engine)
        {
            Initialize(imagePath);
            Velocity = new SiVelocity();
        }

        public override bool TryMunitionHit(MunitionBase munition, SiPoint hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    Hit(munition);
                    if (HullHealth <= 0)
                    {
                        Explode();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}