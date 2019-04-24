// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using CatalogueManager.ItemActivation;
using CatalogueManager.TestsAndSetup.ServicePropogation;
using DataExportManager.CohortUI.CohortSourceManagement.WizardScreens;
using Rdmp.Core.DataExport.Data.DataTables;

namespace DataExportManager.CohortUI.CohortSourceManagement
{
    /// <summary>
    /// Wizard with three screens allowing you to create a Cohort database for use with Data Export Manager.  A cohort database is a database which stores all the patient identifier lists 
    /// for all the projects you have released data for.  A Cohort is a list of patient identifiers.  All identifiers must have the same datatype (If you handle two distinct patient 
    /// identifiers e.g. CHI number and NHS number then you can have 2 cohort databases... or you could just standardise with a mapping table and save yourself a lot of confusion) 
    /// 
    /// <para>This wizard offers 2 methods of allocating Release Identifiers (See Screen2) but because there can be company specific methods for allocating release identifiers (e.g. upload
    /// the private identifiers to an anonymisation web service and wait a week before downloading the corresponding release identifiers) the RDMP does not manage the cohort database 
    /// directly.  If you have such an obscure release identifier allocation policy tell Screen 3 to leave the release identifiers blank and write a process/plugin for updating the cohort
    /// table.</para>
    /// 
    /// <para>Data Export Manager will always link datasets against the private identifier and substitute it for the release identifier when extracting data.</para>
    /// 
    /// </summary>
    public partial class CreateNewCohortDatabaseWizardUI : RDMPUserControl
    {
        Screen1 screen1;
        Screen2 screen2;

        public ExternalCohortTable ExternalCohortTableCreatedIfAny
        {
            get
        {
            return screen2.ExternalCohortTableCreatedIfAny;
        } }

        public CreateNewCohortDatabaseWizardUI()
        {
            InitializeComponent();

            screen1 = new Screen1();
            screen2 = new Screen2();

            pStage.Controls.Clear();
            pStage.Controls.Add(screen1);

            screen1.btnOk.Click += btnOk_Click;
            screen2.btnBack.Click += btnBackScreen2_Click;
        }

        public override void SetItemActivator(IActivateItems activator)
        {
            base.SetItemActivator(activator);
            screen2.SetItemActivator(activator);
        }

        void btnBackScreen2_Click(object sender, EventArgs e)
        {
            pStage.Controls.Clear();
            pStage.Controls.Add(screen1);
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            pStage.Controls.Clear();
            pStage.Controls.Add(screen2);
        }

    }
}
