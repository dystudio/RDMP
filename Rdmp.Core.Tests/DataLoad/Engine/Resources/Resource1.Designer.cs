﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rdmp.Core.Tests.DataLoad.Engine.Resources {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DataLoadEngineTests.Resources.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] TestExcelFile1 {
            get {
                object obj = ResourceManager.GetObject("TestExcelFile1", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;?mso-application progid=&quot;Excel.Sheet&quot;?&gt;
        ///&lt;Workbook xmlns=&quot;urn:schemas-microsoft-com:office:spreadsheet&quot;
        /// xmlns:o=&quot;urn:schemas-microsoft-com:office:office&quot;
        /// xmlns:x=&quot;urn:schemas-microsoft-com:office:excel&quot;
        /// xmlns:ss=&quot;urn:schemas-microsoft-com:office:spreadsheet&quot;
        /// xmlns:html=&quot;http://www.w3.org/TR/REC-html40&quot;&gt;
        /// &lt;DocumentProperties xmlns=&quot;urn:schemas-microsoft-com:office:office&quot;&gt;
        ///  &lt;Author&gt;Marie Pitkethly&lt;/Author&gt;
        ///  &lt;LastAuthor&gt;Thomas Nind&lt;/LastAuthor&gt;
        ///  &lt;Created&gt;2014-10-15T13:59 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TestExcelFile2 {
            get {
                return ResourceManager.GetString("TestExcelFile2", resourceCulture);
            }
        }
    }
}
