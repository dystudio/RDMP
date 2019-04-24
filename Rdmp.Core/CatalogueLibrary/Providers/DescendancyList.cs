// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rdmp.Core.CatalogueLibrary.Providers
{
    /// <summary>
    /// Audit of parents for a given object in the CatalogueChildProvider hierarchy that is used to populate RDMPCollectionUIs.  Every object that is not a root level 
    /// object will have a DescendancyList.  Normally any DatabaseEntity (or node class) has only one DescendancyList (path to reach it) however you can flag BetterRouteExists
    /// on a DescendancyList to indicate that if another DescendancyList is found for the object then that one is to be considered 'better' and used instead.  For example
    /// AggregateConfigurations which are modelling a cohort apper both under their respective Catalogue and their CohortIdentificationConfiguration but sometimes one is an
    /// orphan (it's CohortIdentificationConfiguration has been deleted or it has been removed from it) in which case the only path is the 'less goood' one.
    /// 
    /// <para>It is not allowed to have duplicate objects in Parents.  All objects and parents must have appropriate implements of GetHashCode.</para>
    /// </summary>
    public class DescendancyList
    {

        /// <summary>
        /// All objects that are above the described object in order from the root to the immediate parent.
        /// </summary>
        public object[] Parents;


        /// <summary>
        /// Set to true to indicate that this route should be considered better than any you have seen before for the given object and it's children.  This will cause
        /// other colliding <see cref="DescendancyList"/> paths for the same object to be marked BetterRouteExists
        /// </summary>
        public bool NewBestRoute { get; private set; }

        /// <summary>
        /// Set to true to indicate that you might find a better DescendancyList for the given object and if so that other DescendancyList should be considered 'better'
        /// </summary>
        public bool BetterRouteExists { get; private set; }

        /// <summary>
        /// Declares that an object has hierarchical <paramref name="parents"/> which should be in order from root to immediate parent
        /// </summary>
        /// <param name="parents"></param>
        public DescendancyList(params object[] parents)
        {
            Parents = parents;
        }

        /// <summary>
        /// True if the list is empty (i.e. there are no <see cref="Parents"/>)
        /// </summary>
        public bool IsEmpty { get { return !Parents.Any(); } }

        /// <summary>
        /// Returns a new instance of DescendancyList that includes the new parent appended to the end of parent hierarchy. You can only add to the end so 
        /// if you have Root=>Grandparent then the only thing you should add is Parent.
        /// </summary>
        /// <param name="anotherKnownParent"></param>
        /// <returns></returns>
        public DescendancyList Add(object anotherKnownParent)
        {
            if(Parents.Contains(anotherKnownParent))
                throw new ArgumentException("DecendancyList already contains '" + anotherKnownParent + "'");

            var list = new List<object>(Parents);
            list.Add(anotherKnownParent);
            var toReturn = new DescendancyList(list.ToArray());
            toReturn.BetterRouteExists = BetterRouteExists;
            toReturn.NewBestRoute = NewBestRoute;
            return toReturn;
        }

        /// <summary>
        /// Returns a new DescendancyList with BetterRouteExists set to true, this means the system will bear in mind it might see a better DescendancyList later on
        /// in which case it will use that better route instead
        /// </summary>
        /// <returns></returns>
        public DescendancyList SetBetterRouteExists()
        {
            NewBestRoute = false;
            BetterRouteExists = true;

            var toReturn= new DescendancyList(Parents);
            toReturn.NewBestRoute = false;
            toReturn.BetterRouteExists = true;
            return toReturn;
        }

        /// <summary>
        /// Returns a new DescendancyList with NewBestRoute set to true, this means the system will consider that this DescendancyList can override other colliding DescendancyList
        /// that already exist.
        /// </summary>
        /// <returns></returns>
        public DescendancyList SetNewBestRoute()
        {
            NewBestRoute = true;
            BetterRouteExists = false;

            var toReturn= new DescendancyList(Parents);
            toReturn.NewBestRoute = true;
            toReturn.BetterRouteExists = false;

            return toReturn;
        }
        public override string ToString()
        {
            return "<<"+ string.Join("=>", Parents) + ">>";
        }

        /// <summary>
        /// returns the last object in the chain, for example Root=>GrandParent=>Parent would return 'Parent'
        /// </summary>
        /// <returns></returns>
        public object Last()
        {
            return Parents.Last();
        }
    }
}
