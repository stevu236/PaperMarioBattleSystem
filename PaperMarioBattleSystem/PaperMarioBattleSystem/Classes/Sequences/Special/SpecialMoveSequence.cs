﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base Sequence for all Special Moves.
    /// <para>The starts of Star Spirit and Crystal Star Special Moves are different, 
    /// so they will need done in separate derived classes.</para>
    /// </summary>
    public abstract class SpecialMoveSequence : Sequence
    {
        protected double WalkDuration = 500d;

        protected SpecialMoveAction SpecialAction { get; private set; } = null;

        public SpecialMoveSequence(SpecialMoveAction specialAction) : base(specialAction)
        {
            SpecialAction = specialAction;
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(User.GetIdleAnim());
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch(SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
