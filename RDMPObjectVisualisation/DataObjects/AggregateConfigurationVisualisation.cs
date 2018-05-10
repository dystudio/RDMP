using System;
using System.Windows.Forms;
using CatalogueLibrary.Data.Aggregation;
using CohortManagerLibrary.QueryBuilding;
using ReusableLibraryCode.Checks;
using ReusableUIComponents;

namespace RDMPObjectVisualisation.DataObjects
{
    /// <summary>
    /// TECHNICAL: Allows visualisation of a <see cref="AggregateConfiguration"/> which might be a cohort set, graph of records or 'patient index table'.
    /// 
    /// <para>
    /// Patient index tables are AggregateConfigurations that are used in cohort generation (in CohortManager).  They are queries which when executed will return a table of relevant 
    /// events about a patient set (e.g. all the prescription dates for opiates in Tayside).  This table will be part of a cohort identification configuration (e.g. find patients who
    /// have had a prescription for cannabis within 3 weeks of a prescription for opiates and are alive today and resident in Tayside).
    /// </para>
    /// </summary>
    public partial class AggregateConfigurationVisualisation : UserControl, ICheckable
    {
        private readonly AggregateConfiguration _configuration;

        public AggregateConfigurationVisualisation(AggregateConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
            DoTransparencyProperly.ThisHoversOver(ragSmiley1,pictureBox1);

            this.Check(ragSmiley1);
            
            if (_configuration.IsJoinablePatientIndexTable())
                pictureBox1.Image = Images.BigPatientIndexTable;
            else if (_configuration.IsCohortIdentificationAggregate)
                pictureBox1.Image = Images.BigCohort;
            else
                pictureBox1.Image = Images.BigGraph;
        }

        public void Check(ICheckNotifier notifier)
        {
            try
            {
                var builder = _configuration.GetQueryBuilder();
                notifier.OnCheckPerformed(new CheckEventArgs( "Sql for Aggregate is:" + Environment.NewLine + builder.SQL, CheckResult.Success));
            }
            catch (Exception e)
            {
                notifier.OnCheckPerformed(
                    new CheckEventArgs(
                        "Failed to generate query for AggregateConfiguration " + _configuration,CheckResult.Fail, e));
            }

        }
    }
}
