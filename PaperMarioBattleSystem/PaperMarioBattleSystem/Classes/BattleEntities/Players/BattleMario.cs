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
    /// Mario in battle
    /// </summary>
    public sealed class BattleMario : BattlePlayer, ITattleableEntity
    {
        public MarioStats MStats { get; private set; } = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="marioStats">Mario's stats</param>
        public BattleMario(MarioStats marioStats) : base(marioStats)
        {
            Name = "Mario";
            EntityType = Enumerations.EntityTypes.Player;
            PlayerType = Enumerations.PlayerTypes.Mario;

            MStats = marioStats;

            #region Initialize Animations

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Characters/Mario.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(228, 918, 29, 51), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.RunningName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(228, 918, 29, 51), 30d),
                new Animation.Frame(new Rectangle(228, 861, 29, 49), 30d),
                new Animation.Frame(new Rectangle(68, 1056, 31, 48), 30d)));
            AnimManager.AddAnimation(AnimationGlobals.HurtName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(596, 554, 42, 44), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.DeathName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(57, 1022, 53, 26), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(228, 918, 29, 51), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.JumpStartName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(442, 65, 38, 45), 250d)));
            AnimManager.AddAnimation(AnimationGlobals.JumpRisingName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(394, 803, 31, 49), 100d)));
            AnimManager.AddAnimation(AnimationGlobals.JumpFallingName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(394, 859, 31, 49), 100d),
                new Animation.Frame(new Rectangle(394, 916, 31, 48), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.MarioBattleAnimations.HammerPickupName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(173, 664, 30, 49), 100d),
                new Animation.Frame(new Rectangle(174, 607, 29, 50), 100d),
                new Animation.Frame(new Rectangle(340, 421, 32, 46), 200d)));
            AnimManager.AddAnimation(AnimationGlobals.MarioBattleAnimations.HammerWindupName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(341, 9, 36, 50), 150d),
                new Animation.Frame(new Rectangle(341, 64, 38, 50), 150d)));
            AnimManager.AddAnimation(AnimationGlobals.MarioBattleAnimations.HammerSlamName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(334, 319, 42, 50), 200d),
                new Animation.Frame(new Rectangle(340, 166, 32, 44), 300d)));
            AnimManager.AddAnimation(AnimationGlobals.VictoryName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(447, 281, 42, 50), 100d),
                new Animation.Frame(new Rectangle(446, 337, 42, 50), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.JumpMissName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(444, 499, 45, 39), 700d),
                new Animation.Frame(new Rectangle(499, 536, 31, 42), 100d),
                new Animation.Frame(new Rectangle(499, 487, 31, 43), 100d),
                new Animation.Frame(new Rectangle(499, 438, 31, 44), 100d)));

            AnimManager.AddAnimation(AnimationGlobals.SpikedTipHurtName, new LoopAnimation(spriteSheet, 10,
                new Animation.Frame(new Rectangle(393, 450, 38, 52), 30d),
                new Animation.Frame(new Rectangle(393, 385, 38, 55), 30d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.ChoosingActionName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(402, 569, 34, 51), 30d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.DangerChoosingActionName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(550, 846, 30, 50), 30d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(337, 908, 35, 41), 700d)));

            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.StoneName, new Animation(spriteSheet, new Animation.Frame(new Rectangle(66, 859, 35, 41), 0d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.DizzyName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(117, 512, 31, 42), 1000d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(117, 561, 31, 41), 1000d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.SleepName, new ReverseAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(288, 908, 31, 44), 800d, new Vector2(0, -2)),
                new Animation.Frame(new Rectangle(288, 957, 31, 43), 500d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(288, 1006, 31, 42), 800d)));
            AnimManager.AddAnimation(AnimationGlobals.StatusBattleAnimations.InjuredName, new LoopAnimation(spriteSheet, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(117, 512, 31, 42), 1000d, new Vector2(0, -1)),
                new Animation.Frame(new Rectangle(117, 561, 31, 41), 1000d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.SuperguardName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(392, 335, 42, 45), 700d)));

            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(449, 222, 40, 53), 700d)));
            AnimManager.AddAnimation(AnimationGlobals.PlayerBattleAnimations.StarWishName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(398, 516, 29, 48), 1300d)));

            AnimManager.AddAnimation(AnimationGlobals.JumpLandName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(119, 667, 35, 44), 150d),
                new Animation.Frame(new Rectangle(119, 716, 36, 43), 150d)));

            AnimManager.AddAnimation(AnimationGlobals.MarioBattleAnimations.StoneCapPutOnName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(500, 584, 37, 50), 70d),
                new Animation.Frame(new Rectangle(497, 643, 37, 50), 70d),
                new Animation.Frame(new Rectangle(496, 699, 38, 49), 70d),
                new Animation.Frame(new Rectangle(497, 643, 37, 50), 70d),
                new Animation.Frame(new Rectangle(498, 755, 36, 49), 70d),
                new Animation.Frame(new Rectangle(497, 643, 37, 50), 70d),
                new Animation.Frame(new Rectangle(496, 699, 38, 49), 70d),
                new Animation.Frame(new Rectangle(497, 643, 37, 50), 750d)));

            #endregion
        }

        public override void CleanUp()
        {
            BattleManager.Instance.EntityRemovedEvent -= OnEntityRemoved;
        }

        public override void OnBattleStart()
        {
            BattleManager.Instance.EntityRemovedEvent -= OnEntityRemoved;
            BattleManager.Instance.EntityRemovedEvent += OnEntityRemoved;

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.SpikeShield, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.IcePower, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.ZapTap, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PowerPlus, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PowerBounce, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Multibounce, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.IceSmash, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.HeadRattle, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.TornadoJump, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.QuickChange, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FeelingFine, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FlowerSaver, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Charge, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.AllOrNothing, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoublePain, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.LastStand, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoubleDip, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoubleDip, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.TripleDip, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DeepFocus, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DeepFocus, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DeepFocus, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.ReturnPostage, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PrettyLucky, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PrettyLucky, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PrettyLucky, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.LuckyDay, BadgeGlobals.BadgeFilterType.UnEquipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.HPPlus, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FPPlus, BadgeGlobals.BadgeFilterType.UnEquipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Peekaboo, BadgeGlobals.BadgeFilterType.UnEquipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.LEmblem, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.WEmblem, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.DeactivateAndUnequipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.WEmblem, BadgeGlobals.BadgeFilterType.Equipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PityFlower, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.HPDrain, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.HPDrain, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.HPDrain, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FPDrain, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.FPDrain, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Unsimplifier, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Unsimplifier, BadgeGlobals.BadgeFilterType.UnEquipped));

            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DDownPound, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PiercingBlow, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DDownJump, BadgeGlobals.BadgeFilterType.UnEquipped));

            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.PowerSmash, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.MegaSmash, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.QuakeHammer, BadgeGlobals.BadgeFilterType.UnEquipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Jumpman, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.Hammerman, BadgeGlobals.BadgeFilterType.UnEquipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.LuckyStart, BadgeGlobals.BadgeFilterType.UnEquipped));
            this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.TimingTutor, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.RightOn, BadgeGlobals.BadgeFilterType.UnEquipped));

            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.AttackFXB, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.AttackFXC, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.AttackFXD, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.AttackFXE, BadgeGlobals.BadgeFilterType.UnEquipped));
            //this.ActivateAndEquipBadge(Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.AttackFXF, BadgeGlobals.BadgeFilterType.UnEquipped));

            //MStats.HammerLevel = EquipmentGlobals.HammerLevels.Super;
            //MStats.BootLevel = EquipmentGlobals.BootLevels.Super;

            //BattleObjManager.Instance.AddBattleObject(new AfterImageVFX(this, 4, 4, .2f, AfterImageVFX.AfterImageAlphaSetting.FadeOff, AfterImageVFX.AfterImageAnimSetting.Current));

            base.OnBattleStart();
        }

        protected sealed override BattleMenu GetMainBattleMenu()
        {
            return new MarioBattleMenu();
        }

        public sealed override StarPowerBase GetStarPower(StarPowerGlobals.StarPowerTypes starPowerType)
        {
            return MStats.GetStarPowerFromType(starPowerType);
        }

        public override void TurnUpdate()
        {
            base.TurnUpdate();
        }

        public override void OnPhaseCycleStart()
        {
            base.OnPhaseCycleStart();

            //Gain Star Spirit Star Power every phase cycle
            //Exclude the phase cycle that starts the battle
            if (BattleManager.Instance.PhaseCycleCount > 0)
            {
                MStats.SSStarPower.GainStarPower(StarPowerGlobals.StarSpiritSPUPerTurn);
            }
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
            BattleUIManager.Instance.ClearMenuStack();
        }

        /*public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            //NOTE: This isn't entity-specific right now, so it technically doesn't work properly.
            //For example, if a Partner had Mario's Jump, it could use Power Bounce if Mario had
            //the Badge equipped even if the Partner didn't.

            //NOTE 2: The problem is that all Active Badges are in the same list, and it's impossible
            //to differentiate Badges that only affect Mario from ones that affect both Mario and his Partner
            //without finding the Badge first. I feel there needs to be a new approach to how this is handled.

            BadgeGlobals.BadgeTypes newBadgeType = badgeType;

            //Find the non-Partner version of the Badge
            BadgeGlobals.BadgeTypes? tempBadgeType = BadgeGlobals.GetNonPartnerBadgeType(badgeType);
            if (tempBadgeType != null)
            {
                newBadgeType = tempBadgeType.Value;
            }
            else
            {
                //If there is no non-Partner version, get the Badge and check if it affects Mario
                Badge badge = Inventory.Instance.GetBadge(newBadgeType, BadgeGlobals.BadgeFilterType.Equipped);
                //If the Badge isn't equipped or doesn't affect Both or Mario, none are equipped to Mario
                if (badge == null || badge.AffectedType == BadgeGlobals.AffectedTypes.Partner) return 0;
            }

            return Inventory.Instance.GetActiveBadgeCount(newBadgeType);
        }*/

        //Mario can be tattled by Duplighosts disguised as Goombario
        #region Tattle Information

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            return new string[] { "N/A" };
        }

        public string GetTattleDescription()
        {
            return "It's Mario, silly!\nHe's here to save Princess\nPeach, who was kidnapped by\nBowser. Remember?\n<k><p>" +
                   "He fights until the bitter end,\nno matter what enemies attack.<k>";
        }

        #endregion

        private void OnEntityRemoved(BattleEntity entity)
        {
            //If the Partner was removed, set Mario's BattleIndex to 0
            if (BattleIndex > 0 && BattleManager.Instance.GetPartner() == null)
            {
                SetBattleIndex(0, false);
            }
        }
    }
}
