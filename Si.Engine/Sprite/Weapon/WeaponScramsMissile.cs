﻿using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponScramsMissile : WeaponBase
    {
        static string Name { get; } = "Guided Scrams Missile";
        private const string soundPath = @"Sounds\Weapons\ScramsMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponScramsMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponScramsMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionGuidedFragMissile(_engine, this, Owner, lockedTarget, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                var basePosition = Owner.Location
                    + (Owner.Orientation + SiMath.RADIANS_90 * (_toggle ? 1 : -1)).PointFromAngleAtDistance(new SiVector(10, 10));

                _toggle = !_toggle;

                if (LockedTargets?.Count > 0)
                {
                    foreach (var weaponLock in LockedTargets.Where(o => o.LockType == Library.SiConstants.SiWeaponsLockType.Hard))
                    {
                        _engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, basePosition);
                    }
                }
                else
                {
                    _engine.Sprites.Munitions.Add(this, basePosition);
                }

                return true;
            }
            return false;

        }
    }
}
