﻿using Si.Engine;
using Si.GameEngine.Sprite._Superclass;
using Si.GameEngine.Sprite.Weapon._Superclass;
using Si.GameEngine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprite.Weapon.Munition
{
    internal class MunitionVulcanCannon : ProjectileMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\VulcanCannon.png";

        public MunitionVulcanCannon(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}