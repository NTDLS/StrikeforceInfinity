﻿using Si.Engine;
using Si.GameEngine.Sprite._Superclass;
using Si.GameEngine.Sprite.Weapon._Superclass;
using Si.GameEngine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.IO;

namespace Si.GameEngine.Sprite.Weapon.Munition
{
    internal class MunitionScattershot : ProjectileMunitionBase
    {
        private const string _assetPath = @"Graphics\Weapon\Scattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public MunitionScattershot(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, null, xyOffset)
        {
            selectedImageIndex = SiRandom.Between(0, imageCount - 1);
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}