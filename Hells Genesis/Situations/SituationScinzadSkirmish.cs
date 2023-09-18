﻿using HG.Engine;
using HG.Engine.Types;
using HG.Situations.BaseClasses;
using HG.Sprites.Enemies.BaseClasses;
using HG.Sprites.Enemies.Peons;
using HG.Utility;

namespace HG.Situations
{
    internal class SituationScinzadSkirmish : SituationBase
    {
        public SituationScinzadSkirmish(EngineCore core)
            : base(core, "Scinzad Skirmish")
        {
            TotalWaves = 5;
        }

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _core.Player.Sprite.AddHullHealth(100);
            _core.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }

        private void AddFreshEnemiesCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            if (_core.Sprites.OfType<SpriteEnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    EndSituation();
                    return;
                }

                int enemyCount = HgRandom.Random.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.RandomNumber(0, 800)), AddEnemyCallback);
                }

                _core.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Sprites.Enemies.Create<SpriteEnemyScinzad>();
        }
    }
}
