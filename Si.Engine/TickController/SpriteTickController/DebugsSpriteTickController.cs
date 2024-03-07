﻿using Si.Engine;
using Si.GameEngine.Manager;
using Si.GameEngine.Sprite;
using Si.GameEngine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.TickController.SpriteTickController
{
    public class DebugsSpriteTickController : SpriteTickControllerBase<SpriteDebug>
    {
        public DebugsSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            /*
            if (GameEngine.Player.Sprite != null)
            {
                var anchor = GameEngine.Sprites.Debugs.ByTag("Anchor");
                if (anchor == null)
                {
                    GameEngine.Sprites.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = GameEngine.Sprites.Debugs.ByTag("Anchor");
                }

                var pointer = GameEngine.Sprites.Debugs.ByTag("Pointer");
                if (pointer == null)
                {
                    GameEngine.Sprites.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = GameEngine.Sprites.Debugs.ByTag("Pointer");
                }

                float requiredAngle = GameEngine.Player.Sprite.AngleTo(anchor);
                var offset = SiMath.AngleFromPointAtDistance(new SiAngle(requiredAngle), new SiPoint(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = GameEngine.Player.Sprite.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(GameEngine.Player.Sprite);
            }
            */

            foreach (var debug in Visible())
            {
                debug.ApplyMotion(epoch, displacementVector);
                debug.RenewableResources.RenewAllResources(epoch);
            }
        }
    }
}