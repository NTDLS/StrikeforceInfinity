﻿using Si.Engine;
using Si.GameEngine.Loudout;
using Si.GameEngine.Sprite.Player._Superclass;
using Si.GameEngine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite.Player
{
    internal class SpriteDreadnaughtPlayer : SpritePlayerBase
    {
        public SpriteDreadnaughtPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Dreadnaught;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Titanic Dreadnought ←\n"
                        + "Titanic force of destruction, capable of withstanding\n"
                        + "immense firepower while dishing out colossal damage.",
                    Speed = 3.0f,
                    Boost = 1.5f,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 500)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 10));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 6));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 4));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}