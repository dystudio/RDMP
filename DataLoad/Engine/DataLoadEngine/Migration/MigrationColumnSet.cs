﻿using System;
using System.Collections.Generic;
using System.Linq;
using CatalogueLibrary.Data;
using CatalogueLibrary.Triggers;
using ReusableLibraryCode;
using ReusableLibraryCode.DatabaseHelpers.Discovery;

namespace DataLoadEngine.Migration
{
    /// <summary>
    /// Defines the role of every field involved in a STAGING to LIVE migration during DLE execution.  When performing a selective UPDATE it is important not to
    /// overwrite current records with new records where the 'newness' is an artifact of data loading rather than source data.  For example the field 
    /// hic_dataLoadRunID will always be different between STAGING and LIVE.  This class stores which columns should be used to identify records which exist
    /// in both (PrimaryKeys), which columns indicate significant change and should be promoted (FieldsToDiff) and which are not significant changes but should 
    /// be copied across anyway in the event that the row is new or there is a difference in another significant field in that record (FieldsToUpdate).
    /// </summary>
    public class MigrationColumnSet
    {
        public DiscoveredTable SourceTable { get; set; }
        public DiscoveredTable DestinationTable { get; set; }

        public DiscoveredColumn[] PrimaryKeys { get; private set; }

        public static List<string> GetStandardColumnNames()
        {
            return new List<string> { SpecialFieldNames.DataLoadRunID, SpecialFieldNames.ValidFrom};
        }

        /// <summary>
        /// Fields that will have their values compared for change, to decide whether to overwrite destination data with source data. (some fields might not matter
        /// if they are different e.g. dataLoadRunID)
        /// </summary>
        public List<DiscoveredColumn> FieldsToDiff { get; set; }

        /// <summary>
        /// Fields that will have their values copied across to the new table (this is a superset of fields to diff, and also includes all primary keys).  Note
        /// that the non-standard columns (data load run and valid from do not appear in this list, you are intended to handle their update yourself)
        /// </summary>
        public List<DiscoveredColumn> FieldsToUpdate { get; set; }

        public MigrationColumnSet(DiscoveredTable from, DiscoveredTable to, IMigrationFieldProcessor migrationFieldProcessor)
        {
            var fromCols = from.DiscoverColumns();
            var toCols = to.DiscoverColumns();

            migrationFieldProcessor.ValidateFields(fromCols, toCols);

            SourceTable = from;
            DestinationTable = to;

            PrimaryKeys = fromCols.Where(c=>c.IsPrimaryKey).ToArray();
            FieldsToDiff = new List<DiscoveredColumn>();
            FieldsToUpdate = new List<DiscoveredColumn>();

            foreach (DiscoveredColumn pk in PrimaryKeys)
                if(!toCols.Any(f=>f.GetRuntimeName().Equals(pk.GetRuntimeName(),StringComparison.CurrentCultureIgnoreCase)))
                    throw new MissingFieldException("Column " + pk + " is missing from either the destination table");

            if(!PrimaryKeys.Any())
                throw new Exception("There are no primary keys declared in table " + from);
            
            //figure out things to migrate and whether they matter to diffing
            foreach (DiscoveredColumn field in fromCols)
            {
                if (
                    field.GetRuntimeName().Equals(SpecialFieldNames.DataLoadRunID,StringComparison.CurrentCultureIgnoreCase) ||
                    field.GetRuntimeName().Equals(SpecialFieldNames.ValidFrom,StringComparison.CurrentCultureIgnoreCase))
                    continue;

                if (!toCols.Any(c=>c.GetRuntimeName().Equals(field.GetRuntimeName(),StringComparison.CurrentCultureIgnoreCase)))
                    throw new MissingFieldException("Field " + field + " is missing from destination table");

                migrationFieldProcessor.AssignFieldsForProcessing(field, FieldsToDiff, FieldsToUpdate);
            }
        }
    }
}