﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;

namespace NebulaSiege.Weapons.Bullets
{
    internal class BulletPhotonTorpedo : _BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\BulletPhotonTorpedo.png";

        public BulletPhotonTorpedo(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
