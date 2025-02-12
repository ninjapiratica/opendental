﻿namespace OpenDental {
	partial class FormEhrPatList {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrPatList));
			this.butShow = new System.Windows.Forms.Button();
			this.butClose = new System.Windows.Forms.Button();
			this.butGender = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butLabResult = new System.Windows.Forms.Button();
			this.butMedication = new System.Windows.Forms.Button();
			this.butDisease = new System.Windows.Forms.Button();
			this.butBirthdate = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// butShow
			// 
			this.butShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butShow.Location = new System.Drawing.Point(547, 445);
			this.butShow.Name = "butShow";
			this.butShow.Size = new System.Drawing.Size(75, 23);
			this.butShow.TabIndex = 15;
			this.butShow.Text = "Results";
			this.butShow.UseVisualStyleBackColor = true;
			this.butShow.Click += new System.EventHandler(this.butResults_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(547, 495);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 17;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butGender
			// 
			this.butGender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butGender.Location = new System.Drawing.Point(547, 144);
			this.butGender.Name = "butGender";
			this.butGender.Size = new System.Drawing.Size(75, 23);
			this.butGender.TabIndex = 18;
			this.butGender.Text = "Gender";
			this.butGender.UseVisualStyleBackColor = true;
			this.butGender.Click += new System.EventHandler(this.butGender_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(2, 2);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(529, 527);
			this.gridMain.TabIndex = 10;
			this.gridMain.Title = "Data Elements";
			this.gridMain.TranslationName = "FormPatientList";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butLabResult
			// 
			this.butLabResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butLabResult.Location = new System.Drawing.Point(547, 111);
			this.butLabResult.Name = "butLabResult";
			this.butLabResult.Size = new System.Drawing.Size(75, 23);
			this.butLabResult.TabIndex = 19;
			this.butLabResult.Text = "Lab Result";
			this.butLabResult.UseVisualStyleBackColor = true;
			this.butLabResult.Click += new System.EventHandler(this.butLabResult_Click);
			// 
			// butMedication
			// 
			this.butMedication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMedication.Location = new System.Drawing.Point(547, 78);
			this.butMedication.Name = "butMedication";
			this.butMedication.Size = new System.Drawing.Size(75, 23);
			this.butMedication.TabIndex = 20;
			this.butMedication.Text = "Medication";
			this.butMedication.UseVisualStyleBackColor = true;
			this.butMedication.Click += new System.EventHandler(this.butMedication_Click);
			// 
			// butDisease
			// 
			this.butDisease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDisease.Location = new System.Drawing.Point(547, 45);
			this.butDisease.Name = "butDisease";
			this.butDisease.Size = new System.Drawing.Size(75, 23);
			this.butDisease.TabIndex = 21;
			this.butDisease.Text = "Problem";
			this.butDisease.UseVisualStyleBackColor = true;
			this.butDisease.Click += new System.EventHandler(this.butDisease_Click);
			// 
			// butBirthdate
			// 
			this.butBirthdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBirthdate.Location = new System.Drawing.Point(547, 12);
			this.butBirthdate.Name = "butBirthdate";
			this.butBirthdate.Size = new System.Drawing.Size(75, 23);
			this.butBirthdate.TabIndex = 22;
			this.butBirthdate.Text = "Birthdate";
			this.butBirthdate.UseVisualStyleBackColor = true;
			this.butBirthdate.Click += new System.EventHandler(this.butBirthdate_Click);
			// 
			// FormEhrPatList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(634, 530);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butBirthdate);
			this.Controls.Add(this.butDisease);
			this.Controls.Add(this.butMedication);
			this.Controls.Add(this.butLabResult);
			this.Controls.Add(this.butGender);
			this.Controls.Add(this.butShow);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrPatList";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Patient List";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butShow;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butGender;
		private System.Windows.Forms.Button butLabResult;
		private System.Windows.Forms.Button butMedication;
		private System.Windows.Forms.Button butDisease;
		private System.Windows.Forms.Button butBirthdate;


	}
}