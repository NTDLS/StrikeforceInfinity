﻿using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.TickHandlers.Interfaces;
using HG.Utility.ExtensionMethods;

namespace HG.TickHandlers
{
    internal class PlayerTickHandler : IVectorGeneratorTickManager
    {
        public ActorPlayer Actor { get; set; }
        private readonly EngineCore _core;

        public PlayerTickHandler(EngineCore core)
        {
            _core = core;
        }

        private bool _allowLockPlayerAngleToNearbyEnemy = true;

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public HgPoint ExecuteWorldClockTick()
        {
            var displacementVector = new HgPoint();

            if (Actor.Visable)
            {
                if (_core.Input.IsKeyPressed(HgPlayerKey.PrimaryFire))
                {
                    if (Actor.PrimaryWeapon != null && Actor.PrimaryWeapon.Fire())
                    {
                        if (Actor.PrimaryWeapon?.RoundQuantity == 25)
                        {
                            Actor.AmmoLowSound.Play();
                        }
                        if (Actor.PrimaryWeapon?.RoundQuantity == 0)
                        {
                            Actor.AmmoEmptySound.Play();
                        }
                    }
                }

                if (_core.Input.IsKeyPressed(HgPlayerKey.SecondaryFire))
                {
                    if (Actor.SelectedSecondaryWeapon != null && Actor.SelectedSecondaryWeapon.Fire())
                    {
                        if (Actor.SelectedSecondaryWeapon?.RoundQuantity == 25)
                        {
                            Actor.AmmoLowSound.Play();
                        }
                        if (Actor.SelectedSecondaryWeapon?.RoundQuantity == 0)
                        {
                            Actor.AmmoEmptySound.Play();
                            Actor.SelectFirstAvailableUsableSecondaryWeapon();
                        }
                    }
                }

                if (_core.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    #region //This needs some work. It works, but its odd - the movement is rigid.
                    if (_core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise) == false && _core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = _core.Actors.Enemies.Visible();
                            foreach (var enemy in enemies)
                            {
                                var distanceTo = Actor.DistanceTo(enemy);
                                if (distanceTo < 500)
                                {
                                    if (Actor.IsPointingAt(enemy, 50))
                                    {
                                        if (enemy.IsPointingAway(Actor, 50))
                                        {
                                            var angleTo = Actor.AngleTo(enemy);
                                            Actor.Velocity.Angle.Degrees = angleTo;
                                        }
                                    }
                                }
                            }
                            _allowLockPlayerAngleToNearbyEnemy = false;
                        }
                    }
                    else
                    {
                        _allowLockPlayerAngleToNearbyEnemy = true;
                    }
                    #endregion
                }

                //Make player boost "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost) && _core.Input.IsKeyPressed(HgPlayerKey.Forward)
                    && Actor.Velocity.AvailableBoost > 0 && Actor.Velocity.BoostRebuilding == false)
                {
                    if (Actor.Velocity.BoostPercentage < 1.0)
                    {
                        double boostToAdd = Actor.Velocity.BoostPercentage > 0
                            ? _core.Settings.PlayerThrustRampUp * (1 - Actor.Velocity.BoostPercentage) : _core.Settings.PlayerThrustRampUp;

                        Actor.Velocity.BoostPercentage += boostToAdd;
                    }

                    Actor.Velocity.AvailableBoost -= Actor.Velocity.MaxBoost * Actor.Velocity.BoostPercentage;
                    if (Actor.Velocity.AvailableBoost < 0)
                    {
                        Actor.Velocity.AvailableBoost = 0;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the boost and rebuild available boost.
                    if (Actor.Velocity.BoostPercentage > 0)
                    {
                        Actor.Velocity.BoostPercentage -= _core.Settings.PlayerThrustRampDown;
                        if (Actor.Velocity.BoostPercentage < 0.01)
                        {
                            Actor.Velocity.BoostPercentage = 0;
                        }
                    }

                    if (_core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost) == false && Actor.Velocity.AvailableBoost < _core.Settings.MaxPlayerBoost)
                    {
                        Actor.Velocity.AvailableBoost = (Actor.Velocity.AvailableBoost + 5).Box(0, _core.Settings.MaxPlayerBoost);

                        if (Actor.Velocity.BoostRebuilding && Actor.Velocity.AvailableBoost >= _core.Settings.PlayerBoostRebuildMin)
                        {
                            Actor.Velocity.BoostRebuilding = false;
                        }
                    }
                }

                //Make player thrust "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HgPlayerKey.Forward))
                {
                    if (Actor.Velocity.ThrottlePercentage < 1.0)
                    {
                        //We add the first thrust amount with the amount defined in "PlayerThrustRampUp", after that we add the
                        // calculated inversee-percentage-from-100-percent of "PlayerThrustRampUp" to the ThrottlePercentage until we reach 100%.
                        // This causes our thrust to start qucikly and acceleration to fade fast as we approach full throttle. The last few %-points
                        //  of throttle will take a while. We do the reverse of this to stop. Stopping fast at first and slowly-slowly slowing to a stop.

                        double thrustToAdd = Actor.Velocity.ThrottlePercentage > 0
                            ? _core.Settings.PlayerThrustRampUp * (1 - Actor.Velocity.ThrottlePercentage) : _core.Settings.PlayerThrustRampUp;

                        Actor.Velocity.ThrottlePercentage += thrustToAdd;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the thrust.
                    if (Actor.Velocity.ThrottlePercentage > 0)
                    {
                        //Ramp down to a stop:
                        double thrustToRemove = Actor.Velocity.ThrottlePercentage < 1
                            ? _core.Settings.PlayerThrustRampDown * Actor.Velocity.ThrottlePercentage : _core.Settings.PlayerThrustRampDown;

                        Actor.Velocity.ThrottlePercentage -= thrustToRemove;

                        if (Actor.Velocity.ThrottlePercentage < 0.01)
                        {
                            //Dont overshoot the stop.
                            Actor.Velocity.ThrottlePercentage = 0;
                        }
                    }
                    else if (Actor.Velocity.ThrottlePercentage < 0)
                    {
                        //Ramp up to a stop:
                        double thrustToRemove = Actor.Velocity.ThrottlePercentage * -1 < 1
                            ? _core.Settings.PlayerThrustRampDown * (1 - Actor.Velocity.ThrottlePercentage * -1) : _core.Settings.PlayerThrustRampDown;

                        Actor.Velocity.ThrottlePercentage += thrustToRemove;
                        if (Actor.Velocity.ThrottlePercentage > 0)
                        {
                            //Dont overshoot the stop.
                            Actor.Velocity.ThrottlePercentage = 0;
                        }
                    }
                }

                if (Actor.BoostAnimation != null)
                {
                    Actor.BoostAnimation.Visable =
                        _core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost)
                        && _core.Input.IsKeyPressed(HgPlayerKey.Forward)
                        && Actor.Velocity.AvailableBoost > 0 && Actor.Velocity.BoostRebuilding == false;
                }

                if (Actor.Velocity.AvailableBoost <= 0)
                {
                    Actor.Velocity.AvailableBoost = 0;
                    Actor.Velocity.BoostRebuilding = true;
                }

                if (Actor.ThrustAnimation != null)
                {
                    Actor.ThrustAnimation.Visable = _core.Input.IsKeyPressed(HgPlayerKey.Forward);
                }

                var thrustVector = Actor.Velocity.MaxSpeed * (Actor.Velocity.ThrottlePercentage + -Actor.Velocity.RecoilPercentage);

                if (Actor.Velocity.BoostPercentage > 0)
                {
                    thrustVector += Actor.Velocity.MaxBoost * Actor.Velocity.BoostPercentage;
                }

                displacementVector.X += Actor.Velocity.Angle.X * thrustVector;
                displacementVector.Y += Actor.Velocity.Angle.Y * thrustVector;

                if (Actor.Velocity.BoostPercentage > 0.1)
                {
                    Actor.ShipEngineBoostSound.Play();
                }
                else
                {
                    Actor.ShipEngineBoostSound.Fade();
                }

                if (Actor.Velocity.ThrottlePercentage > 0.1)
                {
                    Actor.ShipEngineRoarSound.Play();
                }
                else
                {
                    Actor.ShipEngineRoarSound.Fade();
                }

                //We are going to restrict the rotation speed to a percentage of thrust.
                var rotationSpeed = _core.Settings.MaxPlayerRotationSpeedDegrees * Actor.Velocity.ThrottlePercentage;

                if (_core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise))
                {
                    Actor.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise))
                {
                    Actor.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            //Scroll the background.
            _core.Display.BackgroundOffset.X += displacementVector.X;
            _core.Display.BackgroundOffset.Y += displacementVector.Y;

            _core.Display.CurrentQuadrant = _core.Display.GetQuadrant(
                Actor.X + _core.Display.BackgroundOffset.X,
                Actor.Y + _core.Display.BackgroundOffset.Y);

            if (Actor.Velocity.RecoilPercentage > 0)
            {
                Actor.Velocity.RecoilPercentage -= Actor.Velocity.RecoilPercentage * 0.01;
                if (Actor.Velocity.RecoilPercentage < 0.01)
                {
                    Actor.Velocity.RecoilPercentage = 0;
                }
            }

            Actor.RenewableResources.RenewAllResources();

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Actor.Reset();

            _core.Actors.RenderRadar = true;
            Actor.Visable = true;
            Actor.ShipEngineIdleSound.Play();
            Actor.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Actor.Visable = false;
            _core.Actors.RenderRadar = false;
            Actor.ShipEngineIdleSound.Stop();
            Actor.ShipEngineRoarSound.Stop();
        }
    }
}
