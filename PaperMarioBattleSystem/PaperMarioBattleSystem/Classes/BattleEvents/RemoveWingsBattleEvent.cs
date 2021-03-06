﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that removes an <see cref="IWingedBehavior"/>'s wings when it stops being targeted.
    /// </summary>
    public sealed class RemoveWingsBattleEvent : BattleEvent
    {
        private IWingedBehavior WingedEntity = null;
        private BattleEntity Entity = null;

        public RemoveWingsBattleEvent(IWingedBehavior wingedEntity, BattleEntity entity)
        {
            WingedEntity = wingedEntity;
            Entity = entity;
        }

        protected override void OnUpdate()
        {
            //Don't do anything with invalid input
            if (WingedEntity == null || Entity == null)
            {
                Debug.LogError($"A null {nameof(IWingedBehavior)} or {nameof(BattleEntity)} has been passed in. Fix this");
                End();
                return;
            }

            //If the entity is no longer targeted, remove its wings and end
            if (Entity.IsTargeted == false)
            {
                WingedEntity.RemoveWings();

                End();
            }
        }
    }
}
