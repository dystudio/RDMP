﻿using System.Drawing;
using CatalogueLibrary.CommandExecution.AtomicCommands;
using CatalogueLibrary.Data;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using CatalogueManager.Validation;
using ReusableLibraryCode.CommandExecution;
using ReusableUIComponents.Icons.IconProvision;

namespace Dashboard.CommandExecution.AtomicCommands
{
    internal class ExecuteCommandConfigureCatalogueValidationRules : BasicCommandExecution,IAtomicCommandWithTarget
    {
        private readonly IActivateItems _itemActivator;
        private Catalogue _catalogue;

        public ExecuteCommandConfigureCatalogueValidationRules(IActivateItems itemActivator)
        {
            _itemActivator = itemActivator;
        }

        public override string GetCommandName()
        {
            return _catalogue == null || string.IsNullOrWhiteSpace(_catalogue.ValidatorXML)? "Configure Validation Rules" : "Change Configuration Rules";
        }

        public Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.DQE, OverlayKind.Edit);
        }

        public IAtomicCommandWithTarget SetTarget(DatabaseEntity target)
        {
            _catalogue = (Catalogue) target;
            return this;
        }

        public override void Execute()
        {
            base.Execute();
            _itemActivator.Activate<ValidationSetupForm, Catalogue>(_catalogue);
        }
    }
}