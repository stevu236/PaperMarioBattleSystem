﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A class for defining Extension Methods
    /// </summary>
    public static class Extensions
    {
        #region Texture2D Extensions

        /// <summary>
        /// Gets the origin of a Texture2D by ratio instead of specifying width and height
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this Texture2D texture2D, float x, float y)
        {
            int xVal = (int)(texture2D.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(texture2D.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Texture2D
        /// </summary>
        /// <param name="texture2D">The texture to get the origin for</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this Texture2D texture2D)
        {
            return texture2D.GetOrigin(.5f, .5f);
        }

        #endregion

        #region SpriteFont Extensions

        /// <summary>
        /// Gets the origin of a SpriteFont by ratio instead of specifying width and height
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for</param>
        /// <param name="text">The text to be displayed</param>
        /// <param name="x">The X ratio of the origin, between 0 and 1</param>
        /// <param name="y">The Y ratio of the origin, between 0 and 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this SpriteFont spriteFont, string text, float x, float y)
        {
            if (string.IsNullOrEmpty(text) == true) return Vector2.Zero;

            Vector2 size = spriteFont.MeasureString(text);
            size.X *= UtilityGlobals.Clamp(x, 0f, 1f);
            size.Y *= UtilityGlobals.Clamp(y, 0f, 1f);

            return size;
        }

        /// <summary>
        /// Gets the center origin of a SpriteFont
        /// </summary>
        /// <param name="spriteFont">The font to get the origin for</param>
        /// <param name="text">The text to be displayed</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this SpriteFont spriteFont, string text)
        {
            return spriteFont.GetOrigin(text, .5f, .5f);
        }

        #endregion

        #region Rectangle Extensions

        /// <summary>
        /// Gets the origin of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for</param>
        /// <param name="x">The X ratio of the origin, from 0 to 1</param>
        /// <param name="y">The Y ratio of the origin, from 0 to 1</param>
        /// <returns>A Vector2 with the origin</returns>
        public static Vector2 GetOrigin(this Rectangle rectangle, float x, float y)
        {
            int xVal = (int)(rectangle.Width * UtilityGlobals.Clamp(x, 0f, 1f));
            int yVal = (int)(rectangle.Height * UtilityGlobals.Clamp(y, 0f, 1f));

            return new Vector2(xVal, yVal);
        }

        /// <summary>
        /// Gets the center origin of a Rectangle
        /// </summary>
        /// <param name="rectangle">The Rectangle to get the origin for</param>
        /// <returns>A Vector2 with the center origin</returns>
        public static Vector2 GetCenterOrigin(this Rectangle rectangle)
        {
            return rectangle.GetOrigin(.5f, .5f);
        }

        #endregion

        #region Vector2 Extensions

        /// <summary>
        /// Halves the Vector2
        /// </summary>
        /// <param name="vector2">The Vector2 to halve</param>
        /// <returns>A Vector2 with the X and Y components halved</returns>
        public static Vector2 Halve(this Vector2 vector2)
        {
            return vector2 / 2f;
        }

        /// <summary>
        /// Halves the Vector2, truncating the X and Y components to the nearest integer
        /// </summary>
        /// <param name="vector2">The Vector2 to halve</param>
        /// <returns>A Vector2 with the X and Y components halved as integer values</returns>
        public static Vector2 HalveInt(this Vector2 vector2)
        {
            return new Vector2((int)(vector2.X / 2f), (int)(vector2.Y / 2f));
        }

        #endregion

        #region BattleEntity Extensions

        /// <summary>
        /// A helper method for adding an integer AdditionalProperty to the BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        /// <param name="additionalProperty">The AdditionalProperty to add. If the BattleEntity already has it, it must have an integer value.</param>
        /// <param name="addVal">The amount to add to the value of this AdditionalProperty.</param>
        public static void AddIntAdditionalProperty(this BattleEntity entity, AdditionalProperty additionalProperty, int addVal)
        {
            int value = entity.EntityProperties.GetAdditionalProperty<int>(additionalProperty) + addVal;
            entity.EntityProperties.AddAdditionalProperty(additionalProperty, value);
        }

        /// <summary>
        /// A helper method for subtracting an integer AdditionalProperty from a BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        /// <param name="additionalProperty">The AdditionalProperty to subtract from. If the BattleEntity already has it, it must have an integer value.</param>
        /// <param name="subtractVal">The amount to subtract from the value of this AdditionalProperty.</param>
        /// <param name="removeWhenZeroOrLess">Whether to keep the property removed if the value afterwards is less than or equal to 0.</param>
        public static void SubtractIntAdditionalProperty(this BattleEntity entity, AdditionalProperty additionalProperty, int subtractVal, bool removeWhenZeroOrLess = true)
        {
            int value = entity.EntityProperties.GetAdditionalProperty<int>(additionalProperty) - subtractVal;
            entity.EntityProperties.RemoveAdditionalProperty(additionalProperty);
            if (removeWhenZeroOrLess == false || value > 0)
            {
                entity.EntityProperties.AddAdditionalProperty(additionalProperty, value);
            }
        }

        /// <summary>
        /// Adds a ShowHP AdditionalProperty to the BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        public static void AddShowHPProperty(this BattleEntity entity)
        {
            AddIntAdditionalProperty(entity, AdditionalProperty.ShowHP, 1);
        }

        /// <summary>
        /// Subtracts a ShowHP AdditionalProperty from the BattleEntity.
        /// </summary>
        /// <param name="entity">The BattleEntity.</param>
        public static void SubtractShowHPProperty(this BattleEntity entity)
        {
            SubtractIntAdditionalProperty(entity, AdditionalProperty.ShowHP, 1);
        }

        /// <summary>
        /// Tells if the BattleEntity is Invincible or not.
        /// </summary>
        /// <param name="entity">The BattleEntity to check for being Invincible.</param>
        /// <returns>true if the BattleEntity has the Invincible AdditionalProperty, otherwise false.</returns>
        public static bool IsInvincible(this BattleEntity entity)
        {
            return (entity.EntityProperties.GetAdditionalProperty<int>(AdditionalProperty.Invincible) > 0);
        }

        #endregion

        #region Dictionary Extensions

        /// <summary>
        /// A convenient wrapper around Dictionary.TryGetValue().
        /// </summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dict">The Dictionary to get the value from.</param>
        /// <param name="key">The key to get the value of.</param>
        /// <returns>A DictionaryVal with a bool indicating if the value was found and the associated value.</returns>
        public static DictionaryVal<V> GetValueOrDefault<K, V>(this Dictionary<K, V> dict, K key)
        {
            V val;
            bool found = dict.TryGetValue(key, out val);

            return new DictionaryVal<V>(found, val);
        }

        #endregion

        #region SortedDictionary Extensions

        /// <summary>
        /// A convenient wrapper around SortedDictionary.TryGetValue().
        /// </summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dict">The SortedDictionary to get the value from.</param>
        /// <param name="key">The key to get the value of.</param>
        /// <returns>A DictionaryVal with a bool indicating if the value was found and the associated value.</returns>
        public static DictionaryVal<V> GetValueOrDefault<K, V>(this SortedDictionary<K, V> dict, K key)
        {
            V val;
            bool found = dict.TryGetValue(key, out val);

            return new DictionaryVal<V>(found, val);
        }

        #endregion
    }
}
