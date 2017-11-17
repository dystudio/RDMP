﻿using System;

namespace MapsDirectlyToDatabaseTable
{
    /// <summary>
    /// Used to indicate when a string or Uri property is 'find and replaceable' through the LocationAdjustment UIs
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class UsefulProperty : Attribute
    {
    }
}