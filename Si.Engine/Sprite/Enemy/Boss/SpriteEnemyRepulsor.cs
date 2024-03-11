﻿using Si.Engine.Loudout;
using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy.Boss
{
    /// <summary>
    /// 100% Experimental
    /// </summary>
    internal class SpriteEnemyRepulsor : SpriteEnemyBossBase
    {
        public const int hullHealth = 100;
        public const int bountyMultiplier = 15;

        private readonly SpriteAttachment _leftGun;
        private readonly SpriteAttachment _rightGun;
        private readonly SpriteAttachment _thrust;

        private readonly float _initialMaxpeed;
        private readonly string _assetPath = @"Graphics\Enemy\Boss\Repulsor\";

        public SpriteEnemyRepulsor(EngineCore engine)
            : base(engine)
        {
            _leftGun = Attach(_assetPath + "Gun.Left.png", true, 3);
            _rightGun = Attach(_assetPath + "Gun.Right.png", true, 3);
            _thrust = Attach(_assetPath + "Jet.png", true, 3);

            SetImage(_assetPath + "Hull.png");

            ShipClass = SiEnemyClass.Repulsor;

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Repulsor ←\n"
                       + "TODO: Add a description\n",
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 2500,
                    ShieldHealth = 3000,
                    Bounty = 100
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            _initialMaxpeed = Velocity.MaximumSpeed;
        }

        public override void VelocityChanged()
        {
            if (_thrust != null)
            {
                bool visibleThrust = Velocity.ForwardVelocity > 0;

                if (_thrust.IsDeadOrExploded == false)
                {
                    _thrust.Visable = visibleThrust;
                }
            }
        }

        public override void LocationChanged()
        {
            if (_leftGun != null && _rightGun != null)
            {
                if (_leftGun?.IsDeadOrExploded == false)
                {
                    var pointLeft = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(25, 25));
                    _leftGun.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _leftGun.Location += pointLeft;
                }

                if (_rightGun?.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(25, 25));
                    _rightGun.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _rightGun.Location += pointRight;
                }

                if (_thrust?.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle + SiPoint.DegreesToRadians(180), new SiPoint(35, 35));
                    _thrust.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _thrust.Location += pointRight;
                }
            }
        }

        #region Artificial Intelligence.

        private enum AIMode
        {
            Approaching,
            Tailing,
            MovingToFallback,
            MovingToApproach,
            LameDuck
        }

        private const float baseDistanceToKeep = 200;
        private float distanceToKeep = baseDistanceToKeep * (SiRandom.NextFloat() + 1);
        private const float baseFallbackDistance = 800;
        private float fallbackDistance;
        private SiAngle fallToAngleRadians;
        private AIMode mode = AIMode.Approaching;
        private int roundsToFireBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            base.ApplyIntelligence(epoch, displacementVector);

            float distanceToPlayer = SiPoint.DistanceTo(this, _engine.Player.Sprite);

            //We have no engines. :(
            if (_thrust?.IsDeadOrExploded == true)
            {
                mode = AIMode.LameDuck;
            }

            //If we get down to one engine, slowly cut the max thrust to half of what it originally was. If we lose both, reduce it to 1.
            if (_thrust?.IsDeadOrExploded == true)
            {
                Velocity.MaximumSpeed -= 0.5f;
                if (Velocity.MaximumSpeed < 1)
                {
                    Velocity.MaximumSpeed = 1;
                }
            }

            if (mode == AIMode.LameDuck)
            {
                if (distanceToPlayer > 2500)
                {
                    Explode();
                }

                //Keep pointing at the player.
                var deltaAngle = DeltaAngleDegrees(_engine.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }

                //Try to stay close.
                if (distanceToPlayer > 300)
                {
                    Velocity.ForwardVelocity += 0.05f;
                    if (Velocity.ForwardVelocity > 1)
                    {
                        Velocity.ForwardVelocity = 1;
                    }
                }
                else
                {
                    //Slow to a stop when close.
                    Velocity.ForwardVelocity -= 0.05f;
                    if (Velocity.ForwardVelocity < 0)
                    {
                        Velocity.ForwardVelocity = 0;
                    }
                }
            }
            else if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    PointAtAndGoto(_engine.Player.Sprite);
                }
                else
                {
                    mode = AIMode.Tailing;
                    roundsToFireBeforeTailing = 25;
                    hpRemainingBeforeTailing = HullHealth;
                }
            }

            if (mode == AIMode.Tailing)
            {
                PointAtAndGoto(_engine.Player.Sprite);

                //Stay on the players tail.
                if (distanceToPlayer > distanceToKeep + 300)
                {
                    Velocity.ForwardVelocity = 1;
                    mode = AIMode.Approaching;
                }
                else
                {
                    Velocity.ForwardVelocity -= 0.05f;
                    if (Velocity.ForwardVelocity < 0)
                    {
                        Velocity.ForwardVelocity = 0;
                    }
                }

                //We we get too close, do too much damage or they fire at us enough, they fall back and come in again
                if (distanceToPlayer < distanceToKeep / 2.0
                    || hpRemainingBeforeTailing - HullHealth > 2
                    || roundsToFireBeforeTailing <= 0)
                {
                    Velocity.ForwardVelocity = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngleRadians = Velocity.ForwardAngle + new SiAngle(180.0f + SiRandom.Between(0, 10)).Radians;
                    fallbackDistance = baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.ForwardAngle - fallToAngleRadians;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }

                if (distanceToPlayer > fallbackDistance)
                {
                    mode = AIMode.MovingToApproach;
                }
            }

            if (mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngleDegrees(_engine.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (SiRandom.NextFloat() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000 && (_rightGun?.IsDeadOrExploded == false || _leftGun?.IsDeadOrExploded == false))
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponDualVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}