﻿using Si.Engine;
using Si.GameEngine.Sprite._Superclass;
using Si.GameEngine.Sprite.Weapon._Superclass;
using Si.GameEngine.Sprite.Weapon.Munition;
using Si.GameEngine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprite.Weapon
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        static new string Name { get; } = "Photon Torpedo";
        private const string soundPath = @"Sounds\Weapons\PhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPhotonTorpedo(EngineCore engine, SpriteShipBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPhotonTorpedo(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 25;
            FireDelayMilliseconds = 1000;
            Speed = 18.75f;
            AngleVarianceDegrees = 0.00f;
            SpeedVariancePercent = 0.00f;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionPhotonTorpedo(_engine, this, _owner, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _engine.Rendering.AddScreenShake(4, 100);
                _fireSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(_owner.Velocity.Angle + SiPoint.DEG_90_RADS, new SiPoint(10, 10));
                    _engine.Sprites.Munitions.Create(this, pointRight);
                }
                else
                {
                    var pointLeft = SiPoint.PointFromAngleAtDistance360(_owner.Velocity.Angle - SiPoint.DEG_90_RADS, new SiPoint(10, 10));
                    _engine.Sprites.Munitions.Create(this, pointLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}