﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Levels.BaseClasses;
using NebulaSiege.Sprites.Enemies.BaseClasses;
using NebulaSiege.Sprites.Enemies.Peons;
using NebulaSiege.Utility;

namespace NebulaSiege.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingGalore : LevelBase
    {
        public LevelDebuggingGalore(EngineCore core)
            : base(core,
                  "Debugging Galore",
                  "The level is dire, the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            TotalWaves = 100;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _core.Player.Sprite.AddHullHealth(100);
            _core.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
            _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddEnemyCallback);
        }

        private void AddFreshEnemiesCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            if (_core.Sprites.OfType<SpriteEnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    End();
                    return;
                }

                //int enemyCount = Utility.Random.Next(CurrentWave + 1, CurrentWave + 5);
                int enemyCount = 1;

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddEnemyCallback);
                }

                _core.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();

            _core.Sprites.Enemies.Create<SpriteEnemyAITracer>();

            //_core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();

            //_core.Sprites.Debugs.CreateAtCenterScreen();
            //_core.Sprites.Enemies.Create<SpriteEnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyPhoenix>();
            //_core.Sprites.Enemies.Create<EnemyPhoenix>();
            //_core.Sprites.Enemies.Create<EnemyPhoenix>();
            //_core.Sprites.Enemies.Create<EnemyDevastator>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemySpectre>();
            //_core.Sprites.Enemies.Create<EnemyDevastator>();
            //_core.Sprites.Enemies.Create<EnemyDevastator>();
        }
    }
}
