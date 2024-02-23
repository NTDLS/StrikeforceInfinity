﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.GameEngine.Core
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    public class EngineSettings
    {
        public int GraphicsAdapterId { get; set; } = 0;
        public int MunitionTraversalThreads { get; set; } = Environment.ProcessorCount * 2;
        public bool EnableSpriteInterrogation { get; set; } = false;
        public bool HighlightNatrualBounds { get; set; } = false;
        public bool HighlightAllSprites { get; set; } = false;

        public Size Resolution { get; set; }

        public bool PreCacheAllAssets { get; set; } = true;
        public bool FullScreen { get; set; }
        public bool AlwaysOnTop { get; set; }

        public bool PlayMusic { get; set; } = true;

        public bool LockPlayerAngleToNearbyEnemy { get; set; } = false;
        public bool AutoZoomWhenMoving { get; set; } = true;

        public double MillisecondPerEpochs { get; set; } = 8.3378;

        public double EnemyThrustRampUp { get; set; } = 0.0375;
        public double EnemyThrustRampDown { get; set; } = 0.0075;

        public double PlayerThrustRampUp { get; set; } = 0.0375;
        public double PlayerThrustRampDown { get; set; } = 0.0075;

        public int MaxHullHealth { get; set; } = 100000;
        public int MaxShieldPoints { get; set; } = 100000;

        public double MaxPlayerBoostAmount { get; set; } = 10000;
        public double PlayerBoostRebuildFloor { get; set; } = 1000;
        public double MaxRecoilPercentage { get; set; } = 0.4; //Max amount that will be substracted from the thrust percentage.
        public double MaxPlayerRotationSpeedDegrees { get; set; } = 1.40;

        public int InitialFrameStarCount { get; set; } = 100;
        public int DeltaFrameTargetStarCount { get; set; } = 200;

        public double MinEnemySpeed { get; set; } = 3.75;
        public double MaxEnemySpeed { get; set; } = 7.5;

        public bool VerticalSync { get; set; } = false;
        public bool AntiAliasing { get; set; } = true;

        public double FramePerSecondLimit { get; set; } = 120;
        public double MunitionSceneDistanceLimit { get; set; } = 2500; //The distance from the scene that a munition can travel before it is cleaned up.
        public double EnemySceneDistanceLimit { get; set; } = 5000; //The distance from the scene that a enemy can travel before it is cleaned up.

        /// <summary>
        /// How much larger than the screen (NatrualScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public double OverdrawScale { get; set; } = 1.5;

        public EngineSettings()
        {
            int x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
            int y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
            if (x % 2 != 0) x++;
            if (y % 2 != 0) y++;
            Resolution = new Size(x, y);
        }
    }
}
