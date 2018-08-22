using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using CatalogueLibrary.Data;
using CatalogueLibrary.QueryBuilding.Parameters;
using MapsDirectlyToDatabaseTable.Injection;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.DatabaseHelpers.Discovery.QuerySyntax;
using IFilter = CatalogueLibrary.Data.IFilter;

namespace CatalogueLibrary.QueryBuilding
{
    /// <summary>
    /// This class maintains a list of user defined ExtractionInformation objects.  It can produce SQL which will try to 
    /// extract this set of ExtractionInformation objects only from the database.  This includes determining which ExtractionInformation
    /// are Lookups, which tables the various objects come from, figuring out whether they can be joined by using JoinInfo in the catalogue
    /// 
    /// <para>It will throw when query SQL if it is not possible to join all the underlying tables or there are any other problems.</para>
    /// 
    /// <para>You can ask it what is on line X or ask what line number has ExtractionInformation Y on it</para>
    /// 
    /// <para>ExtractionInformation is sorted by column order prior to generating the SQL (i.e. not the order you add them to the query builder)</para>
    /// </summary>
    public class QueryBuilder : ISqlQueryBuilder
    {
        private readonly TableInfo[] _forceJoinsToTheseTables;
        object oSQLLock = new object();

        /// <inheritdoc/>
        public string SQL
        {
            get {

                lock (oSQLLock)
                {
                    if (SQLOutOfDate)
                        RegenerateSQL();
                    return _sql;
                }
            }
        }

        public string LimitationSQL { get; private set; }
        
        public List<QueryTimeColumn> SelectColumns { get; private set; }
        public List<TableInfo> TablesUsedInQuery { get; private set; }
        public List<JoinInfo> JoinsUsedInQuery { get; private set; }
        public List<CustomLine> CustomLines { get; private set; }

        public CustomLine TopXCustomLine { get; set; }

        public ParameterManager ParameterManager { get; private set; }
        
        /// <summary>
        /// Optional field, this specifies where to start gargantuan joins such as when there are 3+ joins and multiple primary key tables e.g. in a star schema.
        /// If this is not set and there are too many JoinInfos defined in the Catalogue then the class will bomb out with the Exception 
        /// </summary>
        public TableInfo PrimaryExtractionTable { get; set; }

        /// <summary>
        /// Determines whether the QueryBuilder will sort the input columns according to their .Order paramter, the default value is true
        /// </summary>
        public bool Sort { get; set; }
        
        /// <summary>
        /// A container that contains all the subcontainers and filters to be assembled during the query (use a SpontaneouslyInventedFilterContainer if you want to inject your 
        /// own container tree at runtime rather than referencing a database entity)
        /// </summary>
        public IContainer RootFilterContainer
        {
            get { return _rootFilterContainer; }
            set {
                _rootFilterContainer = value;
                SQLOutOfDate = true;
            }
        }

        public bool CheckSyntax { get; set; }


        private string _salt = null;

        /// <summary>
        /// Only use this if you want IColumns which are marked as requiring Hashing to be hashed.  Once you set this on a QueryEditor all fields so marked will be hashed using the
        /// specified salt
        /// </summary>
        /// <param name="salt">A 3 letter string indicating the desired SALT</param>
        public void SetSalt(string salt)
        {
            if(string.IsNullOrWhiteSpace(salt))
                throw new NullReferenceException("Salt cannot be blank");

            _salt = salt;
        }

        public void SetLimitationSQL(string limitationSQL)
        {
            if(limitationSQL != null && limitationSQL.Contains("top"))
                throw new Exception("Use TopX property instead of limitation SQL to acheive this");

            LimitationSQL = limitationSQL;
            SQLOutOfDate = true;
        }

        public List<IFilter> Filters { get; private set; }

        public int TopX
        {
            get { return _topX; }
            set
            {
                //it already has that value
                if(_topX == value)
                    return;

                _topX = value;
                SQLOutOfDate = true;
            }
        }

        private string _sql;

        /// <inheritdoc/>
        public bool SQLOutOfDate { get; set; }

        private IContainer _rootFilterContainer;
        private string _hashingAlgorithm;
        private int _topX;
        private IQuerySyntaxHelper _syntaxHelper;

        /// <summary>
        /// Used to build extraction queries based on ExtractionInformation sets
        /// </summary>
        /// <param name="limitationSQL">Any text you want after SELECT to limit the results e.g. "DISTINCT" or "TOP 10"</param>
        /// <param name="hashingAlgorithm"></param>
        /// <param name="selectedDatasetsForcedJoins"></param>
        public QueryBuilder(string limitationSQL, string hashingAlgorithm, TableInfo[] forceJoinsToTheseTables = null)
        {
            _forceJoinsToTheseTables = forceJoinsToTheseTables;
            SetLimitationSQL(limitationSQL);
            Sort = true;
            ParameterManager = new ParameterManager();
            CustomLines = new List<CustomLine>();

            CheckSyntax = true;
            SelectColumns = new List<QueryTimeColumn>();

            _hashingAlgorithm = hashingAlgorithm ?? "Work.dbo.HicHash({0},{1})";

            TopX = -1;
        }

        #region public stuff
        public void AddColumnRange(IColumn[] columnsToAdd)
        {
            //add the new ones to the list
            foreach (IColumn col in columnsToAdd)
                AddColumn(col);
                
            SQLOutOfDate = true;
        }

        public void AddColumn(IColumn col)
        {
            QueryTimeColumn toAdd = new QueryTimeColumn(col);

            //if it is new, add it to the list
            if (!SelectColumns.Contains(toAdd))
            {
                SelectColumns.Add(toAdd);
                SQLOutOfDate = true;
            }   
        }
        
        public CustomLine AddCustomLine(string text, QueryComponent positionToInsert)
        {
            SQLOutOfDate = true;
            return SqlQueryBuilderHelper.AddCustomLine(this, text, positionToInsert);
        }
       
        [Pure]
        public JoinInfo[] GetRequiredJoins()
        {
            if (SQLOutOfDate)
                RegenerateSQL();

            return JoinsUsedInQuery.ToArray();
        }

        [Pure]
        public Lookup[] GetRequiredLookups()
        {
            if(SQLOutOfDate)
                RegenerateSQL();

            return SqlQueryBuilderHelper.GetRequiredLookups(this).ToArray();
        }

      
        [Pure]
        public int ColumnCount()
        {
            return SelectColumns.Count;
        }
        
        #endregion

        /// <summary>
        /// Updates .SQL Property, note that this is automatically called when you query .SQL anyway so you do not need to manually call it. 
        /// </summary>
        public void RegenerateSQL()
        {
            var checkNotifier = new ThrowImmediatelyCheckNotifier();

            _sql = "";
            
            //reset the Parameter knowledge
            ParameterManager.ClearNonGlobals();

            #region Setup to output the query, where we figure out all the joins etc
            //reset everything
            
            if (Sort)
                SelectColumns.Sort();
            
            //work out all the filters 
            Filters = SqlQueryBuilderHelper.GetAllFiltersUsedInContainerTreeRecursively(RootFilterContainer);
           
            TableInfo primary;
            TablesUsedInQuery = SqlQueryBuilderHelper.GetTablesUsedInQuery(this, out primary);

            //force join to any TableInfos that would not be normally joined to but the user wants to anyway e.g. if theres WHERE sql that references them but no columns
            if (_forceJoinsToTheseTables != null)
                foreach (var force in _forceJoinsToTheseTables)
                    if (!TablesUsedInQuery.Contains(force))
                        TablesUsedInQuery.Add(force);

            this.PrimaryExtractionTable = primary;
            
            SqlQueryBuilderHelper.FindLookups(this);

            JoinsUsedInQuery = SqlQueryBuilderHelper.FindRequiredJoins(this);

            //deal with case when there are no tables in the query or there are only lookup descriptions in the query
            if (TablesUsedInQuery.Count == 0)
                throw new Exception("There are no TablesUsedInQuery in this dataset");


            _syntaxHelper = SqlQueryBuilderHelper.GetSyntaxHelper(TablesUsedInQuery);

            if (TopX != -1)
                SqlQueryBuilderHelper.HandleTopX(this, _syntaxHelper, TopX);
            else
                SqlQueryBuilderHelper.ClearTopX(this);

            //declare parameters
            ParameterManager.AddParametersFor(Filters);
            
            #endregion

            /////////////////////////////////////////////Assemble Query///////////////////////////////

            #region Preamble (including variable declarations/initializations)
            //assemble the query - never use Environment.Newline, use TakeNewLine() so that QueryBuilder knows what line its got up to
            string toReturn = "";

            foreach (ISqlParameter parameter in ParameterManager.GetFinalResolvedParametersList())
            {
                //if the parameter is one that needs to be told what the query syntax helper is e.g. if it's a global parameter designed to work on multiple datasets
                var needsToldTheSyntaxHelper = parameter as IInjectKnown<IQuerySyntaxHelper>;
                if(needsToldTheSyntaxHelper != null)
                    needsToldTheSyntaxHelper.InjectKnown(_syntaxHelper);
                
                if(CheckSyntax)
                    parameter.Check(checkNotifier);

                toReturn += GetParameterDeclarationSQL(parameter);
            }

            //add user custom Parameter lines
            toReturn = AppendCustomLines(toReturn, QueryComponent.VariableDeclaration);

            #endregion

            #region Select (including all IColumns)
            toReturn += Environment.NewLine;
            toReturn += "SELECT " + LimitationSQL + Environment.NewLine;

            toReturn = AppendCustomLines(toReturn, QueryComponent.SELECT);
            toReturn += Environment.NewLine;

            toReturn = AppendCustomLines(toReturn, QueryComponent.QueryTimeColumn);
            
            for (int i = 0; i < SelectColumns.Count;i++ )
            {
                //output each of the ExtractionInformations that the user requested and record the line number for posterity
                string columnAsSql = SelectColumns[i].GetSelectSQL(_hashingAlgorithm, _salt, _syntaxHelper);

                 //there is another one coming
                 if (i + 1 < SelectColumns.Count)
                     columnAsSql += ",";

                 toReturn += columnAsSql + Environment.NewLine;
            }

            #endregion

            //work out basic JOINS Sql
            toReturn += SqlQueryBuilderHelper.GetFROMSQL(this);

            //add user custom JOIN lines
            toReturn = AppendCustomLines(toReturn, QueryComponent.JoinInfoJoin);
            
            #region Filters (WHERE)

            toReturn += SqlQueryBuilderHelper.GetWHERESQL(this);
            
            toReturn = AppendCustomLines(toReturn, QueryComponent.WHERE);
            toReturn = AppendCustomLines(toReturn, QueryComponent.Postfix);
            
            _sql = toReturn;
            SQLOutOfDate = false;

            #endregion
        }

        private string AppendCustomLines(string toReturn, QueryComponent stage)
        {
            var lines = SqlQueryBuilderHelper.GetCustomLinesSQLForStage(this, stage).ToArray();
            if (lines.Any())
            {
                toReturn += Environment.NewLine;
                toReturn += string.Join(Environment.NewLine, lines.Select(l => l.Text));
            }

            return toReturn;
        }
        

        public IEnumerable<Lookup> GetDistinctRequiredLookups()
        {
            return SqlQueryBuilderHelper.GetDistinctRequiredLookups(this);
        }
        
        public static ConstantParameter DeconstructStringIntoParameter(string sql, IQuerySyntaxHelper syntaxHelper)
        {
            string[] lines = sql.Split(new[] {'\n'},StringSplitOptions.RemoveEmptyEntries);

            string comment = null;

            Regex commentRegex = new Regex(@"/\*(.*)\*/");
            var matchComment = commentRegex.Match(lines[0]);
            if (lines.Length >= 3 && matchComment.Success)
                comment = matchComment.Groups[1].Value;

            string declaration = comment == null ? lines[0]:lines[1];
            declaration = declaration.TrimEnd(new[] {'\r'});

            string valueLine = comment == null ? lines[1] : lines[2];

            if(!valueLine.StartsWith("SET"))
                throw new Exception("Value line did not start with SET:" + sql);

            var valueLineSplit = valueLine.Split(new[] {'='});
            var value = valueLineSplit[1].TrimEnd(new[] {';','\r'});


            return new ConstantParameter(declaration.Trim(), value.Trim(), comment, syntaxHelper);
        }

        public static string GetParameterDeclarationSQL(ISqlParameter sqlParameter)
        {
            string toReturn = "";

            if (!string.IsNullOrWhiteSpace(sqlParameter.Comment))
                toReturn += "/*" + sqlParameter.Comment + "*/" + Environment.NewLine;
            
            toReturn += sqlParameter.ParameterSQL + Environment.NewLine;

            //it's a table valued parameter! advanced
            if (!string.IsNullOrEmpty(sqlParameter.Value) && Regex.IsMatch(sqlParameter.Value, @"\binsert\s+into\b",RegexOptions.IgnoreCase))
                toReturn += sqlParameter.Value + ";" + Environment.NewLine;
            else
                toReturn += "SET " + sqlParameter.ParameterName + "=" + sqlParameter.Value + ";" + Environment.NewLine;//its a regular value
            
            return toReturn;
        }

        public void Invalidate()
        {
            SQLOutOfDate = true;
        }

    }
}
