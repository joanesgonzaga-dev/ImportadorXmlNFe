
using DinnamuS_Desktop_2._0.Model.Produto;
using System.Collections.Generic;

namespace ImportadorXmlNFe
{
    partial class FrmLerXMLNFe
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtLocalXML = new System.Windows.Forms.TextBox();
            this.btnLocalizarXML = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtIE = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateEmissao = new System.Windows.Forms.DateTimePicker();
            this.txtChaveDeAcesso = new System.Windows.Forms.TextBox();
            this.txtEmitente = new System.Windows.Forms.TextBox();
            this.txtNumeroNota = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.lblMensagemStatus = new System.Windows.Forms.Label();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtLeituraArquivo = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCancelarXML = new System.Windows.Forms.Button();
            this.btnImportarXML = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.txtDestinatario = new System.Windows.Forms.TextBox();
            this.txtCnpjEmitente = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_LocaisDeEstoque = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtDestinatarioCNPJ = new System.Windows.Forms.MaskedTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.cProd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cEAN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xProd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qCom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vUnCom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vProd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isExiste = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.isReceber = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.acaoProdNFe = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cad = new System.Windows.Forms.DataGridViewImageColumn();
            this.vinc = new System.Windows.Forms.DataGridViewImageColumn();
            this.groupBox2.SuspendLayout();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLocalXML
            // 
            this.txtLocalXML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalXML.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocalXML.Location = new System.Drawing.Point(9, 24);
            this.txtLocalXML.Margin = new System.Windows.Forms.Padding(2);
            this.txtLocalXML.Name = "txtLocalXML";
            this.txtLocalXML.Size = new System.Drawing.Size(659, 25);
            this.txtLocalXML.TabIndex = 0;
            // 
            // btnLocalizarXML
            // 
            this.btnLocalizarXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocalizarXML.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.btnLocalizarXML.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnLocalizarXML.FlatAppearance.BorderSize = 0;
            this.btnLocalizarXML.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnLocalizarXML.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Goldenrod;
            this.btnLocalizarXML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocalizarXML.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLocalizarXML.ForeColor = System.Drawing.Color.Transparent;
            this.btnLocalizarXML.Location = new System.Drawing.Point(672, 21);
            this.btnLocalizarXML.Margin = new System.Windows.Forms.Padding(2);
            this.btnLocalizarXML.Name = "btnLocalizarXML";
            this.btnLocalizarXML.Size = new System.Drawing.Size(126, 28);
            this.btnLocalizarXML.TabIndex = 1;
            this.btnLocalizarXML.Text = "Localizar Arquivo";
            this.btnLocalizarXML.UseVisualStyleBackColor = false;
            this.btnLocalizarXML.Click += new System.EventHandler(this.btnLocalizarXML_Click);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Black;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(311, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "IE:";
            // 
            // txtIE
            // 
            this.txtIE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIE.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIE.Location = new System.Drawing.Point(313, 150);
            this.txtIE.Name = "txtIE";
            this.txtIE.Size = new System.Drawing.Size(164, 25);
            this.txtIE.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Black;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(309, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "CNPJ:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Black;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(13, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "Chave de Acesso:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Black;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(13, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Fornecedor:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(102, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Data de Emissão:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nº da Nota:";
            // 
            // dateEmissao
            // 
            this.dateEmissao.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateEmissao.Location = new System.Drawing.Point(107, 36);
            this.dateEmissao.Name = "dateEmissao";
            this.dateEmissao.Size = new System.Drawing.Size(284, 25);
            this.dateEmissao.TabIndex = 7;
            // 
            // txtChaveDeAcesso
            // 
            this.txtChaveDeAcesso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChaveDeAcesso.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChaveDeAcesso.Location = new System.Drawing.Point(16, 150);
            this.txtChaveDeAcesso.Margin = new System.Windows.Forms.Padding(2);
            this.txtChaveDeAcesso.Name = "txtChaveDeAcesso";
            this.txtChaveDeAcesso.Size = new System.Drawing.Size(292, 25);
            this.txtChaveDeAcesso.TabIndex = 3;
            // 
            // txtEmitente
            // 
            this.txtEmitente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmitente.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmitente.Location = new System.Drawing.Point(16, 93);
            this.txtEmitente.Margin = new System.Windows.Forms.Padding(2);
            this.txtEmitente.Name = "txtEmitente";
            this.txtEmitente.Size = new System.Drawing.Size(292, 25);
            this.txtEmitente.TabIndex = 2;
            // 
            // txtNumeroNota
            // 
            this.txtNumeroNota.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumeroNota.Location = new System.Drawing.Point(15, 36);
            this.txtNumeroNota.Margin = new System.Windows.Forms.Padding(2);
            this.txtNumeroNota.Name = "txtNumeroNota";
            this.txtNumeroNota.Size = new System.Drawing.Size(85, 25);
            this.txtNumeroNota.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Black;
            this.groupBox2.Controls.Add(this.txtLocalXML);
            this.groupBox2.Controls.Add(this.btnLocalizarXML);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(8, 8);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(806, 65);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Arquivo:";
            // 
            // panelStatus
            // 
            this.panelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelStatus.BackColor = System.Drawing.Color.Green;
            this.panelStatus.Controls.Add(this.lblMensagemStatus);
            this.panelStatus.Location = new System.Drawing.Point(819, 16);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(241, 56);
            this.panelStatus.TabIndex = 18;
            // 
            // lblMensagemStatus
            // 
            this.lblMensagemStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMensagemStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensagemStatus.Location = new System.Drawing.Point(1, 7);
            this.lblMensagemStatus.Name = "lblMensagemStatus";
            this.lblMensagemStatus.Size = new System.Drawing.Size(235, 42);
            this.lblMensagemStatus.TabIndex = 0;
            this.lblMensagemStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProdutos.BackgroundColor = System.Drawing.Color.NavajoWhite;
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cProd,
            this.cEAN,
            this.xProd,
            this.qCom,
            this.vUnCom,
            this.vProd,
            this.isExiste,
            this.isReceber,
            this.acaoProdNFe,
            this.cad,
            this.vinc});
            this.dgvProdutos.Location = new System.Drawing.Point(2, 5);
            this.dgvProdutos.Margin = new System.Windows.Forms.Padding(2);
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.RowHeadersWidth = 62;
            this.dgvProdutos.RowTemplate.Height = 28;
            this.dgvProdutos.Size = new System.Drawing.Size(1039, 108);
            this.dgvProdutos.TabIndex = 4;
            this.dgvProdutos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProdutos_CellClick);
            this.dgvProdutos.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProdutos_CellValueChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(8, 274);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1054, 145);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvProdutos);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1046, 115);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Itens da Nota";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtLeituraArquivo);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1046, 115);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Leitura do Arquivo";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtLeituraArquivo
            // 
            this.txtLeituraArquivo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLeituraArquivo.Location = new System.Drawing.Point(3, 6);
            this.txtLeituraArquivo.Multiline = true;
            this.txtLeituraArquivo.Name = "txtLeituraArquivo";
            this.txtLeituraArquivo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLeituraArquivo.Size = new System.Drawing.Size(759, 207);
            this.txtLeituraArquivo.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.BackColor = System.Drawing.Color.Black;
            this.groupBox3.Controls.Add(this.btnCancelarXML);
            this.groupBox3.Controls.Add(this.btnImportarXML);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(8, 417);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.groupBox3.Size = new System.Drawing.Size(1054, 92);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            // 
            // btnCancelarXML
            // 
            this.btnCancelarXML.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancelarXML.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.btnCancelarXML.FlatAppearance.BorderSize = 0;
            this.btnCancelarXML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelarXML.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarXML.ForeColor = System.Drawing.Color.White;
            this.btnCancelarXML.Location = new System.Drawing.Point(526, 21);
            this.btnCancelarXML.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancelarXML.Name = "btnCancelarXML";
            this.btnCancelarXML.Size = new System.Drawing.Size(284, 54);
            this.btnCancelarXML.TabIndex = 7;
            this.btnCancelarXML.Text = "Cancelar NF-e";
            this.btnCancelarXML.UseVisualStyleBackColor = false;
            this.btnCancelarXML.Click += new System.EventHandler(this.btnCancelarXML_Click);
            // 
            // btnImportarXML
            // 
            this.btnImportarXML.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnImportarXML.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.btnImportarXML.FlatAppearance.BorderSize = 0;
            this.btnImportarXML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportarXML.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportarXML.ForeColor = System.Drawing.Color.White;
            this.btnImportarXML.Location = new System.Drawing.Point(227, 21);
            this.btnImportarXML.Margin = new System.Windows.Forms.Padding(2);
            this.btnImportarXML.Name = "btnImportarXML";
            this.btnImportarXML.Size = new System.Drawing.Size(284, 54);
            this.btnImportarXML.TabIndex = 6;
            this.btnImportarXML.Text = "Importar NF-e";
            this.btnImportarXML.UseVisualStyleBackColor = false;
            this.btnImportarXML.Click += new System.EventHandler(this.btnImportarXML_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtDestinatario);
            this.panel1.Controls.Add(this.txtCnpjEmitente);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cb_LocaisDeEstoque);
            this.panel1.Controls.Add(this.txtIE);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtChaveDeAcesso);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtEmitente);
            this.panel1.Controls.Add(this.txtNumeroNota);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.dateEmissao);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(8, 78);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1052, 190);
            this.panel1.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(507, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "Destinatário:";
            // 
            // txtDestinatario
            // 
            this.txtDestinatario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestinatario.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestinatario.Location = new System.Drawing.Point(510, 93);
            this.txtDestinatario.Name = "txtDestinatario";
            this.txtDestinatario.Size = new System.Drawing.Size(533, 25);
            this.txtDestinatario.TabIndex = 19;
            // 
            // txtCnpjEmitente
            // 
            this.txtCnpjEmitente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCnpjEmitente.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCnpjEmitente.Location = new System.Drawing.Point(313, 93);
            this.txtCnpjEmitente.Mask = "00.000.000/0000-00";
            this.txtCnpjEmitente.Name = "txtCnpjEmitente";
            this.txtCnpjEmitente.Size = new System.Drawing.Size(163, 25);
            this.txtCnpjEmitente.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Black;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(507, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 17);
            this.label7.TabIndex = 17;
            this.label7.Text = "Local de Estoque:";
            // 
            // cb_LocaisDeEstoque
            // 
            this.cb_LocaisDeEstoque.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_LocaisDeEstoque.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_LocaisDeEstoque.FormattingEnabled = true;
            this.cb_LocaisDeEstoque.Location = new System.Drawing.Point(510, 36);
            this.cb_LocaisDeEstoque.Name = "cb_LocaisDeEstoque";
            this.cb_LocaisDeEstoque.Size = new System.Drawing.Size(212, 25);
            this.cb_LocaisDeEstoque.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Location = new System.Drawing.Point(0, -6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(487, 193);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.txtDestinatarioCNPJ);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(493, -5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(556, 192);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            // 
            // txtDestinatarioCNPJ
            // 
            this.txtDestinatarioCNPJ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestinatarioCNPJ.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestinatarioCNPJ.Location = new System.Drawing.Point(17, 155);
            this.txtDestinatarioCNPJ.Mask = "00.000.000/0000-00";
            this.txtDestinatarioCNPJ.Name = "txtDestinatarioCNPJ";
            this.txtDestinatarioCNPJ.Size = new System.Drawing.Size(163, 25);
            this.txtDestinatarioCNPJ.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Black;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(14, 135);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 17);
            this.label9.TabIndex = 24;
            this.label9.Text = "CNPJ:";
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewImageColumn1.FillWeight = 30F;
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.MinimumWidth = 30;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn1.Width = 30;
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewImageColumn2.FillWeight = 32F;
            this.dataGridViewImageColumn2.HeaderText = "";
            this.dataGridViewImageColumn2.MinimumWidth = 30;
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            this.dataGridViewImageColumn2.Visible = false;
            this.dataGridViewImageColumn2.Width = 30;
            // 
            // cProd
            // 
            this.cProd.DataPropertyName = "cProd";
            this.cProd.HeaderText = "Código";
            this.cProd.MinimumWidth = 90;
            this.cProd.Name = "cProd";
            this.cProd.ReadOnly = true;
            this.cProd.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cProd.Width = 90;
            // 
            // cEAN
            // 
            this.cEAN.DataPropertyName = "cEAN";
            this.cEAN.HeaderText = "GTIN";
            this.cEAN.Name = "cEAN";
            this.cEAN.ReadOnly = true;
            // 
            // xProd
            // 
            this.xProd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.xProd.DataPropertyName = "xProd";
            this.xProd.HeaderText = "Descrição";
            this.xProd.Name = "xProd";
            this.xProd.ReadOnly = true;
            // 
            // qCom
            // 
            this.qCom.DataPropertyName = "qCom";
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            this.qCom.DefaultCellStyle = dataGridViewCellStyle1;
            this.qCom.HeaderText = "Quantidade";
            this.qCom.Name = "qCom";
            this.qCom.ReadOnly = true;
            this.qCom.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // vUnCom
            // 
            this.vUnCom.DataPropertyName = "vUnCom";
            dataGridViewCellStyle2.Format = "C2";
            dataGridViewCellStyle2.NullValue = "0";
            this.vUnCom.DefaultCellStyle = dataGridViewCellStyle2;
            this.vUnCom.HeaderText = "Preço";
            this.vUnCom.Name = "vUnCom";
            this.vUnCom.ReadOnly = true;
            // 
            // vProd
            // 
            this.vProd.DataPropertyName = "vProd";
            dataGridViewCellStyle3.Format = "C2";
            dataGridViewCellStyle3.NullValue = "0";
            this.vProd.DefaultCellStyle = dataGridViewCellStyle3;
            this.vProd.HeaderText = "Total";
            this.vProd.Name = "vProd";
            this.vProd.ReadOnly = true;
            // 
            // isExiste
            // 
            this.isExiste.DataPropertyName = "isExiste";
            this.isExiste.FalseValue = "";
            this.isExiste.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.isExiste.HeaderText = "Existe?";
            this.isExiste.Name = "isExiste";
            this.isExiste.ReadOnly = true;
            this.isExiste.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.isExiste.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // isReceber
            // 
            this.isReceber.DataPropertyName = "isReceber";
            this.isReceber.HeaderText = "Receber?";
            this.isReceber.Name = "isReceber";
            // 
            // acaoProdNFe
            // 
            this.acaoProdNFe.AutoComplete = false;
            this.acaoProdNFe.DataPropertyName = "acaoProdNFe";
            this.acaoProdNFe.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.acaoProdNFe.HeaderText = "Ação";
            this.acaoProdNFe.Name = "acaoProdNFe";
            this.acaoProdNFe.Visible = false;
            // 
            // cad
            // 
            this.cad.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cad.FillWeight = 30F;
            this.cad.HeaderText = "";
            this.cad.Image = global::ImportadorXmlNFe.Properties.Resources.edit_16;
            this.cad.MinimumWidth = 30;
            this.cad.Name = "cad";
            this.cad.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cad.Width = 30;
            // 
            // vinc
            // 
            this.vinc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.vinc.FillWeight = 32F;
            this.vinc.HeaderText = "";
            this.vinc.MinimumWidth = 30;
            this.vinc.Name = "vinc";
            this.vinc.Visible = false;
            this.vinc.Width = 32;
            // 
            // FrmLerXMLNFe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1070, 528);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmLerXMLNFe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Leitura e Importação de XML da NFe";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmLerXMLNFe_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtLocalXML;
        private System.Windows.Forms.Button btnLocalizarXML;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvProdutos;
        private System.Windows.Forms.TextBox txtNumeroNota;
        private System.Windows.Forms.TextBox txtChaveDeAcesso;
        private System.Windows.Forms.TextBox txtEmitente;
        private System.Windows.Forms.DateTimePicker dateEmissao;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIE;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtLeituraArquivo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnImportarXML;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cb_LocaisDeEstoque;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label lblMensagemStatus;
        private System.Windows.Forms.MaskedTextBox txtCnpjEmitente;
        private System.Windows.Forms.Button btnCancelarXML;
        private System.Windows.Forms.TextBox txtDestinatario;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.MaskedTextBox txtDestinatarioCNPJ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridViewTextBoxColumn cProd;
        private System.Windows.Forms.DataGridViewTextBoxColumn cEAN;
        private System.Windows.Forms.DataGridViewTextBoxColumn xProd;
        private System.Windows.Forms.DataGridViewTextBoxColumn qCom;
        private System.Windows.Forms.DataGridViewTextBoxColumn vUnCom;
        private System.Windows.Forms.DataGridViewTextBoxColumn vProd;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isExiste;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isReceber;
        private System.Windows.Forms.DataGridViewComboBoxColumn acaoProdNFe;
        private System.Windows.Forms.DataGridViewImageColumn cad;
        private System.Windows.Forms.DataGridViewImageColumn vinc;
    }
}