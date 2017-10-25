using System.Configuration;
using System.Drawing;
using System.Linq;
using CatalogueLibrary.CommandExecution.AtomicCommands;
using CatalogueLibrary.Data;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using Dashboard.CatalogueSummary;
using DataQualityEngine.Data;
using ReusableLibraryCode.CommandExecution;
using ReusableUIComponents.Icons.IconProvision;

namespace Dashboard.CommandExecution.AtomicCommands
{
    public class ExecuteCommandViewDQEResultsForCatalogue : BasicCommandExecution, IAtomicCommandWithTarget
    {
        private readonly IActivateItems _activator;
        private Catalogue _catalogue;

        public ExecuteCommandViewDQEResultsForCatalogue(IActivateItems activator)
        {
            _activator = activator;
        }
        
        public Image GetImage(IIconProvider iconProvider)
        {
            return CatalogueIcons.DQE;
        }

        public IAtomicCommandWithTarget SetTarget(DatabaseEntity target)
        {
            _catalogue = (Catalogue) target;
            
            //must have both of these things to be DQEd
            if (_catalogue.TimeCoverage_ExtractionInformation_ID == null)
            {
                SetImpossible("Catalogue does not have an ExtractionInformation");
                return this;
            }

            if (string.IsNullOrWhiteSpace(_catalogue.ValidatorXML))
            {
                SetImpossible("Catalogue does not have any validation rules configured");
                return this;
            }

            var defaults = new ServerDefaults(_activator.RepositoryLocator.CatalogueRepository);
            var dqeServer = defaults.GetDefaultFor(ServerDefaults.PermissableDefaults.DQE);

            if (dqeServer == null)
            {
                SetImpossible("There is no DQE server");
                return this;
            }

            if (!ServerHasAtLeastOneEvaluation(_catalogue))
                SetImpossible("DQE has never been run for Catalogue");

            return this;
        }

        public override void Execute()
        {
            base.Execute();

            _activator.Activate<CatalogueSummaryScreen, Catalogue>(_catalogue);
        }

        private bool ServerHasAtLeastOneEvaluation(Catalogue c)
        {
            return new DQERepository(_activator.RepositoryLocator.CatalogueRepository).GetAllEvaluationsFor(c).Any();
        }
    }
}