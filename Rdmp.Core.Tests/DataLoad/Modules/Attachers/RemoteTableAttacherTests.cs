﻿// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FAnsi;
using FAnsi.Discovery;
using Moq;
using NUnit.Framework;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.DataFlowPipeline;
using Rdmp.Core.DataLoad.Engine.DatabaseManagement.EntityNaming;
using Rdmp.Core.DataLoad.Engine.Job;
using Rdmp.Core.DataLoad.Modules.Attachers;
using Rdmp.Core.Logging;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.Progress;
using Tests.Common;
using TypeGuesser;

namespace Rdmp.Core.Tests.DataLoad.Modules.Attachers
{
    class RemoteTableAttacherTests : DatabaseTests
    {
        [TestCaseSource(typeof(All),nameof(All.DatabaseTypes))]
        public void TestRemoteTableAttacher_Normal(DatabaseType dbType)
        {
            var db = GetCleanedServer(dbType);

            var attacher = new RemoteTableAttacher();
            //where to go for data
            attacher.RemoteServer = db.Server.Name;
            attacher.RemoteDatabaseName = db.GetRuntimeName();
            attacher.DatabaseType = db.Server.DatabaseType;

            if (db.Server.ExplicitUsernameIfAny != null)
            {
                var creds = new DataAccessCredentials(CatalogueRepository);
                creds.Username = db.Server.ExplicitUsernameIfAny;
                creds.Password = db.Server.ExplicitPasswordIfAny;
                creds.SaveToDatabase();
                attacher.RemoteTableAccessCredentials = creds;
            }

            //the table to get data from
            attacher.RemoteTableName = "table1";
            attacher.RAWTableName = "table2";

            attacher.Initialize(null,db);

            var dt = new DataTable();
            dt.Columns.Add("Col1");
            dt.Rows.Add("fff");

            var tbl1 = db.CreateTable("table1", dt);
            var tbl2 = db.CreateTable("table2", new []{new DatabaseColumnRequest("Col1",new DatabaseTypeRequest(typeof(string),5))});

            Assert.AreEqual(1,tbl1.GetRowCount());
            Assert.AreEqual(0,tbl2.GetRowCount());

            var logManager = new LogManager(new DiscoveredServer(UnitTestLoggingConnectionString));

            var lmd = RdmpMockFactory.Mock_LoadMetadataLoadingTable(tbl2);
            Mock.Get(lmd).Setup(p=>p.CatalogueRepository).Returns(CatalogueRepository);
            logManager.CreateNewLoggingTaskIfNotExists(lmd.GetDistinctLoggingTask());

            var dbConfiguration = new HICDatabaseConfiguration(lmd, RdmpMockFactory.Mock_INameDatabasesAndTablesDuringLoads(db, "table2"));

            var job = new DataLoadJob(RepositoryLocator,"test job",logManager,lmd,new TestLoadDirectory(),new ThrowImmediatelyDataLoadEventListener(),dbConfiguration);
            job.StartLogging();
            attacher.Attach(job, new GracefulCancellationToken());

            Assert.AreEqual(1,tbl1.GetRowCount());
            Assert.AreEqual(1,tbl2.GetRowCount());
            
        }
    }

}
