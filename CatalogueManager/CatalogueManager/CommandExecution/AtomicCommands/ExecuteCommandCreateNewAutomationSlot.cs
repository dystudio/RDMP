﻿using System.Drawing;
using CatalogueLibrary.Data.Automation;
using CatalogueLibrary.Data.Remoting;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using ReusableUIComponents.CommandExecution.AtomicCommands;
using ReusableUIComponents.Icons.IconProvision;

namespace CatalogueManager.CommandExecution.AtomicCommands
{
    public class ExecuteCommandCreateNewAutomationSlot : BasicUICommandExecution, IAtomicCommand
    {
        public ExecuteCommandCreateNewAutomationSlot(IActivateItems activator) : base(activator)
        {
        }

        public override void Execute()
        {
            base.Execute();
            var slot = new AutomationServiceSlot(Activator.RepositoryLocator.CatalogueRepository);
            Publish(slot);
        }

        public Image GetImage(IIconProvider iconProvider)
        {
            return iconProvider.GetImage(RDMPConcept.AutomationServiceSlot, OverlayKind.Add);
        }
    }
}