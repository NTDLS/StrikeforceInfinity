﻿using Si.Engine;
using Si.GameEngine.Sprite._Superclass;
using Si.GameEngine.Sprite.Weapon._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinkning this one out.
    /// </summary>
    internal class EnergyMunitionBase : MunitionBase
    {
        public EnergyMunitionBase(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}