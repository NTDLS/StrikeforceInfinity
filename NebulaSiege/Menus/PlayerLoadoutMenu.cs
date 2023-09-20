﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using System.Linq;

namespace NebulaSiege.Menus
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class PlayerLoadoutMenu : _MenuBase
    {
        private readonly SpriteMenuItem _shipBlurb;

        public PlayerLoadoutMenu(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = currentScaledScreenBounds.X + 40;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new NsPoint(offsetX, offsetY), "Select a Ship Class");
            itemTitle.X = offsetX + 200;
            itemTitle.Y = offsetY - itemTitle.Size.Height;

            offsetY += itemTitle.Height;

            _shipBlurb = CreateAndAddTextItem(new NsPoint(offsetX, offsetY), "");
            _shipBlurb.X = offsetX + 200;
            _shipBlurb.Y = offsetY - _shipBlurb.Size.Height;

            foreach (var loadout in core.PrefabPlayerLoadouts.Collection)
            {
                var menuItem = CreateAndAddMenuItem(new NsPoint(offsetX + 25, offsetY), loadout.Name, loadout.Name);
                menuItem.Y -= menuItem.Size.Height / 2;

                var shipIcon = _core.Sprites.InsertPlayer(new SpritePlayer(_core, loadout) { Name = "MENU_SHIP_SELECT" });

                if (loadout.Name == "Debug")
                {
                    shipIcon.ThrustAnimation.Visable = true;
                }
                else
                {
                    shipIcon.BoostAnimation.Visable = true;
                }
                shipIcon.X = offsetX;
                shipIcon.Y = offsetY;
                offsetY += 50;
            }

            SelectableItems().First().Selected = true;
        }

        private string GetHelpText(string name, string primaryWeapon, string secondaryWeapons,
            string sheilds, string hullStrength, string maxSpeed, string warpDrive, string blurb)
        {
            string result = $"             Name : {name}\n";
            result += $"   Primary weapon : {primaryWeapon}\n";
            result += $"Secondary Weapons : {secondaryWeapons}\n";
            result += $"          Sheilds : {sheilds}\n";
            result += $"    Hull Strength : {hullStrength}\n";
            result += $"        Max Speed : {maxSpeed}\n";
            result += $"       Warp Drive : {warpDrive}\n";
            result += $"\n{blurb}";
            return result;
        }

        public override void SelectionChanged(SpriteMenuItem item)
        {
            var loadout = _core.PrefabPlayerLoadouts.GetByName(item.Key);

            string weaponName = NsReflection.GetStaticPropertyValue(loadout.PrimaryWeapon.Type, "Name");
            string primaryWeapon = $"{weaponName} x{loadout.PrimaryWeapon.Rounds}";

            string secondaryWeapons = string.Empty;
            foreach (var weapon in loadout.SecondaryWeapons)
            {
                weaponName = NsReflection.GetStaticPropertyValue(weapon.Type, "Name");
                secondaryWeapons += $"{weaponName} x{weapon.Rounds}\n{new string(' ', 20)}";
            }

            _shipBlurb.Text = GetHelpText(
                loadout.Name,               //Name
                primaryWeapon.Trim(),       //Primary Weapon
                secondaryWeapons.Trim(),    //Secondary Weapon
                $"{loadout.Sheilds:n0}",    //Sheilds
                $"{loadout.Hull:n0}",       //Hull
                $"{loadout.Speed:n1}",      //Speed
                $"{loadout.Boost:n1}",      //Boost
                $"{loadout.Description}"
            );
        }

        public override void ExecuteSelection(SpriteMenuItem item)
        {
            var loadout = _core.PrefabPlayerLoadouts.GetByName(item.Key);

            _core.Player.Sprite.Reset(loadout);

            _core.Sprites.DeleteAllSpriteByAssetTag("MENU_SHIP_SELECT");

            _core.Sprites.NewGame();
        }
    }
}