﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static HG.Engine.Constants;

namespace HG.Loudouts
{
    internal class ShipLoadout
    {
        public PlayerClass Class { get; set; }
        [JsonIgnore]
        public int ImageIndex => (int)Class;
        [JsonIgnore]
        public string Name => Class.ToString();
        public string Description { get; set; }
        public double Speed { get; set; }
        public double Boost { get; set; }
        public int Hull { get; set; }
        public int Sheilds { get; set; }

        public ShipLoadoutWeapon PrimaryWeapon { get; set; }
        public List<ShipLoadoutWeapon> SecondaryWeapons { get; set; } = new();

        public ShipLoadout()
        {
        }

        public ShipLoadout(PlayerClass shipClass)
        {
            Class = shipClass;
        }

        public class ShipLoadoutWeapon
        {
            public string Type { get; set; }
            public int Rounds { get; set; }

            public ShipLoadoutWeapon()
            {
            }

            public ShipLoadoutWeapon(Type type, int rounds)
            {
                Type = type.Name;
                Rounds = rounds;
            }
        }
    }
}