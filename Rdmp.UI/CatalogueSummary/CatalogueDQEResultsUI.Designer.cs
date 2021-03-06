﻿using Rdmp.UI.CatalogueSummary.DataQualityReporting;
using Rdmp.UI.CatalogueSummary.DataQualityReporting.SubComponents;

namespace Rdmp.UI.CatalogueSummary
{
    partial class CatalogueDQEResultsUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.timePeriodicityChart1 = new TimePeriodicityChart();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dqePivotCategorySelector1 = new DQEPivotCategorySelector();
            this.columnStatesChart1 = new ColumnStatesChart();
            this.evaluationTrackBar1 = new EvaluationTrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.timePeriodicityChart1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1034, 552);
            this.splitContainer1.SplitterDistance = 233;
            this.splitContainer1.TabIndex = 3;
            // 
            // timePeriodicityChart1
            // 
            this.timePeriodicityChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timePeriodicityChart1.Location = new System.Drawing.Point(0, 0);
            this.timePeriodicityChart1.Name = "timePeriodicityChart1";
            this.timePeriodicityChart1.Size = new System.Drawing.Size(1030, 229);
            this.timePeriodicityChart1.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dqePivotCategorySelector1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.columnStatesChart1);
            this.splitContainer3.Size = new System.Drawing.Size(1030, 311);
            this.splitContainer3.SplitterDistance = 163;
            this.splitContainer3.TabIndex = 1;
            // 
            // dqePivotCategorySelector1
            // 
            this.dqePivotCategorySelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dqePivotCategorySelector1.Location = new System.Drawing.Point(0, 0);
            this.dqePivotCategorySelector1.Name = "dqePivotCategorySelector1";
            this.dqePivotCategorySelector1.Size = new System.Drawing.Size(163, 311);
            this.dqePivotCategorySelector1.TabIndex = 0;
            // 
            // columnStatesChart1
            // 
            this.columnStatesChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnStatesChart1.Location = new System.Drawing.Point(0, 0);
            this.columnStatesChart1.Name = "columnStatesChart1";
            this.columnStatesChart1.Size = new System.Drawing.Size(863, 311);
            this.columnStatesChart1.TabIndex = 2;
            // 
            // evaluationTrackBar1
            // 
            this.evaluationTrackBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.evaluationTrackBar1.Evaluations = null;
            this.evaluationTrackBar1.Location = new System.Drawing.Point(0, 552);
            this.evaluationTrackBar1.Name = "evaluationTrackBar1";
            this.evaluationTrackBar1.Size = new System.Drawing.Size(1034, 71);
            this.evaluationTrackBar1.TabIndex = 4;
            this.evaluationTrackBar1.EvaluationSelected += new EvaluationSelectedHandler(this.evaluationTrackBar1_EvaluationSelected);
            // 
            // CatalogueSummaryScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.evaluationTrackBar1);
            this.Name = "CatalogueSummaryScreen";
            this.Size = new System.Drawing.Size(1034, 623);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DataQualityReporting.TimePeriodicityChart timePeriodicityChart1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private DataQualityReporting.SubComponents.DQEPivotCategorySelector dqePivotCategorySelector1;
        private DataQualityReporting.SubComponents.EvaluationTrackBar evaluationTrackBar1;
        private DataQualityReporting.ColumnStatesChart columnStatesChart1;
    }
}
