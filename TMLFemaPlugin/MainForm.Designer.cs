
namespace TMLFemaPlugin
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fetchDisasters = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.cbox1 = new TMLFemaPlugin.ComboxBoxEx();
            this.btnSaveDisaster = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fetchDisasters
            // 
            this.fetchDisasters.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fetchDisasters.Location = new System.Drawing.Point(10, 53);
            this.fetchDisasters.Margin = new System.Windows.Forms.Padding(2);
            this.fetchDisasters.Name = "fetchDisasters";
            this.fetchDisasters.Size = new System.Drawing.Size(117, 24);
            this.fetchDisasters.TabIndex = 1;
            this.fetchDisasters.Text = "Check for Disasters";
            this.fetchDisasters.UseVisualStyleBackColor = true;
            this.fetchDisasters.Click += new System.EventHandler(this.fetchDisasters_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(20, 92);
            this.lblResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(59, 14);
            this.lblResult.TabIndex = 3;
            this.lblResult.Text = "result label";
            // 
            // cbox1
            // 
            this.cbox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbox1.Location = new System.Drawing.Point(148, 53);
            this.cbox1.Margin = new System.Windows.Forms.Padding(2);
            this.cbox1.Name = "cbox1";
            this.cbox1.Size = new System.Drawing.Size(187, 22);
            this.cbox1.TabIndex = 14;
            this.cbox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbox1_KeyPress);
            // 
            // btnSaveDisaster
            // 
            this.btnSaveDisaster.Enabled = false;
            this.btnSaveDisaster.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveDisaster.Location = new System.Drawing.Point(103, 128);
            this.btnSaveDisaster.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveDisaster.Name = "btnSaveDisaster";
            this.btnSaveDisaster.Size = new System.Drawing.Size(117, 24);
            this.btnSaveDisaster.TabIndex = 16;
            this.btnSaveDisaster.Text = "Save Disaster";
            this.btnSaveDisaster.UseVisualStyleBackColor = true;
            this.btnSaveDisaster.Click += new System.EventHandler(this.btnSaveDisaster_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.SteelBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, -3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 33);
            this.label1.TabIndex = 17;
            this.label1.Text = " FEMA Disaster Plugin";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(350, 181);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSaveDisaster);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.fetchDisasters);
            this.Controls.Add(this.cbox1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TML FEMA Plugin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button fetchDisasters;
        private System.Windows.Forms.Label lblResult;
        private TMLFemaPlugin.ComboxBoxEx cbox1;
        private System.Windows.Forms.Button btnSaveDisaster;
        private System.Windows.Forms.Label label1;
    }
}

