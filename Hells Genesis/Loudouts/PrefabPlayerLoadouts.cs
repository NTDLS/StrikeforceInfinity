﻿using HG.Actors.Weapons;
using System.Collections.Generic;
using System.Linq;
using static HG.Engine.Constants;
using static HG.Loudouts.ShipLoadout;

namespace HG.Loudouts
{
    internal class PrefabPlayerLoadouts
    {
        public List<ShipLoadout> Collection { get; private set; } = new();

        public ShipLoadout GetByName(string name)
        {
            return Collection.Where(o => o.Name == name).First();
        }

        public ShipLoadout GetDefault()
        {
            return Collection.First();
        }

        public void CreateDefaults()
        {
#if DEBUG
            //Debug
            {
                var loadout = new ShipLoadout(PlayerClass.Debug)
                {
                    Speed = 3.0,
                    Boost = 4.0,
                    Hull = 10000,
                    Sheilds = 10000,
                    Description = "→ The code crusader ←\n"
                        + "Crude in design but equipped with advanced diagnostics and repair systems.\n"
                        + "Nearly indestructible and inconceivably fast. Its mission is to discover\n"
                        + "glitches in the vast cosmic code, ensuring a smooth journey for all that follow...",
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 100000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponHotPepper), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 100000));

                Collection.Add(loadout);
            }
#endif
            //Frigate
            {
                var loadout = new ShipLoadout(PlayerClass.Frigate)
                {
                    Description = "→ Nimble Interceptor ←\n"
                        + "A nimble interceptor, designed for hit-and-run tactics\n"
                        + "and lightning-fast strikes against enemy forces.",
                    Speed = 3.0,
                    Boost = 1.5,
                    Hull = 500,
                    Sheilds = 100,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponHotPepper), 10000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 16));
                //loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 500)); //I dont have a second non guided missile.

                Collection.Add(loadout);
            }

            //Cruiser
            {
                var loadout = new ShipLoadout(PlayerClass.Cruiser)
                {
                    Description = "→ Heavy Assault Cruiser ←\n"
                       + "A formidable heavy assault vessel, bristling with weaponry\n"
                       + "and to take on any adversary in head-to-head combat.",
                    Speed = 2.2,
                    Boost = 1.0,
                    Hull = 2500,
                    Sheilds = 3000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 16));
                //loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 500 )); //I dont have a second non guided missile.

                Collection.Add(loadout);
            }

            //Destroyer
            {
                var loadout = new ShipLoadout(PlayerClass.Destroyer)
                {
                    Description = "→ Vicious Annihilator ←\n"
                        + "Lives up to its name as a relentless annihilator,\n"
                        + "unleashing devastating firepower to obliterate foes.",
                    Speed = 5.0,
                    Boost = 2.5,
                    Hull = 500,
                    Sheilds = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 16));

                Collection.Add(loadout);
            }

            //Dreadnaught
            {
                var loadout = new ShipLoadout(PlayerClass.Dreadnaught)
                {
                    Description = "→ Titanic Dreadnought ←\n"
                        + "Titanic force of destruction, capable of withstanding\n"
                        + "immense firepower while dishing out colossal damage.",
                    Speed = 4.0,
                    Boost = 2.0,
                    Hull = 500,
                    Sheilds = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 500)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 16));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 10));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 6));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 4));

                Collection.Add(loadout);
            }

            //Reaver
            {
                var loadout = new ShipLoadout(PlayerClass.Reaver)
                {
                    Description = "→ Rogue Reaver ←\n"
                        + "A rogue fighter, known for its hit-and-fade tactics,\n"
                        + "striking and disappearing into the cosmos with warp speed.",
                    Speed = 6.0,
                    Boost = 3.5,
                    Hull = 500,
                    Sheilds = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 8));

                Collection.Add(loadout);
            }

            //Serpent
            {
                var loadout = new ShipLoadout(PlayerClass.Serpent)
                {
                    Description = "→ Stealthy Serpent ←\n"
                        + "A stealthy long distance fighter, expert in covert operations\n"
                        + "and ambushing unsuspecting adversaries with deadly precision.",
                    Speed = 5.0,
                    Boost = 3.5,
                    Hull = 100,
                    Sheilds = 15000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));

                Collection.Add(loadout);
            }

            //Starfighter
            {
                var loadout = new ShipLoadout(PlayerClass.Starfighter)
                {
                    Description = "→ Celestial Aviator ←\n"
                        + "A sleek and versatile spacecraft, built for supremacy among the stars.\n"
                        + "It's the first choice for finesse, agility, and unmatched combat prowess\n"
                        + "without all the fuss of a powerful loadout.",
                    Speed = 5.0,
                    Boost = 3.5,
                    Hull = 100,
                    Sheilds = 15000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));

                Collection.Add(loadout);
            }
        }
    }
}