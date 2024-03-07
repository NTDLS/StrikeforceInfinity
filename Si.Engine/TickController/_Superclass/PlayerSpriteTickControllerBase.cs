﻿using Si.Engine;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.TickController._Superclass
{
    /// <summary>
    /// Tick manager that generates offset vectors for the one and only local player sprite.
    /// </summary>
    public class PlayerSpriteTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore GameEngine { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual SiPoint ExecuteWorldClockTick(float epochTimeepoch) => new();

        public PlayerSpriteTickControllerBase(EngineCore engine)
        {
            GameEngine = engine;
        }
    }
}