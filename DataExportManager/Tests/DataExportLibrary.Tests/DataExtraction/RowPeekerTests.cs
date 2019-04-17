﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataExportLibrary.ExtractionTime.ExtractionPipeline.Sources;
using DataLoadEngine.DataFlowPipeline.Sources;
using NUnit.Framework;
using Rhino.Mocks;

namespace DataExportLibrary.Tests.DataExtraction
{
    class RowPeekerTests
    {
        [Test]
        public void Peeker()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MyCol");
            dt.Rows.Add("fish");
            dt.Rows.Add("dish");
            dt.Rows.Add("splish");

            var mock = MockRepository.GenerateMock<IDbDataCommandDataFlowSource>();
            mock.Expect(m => m.ReadOneRow()).Return(dt.Rows[0]).Repeat.Once();
            mock.Expect(m => m.ReadOneRow()).Return(dt.Rows[1]).Repeat.Once();
            mock.Expect(m => m.ReadOneRow()).Return(dt.Rows[2]).Repeat.Once();
            mock.Expect(m => m.ReadOneRow()).Return(null).Repeat.Once();

            RowPeeker p = new RowPeeker();
            var dt2 = new DataTable();
            dt2.Columns.Add("MyCol");

            //Reads fish and peeks dish
            p.AddWhile(mock,r=>(string) r["MyCol"] == "fish",dt2);

            //read one row
            Assert.AreEqual(1,dt2.Rows.Count);
            Assert.AreEqual("fish",dt2.Rows[0]["MyCol"]);

            var dt3 = new DataTable();
            dt3.Columns.Add("MyCol");
            
            //cannot add while there is a peek stored
            Assert.Throws<Exception>(() => p.AddWhile(mock, r => (string) r["MyCol"] == "fish", dt2));

            //clear the peek
            //unpeeks dish
            p.AddPeekedRowsIfAny(dt3);
            Assert.AreEqual(1,dt3.Rows.Count);
            Assert.AreEqual("dish",dt3.Rows[0]["MyCol"]);

            //now we can read into dt4 but the condition is false
            //Reads nothing but peeks splish
            DataTable dt4 = new DataTable();
            dt4.Columns.Add("MyCol");
            p.AddWhile(mock, r => (string) r["MyCol"] == "fish", dt4);

            Assert.AreEqual(0,dt4.Rows.Count);

            //we passed a null chunk and that pulls back the legit data table
            var dt5 = p.AddPeekedRowsIfAny(null);
            
            Assert.IsNotNull(dt5);
            Assert.AreEqual("splish",dt5.Rows[0]["MyCol"]);

            DataTable dt6 = new DataTable();
            dt6.Columns.Add("MyCol");
            p.AddWhile(mock, r => (string) r["MyCol"] == "fish", dt6);

            Assert.AreEqual(0,dt6.Rows.Count);

            mock.VerifyAllExpectations();
        }

        
    }
}
