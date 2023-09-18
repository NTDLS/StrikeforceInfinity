﻿using HG.Controllers;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Sprites.BaseClasses;
using HG.Sprites.Enemies.BaseClasses;
using HG.TickHandlers.Interfaces;
using HG.Weapons.BaseClasses;
using HG.Weapons.Bullets.BaseClasses;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class ActorBulletTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : BulletBase => _controller.VisibleOfType<subType>();
        public List<BulletBase> Visible() => _controller.VisibleOfType<BulletBase>();
        public List<subType> OfType<subType>() where subType : BulletBase => _controller.OfType<subType>();

        public ActorBulletTickHandler(EngineCore core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            var thingsThatCanBeHit = new List<ActorShipBase>
            {
                _core.Player.Actor
            };

            thingsThatCanBeHit.AddRange(_controller.VisibleOfType<EnemyBossBase>());
            thingsThatCanBeHit.AddRange(_controller.VisibleOfType<EnemyPeonBase>());
            thingsThatCanBeHit.AddRange(_controller.VisibleOfType<ActorAttachment>());

            foreach (var bullet in VisibleOfType<BulletBase>())
            {
                bullet.ApplyMotion(displacementVector); //Move the bullet.

                var hitTestPosition = bullet.Location.ToWriteableCopy(); //Grab the new location of the bullet.

                //Loop backwards and hit-test each position along the bullets path.
                for (int i = 0; i < bullet.Velocity.MaxSpeed; i++)
                {
                    hitTestPosition.X -= bullet.Velocity.Angle.X;
                    hitTestPosition.Y -= bullet.Velocity.Angle.Y;

                    foreach (var thing in thingsThatCanBeHit)
                    {
                        if (thing.TestHit(displacementVector, bullet, hitTestPosition))
                        {
                            bullet.Explode();
                            break;
                        }
                    }
                }

                bullet.ApplyIntelligence(displacementVector);
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_controller.Collection)
            {
                _controller.OfType<BulletBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public BulletBase CreateLocked(WeaponBase weapon, ActorBase firedFrom, ActorBase lockedTarget, HgPoint xyOffset = null)
        {
            lock (_controller.Collection)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public BulletBase Create(WeaponBase weapon, ActorBase firedFrom, HgPoint xyOffset = null)
        {
            lock (_controller.Collection)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(BulletBase obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
