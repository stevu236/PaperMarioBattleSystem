﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Jump submenu for all Jump attacks
    /// </summary>
    public class JumpSubMenu : ActionSubMenu
    {
        public JumpSubMenu()
        {
            Name = "Jump";
            Position = new Vector2(230, 150);
            AutoSelectSingle = true;

            BattleActions.Add(new Jump());
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.PowerBounce) > 0)
            {
                BattleActions.Add(new PowerBounce());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.Multibounce) > 0)
            {
                BattleActions.Add(new Multibounce());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.TornadoJump) > 0)
            {
                BattleActions.Add(new TornadoJump());
            }
            if (BattleManager.Instance.EntityTurn.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DDownJump) > 0)
            {
                BattleActions.Add(new DDownJumpAction());
            }
        }
    }
}
