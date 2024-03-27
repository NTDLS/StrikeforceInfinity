﻿using System.Collections.Generic;

namespace Si.GameEngine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class InteractiveSpriteMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Speed { get; set; } = 1f;
        public float Boost { get; set; } = 0f;

        /// <summary>
        /// How much does the sprite weigh?
        /// </summary>
        public float Mass { get; set; } = 1f;
        public int HullHealth { get; set; } = 0;
        public int ShieldHealth { get; set; } = 0;
        public int Bounty { get; set; } = 0;
        public bool TakesMunitionDamage { get; set; } = false;
        public bool CollisionDetection { get; set; } = false;

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        public InteractiveSpriteWeapon PrimaryWeapon { get; set; }
        public List<InteractiveSpriteAttachment> Attachments { get; set; } = new();
        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}