﻿namespace Si.Library.Mathematics
{
    /// <summary>
    /// Used to keep track of the FPS that the world clock is executing at.
    /// </summary>
    public class SiFrameCounter
    {
        private DateTime _lastFrame;
        private int _frameRateSamples;
        private float _totalFrameRate;
        private readonly int _maxSamples = 1000;

        public float CurrentFrameRate { get; private set; }
        public float AverageFrameRate { get; private set; }
        public float MinimumFrameRate { get; private set; }
        public float MaximumFrameRate { get; private set; }

        public SiFrameCounter()
        {
            Reset();
        }

        public void Reset()
        {
            _lastFrame = DateTime.MinValue;
            _totalFrameRate = 0;
            _frameRateSamples = 0;

            CurrentFrameRate = 0;
            AverageFrameRate = float.PositiveInfinity;
            MinimumFrameRate = float.PositiveInfinity;
            MaximumFrameRate = float.NegativeInfinity;
        }

        public void Calculate()
        {
            if (_lastFrame != DateTime.MinValue)
            {
                if (_frameRateSamples == 0 || _frameRateSamples > _maxSamples)
                {
                    _frameRateSamples = 1;
                    _totalFrameRate = 0;
                }

                CurrentFrameRate = 1000.0f / (float)(DateTime.Now - _lastFrame).TotalMilliseconds;
                _totalFrameRate += CurrentFrameRate;

                if (_frameRateSamples > 100)
                {
                    MinimumFrameRate = CurrentFrameRate < MinimumFrameRate ? CurrentFrameRate : MinimumFrameRate;
                    MaximumFrameRate = CurrentFrameRate > MaximumFrameRate ? CurrentFrameRate : MaximumFrameRate;
                    AverageFrameRate = _totalFrameRate / _frameRateSamples;
                }
                _frameRateSamples++;
            }

            _lastFrame = DateTime.Now;
        }
    }
}
