﻿using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelSerfFormations : LevelBase
    {
        private bool _waitingOnPopulation = false;

        public LevelSerfFormations(GameEngineCore gameEngine)
            : base(gameEngine,
                  "Serf Formations",
                  "They fly in formation, which look like easy targets...."
                  )
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(500, FirstShowPlayerCallback);
            AddRecuringFireEvent(1, AdvanceWaveCallback);
            AddRecuringFireEvent(5, RedirectFormationCallback);

            _gameEngine.Player.Sprite.AddHullHealth(100);
            _gameEngine.Player.Sprite.AddShieldHealth(10);
        }

        private void RedirectFormationCallback(SiEngineCallbackEvent sender, object refObj)
        {
            var formationSerfs = _gameEngine.Sprites.Enemies.VisibleOfType<SpriteEnemySerf>()
                .Where(o => o.Mode == SpriteEnemySerf.AIMode.InFormation).ToList();

            if (formationSerfs.Count > 0)
            {
                if (formationSerfs.Exists(o => o.IsWithinCurrentScaledScreenBounds == true) == false)
                {
                    float angleToPlayer = formationSerfs.First().AngleTo360(_gameEngine.Player.Sprite);

                    foreach (SpriteEnemySerf enemy in formationSerfs)
                    {
                        enemy.Velocity.Angle.Degrees = angleToPlayer;
                    }
                }
            }
        }

        private void FirstShowPlayerCallback(SiEngineCallbackEvent sender, object refObj)
        {
            _gameEngine.Player.ResetAndShow();
        }

        private void AdvanceWaveCallback(SiEngineCallbackEvent sender, object refObj)
        {
            if (_gameEngine.Sprites.OfType<SpriteEnemyBase>().Count == 0 && !_waitingOnPopulation)
            {
                if (CurrentWave == TotalWaves && _waitingOnPopulation != true)
                {
                    End();
                    return;
                }

                _waitingOnPopulation = true;
                AddSingleFireEvent(5, AddFreshEnemiesCallback);
                CurrentWave++;
            }
        }

        private void AddFreshEnemiesCallback(SiEngineCallbackEvent sender, object refObj)
        {
            SiVector baseLocation = _gameEngine.Display.RandomOffScreenLocation();
            CreateTriangleFormation(baseLocation, 100 - (CurrentWave + 1) * 10, CurrentWave + 2);
            _gameEngine.Audio.RadarBlipsSound.Play();
            _waitingOnPopulation = false;
        }

        private SpriteEnemySerf AddOneEnemyAt(float x, float y, float angle)
        {
            var enemy = _gameEngine.Sprites.Enemies.Create<SpriteEnemySerf>();
            enemy.X = x;
            enemy.Y = y;
            enemy.Velocity.ThrottlePercentage = 0.8f;
            enemy.Velocity.Speed = 6;
            enemy.Velocity.Angle.Degrees = angle;
            return enemy;
        }

        private void CreateTriangleFormation(SiVector baseLocation, float spacing, int depth)
        {
            float angle = SiVector.AngleTo360(baseLocation, _gameEngine.Player.Sprite);

            for (int col = 0; col < depth; col++)
            {
                for (int row = 0; row < depth - col; row++)
                {
                    AddOneEnemyAt(baseLocation.X + col * spacing,
                        baseLocation.Y + row * spacing + col * spacing / 2,
                        angle);
                }
            }
        }
    }
}
