namespace OpenDental{
	partial class FormSheetDef {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetDef));
			this.labelSheetType = new System.Windows.Forms.Label();
			this.listSheetType = new OpenDental.UI.ListBoxOD();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.textFontSize = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboFontName = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIsLandscape = new OpenDental.UI.CheckBox();
			this.textHeight = new OpenDental.ValidNum();
			this.textWidth = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkBypassLockDate = new OpenDental.UI.CheckBox();
			this.checkHasMobileLayout = new OpenDental.UI.CheckBox();
			this.checkAutoSaveCheck = new OpenDental.UI.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelSheetType
			// 
			this.labelSheetType.Location = new System.Drawing.Point(-2, 39);
			this.labelSheetType.Name = "labelSheetType";
			this.labelSheetType.Size = new System.Drawing.Size(154, 48);
			this.labelSheetType.TabIndex = 86;
			this.labelSheetType.Text = "Sheet Type\r\n(cannot be changed later)";
			this.labelSheetType.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listSheetType
			// 
			this.listSheetType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listSheetType.Location = new System.Drawing.Point(154, 38);
			this.listSheetType.Name = "listSheetType";
			this.listSheetType.Size = new System.Drawing.Size(142, 212);
			this.listSheetType.TabIndex = 85;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.textFontSize);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.comboFontName);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(92, 306);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(283, 72);
			this.groupBox1.TabIndex = 88;
			this.groupBox1.Text = "Default Font";
			// 
			// textFontSize
			// 
			this.textFontSize.Location = new System.Drawing.Point(80, 41);
			this.textFontSize.Name = "textFontSize";
			this.textFontSize.Size = new System.Drawing.Size(44, 20);
			this.textFontSize.TabIndex = 89;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(7, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(71, 16);
			this.label4.TabIndex = 89;
			this.label4.Text = "Size";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFontName
			// 
			this.comboFontName.Location = new System.Drawing.Point(80, 14);
			this.comboFontName.Name = "comboFontName";
			this.comboFontName.Size = new System.Drawing.Size(197, 21);
			this.comboFontName.TabIndex = 88;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 16);
			this.label3.TabIndex = 87;
			this.label3.Text = "Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(101, 384);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(71, 16);
			this.label7.TabIndex = 94;
			this.label7.Text = "Width";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(101, 410);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(71, 16);
			this.label8.TabIndex = 96;
			this.label8.Text = "Height";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(154, 12);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(215, 20);
			this.textDescription.TabIndex = 99;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(39, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 16);
			this.label1.TabIndex = 98;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsLandscape
			// 
			this.checkIsLandscape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkIsLandscape.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsLandscape.Location = new System.Drawing.Point(60, 435);
			this.checkIsLandscape.Name = "checkIsLandscape";
			this.checkIsLandscape.Size = new System.Drawing.Size(126, 20);
			this.checkIsLandscape.TabIndex = 100;
			this.checkIsLandscape.Text = "Landscape";
			this.checkIsLandscape.CheckedChanged += new System.EventHandler(this.checkIsLandscape_Click);
			// 
			// textHeight
			// 
			this.textHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textHeight.Location = new System.Drawing.Point(172, 409);
			this.textHeight.MaxVal = 2000;
			this.textHeight.MinVal = -100;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(69, 20);
			this.textHeight.TabIndex = 97;
			// 
			// textWidth
			// 
			this.textWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textWidth.Location = new System.Drawing.Point(172, 383);
			this.textWidth.MaxVal = 2000;
			this.textWidth.MinVal = -100;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(69, 20);
			this.textWidth.TabIndex = 95;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(319, 449);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(409, 449);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkBypassLockDate
			// 
			this.checkBypassLockDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBypassLockDate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBypassLockDate.Location = new System.Drawing.Point(22, 281);
			this.checkBypassLockDate.Name = "checkBypassLockDate";
			this.checkBypassLockDate.Size = new System.Drawing.Size(164, 20);
			this.checkBypassLockDate.TabIndex = 101;
			this.checkBypassLockDate.Text = "Bypass Global Lock Date";
			// 
			// checkHasMobileLayout
			// 
			this.checkHasMobileLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkHasMobileLayout.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHasMobileLayout.Location = new System.Drawing.Point(60, 461);
			this.checkHasMobileLayout.Name = "checkHasMobileLayout";
			this.checkHasMobileLayout.Size = new System.Drawing.Size(126, 20);
			this.checkHasMobileLayout.TabIndex = 102;
			this.checkHasMobileLayout.Text = "Use Mobile Layout";
			this.checkHasMobileLayout.CheckedChanged += new System.EventHandler(this.CheckHasMobileLayout_CheckedChanged);
			// 
			// checkAutoSaveCheck
			// 
			this.checkAutoSaveCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkAutoSaveCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoSaveCheck.Location = new System.Drawing.Point(12, 260);
			this.checkAutoSaveCheck.Name = "checkAutoSaveCheck";
			this.checkAutoSaveCheck.Size = new System.Drawing.Size(174, 20);
			this.checkAutoSaveCheck.TabIndex = 103;
			this.checkAutoSaveCheck.Text = "Check \'Autosave\' By Default";
			this.checkAutoSaveCheck.Visible = false;
			// 
			// FormSheetDef
			// 
			this.ClientSize = new System.Drawing.Size(496, 485);
			this.Controls.Add(this.checkAutoSaveCheck);
			this.Controls.Add(this.checkHasMobileLayout);
			this.Controls.Add(this.checkBypassLockDate);
			this.Controls.Add(this.checkIsLandscape);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelSheetType);
			this.Controls.Add(this.listSheetType);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDef";
			this.Text = "Sheet Def";
			this.Load += new System.EventHandler(this.FormSheetDef_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelSheetType;
		private OpenDental.UI.ListBoxOD listSheetType;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ComboBox comboFontName;
		private System.Windows.Forms.TextBox textFontSize;
		private System.Windows.Forms.Label label4;
		private ValidNum textWidth;
		private System.Windows.Forms.Label label7;
		private ValidNum textHeight;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.CheckBox checkIsLandscape;
		private OpenDental.UI.CheckBox checkBypassLockDate;
		private OpenDental.UI.CheckBox checkHasMobileLayout;
		private OpenDental.UI.CheckBox checkAutoSaveCheck;
	}
}