using DinnamuS_Desktop_2._0.Data;
using DinnamuS_Desktop_2._0.Data.Persistence;
using DinnamuS_Desktop_2._0.Model;
using DinnamuS_Desktop_2._0.Model.Fiscal;
using DinnamuS_Desktop_2._0.Model.Produto;
using DinnamuS_Desktop_2._0.Model.Produto.ProdutoXMLNFe;
using DinnamuS_Desktop_2._0.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportadorXmlNFe
{
    public partial class FrmCadProdutoXmlNfe : Form
    {
        SqlConnection _connection;
        ConectaDB _conectaDB;
        CadProdutoRepository produtoRepository;
        SituacaoTributaria_Repository _SituacaoTributaria_Repository;
        public ProdutoNFe produtoNfe;
        private ProdutoNFe produtoNfe_Restore;


        #region Variaveis de formulario
        private int tolerance = 12;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;
        int lx, ly;
        int sw, sh;
        #endregion

        public FrmCadProdutoXmlNfe()
        {
            _conectaDB = new ConectaDB();
            _SituacaoTributaria_Repository = new SituacaoTributaria_Repository();
            InitializeComponent();   
        }

        #region Eventos De Layout do Formulário

        //metodo para arrastar o formulario
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();


        //WndProc permite redimensionar o Form clicando e movendo o mouse no canto inferior direito do Form
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            lx = this.Location.X;
            ly = this.Location.Y;
            sw = this.Size.Width;
            sh = this.Size.Height;


            this.WindowState = FormWindowState.Maximized;
            btnMax.Visible = false;

            btnRestore.Location = btnMax.Location;
            btnRestore.Visible = true;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnRestore.Visible = false;
            btnMax.Visible = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);
            region.Exclude(sizeGripRectangle);
            this.panelBase.Region = region;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(244, 244, 244));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);
            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
        }

        #endregion
        private void frmCadProdutoXmlNfe_Load(object sender, EventArgs e)
        {
            produtoNfe_Restore = produtoNfe;
            VinculaComponentesForm(produtoNfe);
            CarregaComboModalidadeDeterminaBCIcms();
            CarregaCombocstICMS(produtoNfe.emitenteNFe.CRT);
            CarregaCombocstIPI();
            CarregaComboPisCST();
            CarregaComboCOFINSCST();
            //CarregaPanelGrade();
            CarregaDGVTabelasDePrecos();

            DefineSelecaoComboBoxes(produtoNfe);

            AdicionaManipuladoresDeEventosAosComponentes();

            FormataDataGridView();

            isPermiteEdicao(produtoNfe.isExiste);

            if (!produtoNfe.isAlteraPreco)
            {
                chkb_AlterarPrecos.Checked = true; //força o evento
                chkb_AlterarPrecos.Checked = false;
            }

        }

        #region Manipuladores de Eventos Atribuidos aos ComboBoxes manualmente
        private void CbModalidadeDeterminaBC_SelectedValueChanged(object sender, EventArgs e)
        {
            SetModalidadeDeterminaBC();
        }

        private void Cb_cstICMS_SelectedValueChanged(object sender, EventArgs e)
        {
            SetCstICMS();
        }
        private void Cb_cstIPI_SelectedValueChanged(object sender, EventArgs e)
        {
            SetCstIPI();
        }
        private void Cb_CstPIS_SelectedValueChanged(object sender, EventArgs e)
        {
            SetCstPIS();
        }

        private void Cb_CstCofins_SelectedValueChanged(object sender, EventArgs e)
        {
            SetCstCofins();
        }
        #endregion

        private void CarregaPanelGrade()
        {
            List<Grade> grades = RetornaGrades();

            int Y = 5;
            foreach (var item in grades)
            {
                RadioButton rb = new RadioButton();

                rb.Name = "rb" + Y;
                rb.Tag = item.chaveUnica;
                rb.Text = item.descricao;
                rb.Location = new Point(10, Y);
                rb.Width = 220;
                rb.CheckedChanged += new EventHandler(CapturaRadioButton);

                panelGrade.Controls.Add(rb);

                Y += 30;
            }
        }

        private List<Grade> RetornaGrades()
        {
            try
            {
                List<Grade> grades = new List<Grade>();

                using (_connection = _conectaDB.RetornaConexao())
                {
                    _connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT chaveunica, descricao FROM grade WHERE descricao IS NOT NULL ORDER BY descricao";
                    cmd.Connection = _connection;

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        grades.Add(
                            new Grade
                            {
                                chaveUnica = dr.GetInt64(0),
                                descricao = dr.GetString(1)
                            }
                            );
                    }
                }

                return grades;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Métodos setadores de propriedades de ProdutoNFe a partir dos ComboBoxes
        private void SetModalidadeDeterminaBC()
        {
            if (cbModalidadeDeterminaBC.SelectedValue != null)
            {
                produtoNfe.modBC = (int)cbModalidadeDeterminaBC.SelectedValue;
            }
        }
        private void SetCstICMS()
        {
            if (cb_cstICMS.SelectedValue != null)
            {
                produtoNfe.cstICMS = cb_cstICMS.SelectedValue.ToString();
            }

        }
        private void SetCstIPI()
        {
            if (cb_cstIPI.SelectedValue != null)
            {
                produtoNfe.cstIPI = cb_cstIPI.SelectedValue.ToString();
            }
        }
        private void SetCstPIS()
        {
            if (cb_CstPIS.SelectedValue != null)
            {
                produtoNfe.cstPIS = cb_CstPIS.SelectedValue.ToString();
            }
        }

        private void SetCstCofins()
        {
            if (cb_CstCofins.SelectedValue != null)
            {
                produtoNfe.cstCOFINS = cb_CstCofins.SelectedValue.ToString();
            }
        }
        #endregion

        private void AdicionaManipuladoresDeEventosAosComponentes()
        {
            cbModalidadeDeterminaBC.SelectedValueChanged += CbModalidadeDeterminaBC_SelectedValueChanged;
            cb_cstICMS.SelectedValueChanged += Cb_cstICMS_SelectedValueChanged;
            cb_cstIPI.SelectedValueChanged += Cb_cstIPI_SelectedValueChanged;
            cb_CstPIS.SelectedValueChanged += Cb_CstPIS_SelectedValueChanged;
            cb_CstCofins.SelectedValueChanged += Cb_CstCofins_SelectedValueChanged;
        }


        #region carregamento de ComboBoxes e ListBoxes

        private void CarregaListBoxItensTabGrade(string chaveGrade)
        {
            lbItensTabGrade.Items.Clear();


            try
            {
                List<ItemTabGrade> itens = new List<ItemTabGrade>();

                using (_connection = _conectaDB.RetornaConexao())
                {
                    _connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT chaveunica, tamanho, codigo, sequencia FROM itenstabgrade WHERE codigo=@codigo ORDER BY sequencia";
                    cmd.Parameters.AddWithValue("@codigo", chaveGrade);
                    cmd.Connection = _connection;

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        itens.Add(
                            new ItemTabGrade
                            {
                                chaveunica = dr.GetInt32(0),
                                tamanho = dr.GetString(1),
                                codigo = dr.GetInt64(2),
                                sequencia = dr.GetInt32(3)
                            }
                            );
                    }
                }

                foreach (var item in itens)
                {
                    lbItensTabGrade.Items.Add(item.tamanho);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void CarregaComboModalidadeDeterminaBCIcms()
        {
            try
            {
                List<ModalidadeDeterminaBC> modalidades = new List<ModalidadeDeterminaBC>();

                using (_connection = _conectaDB.RetornaConexao())
                {
                    _connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT codigo, nome FROM NFE_ModalidadeDetermBCICMS ORDER BY codigo";
                    cmd.Connection = _connection;

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        modalidades.Add(
                            new ModalidadeDeterminaBC
                            {
                                Codigo = dr.GetInt32(0),
                                Nome = dr.GetString(1)
                            }
                            );
                    }
                }

                //modalidades = _SituacaoTributaria_Repository.RetornaSituacaoTributariaICMSPorRegimeTodas(2);

                cbModalidadeDeterminaBC.ValueMember = "Codigo";
                cbModalidadeDeterminaBC.DisplayMember = "DisplayMember";
                cbModalidadeDeterminaBC.DataSource = modalidades;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Não foi possível carregar as informações! \n" + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregaComboPisCST()
        {
            try
            {
                List<SituacaoTributaria> situacoesPIS = new List<SituacaoTributaria>();

                situacoesPIS = _SituacaoTributaria_Repository.RetornaSituacaoTributariaPISTodas();

                cb_CstPIS.ValueMember = "Codigo";
                cb_CstPIS.DisplayMember = "DisplayMember";

                cb_CstPIS.DataSource = situacoesPIS; //.OrderBy(pis => pis.Codigo).ToList<SituacaoTributaria>();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Não foi possível carregar as informações! \n" + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregaComboCOFINSCST()
        {
            try
            {
                List<SituacaoTributaria> situacoesCOFINS = new List<SituacaoTributaria>();

                situacoesCOFINS = _SituacaoTributaria_Repository.RetornaSituacaoTributariaCOFINSTodas();

                cb_CstCofins.ValueMember = "Codigo";
                cb_CstCofins.DisplayMember = "DisplayMember";

                cb_CstCofins.DataSource = situacoesCOFINS; //.OrderBy(cofins => cofins.Codigo).ToList<SituacaoTributaria>();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Não foi possível carregar as informações! \n" + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregaCombocstICMS(int crtEmitente)
        {
            try
            {
                List<SituacaoTributaria> situacoes = new List<SituacaoTributaria>();

                int codRegime = crtEmitente > 2 ? 1 : 2;
                situacoes = _SituacaoTributaria_Repository.RetornaSituacaoTributariaICMSPorRegimeTodas(crtEmitente);

                cb_cstICMS.ValueMember = "Codigo";
                cb_cstICMS.DisplayMember = "DisplayMember";
                //situacoes.OrderBy(f => f.Codigo).ToList<NFE_PROD_SituacaoTributaria>();
                cb_cstICMS.DataSource = situacoes;
            }

            catch (Exception ex)
            {

                MessageBox.Show("Não foi possível carregar as informações! \n " + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregaCombocstIPI()
        {
            try
            {
                List<SituacaoTributaria> situacoes = new List<SituacaoTributaria>();

                situacoes = _SituacaoTributaria_Repository.RetornaSituacaoTributariaIPITodas();
                cb_cstIPI.ValueMember = "Codigo";
                cb_cstIPI.DisplayMember = "DisplayMember";
                cb_cstIPI.DataSource = situacoes.OrderBy(f => f.Codigo).ToList<SituacaoTributaria>();

            }

            catch (Exception ex)
            {

                MessageBox.Show("Não foi possível carregar as informações! \n " + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void DefineSelecaoComboBoxes(ProdutoNFe produtoNFe)
        {
            if (produtoNFe != null)
            {
                cbModalidadeDeterminaBC.SelectedValue = produtoNFe.modBC;
                cb_cstICMS.SelectedValue = produtoNFe.cstICMS ?? string.Empty;
                cb_cstIPI.SelectedValue = produtoNfe.cstIPI ?? string.Empty;
                cb_CstPIS.SelectedValue = produtoNFe.cstPIS;
                cb_CstCofins.SelectedValue = produtoNFe.cstCOFINS;
            }
        }

        private void isPermiteEdicao(bool isExiste)
        {
            txt_Nome.ReadOnly = isExiste;
            txt_Un.ReadOnly = isExiste;
            chkb_isFracionado.Enabled = !isExiste;
            txt_Ncm.ReadOnly = isExiste;
            txt_CodigoBarras.ReadOnly = isExiste;
            txt_Referencia.ReadOnly = isExiste;
        }

        private void CarregaDGVTabelasDePrecos()
        {
            try
            {
                dgvTabelasDePrecos.DataSource = produtoNfe.ItensGradeProdutos[0].Precos;
                dgvTabelasDePrecos.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro!:\nOcorreu um erro ao tentar carregar os preços para o produto: Método: CarregaDGVTabelasDePrecos(), Classe: FrmCadProdutoXmlNfe\n" + ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }
        private void VinculaComponentesForm(ProdutoNFe dataSourceProdutoNFe)
        {
            System.Windows.Forms.Binding bndTxt_Nome;
            bndTxt_Nome = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "xProd");
            bndTxt_Nome.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Nome.DataBindings.Clear();
            txt_Nome.DataBindings.Add(bndTxt_Nome);
            
            System.Windows.Forms.Binding bndTxt_Un;
            bndTxt_Un = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "uCom");
            bndTxt_Un.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Un.DataBindings.Clear();
            txt_Un.DataBindings.Add(bndTxt_Un);

            System.Windows.Forms.Binding bndTxt_REF;
            bndTxt_REF = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "Referencia");
            bndTxt_REF.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Referencia.DataBindings.Clear();
            txt_Referencia.DataBindings.Add(bndTxt_REF);

            System.Windows.Forms.Binding bndTxt_XmlLink;
            bndTxt_XmlLink = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "XmlLink");
            bndTxt_XmlLink.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_XmlLink.DataBindings.Clear();
            txt_XmlLink.DataBindings.Add(bndTxt_XmlLink);

            //System.Windows.Forms.Binding bndTxt_CodProdutoVincular;
            //bndTxt_CodProdutoVincular = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe.ProdutoVinculado ?? new CadProduto(), "Codigo");
            //bndTxt_CodProdutoVincular.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            //Txt_CodProdutoVincular.DataBindings.Clear();
            //Txt_CodProdutoVincular.DataBindings.Add(bndTxt_CodProdutoVincular);
            Txt_CodProdutoVincular.Text = produtoNfe.ProdutoVinculado is null ? "": produtoNfe.ProdutoVinculado.Codigo.ToString();

            System.Windows.Forms.Binding bndLbl_ProdutoVinculado;
            bndLbl_ProdutoVinculado = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe.ProdutoVinculado ?? new CadProduto() {Nome = "nenhum" }, "Nome");
            bndLbl_ProdutoVinculado.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            lbl_ProdutoVinculado.DataBindings.Clear();
            lbl_ProdutoVinculado.DataBindings.Add(bndLbl_ProdutoVinculado);

            System.Windows.Forms.Binding bndTxt_CodBarras;
            bndTxt_CodBarras = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "CodBarraForn");
            bndTxt_CodBarras.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_CodigoBarras.DataBindings.Clear();
            txt_CodigoBarras.DataBindings.Add(bndTxt_CodBarras);

            System.Windows.Forms.Binding bndTxt_vUnCom;
            bndTxt_vUnCom = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vUnCom");
            bndTxt_vUnCom.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_PrecoCompra.DataBindings.Clear();
            txt_PrecoCompra.DataBindings.Add(bndTxt_vUnCom);

            System.Windows.Forms.Binding bndChk_isPesavel;
            bndChk_isPesavel = new System.Windows.Forms.Binding("Checked", dataSourceProdutoNFe, "isPesavel");
            bndChk_isPesavel.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            chkb_isFracionado.DataBindings.Clear();
            chkb_isFracionado.DataBindings.Add(bndChk_isPesavel);

            System.Windows.Forms.Binding bndChk_isAlterarPrecos;
            bndChk_isAlterarPrecos = new System.Windows.Forms.Binding("Checked", dataSourceProdutoNFe, "isAlteraPreco");
            bndChk_isAlterarPrecos.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            chkb_AlterarPrecos.DataBindings.Clear();
            chkb_AlterarPrecos.DataBindings.Add(bndChk_isAlterarPrecos);

            System.Windows.Forms.Binding bndTxt_Quantidade;
            bndTxt_Quantidade = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "qCom");
            bndTxt_Quantidade.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Quantidade.DataBindings.Clear();
            txt_Quantidade.DataBindings.Add(bndTxt_Quantidade);

            System.Windows.Forms.Binding bndTxt_Gtin;
            bndTxt_Gtin = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "cEAN");
            bndTxt_Gtin.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Gtin.DataBindings.Clear();
            txt_Gtin.DataBindings.Add(bndTxt_Gtin);

            System.Windows.Forms.Binding bndTxt_Ncm;
            bndTxt_Ncm = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "NCM");
            bndTxt_Ncm.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Ncm.DataBindings.Clear();
            txt_Ncm.DataBindings.Add(bndTxt_Ncm);

            System.Windows.Forms.Binding bndTxt_CEST;
            bndTxt_CEST = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "CEST");
            bndTxt_CEST.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_Cest.DataBindings.Clear();
            txt_Cest.DataBindings.Add(bndTxt_CEST);

            System.Windows.Forms.Binding bndTxt_pICMS;
            bndTxt_pICMS = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "pICMS");
            bndTxt_pICMS.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_pICMS.DataBindings.Clear();
            txt_pICMS.DataBindings.Add(bndTxt_pICMS);

            System.Windows.Forms.Binding bndTxt_vICMS;
            bndTxt_vICMS = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vICMS");
            bndTxt_vICMS.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vICMS.DataBindings.Clear();
            txt_vICMS.DataBindings.Add(bndTxt_vICMS);

            System.Windows.Forms.Binding bndTxt_vBC_ICMS;
            bndTxt_vBC_ICMS = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vBC_ICMS");
            bndTxt_vBC_ICMS.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vBC_ICMS.DataBindings.Clear();
            txt_vBC_ICMS.DataBindings.Add(bndTxt_vBC_ICMS);

            System.Windows.Forms.Binding bndTxt_pIpi;
            bndTxt_pIpi = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "pIPI");
            bndTxt_pIpi.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_pIPI.DataBindings.Clear();
            txt_pIPI.DataBindings.Add(bndTxt_pIpi);

            System.Windows.Forms.Binding bndTxt_vIpi;
            bndTxt_vIpi = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vIPI");
            bndTxt_vIpi.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vIPI.DataBindings.Clear();
            txt_vIPI.DataBindings.Add(bndTxt_vIpi);

            System.Windows.Forms.Binding bndTxt_vBC_Ipi;
            bndTxt_vBC_Ipi = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vBC_IPI");
            bndTxt_vBC_Ipi.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vBC_IPI.DataBindings.Clear();
            txt_vBC_IPI.DataBindings.Add(bndTxt_vBC_Ipi);

            System.Windows.Forms.Binding bndTxt_cEnq;
            bndTxt_cEnq = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "cEnq");
            bndTxt_cEnq.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_EnquadramentoIPI.DataBindings.Clear();
            txt_EnquadramentoIPI.DataBindings.Add(bndTxt_cEnq);

            System.Windows.Forms.Binding bndTxt_pPis;
            bndTxt_pPis = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "pPis");
            bndTxt_pPis.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_pPis.DataBindings.Clear();
            txt_pPis.DataBindings.Add(bndTxt_pPis);

            System.Windows.Forms.Binding bndTxt_vPis;
            bndTxt_vPis = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vPis");
            bndTxt_vPis.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vPis.DataBindings.Clear();
            txt_vPis.DataBindings.Add(bndTxt_vPis);

            System.Windows.Forms.Binding bndTxt_vBC_Pis;
            bndTxt_vBC_Pis = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vBC_PIS");
            bndTxt_vBC_Pis.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vBC_PIS.DataBindings.Clear();
            txt_vBC_PIS.DataBindings.Add(bndTxt_vBC_Pis);

            System.Windows.Forms.Binding bndTxt_pCofins;
            bndTxt_pCofins = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "pCofins");
            bndTxt_pCofins.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_pCOFINS.DataBindings.Clear();
            txt_pCOFINS.DataBindings.Add(bndTxt_pCofins);

            System.Windows.Forms.Binding bndTxt_vCofins;
            bndTxt_vCofins = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vCofins");
            bndTxt_vCofins.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vCOFINS.DataBindings.Clear();
            txt_vCOFINS.DataBindings.Add(bndTxt_vCofins);

            System.Windows.Forms.Binding bndTxt_vBC_COFINS;
            bndTxt_vBC_COFINS = new System.Windows.Forms.Binding("Text", dataSourceProdutoNFe, "vBC_COFINS");
            bndTxt_vBC_COFINS.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            txt_vBC_COFINS.DataBindings.Clear();
            txt_vBC_COFINS.DataBindings.Add(bndTxt_vBC_COFINS);
            //txt_Preco.Text = produtoNFe.vUnCom.ToString();
            //txt_Preco.Text = produtoNFe.vUnCom.ToString();
        }
        private void CapturaRadioButton(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            CarregaListBoxItensTabGrade(rb.Tag.ToString());
        } 
        private void chkb_AlterarPrecos_CheckStateChanged(object sender, EventArgs e)
        {
            if (chkb_AlterarPrecos.CheckState == CheckState.Checked)
            {
                dgvTabelasDePrecos.Enabled = true;
            }
            else
            {
                foreach (var preco in produtoNfe.ItensGradeProdutos[0].Precos)
                {
                    preco.PrecoVenda = 0.0M;
                }
                CarregaDGVTabelasDePrecos();
                dgvTabelasDePrecos.Enabled = false;
                dgvTabelasDePrecos.Refresh();
            }
        }

        private void Btn_Vincular_Click(object sender, EventArgs e)
        {
            string codigoProduto = Txt_CodProdutoVincular.Text;
            codigoProduto = codigoProduto.Trim();
            Txt_CodProdutoVincular.Text = codigoProduto;

            if (codigoProduto == string.Empty)
            {
                return;
            }

            else
            {
                for (int i = 0; i < codigoProduto.Length; i++)
                {
                    char c = codigoProduto[i];

                    if (!DinnamuS_Desktop_2._0.Utils.ValidaNumero.Validar(c))
                    {
                        MessageBox.Show("O valor digitado é inválido! Digite apenas números", "Erro do Operador", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Txt_CodProdutoVincular.Focus();
                        return;
                    }
                }
            }

            if (produtoNfe.ProdutoVinculado is null)
            {
                produtoRepository = new CadProdutoRepository();
                CadProduto produtoAVincular = produtoRepository.RetornaProduto(TiposParametrosConsultaProdutos.Parametro.Codigo, long.Parse(codigoProduto));

                if (produtoAVincular != null)
                {
                    if (!string.IsNullOrEmpty(produtoAVincular.ItensGrade[0].XmlLink))
                    {
                        if (MessageBox.Show($"O produto de código {codigoProduto} - ({produtoAVincular.Nome}) já foi vinculado a outro item via Nota Fiscal.\nDeseja atualizar este vínculo?", "Vincular item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            //Cria novo vínculo
                            VinculaItemNFe(produtoAVincular, produtoAVincular.ItensGrade[0].ChaveUnica, produtoNfe.XmlLink);
                        }
                    }
                    else
                    {
                        VinculaItemNFe(produtoAVincular, produtoAVincular.ItensGrade[0].ChaveUnica, produtoNfe.XmlLink);
                    }
                }
            }

            else
            {
                if (produtoNfe.ProdutoVinculado.Codigo.ToString() == codigoProduto)
                {
                    return;
                }

                produtoRepository = new CadProdutoRepository();
                CadProduto produtoAVincular = produtoRepository.RetornaProduto(TiposParametrosConsultaProdutos.Parametro.Codigo, long.Parse(codigoProduto));

                if (produtoAVincular is null)
                {
                    return;
                }

                if (MessageBox.Show($"O produto de código {codigoProduto} - ({produtoAVincular.Nome}) já foi vinculado a outro item via Nota Fiscal.\nDeseja atualizar este vínculo?", "Vincular item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    //Quebra o link atual
                    produtoRepository.UpdateXmlLink(produtoNfe.ProdutoVinculado.Chaveunica, produtoNfe.ItensGradeProdutos[0].ChaveUnica, string.Empty);
                    //Cria novo vínculo
                    VinculaItemNFe(produtoNfe.ProdutoVinculado, produtoNfe.ProdutoVinculado.ItensGrade[0].ChaveUnica, produtoNfe.XmlLink);
                }                
            }
        }

        private void VinculaItemNFe(CadProduto produtoAVincular, int chaveItemGrade, string xmlLink)
        {
            int retorno = produtoRepository.UpdateXmlLink(produtoAVincular.Chaveunica, chaveItemGrade, xmlLink);
            if (retorno > 0)
            {
                produtoNfe.ProdutoVinculado = produtoAVincular;
                produtoNfe.isExiste = true;
                lbl_ProdutoVinculado.Text = $"{produtoAVincular.Nome}";
                MessageBox.Show($"O item {produtoNfe.xProd} foi vinculado ao produto {produtoAVincular.Nome}", "Vinculação de Produtos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Função em Desenvolvimento. Entre em contato com o suporte para previsão", "Mensagem da DinnamuS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormataDataGridView()
        {
            dgvTabelasDePrecos.AllowUserToAddRows = false;
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

            cellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvTabelasDePrecos.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.BottomCenter };
            dgvTabelasDePrecos.DefaultCellStyle = cellStyle;

            dgvTabelasDePrecos.Columns[1].DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.BottomLeft };

            dgvTabelasDePrecos.RowTemplate.Height = 28;

            dgvTabelasDePrecos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvTabelasDePrecos.ColumnHeadersHeight = 40;
            dgvTabelasDePrecos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvTabelasDePrecos.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

    }
}
