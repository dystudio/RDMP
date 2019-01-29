using System;
using System.ComponentModel;
using System.Windows.Forms;
using CatalogueLibrary.Data.DataLoad;
using CatalogueLibrary.Repositories;
using CatalogueManager.Collections;
using CatalogueManager.Icons.IconProvision;
using CatalogueManager.ItemActivation;
using CatalogueManager.Rules;
using CatalogueManager.SimpleControls;
using CatalogueManager.TestsAndSetup.ServicePropogation;
using DataLoadEngine.DatabaseManagement.EntityNaming;
using DataLoadEngine.LoadExecution.Components.Arguments;
using DataLoadEngine.LoadExecution.Components.Runtime;
using CatalogueManager.PipelineUIs.DemandsInitializationUIs;
using ReusableUIComponents;
using ReusableUIComponents.ChecksUI;
using ReusableUIComponents.Dialogs;

namespace CatalogueManager.DataLoadUIs.LoadMetadataUIs.ProcessTasks
{

    /// <summary>
    /// Lets you view/edit a single data load module.  This is a pre-canned class e.g. FTPDownloader or a custom plugin you have written.  You should ensure
    /// that the Name field accurately describes (in plenty of detail) what the module/script is intended to do.  
    /// 
    /// <para>These can be either:
    /// Attacher - Run the named C# class (which implements the interface IAttacher).  This only works in Mounting stage.  This usually results in records being loaded into the RAW bubble (e.g. AnySeparatorFileAttacher)
    /// DataProvider - Run the named C# class (which implements IDataProvider).  Normally this runs in GetFiles but really it can run on any Stage.  This usually results in files being created or modified (e.g. FTPDownloader)
    /// MutilateDataTable - Run the named C# class (which implements IMutilateDataTables).  Runs in any Adjust/PostLoad stage.  These are dangerous operations which operate pre-canned functionality directly
    /// on the DataTable being loaded e.g. resolving primary key collisions (which can result in significant data loss if you have not configured the correct primary keys on your dataset).</para>
    /// 
    /// <para>Each C# module based task has a collection of arguments which each have a description of how they change the behaviour of the module.  Make sure to click on each Argument in turn
    /// and set an appropriate value such that you understand ahead of time what the module will do when it is run.</para>
    /// 
    /// <para>The data load engine design (RAW,STAGING,LIVE) makes it quite difficult to corrupt your data without realising but you should still adopt best practice: Do as much data modification
    /// in the RAW bubble (i.e. not as a post load operation), only use modules you understand the function of and try to restrict the scope of your adjustment operations (it is usually better
    /// to write an extraction transform than to transform the data during load in case there is a mistake or a researcher wants uncorrupted original data).</para>
    /// </summary>
    public partial class PluginProcessTaskUI : PluginProcessTaskUI_Design, ISaveableUI
    {
        private ArgumentCollection _argumentCollection;
        private Type _underlyingType;
        private ProcessTask _processTask;
        private RAGSmileyToolStrip _ragSmiley;

        public PluginProcessTaskUI()
        {
            InitializeComponent();
            AssociatedCollection = RDMPCollection.DataLoad;

            _ragSmiley = new RAGSmileyToolStrip(this);
        }

        public override void SetDatabaseObject(IActivateItems activator, ProcessTask databaseObject)
        {
            base.SetDatabaseObject(activator, databaseObject);
            _processTask = databaseObject;

            if(_argumentCollection == null)
            {
                var repo = (CatalogueRepository) databaseObject.Repository;

                _argumentCollection = new ArgumentCollection();
                try
                {
                    _underlyingType = repo.MEF.GetTypeByNameFromAnyLoadedAssembly(databaseObject.GetClassNameWhoArgumentsAreFor());

                    if(_underlyingType == null)
                        throw new Exception("Could not find Type '" + databaseObject.GetClassNameWhoArgumentsAreFor() +"' for ProcessTask '" + _processTask.Name + "'");
                }
                catch (Exception e)
                {
                    ExceptionViewer.Show(e);
                    return;
                }

                _argumentCollection.Setup(databaseObject, _underlyingType,_activator.RepositoryLocator.CatalogueRepository);

                _argumentCollection.Dock = DockStyle.Fill;
                pArguments.Controls.Add(_argumentCollection);
            }

            Add(_ragSmiley);

            CheckComponent();
            
            loadStageIconUI1.Setup(_activator.CoreIconProvider,_processTask.LoadStage);

            Add(new ToolStripButton("Check", FamFamFamIcons.arrow_refresh, (s, e) => CheckComponent()));
        }

        protected override void SetBindings(BinderWithErrorProviderFactory rules, ProcessTask databaseObject)
        {
            base.SetBindings(rules, databaseObject);

            Bind(tbName, "Text", "Name", d => d.Name);
            Bind(tbID, "Text", "ID", d => d.ID);
        }

        private void CheckComponent()
        {
            try
            {
                var factory = new RuntimeTaskFactory(_activator.RepositoryLocator.CatalogueRepository);

                var lmd = _processTask.LoadMetadata;
                var argsDictionary = new LoadArgsDictionary(lmd, new HICDatabaseConfiguration(lmd).DeployInfo);
                var mefTask = (IMEFRuntimeTask) factory.Create(_processTask, argsDictionary.LoadArgs[_processTask.LoadStage]);
            
                _ragSmiley.StartChecking(mefTask.MEFPluginClassInstance);
            }
            catch (Exception e)
            {
                _ragSmiley.Fatal(e);
            }
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                tbName.Text = "No Name";
                tbName.SelectAll();
            }

            _processTask.Name = tbName.Text;
        }
    }

    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<PluginProcessTaskUI_Design, UserControl>))]
    public abstract class PluginProcessTaskUI_Design:RDMPSingleDatabaseObjectControl<ProcessTask>
    {
    }
}
