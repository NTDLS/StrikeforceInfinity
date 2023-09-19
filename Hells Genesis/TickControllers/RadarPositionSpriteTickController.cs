﻿using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.Controller
{
    internal class RadarPositionSpriteTickController : IUnvectoredTickController<SpriteRadarPositionTextBlock>
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public RadarPositionSpriteTickController(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick()
        {
            var overlappingIndicators = new Func<List<List<SpriteRadarPositionTextBlock>>>(() =>
            {
                var accountedFor = new HashSet<SpriteRadarPositionTextBlock>();
                var groups = new List<List<SpriteRadarPositionTextBlock>>();
                var radarTexts = _core.Sprites.VisibleOfType<SpriteRadarPositionTextBlock>();

                foreach (var parent in radarTexts)
                {
                    if (accountedFor.Contains(parent) == false)
                    {
                        var group = new List<SpriteRadarPositionTextBlock>();
                        foreach (var child in radarTexts)
                        {
                            if (accountedFor.Contains(child) == false)
                            {
                                if (parent != child && parent.Intersects(child, new HgPoint(100, 100)))
                                {
                                    group.Add(child);
                                    accountedFor.Add(child);
                                }
                            }
                        }
                        if (group.Count > 0)
                        {
                            group.Add(parent);
                            accountedFor.Add(parent);
                            groups.Add(group);
                        }
                    }
                }
                return groups;
            })();

            if (overlappingIndicators.Count > 0)
            {
                foreach (var group in overlappingIndicators)
                {
                    var min = group.Min(o => o.DistanceValue);
                    var max = group.Min(o => o.DistanceValue);

                    foreach (var member in group)
                    {
                        member.Visable = false;
                    }

                    group[0].Text = min.ToString("#,#") + "-" + max.ToString("#,#");
                    group[0].Visable = true;
                }
            }
        }

        #region Factories.

        public SpriteRadarPositionIndicator Create()
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteRadarPositionIndicator(_core);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteRadarPositionIndicator obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                obj.Visable = false;
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
