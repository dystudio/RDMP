// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using NUnit.Framework;
using Rdmp.Core.Validation;
using Rdmp.Core.Validation.Constraints;
using Rdmp.Core.Validation.Constraints.Primary;
using Rdmp.Core.Validation.Constraints.Secondary;
using Rdmp.Core.Validation.Constraints.Secondary.Predictor;

namespace Rdmp.Core.Tests.Validation
{

    [Category("Unit")]
    class ExceptionHandlingTests
    {

        [Test]
        public void Validate_WhenMultipleErrors_ReturnsAllErrors()
        {
            var validator = new Validator();
            
            var chi = new ItemValidator();
            chi.PrimaryConstraint = (PrimaryConstraint) Validator.CreateConstraint("chi",Consequence.Wrong);
            var prediction = new Prediction(new ChiSexPredictor(), "gender");
            chi.AddSecondaryConstraint(prediction);
            validator.AddItemValidator(chi, "chi", typeof(string));

            var age = new ItemValidator();
            BoundDouble ageConstraint = (BoundDouble)Validator.CreateConstraint("bounddouble",Consequence.Wrong);
            ageConstraint.Lower = 0;
            ageConstraint.Upper = 30;
            age.AddSecondaryConstraint(ageConstraint);
            validator.AddItemValidator(age, "age", typeof(int));

            var row = new Dictionary<string, object>();
            row.Add("chi", TestConstants._INVALID_CHI_CHECKSUM);
            row.Add("age", 31);
            row.Add("gender", "F");

            ValidationFailure result =  validator.Validate(row);

            Assert.AreEqual(2, result.GetExceptionList().Count);
            

        }
    }
}
