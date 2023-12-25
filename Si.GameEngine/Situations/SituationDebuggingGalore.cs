﻿using Si.GameEngine.Engine;
using Si.GameEngine.Levels;
using Si.GameEngine.Situations.BasesAndInterfaces;

namespace Si.GameEngine.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class SituationDebuggingGalore : SituationBase
    {
        public SituationDebuggingGalore(EngineCore gameCore)
            : base(gameCore,
                  "Debugging Galore",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            Levels.Add(new LevelDebuggingGalore(gameCore));
            Levels.Add(new LevelFreeFlight(gameCore));
        }
    }
}