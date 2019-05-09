// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MapsDirectlyToDatabaseTable;
using MapsDirectlyToDatabaseTable.Attributes;
using MapsDirectlyToDatabaseTable.Injection;
using Rdmp.Core.Curation.DataHelper;
using Rdmp.Core.QueryBuilding;
using Rdmp.Core.QueryBuilding.SyntaxChecking;
using Rdmp.Core.Repositories;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;

namespace Rdmp.Core.Curation.Data.Aggregation
{
    /// <summary>
    /// This class allows you to associate a specific extractioninformation for use in aggregate generation.  For example a dataset might have a date field AdmissionDate which you
    /// want to create an aggregate configuration (when patients were admitted) over time.  However the class also allows you to specify new SelectSQL which can change how the field
    /// is extracted e.g. you might want to change "[MyDatabase].[MyTable].[AdmissionDate]" into "YEAR([MyDatabase].[MyTable].[AdmissionDate]) as AdmissionDate" 
    /// </summary>
    public class AggregateDimension : DatabaseEntity, ISaveable, IDeleteable, IColumn, IHasDependencies, IInjectKnown<ExtractionInformation>
    {
        #region Database Properties

        private int _aggregateConfigurationID;
        private int _extractionInformationID;
        private string _alias;
        private string _selectSQL;
        private int _order;
        

        /// <summary>
        /// An <see cref="AggregateDimension"/> is a column in the SELECT, GROUP BY and ORDER BY sections of an <see cref="AggregateConfiguration"/>.  This property returns
        /// the ID of the <see cref="AggregateConfiguration"/> that this column is declared on.  
        /// </summary>
        public int AggregateConfiguration_ID
        {
            get { return _aggregateConfigurationID; }
            set { SetField(ref  _aggregateConfigurationID, value); }
        }

        /// <summary>
        /// An <see cref="AggregateDimension"/> is a column in the SELECT, GROUP BY and/or ORDER BY sections of an <see cref="AggregateConfiguration"/>.  The column must have
        /// come from an extractable column in the parent <see cref="Catalogue"/>.  The Catalogue column definition is an <see cref="ExtractionInformation"/> and documents the
        /// master SELECT Sql (which can be overriden in the current AggregateDimension) as well as what the underlying <see cref="ColumnInfo"/> / <see cref="TableInfo"/>. 
        /// 
        /// <para>This property is the ID of the associated Catalogue master <see cref="ExtractionInformation"/>.</para>
        /// </summary>
        public int ExtractionInformation_ID
        {
            get { return _extractionInformationID; }
            set { SetField(ref  _extractionInformationID, value); }
        }

        /// <summary>
        /// Specifies the column alias section of the SELECT statement.  When building the query (See AggregateBuilder) the Alias will be added in the SELECT section
        /// of the query generated e.g. if the Alias is 'Bob' and the SelectSQL is 'GetDate()' then the resultant line of SELECT in the query will be 'GetDate() as Bob'.
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { SetField(ref  _alias, value); }
        }

        /// <summary>
        /// An <see cref="AggregateDimension"/> is a column in the SELECT, GROUP BY and/or ORDER BY sections of an <see cref="AggregateConfiguration"/>.  This property defines
        /// the Sql that should appear in SELECT, GROUP BY and/or ORDER BY sections of the query when it is built by the AggregateBuilder.  This will start out
        /// with the exact same string as the parent <see cref="ExtractionInformation_ID"/> but can be changed as needed e.g. wrapping in UPPER.  If you change the SelectSQL
        /// to a scalar function you should add an <see cref="Alias"/>.
        /// </summary>
        [Sql]
        public string SelectSQL
        {
            get { return _selectSQL; }
            set { SetField(ref  _selectSQL, value); }
        }

        /// <summary>
        /// An <see cref="AggregateDimension"/> is a column in the SELECT, GROUP BY and/or ORDER BY sections of an <see cref="AggregateConfiguration"/>.  The Order property determines
        /// where in the SELECT, GROUP BY and/or ORDER BY list the current <see cref="AggregateDimension"/> will appear relative to the other AggregateDimensions in the
        ///  <see cref="AggregateConfiguration"/>.
        /// </summary>
        public int Order
        {
            get { return _order; }
            set { SetField(ref  _order, value); }
        }
        #endregion

        
        #region Relationships

        /// <inheritdoc cref="IColumn.HashOnDataRelease"/>
        [NoMappingToDatabase]
        public bool HashOnDataRelease { get { return _knownExtractionInformation.Value.HashOnDataRelease; } }

        /// <inheritdoc cref="IColumn.IsExtractionIdentifier"/>
        [NoMappingToDatabase]
        public bool IsExtractionIdentifier { get { return _knownExtractionInformation.Value.IsExtractionIdentifier; } }

        /// <inheritdoc cref="IColumn.IsPrimaryKey"/>
        [NoMappingToDatabase]
        public bool IsPrimaryKey { get { return _knownExtractionInformation.Value.IsPrimaryKey; } }

        /// <inheritdoc cref="IColumn.ColumnInfo"/>
        [NoMappingToDatabase]
        public ColumnInfo ColumnInfo { get { return _knownExtractionInformation.Value.ColumnInfo; } }

        /// <summary>
        /// An <see cref="AggregateConfiguration"/> can have a single <see cref="AggregateContinuousDateAxis"/> declared on it (if it is not functioning in a cohort identification
        /// capacity).  This property will return the axis if this AggregateDimension has one declared on it.
        /// </summary>
        /// <seealso cref="Aggregation.AggregateContinuousDateAxis.AggregateDimension_ID"/>
        [NoMappingToDatabase]
        public AggregateContinuousDateAxis AggregateContinuousDateAxis
        {
            get
            {
                return Repository.GetAllObjectsWithParent<AggregateContinuousDateAxis>(this).SingleOrDefault();
            }
        }

        /// <inheritdoc cref="ExtractionInformation_ID"/>
        [NoMappingToDatabase]
        public ExtractionInformation ExtractionInformation {get { return Repository.GetObjectByID<ExtractionInformation>(ExtractionInformation_ID); } }

        /// <inheritdoc cref="AggregateConfiguration_ID"/>
        [NoMappingToDatabase]
        public AggregateConfiguration AggregateConfiguration { get { return Repository.GetObjectByID<AggregateConfiguration>(AggregateConfiguration_ID); } }

        #endregion

        /// <summary>
        /// Declares a new column in an <see cref="AggregateConfiguration"/> (GROUP BY query).  The new column will be based on the master Catalogue column
        /// (<see cref="ExtractionInformation"/>).
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="basedOnColumn"></param>
        /// <param name="configuration"></param>
        public AggregateDimension(ICatalogueRepository repository, ExtractionInformation basedOnColumn, AggregateConfiguration configuration)
        {
            object alias = DBNull.Value;
            if (basedOnColumn.Alias != null) alias = basedOnColumn.Alias;

            repository.InsertAndHydrate(this,new Dictionary<string, object>
            {
                {"AggregateConfiguration_ID", configuration.ID},
                {"ExtractionInformation_ID", basedOnColumn.ID},
                {"SelectSQL", basedOnColumn.SelectSQL},
                {"Alias", alias},
                {"Order", basedOnColumn.Order}
            });

            ClearAllInjections();
        }

        internal AggregateDimension(ICatalogueRepository repository,DbDataReader r) : base(repository,r)
        {
            AggregateConfiguration_ID = int.Parse(r["AggregateConfiguration_ID"].ToString());
            ExtractionInformation_ID = int.Parse(r["ExtractionInformation_ID"].ToString());
            
            SelectSQL = r["SelectSQL"] as string;
            Alias = r["Alias"] as string;

            Order = int.Parse(r["Order"].ToString());
            
            ClearAllInjections();
        }

        /// <inheritdoc/>
        public string GetRuntimeName()
        {
            return RDMPQuerySyntaxHelper.GetRuntimeName(this);
        }

        
        private Lazy<ExtractionInformation> _knownExtractionInformation;

        public void InjectKnown(ExtractionInformation instance)
        {
            _knownExtractionInformation = new Lazy<ExtractionInformation>(()=>instance);
        }

        public void ClearAllInjections()
        {
            _knownExtractionInformation = new Lazy<ExtractionInformation>(() => Repository.GetObjectByID<ExtractionInformation>(ExtractionInformation_ID));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            try
            {
                return GetRuntimeName();
            }
            catch (Exception)
            {
                return "Unamed AggregateDimension ID " + ID;
            }
        }

        /// <inheritdoc cref="ColumnSyntaxChecker"/>
        public void Check(ICheckNotifier notifier)
        {
            new ColumnSyntaxChecker(this).Check(notifier);
        }

        /// <inheritdoc/>
        public IHasDependencies[] GetObjectsThisDependsOn()
        {
            return new[] {ExtractionInformation};
        }

        /// <inheritdoc/>
        public IHasDependencies[] GetObjectsDependingOnThis()
        {
            return new[] { AggregateConfiguration };
        }

        
    }
}