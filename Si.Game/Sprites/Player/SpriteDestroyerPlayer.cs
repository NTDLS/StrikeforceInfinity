﻿using Si.Game.Engine;
using Si.Shared.GameEngine.Loudouts;
using Si.Game.Sprites.Player.BasesAndInterfaces;
using Si.Game.Weapons;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.Game.Sprites.Player
{
    internal class SpriteDestroyerPlayer : SpritePlayerBase
    {
        public SpriteDestroyerPlayer(EngineCore gameCore)
            : base(gameCore)
        {
            ShipClass = SiPlayerClass.Destroyer;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Vicious Annihilator ←\n"
                        + "Lives up to its name as a relentless annihilator,\n"
                        + "unleashing devastating firepower to obliterate foes.",
                    MaxSpeed = 5.0,
                    MaxBoost = 2.5,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 42));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
