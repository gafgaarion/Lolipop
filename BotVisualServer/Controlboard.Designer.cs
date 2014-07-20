namespace BotVisualServer
{
    partial class Controlboard
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
            this.tcCharacters = new System.Windows.Forms.TabControl();
            this.cbLeader = new System.Windows.Forms.ComboBox();
            this.lblLeader = new System.Windows.Forms.Label();
            this.btnApplyStrategy = new System.Windows.Forms.Button();
            this.lstAggro = new System.Windows.Forms.ListView();
            this.lblAggroList = new System.Windows.Forms.Label();
            this.lstEnabledChars = new System.Windows.Forms.ListView();
            this.lblEnabledChar = new System.Windows.Forms.Label();
            this.cbStrategy = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tcCharacters
            // 
            this.tcCharacters.Location = new System.Drawing.Point(-4, 358);
            this.tcCharacters.Name = "tcCharacters";
            this.tcCharacters.SelectedIndex = 0;
            this.tcCharacters.Size = new System.Drawing.Size(1036, 426);
            this.tcCharacters.TabIndex = 1;
            // 
            // cbLeader
            // 
            this.cbLeader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLeader.FormattingEnabled = true;
            this.cbLeader.Location = new System.Drawing.Point(399, 331);
            this.cbLeader.Name = "cbLeader";
            this.cbLeader.Size = new System.Drawing.Size(121, 21);
            this.cbLeader.TabIndex = 7;
            this.cbLeader.SelectedIndexChanged += new System.EventHandler(this.cbLeader_SelectedIndexChanged);
            // 
            // lblLeader
            // 
            this.lblLeader.AutoSize = true;
            this.lblLeader.Location = new System.Drawing.Point(350, 334);
            this.lblLeader.Name = "lblLeader";
            this.lblLeader.Size = new System.Drawing.Size(43, 13);
            this.lblLeader.TabIndex = 8;
            this.lblLeader.Text = "Leader:";
            // 
            // btnApplyStrategy
            // 
            this.btnApplyStrategy.Location = new System.Drawing.Point(225, 329);
            this.btnApplyStrategy.Name = "btnApplyStrategy";
            this.btnApplyStrategy.Size = new System.Drawing.Size(109, 23);
            this.btnApplyStrategy.TabIndex = 10;
            this.btnApplyStrategy.Text = "ApplyStrategy";
            this.btnApplyStrategy.UseVisualStyleBackColor = true;
            this.btnApplyStrategy.Click += new System.EventHandler(this.btnApplyStrategy_Click);
            // 
            // lstAggro
            // 
            this.lstAggro.Location = new System.Drawing.Point(399, 31);
            this.lstAggro.Name = "lstAggro";
            this.lstAggro.Size = new System.Drawing.Size(121, 202);
            this.lstAggro.TabIndex = 11;
            this.lstAggro.UseCompatibleStateImageBehavior = false;
            this.lstAggro.View = System.Windows.Forms.View.List;
            // 
            // lblAggroList
            // 
            this.lblAggroList.AutoSize = true;
            this.lblAggroList.Location = new System.Drawing.Point(396, 15);
            this.lblAggroList.Name = "lblAggroList";
            this.lblAggroList.Size = new System.Drawing.Size(53, 13);
            this.lblAggroList.TabIndex = 12;
            this.lblAggroList.Text = "Aggro list:";
            // 
            // lstEnabledChars
            // 
            this.lstEnabledChars.Location = new System.Drawing.Point(272, 31);
            this.lstEnabledChars.Name = "lstEnabledChars";
            this.lstEnabledChars.Size = new System.Drawing.Size(121, 115);
            this.lstEnabledChars.TabIndex = 13;
            this.lstEnabledChars.UseCompatibleStateImageBehavior = false;
            this.lstEnabledChars.View = System.Windows.Forms.View.List;
            // 
            // lblEnabledChar
            // 
            this.lblEnabledChar.AutoSize = true;
            this.lblEnabledChar.Location = new System.Drawing.Point(269, 15);
            this.lblEnabledChar.Name = "lblEnabledChar";
            this.lblEnabledChar.Size = new System.Drawing.Size(102, 13);
            this.lblEnabledChar.TabIndex = 14;
            this.lblEnabledChar.Text = "Enabled characters:";
            // 
            // cbStrategy
            // 
            this.cbStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStrategy.FormattingEnabled = true;
            this.cbStrategy.Items.AddRange(new object[] {
            "ZeruhnStrategy",
            "VoidwatchStrategy",
            "DemoStrategy",
            "SpellSkillUpStrategy",
            "DynamisStrategy",
            "RangedSkillUpStrategy"});
            this.cbStrategy.Location = new System.Drawing.Point(98, 331);
            this.cbStrategy.Name = "cbStrategy";
            this.cbStrategy.Size = new System.Drawing.Size(121, 21);
            this.cbStrategy.TabIndex = 15;
            // 
            // Controlboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 888);
            this.Controls.Add(this.cbStrategy);
            this.Controls.Add(this.lblEnabledChar);
            this.Controls.Add(this.lstEnabledChars);
            this.Controls.Add(this.lblAggroList);
            this.Controls.Add(this.lstAggro);
            this.Controls.Add(this.btnApplyStrategy);
            this.Controls.Add(this.lblLeader);
            this.Controls.Add(this.cbLeader);
            this.Controls.Add(this.tcCharacters);
            this.Name = "Controlboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Controlboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Controlboard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Controlboard_FormClosed);
            this.Load += new System.EventHandler(this.Controlboard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tcCharacters;
        private System.Windows.Forms.ComboBox cbLeader;
        private System.Windows.Forms.Label lblLeader;
        private System.Windows.Forms.Button btnApplyStrategy;
        private System.Windows.Forms.ListView lstAggro;
        private System.Windows.Forms.Label lblAggroList;
        private System.Windows.Forms.ListView lstEnabledChars;
        private System.Windows.Forms.Label lblEnabledChar;
        private System.Windows.Forms.ComboBox cbStrategy;
    }
}

