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
    /// Defines all types of interactions that exist among BattleEntities.
    /// <para>This includes Element-PhysicalAttribute interactions, ContactType-PhysicalAttribute interactions, and more.</para>
    /// </summary>
    public static class Interactions
    {
        #region Fields    
    
        /// <summary>
        /// The table that determines the result of a particular ContactType and a particular PhysicalAttribute.
        /// <para>The default value is a Success, meaning the ContactType will deal damage. A Failure indicates a backfire (Ex. Mario
        /// jumping on a spiked enemy)</para>
        /// </summary>
        private static Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResultInfo>> ContactTable = null;

        #endregion

        static Interactions()
        {
            InitalizeContactTable();
        }

        #region Contact Table Initialization Methods

        private static void InitalizeContactTable()
        {
            ContactTable = new Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResultInfo>>();

            InitializeNoneContactTable();
            InitializeJumpContactTable();
            InitializeHammerContactTable();
        }

        private static void InitializeNoneContactTable()
        {

        }

        private static void InitializeJumpContactTable()
        {
            ContactTable.Add(ContactTypes.JumpContact, new Dictionary<PhysicalAttributes, ContactResultInfo>()
            {
                { PhysicalAttributes.Spiked, new ContactResultInfo(Elements.Sharp, ContactResult.Failure) },
                { PhysicalAttributes.Electrified, new ContactResultInfo(Elements.Electric, ContactResult.PartialSuccess) },
                { PhysicalAttributes.Fiery, new ContactResultInfo(Elements.Fire, ContactResult.Failure) }
            });
        }

        private static void InitializeHammerContactTable()
        {

        }

        #endregion

        #region Contact Table Methods

        /// <summary>
        /// Gets the result of a ContactType on a set of PhysicalAttributes
        /// </summary>
        /// <param name="contactType">The ContactType performed</param>
        /// <param name="physAttributes">The set of PhysicalAttributes to test against</param>
        /// <param name="attributesToIgnore">A set of PhysicalAttributes to ignore</param>
        /// <returns>A ContactResultInfo of the interaction</returns>
        public static ContactResultInfo GetContactResult(ContactTypes contactType, PhysicalAttributes[] physAttributes, params PhysicalAttributes[] attributesToIgnore)
        {
            //Return the default value
            if (ContactTable.ContainsKey(contactType) == false || physAttributes == null)
            {
                Debug.LogWarning($"{nameof(physAttributes)} array is null or {nameof(ContactTable)} does not contain the ContactType {contactType}!");
                return ContactResultInfo.Default;
            }

            //Look through the attributes and find the first match
            for (int i = 0; i < physAttributes.Length; i++)
            {
                Dictionary<PhysicalAttributes, ContactResultInfo> tableForContact = ContactTable[contactType];
                PhysicalAttributes attribute = physAttributes[i];

                //If this attribute is ignored, move onto the next
                if (attributesToIgnore?.Contains(attribute) == true)
                    continue;

                if (tableForContact.ContainsKey(attribute) == true)
                {
                    ContactResultInfo contactResult = tableForContact[attribute];
                    return contactResult;
                }
            }

            return ContactResultInfo.Default;
        }

        #endregion
    }
}