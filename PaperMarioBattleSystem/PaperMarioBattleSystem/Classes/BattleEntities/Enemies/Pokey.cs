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
    /// A Pokey enemy. It has 3 segments.
    /// </summary>
    public class Pokey : BattleEnemy, ISegmentEntity, ITattleableEntity
    {
        private const int SegmentHeight = 23;

        public ISegmentBehavior SegmentBehavior { get; protected set; } = null;

        /// <summary>
        /// The visual segments of the Pokey.
        /// <para>NOTE: There should be a better way to do this, but this'll work for now.</para>
        /// </summary>
        protected List<CroppedTexture2D> VisualSegments = new List<CroppedTexture2D>();

        public Pokey() : base(new Stats(11, 4, 0, 2, 0))
        {
            Name = "Pokey";

            AIBehavior = new GoombaAI(this);

            #region Entity Property Setup

            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Sleep, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Dizzy, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Confused, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Tiny, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Stop, new StatusPropertyHolder(80, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.DEFDown, new StatusPropertyHolder(95, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Burn, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Frozen, new StatusPropertyHolder(60, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Fright, new StatusPropertyHolder(100, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.Blown, new StatusPropertyHolder(90, 0));
            EntityProperties.AddStatusProperty(Enumerations.StatusTypes.KO, new StatusPropertyHolder(100, 0));

            EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Spiked);
            EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Constant, Enumerations.PhysicalAttributes.Spiked,
                Enumerations.Elements.Sharp, new Enumerations.ContactTypes[] { Enumerations.ContactTypes.Latch, Enumerations.ContactTypes.TopDirect, Enumerations.ContactTypes.SideDirect },
                new Enumerations.ContactProperties[] { Enumerations.ContactProperties.None },
                Enumerations.ContactResult.Failure, Enumerations.ContactResult.Failure, 1, null));

            EntityProperties.SetVulnerableDamageEffects(Enumerations.DamageEffects.RemovesSegment);

            #endregion

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies/Pokey.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new ReverseAnimation(null, AnimationGlobals.InfiniteLoop,
                new Animation.Frame(new Rectangle(33, 65, 30, 30), 200d),
                new Animation.Frame(new Rectangle(97, 65, 30, 30), 200d),
                new Animation.Frame(new Rectangle(65, 66, 30, 29), 200d, new Vector2(0, 1))));
            //AnimManager.AddAnimationChildFrame(AnimationGlobals.IdleName)
        }

        public override void CleanUp()
        {
            base.CleanUp();
            
            SegmentBehavior?.CleanUp();

            if (VisualSegments != null)
            {
                VisualSegments.Clear();
                VisualSegments = null;
            }
        }

        protected virtual void SetSegmentBehavior()
        {
            SegmentBehavior?.CleanUp();
            SegmentBehavior = new PokeySegmentBehavior(this, 3, Enumerations.DamageEffects.RemovesSegment);
        }

        public override void OnBattleStart()
        {
            base.OnBattleStart();

            SetSegmentBehavior();

            Vector2 pos = BattlePosition;

            //Add the visual segments
            for (int i = 0; i < SegmentBehavior.CurSegmentCount; i++)
            {
                VisualSegments.Add(new CroppedTexture2D(AnimManager.SpriteSheet, new Rectangle(99, 38, 28, 23)));
                pos.Y -= SegmentHeight;
            }

            if (pos != BattlePosition)
            {
                SetBattlePosition(pos);
                Position = pos;
            }
        }

        protected override void OnTakeDamage(InteractionHolder damageInfo)
        {
            base.OnTakeDamage(damageInfo);
        }

        #region Tattle Info

        public bool CanBeTattled { get; set; } = true;

        public string[] GetTattleLogEntry()
        {
            return new string[]
            {
                $"HP: {BattleStats.MaxHP} Attack: {BattleStats.BaseAttack}\nDefense: {BattleStats.BaseDefense}",
                $"A cactus ghoul covered from\nhead to base in nasty spines.",
                "It attacks by lobbing sections\nof itself at you, and can even",
                "call other Pokeys to come\nfight alongside it."
            };
        }

        public string GetTattleDescription()
        {
            return "That's a Pokey. It's a cactus ghoul that's got nasty spines all over its body.\n<k><p>" +
                   $"Max HP is {BattleStats.MaxHP}, Attack is {BattleStats.BaseAttack}, and Defense is {BattleStats.BaseDefense}.\n<k><p>" +
                   "Look at those spines... Those would TOTALLY hurt. If you stomp on it, you'll regret it.\n<k><p>" +
                   "Pokeys attack by lobbing parts of their bodies and by charging at you...\n<k><p>" +
                   "They can even call friends in for help, so be quick about taking them out.<k>";
        }

        #endregion

        public override void Update()
        {
            base.Update();

            //Find segment differences
            if (VisualSegments != null && SegmentBehavior != null)
            {
                //Check if the segment count differs
                int diff = VisualSegments.Count - (int)SegmentBehavior.CurSegmentCount;

                //We removed segments
                if (diff > 0)
                {
                    VisualSegments.RemoveRange(0, diff);
                }
                //We added segments
                else if (diff < 0)
                {
                    int absDiff = -diff;
                    for (int i = 0; i < absDiff; i++)
                    {
                        VisualSegments.Add(new CroppedTexture2D(AnimManager.SpriteSheet, new Rectangle(99, 38, 28, 23)));
                    }
                }

                //If segments have been added or removed, update the Pokey's Position and BattlePosition
                if (diff != 0)
                {
                    Vector2 pos = BattlePosition;
                    pos.Y += (diff * SegmentHeight);

                    if (pos != BattlePosition)
                    {
                        SetBattlePosition(pos);
                        Position = new Vector2(Position.X, pos.Y);
                    }
                }
            }
        }

        protected override void DrawEntity()
        {
            base.DrawEntity();

            Animation.Frame curFrame = AnimManager.CurrentAnim.CurFrame;

            //Draw the visual segments
            for (int i = 0; i < VisualSegments.Count; i++)
            {
                Vector2 pos = Position;
                pos.Y += (i * (Scale.Y * SegmentHeight)) + ((curFrame.PosOffset.Y + curFrame.DrawRegion.Height) * Scale.Y);
                SpriteRenderer.Instance.Draw(VisualSegments[i].Tex, pos, VisualSegments[i].SourceRect, TintColor, 0f, Vector2.Zero, Scale, EntityType == Enumerations.EntityTypes.Player, false, .09f);
            }
        }
    }
}
