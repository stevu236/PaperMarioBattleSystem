﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The DEFUp Status Effect.
    /// The entity's Defense is raised by a certain value until it ends.
    /// </summary>
    public class DEFUpStatus : StatusEffect
    {
        /// <summary>
        /// The amount to raise the entity's Defense by.
        /// </summary>
        protected int DefenseValue = 0;

        public DEFUpStatus(int defenseValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.DEFUp;
            Alignment = StatusAlignments.Positive;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(604, 156, 38, 46));

            DefenseValue = defenseValue;
            Duration = duration;

            AfflictedMessage = "Defense is boosted!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.RaiseDefense(DefenseValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.LowerDefense(DefenseValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.LowerDefense(DefenseValue);
            }
        }

        protected override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                EntityAfflicted.RaiseDefense(DefenseValue);
            }
        }

        public override StatusEffect Copy()
        {
            return new DEFUpStatus(DefenseValue, Duration);
        }
    }
}