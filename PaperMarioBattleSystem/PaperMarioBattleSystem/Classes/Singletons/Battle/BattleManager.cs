﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles turns in battle
    /// <para>This is a Singleton</para>
    /// </summary>
    public class BattleManager
    {
        #region Singleton Fields

        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }

                return instance;
            }
        }    

        private static BattleManager instance = null;

        #endregion

        #region Enumerations

        public enum BattlePhase
        {
            Player, Enemy
        }

        public enum BattleState
        {
            Init, Turn, TurnEnd, Done
        }

        #endregion

        //Starting positions
        private readonly Vector2 MarioPos = new Vector2(-150, 100);
        private readonly Vector2 PartnerPos = new Vector2(-190, 120);
        private readonly Vector2 EnemyStartPos = new Vector2(150, 125);
        private readonly int PositionXDiff = 30;

        /// <summary>
        /// How many phase cycles (Player and Enemy turns) passed
        /// </summary>
        public int PhaseCycleCount { get; private set; } = 0;

        /// <summary>
        /// Unless scripted, the battle always starts on the player phase, with Mario always going first
        /// </summary>
        private BattlePhase Phase = BattlePhase.Player;

        /// <summary>
        /// The current state of the battle
        /// </summary>
        private BattleState State = BattleState.Init;

        /// <summary>
        /// The current entity going
        /// </summary>
        public BattleEntity EntityTurn { get; private set; } = null;
        private int EnemyTurn = 0;

        /// <summary>
        /// The BattlePlayer in the Front
        /// </summary>
        private BattlePlayer FrontPlayer = null;

        /// <summary>
        /// The BattlePlayer in the Back
        /// </summary>
        private BattlePlayer BackPlayer = null;

        /// <summary>
        /// Mario reference
        /// </summary>
        private BattleMario Mario = null;

        /// <summary>
        /// Partner reference
        /// </summary>
        private BattlePartner Partner = null;

        /// <summary>
        /// Enemy list. Enemies are displayed in order
        /// </summary>
        private List<BattleEnemy> Enemies = new List<BattleEnemy>(BattleGlobals.MaxEnemies);

        /// <summary>
        /// The number of enemies alive
        /// </summary>
        private int EnemiesAlive = 0;

        /// <summary>
        /// Helper property showing the max number of enemies
        /// </summary>
        private int MaxEnemies => Enemies.Capacity;

        /// <summary>
        /// Helper property telling whether enemy spots are available or not
        /// </summary>
        private bool EnemySpotsAvailable => (EnemiesAlive < MaxEnemies);

        private BattleManager()
        {
            SoundManager.Instance.SoundVolume = 0f;

            //TEMPORARY: For compatibility with the old array system until we migrate over completely
            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemies.Add(null);
            }
        }

        /// <summary>
        /// Initializes the battle
        /// </summary>
        /// <param name="mario">Mario</param>
        /// <param name="partner">Mario's partner</param>
        /// <param name="enemies">The enemies, in order</param>
        public void Initialize(BattleMario mario, BattlePartner partner, List<BattleEnemy> enemies)
        {
            Mario = mario;
            Partner = partner;

            //Mario always starts out in the front, and the Partner always starts out in the back
            FrontPlayer = Mario;
            BackPlayer = Partner;

            Mario.Position = MarioPos;
            Mario.SetBattlePosition(MarioPos);

            //Start battle for Mario
            Mario.OnBattleStart();

            if (Partner != null)
            {
                Partner.Position = PartnerPos;
                Partner.SetBattlePosition(PartnerPos);

                //Start battle for the Partner
                Partner.OnBattleStart();
            }

            //Add and initialize enemies
            AddEnemies(enemies);

            StartBattle();
        }

        public void Update()
        {
            //If a turn just ended, update the current state
            if (State == BattleState.TurnEnd)
            {
                TurnStart();
            }

            if (State == BattleState.Turn)
            {
                EntityTurn.TurnUpdate();
            }

            Mario.Update();
            Partner?.Update();

            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemies[i]?.Update();
            }
        }

        public void Draw()
        {
            Mario.Draw();
            Partner?.Draw();

            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemies[i]?.Draw();
            }

            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, $"Current turn: {EntityTurn.Name}", new Vector2(250, 10), Color.White, 0f, Vector2.Zero, 1.3f, .2f);
        }

        /// <summary>
        /// Starts the Battle
        /// </summary>
        public void StartBattle()
        {
            State = BattleState.TurnEnd;
            SwitchPhase(BattlePhase.Player);
        }

        /// <summary>
        /// Ends the Battle
        /// </summary>
        public void EndBattle()
        {
            State = BattleState.Done;
        }

        private void SwitchPhase(BattlePhase phase)
        {
            Phase = phase;

            //NOTE: This breaks Immobilization similar turn-hindering Status Effects for the first entity.
            //We still assume the first entity should go, but what we really need to do is check who can go next
            //Make this use FindNextEntityTurn()

            if (Phase == BattlePhase.Player)
            {
                //Increment the phase cycles when switching to the Player phase
                //This is because the cycle always starts with the Player phase in the Paper Mario games
                PhaseCycleCount++;

                Debug.Log($"Started new phase cycle. Current cycle count: {PhaseCycleCount}");

                Mario.OnPhaseCycleStart();
                Partner.OnPhaseCycleStart();

                Mario.OnPhaseStart();
                Partner.OnPhaseStart();

                for (int i = 0; i < MaxEnemies; i++)
                {
                    Enemies[i]?.OnPhaseEnd();
                    Enemies[i]?.OnPhaseCycleStart();
                }
            }
            else if (Phase == BattlePhase.Enemy)
            {                
                Mario.OnPhaseEnd();
                Partner.OnPhaseEnd();

                for (int i = 0; i < MaxEnemies; i++)
                {
                    Enemies[i]?.OnPhaseStart();
                }
            }

            FindNextEntityTurn();
        }

        /// <summary>
        /// Switches Mario and his Partner's positions and updates the Front and Back player references
        /// </summary>
        /// <param name="frontPlayer"></param>
        /// <param name="backPlayer"></param>
        private void SwitchPlayers(BattlePlayer frontPlayer, BattlePlayer backPlayer)
        {
            Vector2 frontBattlePosition = FrontPlayer.BattlePosition;
            Vector2 backBattlePosition = BackPlayer.BattlePosition;

            FrontPlayer.SetBattlePosition(BackPlayer.BattlePosition);
            FrontPlayer.Position = FrontPlayer.BattlePosition;

            BackPlayer.SetBattlePosition(frontBattlePosition);
            BackPlayer.Position = BackPlayer.BattlePosition;

            FrontPlayer = frontPlayer;
            BackPlayer = backPlayer;
        }

        /// <summary>
        /// Switches Mario and his Partner's positions in battle. This cannot be performed if either have already used their turn
        /// </summary>
        /// <param name="playerType">The PlayerTypes to switch to - either Mario or the Partner</param>
        /// <param name="setTurn">If true, will set the turn to the Player switched to.
        /// This should only be true when manually switching during either Mario or the Partner's turn</param>
        public void SwitchToTurn(PlayerTypes playerType, bool setTurn)
        {
            if (playerType == PlayerTypes.Partner)
            {
                //If we're not setting the turn, this means we're forcing them to switch because the Partner is dead
                //In this case, don't worry about whether the turn was used or not
                if (Partner.UsedTurn == true && setTurn == true)
                {
                    Debug.LogError($"Cannot swap turns with {Partner.Name} because he/she already used his/her turn!");
                    return;
                }

                SwitchPlayers(Partner, Mario);

                //Perform Mario-specific turn end logic
                if (setTurn == true)
                {
                    EntityTurn.OnTurnEnd();
                    EntityTurn = Partner;
                }
            }
            else
            {
                //If we're not setting the turn, this means we're forcing them to switch because the Partner is dead
                //In this case, don't worry about whether the turn was used or not
                if (Mario.UsedTurn == true && setTurn == true)
                {
                    Debug.LogError($"Cannot swap turns with Mario because he already used his turn!");
                    return;
                }

                SwitchPlayers(Mario, Partner);

                if (setTurn == true)
                {
                    //Perform Partner-specific turn end logic
                    EntityTurn.OnTurnEnd();
                    EntityTurn = Mario;
                }
            }

            //NOTE: This doesn't look like it works right for Partner death...
            //Move these lines into the setTurn checks

            //Perform Mario or Partner-specific turn start logic
            EntityTurn.OnTurnStart();

            SoundManager.Instance.PlaySound(SoundManager.Sound.SwitchPartner);
        }

        /// <summary>
        /// Swaps out Mario's current Partner for a different one.
        /// </summary>
        /// <param name="newPartner">The new BattlePartner to take part in battle.</param>
        public void SwapPartner(BattlePartner newPartner)
        {
            BattlePartner oldPartner = Partner;

            Partner = newPartner;
            Partner.Position = oldPartner.Position;
            Partner.SetBattlePosition(oldPartner.BattlePosition);

            //Set the new Partner to use the same max number of turns all Partners have this phase cycle
            Partner.SetMaxTurns(BattlePartner.PartnerMaxTurns);

            //If the entity swapping out partners is the old one increment the turn count for the new partner,
            //as the old one's turn count will be incremented after the action is finished
            if (EntityTurn == oldPartner)
            {
                Partner.SetTurnsUsed(oldPartner.TurnsUsed + 1);
            }
            //Otherwise, the entity swapping out partners must be Mario, so set the new Partner's turn count to the old one's
            //(or an enemy via an attack, but none of those attacks exist in the PM games...I'm hinting at a new attack idea :P)
            else
            {
                Partner.SetTurnsUsed(oldPartner.TurnsUsed);
            }

            //Swap Partner badges with the new Partner
            BattlePartner.SwapPartnerBadges(oldPartner, Partner);

            //Check if the Partner is in the front or back and set the correct reference
            if (oldPartner == FrontPlayer)
            {
                FrontPlayer = Partner;
            }
            else if (oldPartner == BackPlayer)
            {
                BackPlayer = Partner;
            }
        }

        public void TurnStart()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to START turn when the battle is over!");
                return;
            }

            State = BattleState.Turn;

            EntityTurn.OnTurnStart();
        }

        public void TurnEnd()
        {
            if (State == BattleState.Done)
            {
                Debug.LogError($"Attemping to END turn when the battle is over!");
                return;
            }

            EntityTurn.OnTurnEnd();

            //Handle all dead entities
            CheckDeadEntities();

            //Update the battle state
            UpdateBattleState();

            //The battle is finished
            if (State == BattleState.Done)
            {
                return;
            }

            State = BattleState.TurnEnd;

            //Find the next entity to go
            FindNextEntityTurn();
        }

        /// <summary>
        /// Updates the battle state, checking if the battle should be over.
        /// It's game over if Mario has 0 HP, otherwise it's victory if no enemies are alive
        /// </summary>
        private void UpdateBattleState()
        {
            if (Mario.IsDead == true)
            {
                State = BattleState.Done;
                Debug.Log("GAME OVER");
            }
            else if (EnemiesAlive <= 0)
            {
                State = BattleState.Done;
                Mario.PlayAnimation(AnimationGlobals.VictoryName);
                Partner.PlayAnimation(AnimationGlobals.VictoryName);
                Debug.Log("VICTORY");
            }
        }

        /// <summary>
        /// Checks for dead entities and handles them accordingly
        /// </summary>
        private void CheckDeadEntities()
        {
            if (Mario.IsDead == true)
            {
                Mario.PlayAnimation(AnimationGlobals.DeathName);
            }
            if (Partner.IsDead == true)
            {
                //If the Partner died and is in front, switch places with Mario
                if (Partner == FrontPlayer)
                {
                    SwitchToTurn(PlayerTypes.Mario, false);
                }
                
                //NOTE: Don't play this animation again if the Partner is still dead
                Partner.PlayAnimation(AnimationGlobals.DeathName);
            }

            List<BattleEnemy> deadEnemies = new List<BattleEnemy>();

            for (int i = 0; i < MaxEnemies; i++)
            {
                if (Enemies[i] != null && Enemies[i].IsDead == true)
                {
                    deadEnemies.Add(Enemies[i]);
                }
            }

            //Remove enemies from battle here
            RemoveEnemies(deadEnemies, true);
        }

        /// <summary>
        /// Finds the next BattleEntity that should go.
        /// </summary>
        private void FindNextEntityTurn()
        {
            //Enemy phase
            if (Phase == BattlePhase.Enemy)
            {
                //Look through all enemies, starting from the current
                //Find the first one that has turns remaining
                int nextEnemy = -1;
                for (int i = EnemyTurn; i < Enemies.Count; i++)
                {
                    if (Enemies[i] != null && Enemies[i].UsedTurn == false)
                    {
                        nextEnemy = EnemyTurn = i;
                        EntityTurn = Enemies[nextEnemy];
                        break;
                    }
                }

                //If all enemies are done with their turns, go to the player phase
                if (nextEnemy < 0)
                {
                    SwitchPhase(BattlePhase.Player);
                }
            }
            //Player phase
            else
            {
                //If the front player has turns remaining, it goes up next
                if (FrontPlayer.UsedTurn == false)
                {
                    EntityTurn = FrontPlayer;
                }
                //Next check the back player - if it has turns remaining, it goes up
                //The dead check is only for the BackPlayer because any dead Partners
                //get moved to the back. If Mario dies, it shouldn't get here because
                //the battle would be over
                else if (BackPlayer.UsedTurn == false && BackPlayer.IsDead == false)
                {
                    EntityTurn = BackPlayer;
                }
                //Neither player has turns remaining, so go to the enemy phase
                else
                {
                    SwitchPhase(BattlePhase.Enemy);
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// Adds enemies to battle
        /// </summary>
        /// <param name="enemies">A list containing the enemies to add to battle</param>
        public void AddEnemies(List<BattleEnemy> enemies)
        {
            //Look through all enemies and add one to the specified position
            for (int i = 0; i < enemies.Count; i++)
            {
                if (EnemySpotsAvailable == false)
                {
                    Debug.LogError($"Cannot add enemy {enemies[i].Name} because there are no available spots left in battle! Exiting loop!");
                    break;
                }
                int index = FindAvailableEnemyIndex(0);

                BattleEnemy enemy = enemies[i];

                //Set reference and position, then increment the number alive
                Enemies[index] = enemy;

                Vector2 battlepos = EnemyStartPos + new Vector2(PositionXDiff * index, 0);
                enemy.Position = battlepos;
                enemy.SetBattlePosition(battlepos);
                enemy.SetBattleIndex(index);

                //Start battle for the enemy
                enemy.OnBattleStart();

                IncrementEnemiesAlive();
            }
        }

        /// <summary>
        /// Removes enemies from battle
        /// </summary>
        /// <param name="enemies">A list containing the enemies to remove from battle</param>
        /// <param name="removedFromDeath">Whether the enemies are removed because they died in battle. If true, will play the death sound.</param>
        public void RemoveEnemies(List<BattleEnemy> enemies, bool removedFromDeath)
        {
            //Go through all the enemies and remove them from battle
            for (int i = 0; i < enemies.Count; i++)
            {
                if (EnemiesAlive == 0)
                {
                    Debug.LogError($"No enemies currently alive in battle so removing is impossible!");
                    return;
                }

                int enemyIndex = enemies[i].BattleIndex;
                if (enemyIndex < 0)
                {
                    Debug.LogError($"Enemy {enemies[i].Name} cannot be removed from battle because it isn't in battle!");
                    continue;
                }

                //Set to null and decrease number alive
                Enemies[enemyIndex] = null;
                DecrementEnemiesAlive();

                if (removedFromDeath)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDeath);
                }
            }
        }

        /// <summary>
        /// Returns all entities of a specified type in a list.
        /// This method is used internally in the BattleManager to allow for easy manipulation of the returned list.
        /// </summary>
        /// <param name="entityType">The type of entities to return</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states</param>
        /// <returns>Entities matching the type and height states specified</returns>
        private List<BattleEntity> GetEntitiesList(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = new List<BattleEntity>();

            if (entityType == EntityTypes.Enemy)
            {
                entities.AddRange(GetAliveEnemies());
            }
            else if (entityType == EntityTypes.Player)
            {
                entities.Add(FrontPlayer);
                entities.Add(BackPlayer);
            }

            //Filter by height states
            FilterEntitiesByHeights(entities, heightStates);

            return entities;
        }

        /// <summary>
        /// Returns all entities of a specified type in an array
        /// </summary>
        /// <param name="entityType">The type of entities to return</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null, will include entities of all height states</param>
        /// <returns>Entities matching the type and height states specified</returns>
        public BattleEntity[] GetEntities(EntityTypes entityType, params HeightStates[] heightStates)
        {
            List<BattleEntity> entities = GetEntitiesList(entityType, heightStates);
            return entities.ToArray();
        }

        /// <summary>
        /// Returns all alive enemies in an array
        /// </summary>
        /// <returns>An array of all alive enemies. An empty array is returned if no enemies are alive</returns>
        private BattleEnemy[] GetAliveEnemies()
        {
            List<BattleEnemy> aliveEnemies = new List<BattleEnemy>();

            for (int i = 0; i < MaxEnemies; i++)
            {
                if (Enemies[i] != null)
                {
                    aliveEnemies.Add(Enemies[i]);
                }
            }

            return aliveEnemies.ToArray();
        }

        public BattleMario GetMario()
        {
            return Mario;
        }

        public BattlePartner GetPartner()
        {
            return Partner;
        }

        public BattlePlayer GetFrontPlayer()
        {
            return FrontPlayer;
        }

        public BattlePlayer GetBackPlayer()
        {
            return BackPlayer;
        }

        /// <summary>
        /// Filters a set of entities by specified height states. This method is called internally by the BattleManager.
        /// </summary>
        /// <param name="entities">The list of entities to filter. This list is modified directly.</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        private void FilterEntitiesByHeights(List<BattleEntity> entities, params HeightStates[] heightStates)
        {
            //Return immediately if either input is null
            if (entities == null || heightStates == null || heightStates.Length == 0) return;

            for (int i = 0; i < entities.Count; i++)
            {
                BattleEntity entity = entities[i];

                //Remove the entity if it wasn't in any of the height states passed in
                if (heightStates.Contains(entity.HeightState) == false)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Filters a set of entities by specified height states
        /// </summary>
        /// <param name="entities">The array of entities to filter</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        /// <returns>An array of BattleEntities filtered by HeightStates</returns>
        public BattleEntity[] FilterEntitiesByHeights(BattleEntity[] entities, params HeightStates[] heightStates)
        {
            if (entities == null || entities.Length == 0 || heightStates == null || heightStates.Length == 0) return entities;

            List<BattleEntity> filteredEntities = new List<BattleEntity>(entities);
            FilterEntitiesByHeights(filteredEntities, heightStates);

            return filteredEntities.ToArray();
        }

        /// <summary>
        /// Filters out dead BattleEntities from a set
        /// </summary>
        /// <param name="entities">The BattleEntities to filter</param>
        /// <returns>An array of all the alive BattleEntities</returns>
        public BattleEntity[] FilterDeadEntities(BattleEntity[] entities)
        {
            if (entities == null || entities.Length == 0) return entities;

            List<BattleEntity> aliveEntities = new List<BattleEntity>(entities);

            for (int i = 0; i < aliveEntities.Count; i++)
            {
                if (aliveEntities[i].IsDead == true)
                {
                    aliveEntities.RemoveAt(i);
                    i--;
                }
            }

            return aliveEntities.ToArray();
        }

        /// <summary>
        /// Gets all allies of a particular BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity whose allies to get</param>
        /// <param name="heightStates">The height states to filter entities by. Entities with any of the state will be included.
        /// If null or empty, will return the entities passed in</param>
        /// <returns>An array of allies the BattleEntity has</returns>
        public BattleEntity[] GetEntityAllies(BattleEntity entity, params HeightStates[] heightStates)
        {
            List<BattleEntity> allies = GetEntitiesList(entity.EntityType, heightStates);
            allies.Remove(entity);

            //Return all allies
            return allies.ToArray();
        }

        /// <summary>
        /// Gets the BattleEntities adjacent to a particular BattleEntity.
        /// <para>This considers all foreground entities (Ex. Adjacent to Mario would be his Partner and the first Enemy)</para>
        /// </summary>
        /// <param name="entity">The BattleEntity to find entities adjacent to</param>
        /// <param name="getDead">Gets any adjacent entities even if they're dead</param>
        /// <returns>An array of adjacent BattleEntities. If none are adjacent, an empty array.</returns>
        public BattleEntity[] GetAdjacentEntities(BattleEntity entity)
        {
            List<BattleEntity> adjacentEntities = new List<BattleEntity>();

            //If the entity is an enemy, it can either be two Enemies or the front Player and another Enemy
            if (entity.EntityType == EntityTypes.Enemy)
            {
                BattleEnemy enemy = entity as BattleEnemy;

                int enemyIndex = enemy.BattleIndex;
                int prevEnemyIndex = FindOccupiedEnemyIndex(enemyIndex - 1, true);
                int nextEnemyIndex = FindOccupiedEnemyIndex(enemyIndex + 1);

                //Check if there's an Enemy before this one
                if (prevEnemyIndex >= 0) adjacentEntities.Add(Enemies[prevEnemyIndex]);
                //There's no Enemy, so target the Front Player
                else adjacentEntities.Add(FrontPlayer);

                //Check if there's an Enemy after this one
                if (nextEnemyIndex >= 0) adjacentEntities.Add(Enemies[nextEnemyIndex]);
            }
            //If it's a Player, it will be either Mario/Partner and the first enemy
            else if (entity.EntityType == EntityTypes.Player)
            {
                //The previous entity for Players is always Mario or his Partner, unless the latter has 0 HP
                BattlePlayer player = entity as BattlePlayer;

                if (player.PlayerType == PlayerTypes.Partner)
                    adjacentEntities.Add(GetMario());
                else if (player.PlayerType == PlayerTypes.Mario && Partner.IsDead == false)
                    adjacentEntities.Add(GetPartner());

                //Add the next enemy
                int nextEnemy = FindOccupiedEnemyIndex(0);
                if (nextEnemy >= 0) adjacentEntities.Add(Enemies[nextEnemy]);
            }

            return adjacentEntities.ToArray();
        }

        /// <summary>
        /// Gets the BattleEntities behind a particular BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity to find entities behind</param>
        /// <returns>An array of BattleEntities behind the given one. If none are behind, an empty array.</returns>
        public BattleEntity[] GetEntitiesBehind(BattleEntity entity)
        {
            List<BattleEntity> behindEntities = new List<BattleEntity>();

            //If it's a Player, check if the entity is in the front or the back
            if (entity.EntityType == EntityTypes.Player)
            {
                //If the entity is in the front, return the Back player
                if (entity == FrontPlayer)
                    behindEntities.Add(BackPlayer);
            }
            else
            {
                BattleEnemy battleEnemy = entity as BattleEnemy;

                //Get this enemy's BattleIndex
                int enemyIndex = battleEnemy.BattleIndex;

                //Look for all enemies with a BattleIndex greater than this one, which indicates it's behind
                for (int i = 0; i < Enemies.Count; i++)
                {
                    BattleEnemy enemy = Enemies[i];
                    if (enemy != null && enemy.BattleIndex > enemyIndex)
                    {
                        behindEntities.Add(enemy);
                    }
                }
            }

            return behindEntities.ToArray();
        }

        /// <summary>
        /// Finds the next available enemy index
        /// </summary>
        /// <param name="start">The index to start searching from</param>
        /// <param name="backwards">Whether to search backwards or not</param>
        /// <returns>The next available enemy index if found, otherwise -1</returns>
        private int FindAvailableEnemyIndex(int start, bool backwards = false)
        {
            //More code, but more readable too
            if (backwards == false)
            {
                for (int i = start; i < MaxEnemies; i++)
                {
                    if (Enemies[i] == null)
                        return i;
                }
            }
            else
            {
                for (int i = start; i >= 0; i--)
                {
                    if (Enemies[i] == null)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the next occupied enemy index
        /// </summary>
        /// <param name="start">The index to start searching from</param>
        /// <param name="backwards">Whether to search backwards or not</param>
        /// <returns>The next occupied enemy index if found, otherwise -1</returns>
        private int FindOccupiedEnemyIndex(int start, bool backwards = false)
        {
            //More code, but more readable too
            if (backwards == false)
            {
                for (int i = start; i < MaxEnemies; i++)
                {
                    if (Enemies[i] != null)
                        return i;
                }
            }
            else
            {
                for (int i = start; i >= 0; i--)
                {
                    if (Enemies[i] != null)
                        return i;
                }
            }

            return -1;
        }

        private void IncrementEnemiesAlive()
        {
            EnemiesAlive++;
            if (EnemiesAlive > MaxEnemies) EnemiesAlive = MaxEnemies;
        }

        private void DecrementEnemiesAlive()
        {
            EnemiesAlive--;
            if (EnemiesAlive < 0) EnemiesAlive = 0;
        }

        /// <summary>
        /// Gets the position in front of an entity's battle position
        /// </summary>
        /// <param name="entity">The entity to get the position in front of</param>
        /// <returns>A Vector2 with the position in front of the entity</returns>
        public Vector2 GetPositionInFront(BattleEntity entity)
        {
            Vector2 xdiff = new Vector2(PositionXDiff, 0f);
            if (entity.EntityType == EntityTypes.Enemy) xdiff.X = -xdiff.X;

            return (entity.BattlePosition + xdiff);
        }

        /// <summary>
        /// Sorts enemies by battle indices, with lower indices appearing first
        /// </summary>
        private int SortEnemiesByBattleIndex(BattleEnemy enemy1, BattleEnemy enemy2)
        {
            if (enemy1 == null)
                return 1;
            else if (enemy2 == null)
                return -1;

            //Compare battle indices
            if (enemy1.BattleIndex < enemy2.BattleIndex)
                return -1;
            else if (enemy1.BattleIndex > enemy2.BattleIndex)
                return 1;

            return 0;
        }

        #endregion
    }
}
