﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites.Enemies.BaseClasses;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using System;

namespace StrikeforceInfinity.Game.Controller
{
    internal class EnemySpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        public EnemySpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            if (GameCore.Player.Sprite != null)
            {
                GameCore.Player.Sprite.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                foreach (var weapon in enemy.Weapons)
                {
                    weapon.LockedOnObjects.Clear();
                }

                if (GameCore.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (GameCore.Player.Sprite.SelectedSecondaryWeapon != null)
                    {
                        GameCore.Player.Sprite.SelectedSecondaryWeapon.ApplyWeaponsLock(displacementVector, enemy); //Player lock-on to enemy. :D
                    }
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { GameCore };
                SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = GameCore.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Generator.Next(0, 360);

                obj.BeforeCreate();
                SpriteManager.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }
    }
}