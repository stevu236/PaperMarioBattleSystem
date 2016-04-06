﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Jump action
    /// </summary>
    public sealed class Jump : BattleAction
    {
        public Jump()
        {
            Name = "Jump";
            Description = "Jump and stomp on an enemy.";

            Command = new JumpCommand(this);
        }

        public override void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection((targets) =>
            {
                for (int i = 0; i < targets.Length; i++)
                    targets[i].LoseHP(1);
                BattleManager.Instance.EntityTurn.UsedTurn = true;
                BattleManager.Instance.EntityTurn.EndTurn();
            }, SelectionType, BattleManager.Instance.GetAliveEnemies());
        }
    }
}
