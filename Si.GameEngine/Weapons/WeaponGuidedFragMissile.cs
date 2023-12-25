﻿using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Utility;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Weapons
{
    internal class WeaponGuidedFragMissile : WeaponBase
    {
        static new string Name { get; } = "Guided Frag Missile";
        private const string soundPath = @"Sounds\Weapons\GuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponGuidedFragMissile(EngineCore gameCore, _SpriteShipBase owner)
            : base(gameCore, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponGuidedFragMissile(EngineCore gameCore)
            : base(gameCore, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 10;
            FireDelayMilliseconds = 1000;
            Speed = 12;
            SpeedVariancePercent = 0.10;

            CanLockOn = true;
            MinLockDistance = 100;
            MaxLockDistance = 600;
            MaxLocks = 2;
            MaxLockOnAngle = 40;
            ExplodesOnImpact = true;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionGuidedFragMissile(_gameCore, this, _owner, targetOfLock, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedOnObjects == null || LockedOnObjects.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(10, 10));
                        _gameCore.Sprites.Munitions.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new SiPoint(10, 10));
                        _gameCore.Sprites.Munitions.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var lockedOn in LockedOnObjects)
                    {
                        if (_toggle)
                        {
                            var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(10, 10));
                            _gameCore.Sprites.Munitions.CreateLocked(this, lockedOn, pointRight);
                        }
                        else
                        {
                            var pointLeft = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new SiPoint(10, 10));
                            _gameCore.Sprites.Munitions.CreateLocked(this, lockedOn, pointLeft);
                        }
                        _toggle = !_toggle;
                    }
                }


                return true;
            }
            return false;

        }
    }
}