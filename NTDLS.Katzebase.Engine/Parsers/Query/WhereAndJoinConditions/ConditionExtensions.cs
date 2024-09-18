﻿using NTDLS.Katzebase.Engine.Parsers.Query.Fields;

namespace NTDLS.Katzebase.Engine.Parsers.Query.WhereAndJoinConditions
{
    internal static class ConditionExtensions
    {
        /// <summary>
        /// Recursively rolls through nested conditions, producing a flat list of the group references. 
        /// </summary>
        public static List<ConditionGroup> FlattenToGroups(this List<ICondition> givenConditions)
        {
            var results = new List<ConditionGroup>();
            FlattenToGroups(givenConditions, results);
            return results;

            static void FlattenToGroups(List<ICondition> conditions, List<ConditionGroup> refGroups)
            {
                foreach (var condition in conditions)
                {
                    if (condition is ConditionGroup group)
                    {
                        refGroups.Add(group);
                        FlattenToGroups(group.Entries, refGroups);
                    }
                }
            }
        }

        /// <summary>
        /// Recursively rolls through nested conditions, producing a flat list of the group references. 
        /// </summary>
        public static List<ConditionGroup> FlattenToGroups(this ConditionCollection givenConditions)
            => FlattenToGroups(givenConditions.Entries);

        /// <summary>
        /// Recursively rolls through nested conditions, producing a flat list of the entry references.
        /// </summary>
        public static List<ConditionEntry> FlattenToEntries(this List<ICondition> givenConditions)
        {
            var results = new List<ConditionEntry>();
            FlattenToEntries(givenConditions, results);
            return results;

            static void FlattenToEntries(List<ICondition> conditions, List<ConditionEntry> refEntries)
            {
                foreach (var condition in conditions)
                {
                    if (condition is ConditionGroup group)
                    {
                        FlattenToEntries(group.Entries, refEntries);
                    }
                    else if (condition is ConditionEntry entry)
                    {
                        refEntries.Add(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Recursively rolls through nested conditions, producing a flat list of the entry references.
        /// </summary>
        public static List<ConditionEntry> FlattenToEntries(this ConditionCollection givenConditions)
            => FlattenToEntries(givenConditions.Entries);

        /// <summary>
        /// Recursively rolls through nested conditions, producing a flat list of the entry references where the left side is a document identifier.
        /// </summary>
        public static List<ConditionEntry> FlattenToDocumentIdentifiers(this List<ICondition> givenConditions)
        {
            var results = new List<ConditionEntry>();
            FlattenToDocumentIdentifiers(givenConditions, results);
            return results;

            static void FlattenToDocumentIdentifiers(List<ICondition> conditions, List<ConditionEntry> refConditions)
            {
                foreach (var condition in conditions)
                {
                    if (condition is ConditionGroup group)
                    {
                        FlattenToDocumentIdentifiers(group.Entries, refConditions);
                    }
                    else if (condition is ConditionEntry entry)
                    {
                        if (entry.Left is QueryFieldDocumentIdentifier)
                        {
                            refConditions.Add(entry);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        /// <summary>
        /// Recursively rolls through nested conditions, producing a flat list of the entry references where the left side is a document identifier.
        /// </summary>
        public static List<ConditionEntry> FlattenToDocumentIdentifiers(this ConditionCollection givenConditions)
            => FlattenToDocumentIdentifiers(givenConditions.Entries);
    }
}
