﻿using HG.Engine;
using HG.Types;
using System.Drawing;
using System.IO;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletScramsMissile : BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\Missiles\BulletScramsMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public BulletScramsMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            _selectedHitExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));

        }

        public override void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngle(LockedTarget);

                    if (deltaAngle >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 10;
                    }
                    else if (deltaAngle < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 10;
                    }
                }
            }

            base.ApplyIntelligence(displacementVector);
        }
    }
}
