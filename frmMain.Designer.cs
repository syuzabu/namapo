namespace namapo
{
  partial class frmMain
  {
    /// <summary>
    /// 必要なデザイナー変数です。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 使用中のリソースをすべてクリーンアップします。
    /// </summary>
    /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows フォーム デザイナーで生成されたコード

    /// <summary>
    /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
    /// コード エディターで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
            this.btnSend = new System.Windows.Forms.Button();
            this.cbxNic = new System.Windows.Forms.ComboBox();
            this.lblNic = new System.Windows.Forms.Label();
            this.lblIPa = new System.Windows.Forms.Label();
            this.txtIPaL = new System.Windows.Forms.TextBox();
            this.lblMac = new System.Windows.Forms.Label();
            this.txtMacL = new System.Windows.Forms.TextBox();
            this.lblPayload = new System.Windows.Forms.Label();
            this.lblInv = new System.Windows.Forms.Label();
            this.txtInv = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPortL = new System.Windows.Forms.TextBox();
            this.lblL = new System.Windows.Forms.Label();
            this.txtIPaR = new System.Windows.Forms.TextBox();
            this.lblR = new System.Windows.Forms.Label();
            this.txtMacR = new System.Windows.Forms.TextBox();
            this.txtPortR = new System.Windows.Forms.TextBox();
            this.btnRecv = new System.Windows.Forms.Button();
            this.txtResR = new System.Windows.Forms.TextBox();
            this.txtFrasize = new System.Windows.Forms.TextBox();
            this.lblFrasize = new System.Windows.Forms.Label();
            this.nudPayload = new System.Windows.Forms.NumericUpDown();
            this.txtResS = new System.Windows.Forms.TextBox();
            this.chkMacL = new System.Windows.Forms.CheckBox();
            this.chkIPaL = new System.Windows.Forms.CheckBox();
            this.chkPortL = new System.Windows.Forms.CheckBox();
            this.btnIFset = new System.Windows.Forms.Button();
            this.lblUnit = new System.Windows.Forms.Label();
            this.cbxUnit = new System.Windows.Forms.ComboBox();
            this.lblCalc = new System.Windows.Forms.Label();
            this.cbxCalc = new System.Windows.Forms.ComboBox();
            this.btnReload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudPayload)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(682, 252);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(90, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "送信テスト開始";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cbxNic
            // 
            this.cbxNic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNic.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cbxNic.FormattingEnabled = true;
            this.cbxNic.Location = new System.Drawing.Point(74, 6);
            this.cbxNic.Name = "cbxNic";
            this.cbxNic.Size = new System.Drawing.Size(627, 20);
            this.cbxNic.TabIndex = 1;
            // 
            // lblNic
            // 
            this.lblNic.AutoSize = true;
            this.lblNic.Location = new System.Drawing.Point(12, 9);
            this.lblNic.Name = "lblNic";
            this.lblNic.Size = new System.Drawing.Size(56, 12);
            this.lblNic.TabIndex = 2;
            this.lblNic.Text = "Interfaces";
            // 
            // lblIPa
            // 
            this.lblIPa.AutoSize = true;
            this.lblIPa.Location = new System.Drawing.Point(12, 95);
            this.lblIPa.Name = "lblIPa";
            this.lblIPa.Size = new System.Drawing.Size(51, 12);
            this.lblIPa.TabIndex = 2;
            this.lblIPa.Text = "IPアドレス";
            // 
            // txtIPaL
            // 
            this.txtIPaL.Location = new System.Drawing.Point(92, 92);
            this.txtIPaL.Name = "txtIPaL";
            this.txtIPaL.Size = new System.Drawing.Size(119, 19);
            this.txtIPaL.TabIndex = 3;
            this.txtIPaL.Text = "172.16.0.1";
            // 
            // lblMac
            // 
            this.lblMac.AutoSize = true;
            this.lblMac.Location = new System.Drawing.Point(12, 68);
            this.lblMac.Name = "lblMac";
            this.lblMac.Size = new System.Drawing.Size(66, 12);
            this.lblMac.TabIndex = 2;
            this.lblMac.Text = "MACアドレス";
            // 
            // txtMacL
            // 
            this.txtMacL.Location = new System.Drawing.Point(92, 65);
            this.txtMacL.Name = "txtMacL";
            this.txtMacL.Size = new System.Drawing.Size(119, 19);
            this.txtMacL.TabIndex = 3;
            this.txtMacL.Text = "12-34-56-78-EF-01";
            // 
            // lblPayload
            // 
            this.lblPayload.AutoSize = true;
            this.lblPayload.Location = new System.Drawing.Point(16, 196);
            this.lblPayload.Name = "lblPayload";
            this.lblPayload.Size = new System.Drawing.Size(79, 12);
            this.lblPayload.TabIndex = 2;
            this.lblPayload.Text = "Payload (byte)";
            // 
            // lblInv
            // 
            this.lblInv.AutoSize = true;
            this.lblInv.Location = new System.Drawing.Point(12, 169);
            this.lblInv.Name = "lblInv";
            this.lblInv.Size = new System.Drawing.Size(83, 12);
            this.lblInv.TabIndex = 2;
            this.lblInv.Text = "計測時間 (sec)";
            // 
            // txtInv
            // 
            this.txtInv.Location = new System.Drawing.Point(101, 166);
            this.txtInv.Name = "txtInv";
            this.txtInv.Size = new System.Drawing.Size(95, 19);
            this.txtInv.TabIndex = 4;
            this.txtInv.Text = "10";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(12, 124);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(57, 12);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "ポート番号";
            // 
            // txtPortL
            // 
            this.txtPortL.Location = new System.Drawing.Point(92, 121);
            this.txtPortL.Name = "txtPortL";
            this.txtPortL.Size = new System.Drawing.Size(119, 19);
            this.txtPortL.TabIndex = 3;
            this.txtPortL.Text = "11111";
            // 
            // lblL
            // 
            this.lblL.AutoSize = true;
            this.lblL.Location = new System.Drawing.Point(90, 39);
            this.lblL.Name = "lblL";
            this.lblL.Size = new System.Drawing.Size(67, 12);
            this.lblL.TabIndex = 2;
            this.lblL.Text = "ローカル設定";
            // 
            // txtIPaR
            // 
            this.txtIPaR.Location = new System.Drawing.Point(254, 92);
            this.txtIPaR.Name = "txtIPaR";
            this.txtIPaR.Size = new System.Drawing.Size(119, 19);
            this.txtIPaR.TabIndex = 3;
            this.txtIPaR.Text = "255.255.255.255";
            // 
            // lblR
            // 
            this.lblR.AutoSize = true;
            this.lblR.Location = new System.Drawing.Point(252, 39);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(63, 12);
            this.lblR.TabIndex = 2;
            this.lblR.Text = "リモート設定";
            // 
            // txtMacR
            // 
            this.txtMacR.Location = new System.Drawing.Point(254, 65);
            this.txtMacR.Name = "txtMacR";
            this.txtMacR.Size = new System.Drawing.Size(119, 19);
            this.txtMacR.TabIndex = 3;
            this.txtMacR.Text = "12-34-56-78-70-02";
            // 
            // txtPortR
            // 
            this.txtPortR.Location = new System.Drawing.Point(254, 121);
            this.txtPortR.Name = "txtPortR";
            this.txtPortR.Size = new System.Drawing.Size(119, 19);
            this.txtPortR.TabIndex = 3;
            this.txtPortR.Text = "12345";
            // 
            // btnRecv
            // 
            this.btnRecv.Location = new System.Drawing.Point(520, 252);
            this.btnRecv.Name = "btnRecv";
            this.btnRecv.Size = new System.Drawing.Size(90, 23);
            this.btnRecv.TabIndex = 0;
            this.btnRecv.Text = "受信テスト開始";
            this.btnRecv.UseVisualStyleBackColor = true;
            this.btnRecv.Click += new System.EventHandler(this.btnRecv_Click);
            // 
            // txtResR
            // 
            this.txtResR.Location = new System.Drawing.Point(426, 39);
            this.txtResR.Multiline = true;
            this.txtResR.Name = "txtResR";
            this.txtResR.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResR.Size = new System.Drawing.Size(184, 207);
            this.txtResR.TabIndex = 5;
            // 
            // txtFrasize
            // 
            this.txtFrasize.Location = new System.Drawing.Point(300, 192);
            this.txtFrasize.Name = "txtFrasize";
            this.txtFrasize.ReadOnly = true;
            this.txtFrasize.Size = new System.Drawing.Size(54, 19);
            this.txtFrasize.TabIndex = 4;
            // 
            // lblFrasize
            // 
            this.lblFrasize.AutoSize = true;
            this.lblFrasize.Location = new System.Drawing.Point(230, 196);
            this.lblFrasize.Name = "lblFrasize";
            this.lblFrasize.Size = new System.Drawing.Size(64, 12);
            this.lblFrasize.TabIndex = 2;
            this.lblFrasize.Text = "= framesize";
            // 
            // nudPayload
            // 
            this.nudPayload.Location = new System.Drawing.Point(101, 194);
            this.nudPayload.Maximum = new decimal(new int[] {
            1472,
            0,
            0,
            0});
            this.nudPayload.Name = "nudPayload";
            this.nudPayload.Size = new System.Drawing.Size(95, 19);
            this.nudPayload.TabIndex = 6;
            this.nudPayload.Value = new decimal(new int[] {
            1472,
            0,
            0,
            0});
            this.nudPayload.ValueChanged += new System.EventHandler(this.nudPayload_ValueChanged);
            // 
            // txtResS
            // 
            this.txtResS.Location = new System.Drawing.Point(616, 39);
            this.txtResS.Multiline = true;
            this.txtResS.Name = "txtResS";
            this.txtResS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResS.Size = new System.Drawing.Size(156, 207);
            this.txtResS.TabIndex = 5;
            // 
            // chkMacL
            // 
            this.chkMacL.AutoSize = true;
            this.chkMacL.Checked = true;
            this.chkMacL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMacL.Location = new System.Drawing.Point(217, 68);
            this.chkMacL.Name = "chkMacL";
            this.chkMacL.Size = new System.Drawing.Size(15, 14);
            this.chkMacL.TabIndex = 7;
            this.chkMacL.UseVisualStyleBackColor = true;
            // 
            // chkIPaL
            // 
            this.chkIPaL.AutoSize = true;
            this.chkIPaL.Location = new System.Drawing.Point(217, 95);
            this.chkIPaL.Name = "chkIPaL";
            this.chkIPaL.Size = new System.Drawing.Size(15, 14);
            this.chkIPaL.TabIndex = 7;
            this.chkIPaL.UseVisualStyleBackColor = true;
            // 
            // chkPortL
            // 
            this.chkPortL.AutoSize = true;
            this.chkPortL.Location = new System.Drawing.Point(217, 124);
            this.chkPortL.Name = "chkPortL";
            this.chkPortL.Size = new System.Drawing.Size(15, 14);
            this.chkPortL.TabIndex = 7;
            this.chkPortL.UseVisualStyleBackColor = true;
            // 
            // btnIFset
            // 
            this.btnIFset.Location = new System.Drawing.Point(194, 34);
            this.btnIFset.Name = "btnIFset";
            this.btnIFset.Size = new System.Drawing.Size(17, 23);
            this.btnIFset.TabIndex = 8;
            this.btnIFset.Text = "↓";
            this.btnIFset.UseVisualStyleBackColor = true;
            this.btnIFset.Click += new System.EventHandler(this.btnIFset_Click);
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(16, 222);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(71, 12);
            this.lblUnit.TabIndex = 2;
            this.lblUnit.Text = "計測bps単位";
            // 
            // cbxUnit
            // 
            this.cbxUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxUnit.FormattingEnabled = true;
            this.cbxUnit.Items.AddRange(new object[] {
            "1000bps=1kbps, 1000kbps=1Mbps  (Wireshark)",
            "1024bps=1kbps, 1024kbps=1Mbps"});
            this.cbxUnit.Location = new System.Drawing.Point(101, 219);
            this.cbxUnit.Name = "cbxUnit";
            this.cbxUnit.Size = new System.Drawing.Size(272, 20);
            this.cbxUnit.TabIndex = 9;
            // 
            // lblCalc
            // 
            this.lblCalc.AutoSize = true;
            this.lblCalc.Location = new System.Drawing.Point(16, 252);
            this.lblCalc.Name = "lblCalc";
            this.lblCalc.Size = new System.Drawing.Size(53, 12);
            this.lblCalc.TabIndex = 2;
            this.lblCalc.Text = "計測箇所";
            // 
            // cbxCalc
            // 
            this.cbxCalc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCalc.FormattingEnabled = true;
            this.cbxCalc.Items.AddRange(new object[] {
            "framesize(パケット長) で計測する (Wireshark)",
            "Payload(データ長) で計測する"});
            this.cbxCalc.Location = new System.Drawing.Point(101, 252);
            this.cbxCalc.Name = "cbxCalc";
            this.cbxCalc.Size = new System.Drawing.Size(272, 20);
            this.cbxCalc.TabIndex = 9;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(707, 5);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(65, 23);
            this.btnReload.TabIndex = 10;
            this.btnReload.Text = "更新";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 281);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.cbxCalc);
            this.Controls.Add(this.cbxUnit);
            this.Controls.Add(this.btnIFset);
            this.Controls.Add(this.chkPortL);
            this.Controls.Add(this.chkIPaL);
            this.Controls.Add(this.chkMacL);
            this.Controls.Add(this.nudPayload);
            this.Controls.Add(this.txtResS);
            this.Controls.Add(this.txtResR);
            this.Controls.Add(this.txtInv);
            this.Controls.Add(this.txtFrasize);
            this.Controls.Add(this.txtPortR);
            this.Controls.Add(this.txtPortL);
            this.Controls.Add(this.txtMacR);
            this.Controls.Add(this.txtMacL);
            this.Controls.Add(this.lblInv);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblFrasize);
            this.Controls.Add(this.lblCalc);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.lblPayload);
            this.Controls.Add(this.lblR);
            this.Controls.Add(this.lblL);
            this.Controls.Add(this.txtIPaR);
            this.Controls.Add(this.lblMac);
            this.Controls.Add(this.txtIPaL);
            this.Controls.Add(this.lblIPa);
            this.Controls.Add(this.lblNic);
            this.Controls.Add(this.cbxNic);
            this.Controls.Add(this.btnRecv);
            this.Controls.Add(this.btnSend);
            this.Name = "frmMain";
            this.Text = "UDP速度計測ツール";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudPayload)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnSend;
    private System.Windows.Forms.ComboBox cbxNic;
    private System.Windows.Forms.Label lblNic;
    private System.Windows.Forms.Label lblIPa;
    private System.Windows.Forms.TextBox txtIPaL;
    private System.Windows.Forms.Label lblMac;
    private System.Windows.Forms.TextBox txtMacL;
    private System.Windows.Forms.Label lblPayload;
    private System.Windows.Forms.Label lblInv;
    private System.Windows.Forms.TextBox txtInv;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.TextBox txtPortL;
    private System.Windows.Forms.Label lblL;
    private System.Windows.Forms.TextBox txtIPaR;
    private System.Windows.Forms.Label lblR;
    private System.Windows.Forms.TextBox txtMacR;
    private System.Windows.Forms.TextBox txtPortR;
    private System.Windows.Forms.Button btnRecv;
    private System.Windows.Forms.TextBox txtResR;
    private System.Windows.Forms.TextBox txtFrasize;
    private System.Windows.Forms.Label lblFrasize;
    private System.Windows.Forms.NumericUpDown nudPayload;
    private System.Windows.Forms.TextBox txtResS;
    private System.Windows.Forms.CheckBox chkMacL;
    private System.Windows.Forms.CheckBox chkIPaL;
    private System.Windows.Forms.CheckBox chkPortL;
    private System.Windows.Forms.Button btnIFset;
    private System.Windows.Forms.Label lblUnit;
    private System.Windows.Forms.ComboBox cbxUnit;
    private System.Windows.Forms.Label lblCalc;
    private System.Windows.Forms.ComboBox cbxCalc;
    private System.Windows.Forms.Button btnReload;
  }
}

