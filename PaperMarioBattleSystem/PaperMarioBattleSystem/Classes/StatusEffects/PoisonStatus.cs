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
    /// The Poison Status Effect.
    /// The entity takes 1 HP in Poison damage at the start of each phase cycle
    /// </summary>
    public sealed class PoisonStatus : StatusEffect
    {
        public PoisonStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Poison;
            Alignment = StatusAlignments.Negative;

            StatusIcon = new CroppedTexture2D(AssetManager.Instance.LoadAsset<Texture2D>($"{ContentGlobals.UIRoot}/Battle/BattleGFX"),
                new Rectangle(555, 58, 38, 46));

            Duration = duration;

            AfflictedMessage = "Poisoned! The toxins will\nsteadily do damage!";
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnPhaseCycleStart()
        {
            EntityAfflicted.TakeDamage(Enumerations.Elements.Poison, 1, true);
            IncrementTurns();
        }

        protected override void OnSuspend()
        {

        }

        protected override void OnResume()
        {

        }

        public override StatusEffect Copy()
        {
            return new PoisonStatus(Duration);
        }
    }
}
