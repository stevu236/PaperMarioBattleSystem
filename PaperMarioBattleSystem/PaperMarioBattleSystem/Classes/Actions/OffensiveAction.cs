﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleAction that can deal damage to BattleEntities
    /// </summary>
    public abstract class OffensiveAction : CommandMoveAction
    {
        /// <summary>
        /// The base damage of the action
        /// </summary>
        public int BaseDamage { get; protected set; } = 0;

        /// <summary>
        /// The type of Elemental damage this action deals
        /// </summary>
        public Elements Element { get; protected set; } = Elements.Normal;

        /// <summary>
        /// The type of contact this action makes
        /// </summary>
        public ContactTypes ContactType { get; protected set; } = ContactTypes.None;

        /// <summary>
        /// The Status Effects this action can afflict
        /// </summary>
        public StatusEffect[] StatusesInflicted { get; protected set; } = null;

        /// <summary>
        /// Whether the action deals Piercing damage or not
        /// </summary>
        public bool Piercing { get; protected set; } = false;

        protected OffensiveAction()
        {

        }

        /// <summary>
        /// Attempt to deal damage to a set of entities with this BattleAction.
        /// <para>Based on the ContactType of this BattleAction, this can fail, resulting in an interruption.
        /// In the event of an interruption, no further entities are tested, the ActionCommand is ended, and 
        /// we go into the Interruption branch</para>
        /// </summary>
        /// <param name="damage">The damage the BattleAction deals to the entity if the attempt was successful</param>
        /// <param name="entities">The BattleEntities to attempt to inflict damage on</param>
        /// <param name="isTotalDamage">Whether the damage passed in is the total damage or not.
        /// If false, the total damage will be calculated</param>
        /// <returns>An int array containing the damage dealt to each BattleEntity targeted, in order</returns>
        protected int[] AttemptDamage(int damage, BattleEntity[] entities, bool isTotalDamage)
        {
            if (entities == null || entities.Length == 0)
            {
                Debug.LogWarning($"{nameof(entities)} is null or empty in {nameof(AttemptDamage)} for Action {Name}!");
                return new int[0];
            }

            int totalDamage = isTotalDamage == true ? damage : GetTotalDamage(damage);

            //Check for the All or Nothing Badge
            //If it's equipped, add 1 if the Action Command succeeded, otherwise set the damage to the minimum value
            int allOrNothingCount = User.EntityProperties.GetMiscProperty(MiscProperty.AllOrNothingCount).IntValue;
            if (allOrNothingCount > 0)
            {
                if (CommandResult == ActionCommand.CommandResults.Success)
                {
                    totalDamage += allOrNothingCount;
                }
                else if (CommandResult == ActionCommand.CommandResults.Failure)
                {
                    totalDamage = int.MinValue;
                }
            }

            //The damage dealt to each BattleEntity
            int[] damageValues = new int[entities.Length];

            //Go through all the entities and attempt damage
            for (int i = 0; i < entities.Length; i++)
            {
                BattleEntity victim = entities[i];

                InteractionResult finalResult = Interactions.GetDamageInteraction(new InteractionParamHolder(User, victim, totalDamage, Element, Piercing, ContactType, StatusesInflicted));

                //Set the total damage dealt to the victim
                damageValues[i] = finalResult.VictimResult.TotalDamage;

                //Make the victim take damage upon a PartialSuccess or a Success
                if (finalResult.VictimResult.HasValue == true)
                {
                    //Check if the attacker hit
                    if (finalResult.VictimResult.Hit == true)
                    {
                        finalResult.VictimResult.Entity.TakeDamage(finalResult.VictimResult);
                    }
                    //Handle a miss otherwise
                    else
                    {
                        OnMiss();
                    }
                }

                //Make the attacker take damage upon a PartialSuccess or a Failure
                //Break out of the loop when the attacker takes damage
                if (finalResult.AttackerResult.HasValue == true)
                {
                    finalResult.AttackerResult.Entity.TakeDamage(finalResult.AttackerResult);

                    break;
                }
            }

            return damageValues;
        }

        /// <summary>
        /// Convenience function for attempting damage with only one entity.
        /// </summary>
        /// <param name="damage">The damage the BattleAction deals to the entity if the attempt was successful</param>
        /// <param name="entity">The BattleEntity to attempt to inflict damage on</param>
        /// <param name="isTotalDamage">Whether the damage passed in is the total damage or not.
        /// If false, the total damage will be calculated</param>
        protected void AttemptDamage(int damage, BattleEntity entity, bool isTotalDamage)
        {
            AttemptDamage(damage, new BattleEntity[] { entity }, isTotalDamage);
        }

        /// <summary>
        /// Gets the total raw damage a BattleEntity can deal using this BattleAction.
        /// This factors in a BattleEntity's Attack stat and anything else that may influence the damage dealt.
        /// </summary>
        /// <param name="actionDamage">The damage the BattleAction deals</param>
        /// <returns>An int with the total raw damage the BattleEntity can deal when using this BattleAction</returns>
        protected int GetTotalDamage(int actionDamage)
        {
            int totalDamage = actionDamage + User.BattleStats.Attack;

            return totalDamage;
        }
    }
}