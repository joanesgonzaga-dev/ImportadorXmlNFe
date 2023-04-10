using DinnamuS_Desktop_2._0.Data;
using DinnamuS_Desktop_2._0.Data.Persistence;
using DinnamuS_Desktop_2._0.Model;
using DinnamuS_Desktop_2._0.Model.Estoque;
using DinnamuS_Desktop_2._0.Model.Infra;
using DinnamuS_Desktop_2._0.Model.NFe;
using DinnamuS_Desktop_2._0.Model.Produto;
using DinnamuS_Desktop_2._0.Model.Produto.ProdutoXMLNFe;
using DinnamuS_Desktop_2._0.Utils;
using DinnamuS_Desktop_2._0.Utils.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace ImportadorXmlNFe
{
    public partial class frmLerXMLNFe : Form
    {
        #region Variaveis
        SqlConnection _connection;
        TabelaDePrecoRepository tabelaDePrecoRepository;
        StatusNota statusNota;
        string caminhoArquivo;

        #region Inteiros representam os ídices das Columns do DataGridView de Produtos
        private int colunaCodigo;
        private int colunaEAN;
        #endregion
        Loja loja;
        NFe dadosNFe;

        /// <summary>
        /// Objeto representa o emissor da NFe
        /// </summary>
        EmitenteNFe emitenteNFe;
        DestinatarioNFe destinatarioNFe;

        /// <summary>
        /// Objeto representa um produto na NFe
        /// </summary>
        ProdutoNFe produtoNFe;

        DadosMoviEstoque notaConsultada;

        private List<CadProduto> cadProdutos;

        /// <summary>
        /// Objeto gerador das colunas do DataGridView
        /// </summary>
        ProdutoNFeDataGridColumns produtoDataGrid;
        BindingSource bndSourceProdutosDataGrid;
        List<ProdutoNFe> produtosNFE;
        List<TipoMovEstoque> tiposMovEstoque;
        /// <summary>
        /// List para popular o DataGridView de produtos da NFe
        /// </summary>
        ObservableCollection<ProdutoNFeDataGridColumns> produtosParaDataGridColumns;

        CadProdutoRepository produtoRepository;
        DadosMoviEstoqueRepository dadosMoviEstoqueRepository;

        #endregion

        public frmLerXMLNFe()
        {
            try
            {
                _connection = ConnectionSingleton.GetInstance();

                produtoRepository = new CadProdutoRepository();
                tabelaDePrecoRepository = new TabelaDePrecoRepository(Program.Loja);
                dadosMoviEstoqueRepository = new DadosMoviEstoqueRepository();
                InitializeComponent();
                CarregaComboLocaisDeEstoque();
                tiposMovEstoque = dadosMoviEstoqueRepository.RetornaTiposMovEstoque();
                produtosNFE = new List<ProdutoNFe>();
                BindAcoes(new ObservableCollection<ProdutoNFeDataGridColumns>());
                FormataDataGridView();

                statusNota = new StatusNota();
                statusNota.PropertyChanged += StatusNota_PropertyChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao construir o aplicativo \n" + "new frmLerXMLNFe()" + ex.Message + "\n" + ex.StackTrace);
            }
            
            

        }

        private void frmLerXMLNFe_Load(object sender, EventArgs e)
        {
            statusNota.status = StatusNota.Status.ALER;
            cadProdutos = new List<CadProduto>();
            produtoDataGrid = new ProdutoNFeDataGridColumns();
            BindAcoes(new ObservableCollection<ProdutoNFeDataGridColumns>());

        }

        private void btnLocalizarXML_Click(object sender, EventArgs e)
        {
            if (produtosNFE.Count > 0)
            {
                if (MessageBox.Show("Uma nova busca irá APAGAR todas as definições atuais para a importação" +
                        ". Confirma esta operação?", "Procurar arquivo XML", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            LimpaComponentes();
            produtosNFE = new List<ProdutoNFe>();
            produtosParaDataGridColumns = new ObservableCollection<ProdutoNFeDataGridColumns>();

            caminhoArquivo = AbrirArquivo(null);

            txtLocalXML.Text = caminhoArquivo;

            LerDadosXmlNFe();
            LerDadosEmitenteXmlNFe();
            LerDadosDestinatarioNFe();
            LerDadosProdutosXmlNFe();

            if (isNotaRecebida(dadosNFe.IdNFe))
            {
                notaConsultada  = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(dadosNFe.IdNFe);
                statusNota.status = StatusNota.Status.RECEBIDA;
            }
            else
            {
                statusNota.status = StatusNota.Status.ARECEBER;
            }

            //if (!ValidaDestinatario(destinatarioNFe, loja))
            //{
            //    statusNota.status = StatusNota.Status.NAORECEBER;
            //    btnImportarXML.Enabled = false;
            //    btnCancelarXML.Enabled = false;
            //}
            //else
            //{
            //    btnImportarXML.Enabled = true;
            //    btnCancelarXML.Enabled = true;
            //}

            

        }

        private bool ValidaDestinatario(DestinatarioNFe destinatarioNFe, Loja loja)
        {
            return (loja.CNPJ == destinatarioNFe.CNPJ) || (loja.CNPJ == destinatarioNFe.CPF) ? true : false;
           
        }

        private bool isNotaRecebida(string chaveDeAcesso)
        {
            try
            {
                if (string.IsNullOrEmpty(chaveDeAcesso))
                {
                    MessageBox.Show("Não há chave de acesso forneceida para a consulta", "PesquisaR NFe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                {
                    return dadosMoviEstoqueRepository.ConsultaLancamentoNFe(chaveDeAcesso);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LerDadosXmlNFe()
        {
            try
            {
                if (caminhoArquivo != null)
                {
                    dadosNFe = new NFe();

                    using (XmlReader reader = XmlReader.Create(caminhoArquivo))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "infNFe")
                            {
                                dadosNFe.IdNFe = reader.GetAttribute("Id");
                                txtChaveDeAcesso.Text = dadosNFe.IdNFe;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "cNF")
                            {
                                dadosNFe.cNF = reader.ReadElementContentAsString();
                                txtNumeroNota.Text = dadosNFe.cNF;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "dhEmi")
                            {
                                dadosNFe.dhEmi = reader.ReadElementContentAsDateTime();
                                dateEmissao.Value = dadosNFe.dhEmi;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "pag")
                            {
                                if (reader.ReadToDescendant("vPag"))
                                {
                                    dadosNFe.totalNFe = reader.ReadElementContentAsDecimal();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao ler o cabeçalho da NFe \n" + "frmLerXMLNFe.LerDadosXmlNFe()" + ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void LerDadosEmitenteXmlNFe()
        {
            if (caminhoArquivo != null)
            {
                emitenteNFe = new EmitenteNFe();
                bool isEmitente = false;

                using (XmlReader reader = XmlReader.Create(caminhoArquivo))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "emit")
                        {
                            isEmitente = true;
                        }

                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "dest")
                        {
                            break;
                        }

                        if (isEmitente && reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "CNPJ")
                            {
                                emitenteNFe.CNPJ = reader.ReadElementContentAsString();
                                txtCnpjEmitente.Text = emitenteNFe.CNPJ;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "xNome")
                            {
                                emitenteNFe.xNome = reader.ReadElementContentAsString();
                                txtEmitente.Text = emitenteNFe.xNome;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "xFant")
                            {
                                emitenteNFe.xFant = reader.ReadElementContentAsString();
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "IE")
                            {
                                emitenteNFe.IE = reader.ReadElementContentAsString();
                                txtIE.Text = emitenteNFe.IE;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "CRT")
                            {
                                emitenteNFe.CRT = reader.ReadElementContentAsInt();

                            }
                        }
                    }
                }
            }
        }
        private void LerDadosDestinatarioNFe()
        {
            if (caminhoArquivo != null)
            {
                destinatarioNFe = new DestinatarioNFe();
                bool isDestinatario = false;

                using (XmlReader reader = XmlReader.Create(caminhoArquivo))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "dest")
                        {
                            isDestinatario = true;
                        }

                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "det")
                        {
                            break;
                        }

                        if (isDestinatario && reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "CPF")
                            {
                                destinatarioNFe.CNPJ = reader.ReadElementContentAsString();
                                txtDestinatarioCNPJ.Text = destinatarioNFe.CNPJ;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "CNPJ")
                            {
                                destinatarioNFe.CNPJ = reader.ReadElementContentAsString();
                                txtDestinatarioCNPJ.Text = destinatarioNFe.CNPJ;
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "xNome")
                            {
                                string nome = reader.ReadElementContentAsString();
                                destinatarioNFe.xNome = string.IsNullOrEmpty(nome) ? "Nome desconhecido" : nome;
                                txtDestinatario.Text = destinatarioNFe.xNome;
                            }

                            //if (reader.NodeType == XmlNodeType.Element && reader.Name == "xFant")
                            //{
                            //    emitenteNFe.xFant = reader.ReadElementContentAsString();
                            //}

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "IE")
                            {
                                destinatarioNFe.IE = reader.ReadElementContentAsString();
                                //txtIE.Text = emitenteNFe.IE;
                            }

                            //if (reader.NodeType == XmlNodeType.Element && reader.Name == "CRT")
                            //{
                            //    emitenteNFe.CRT = reader.ReadElementContentAsInt();

                            //}
                        }
                    }
                }
            }
        }

        private void LimpaComponentes()
        {
            txtNumeroNota.Text = "";
            dateEmissao.Value = DateTime.Today;
            txtEmitente.Text = "";
            txtCnpjEmitente.Text = "";
            txtChaveDeAcesso.Text = "";
            txtIE.Text = "";

            txtDestinatario.Text = "Não informado";
            txtDestinatarioCNPJ.Text = "";
        }

        private void LerDadosProdutosXmlNFe()
        {
            if (caminhoArquivo != null)
            {
                try
                {
                    using (XmlReader reader = XmlReader.Create(caminhoArquivo))
                    {
                        var fimItens = false;
                        int intExiste = 0;

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "det")
                            {
                                produtoNFe = new ProdutoNFe();
                                produtoDataGrid = new ProdutoNFeDataGridColumns();
                            }

                            else if (reader.NodeType == XmlNodeType.Element && reader.Name == "total")
                            {
                                fimItens = true;
                            }

                            if (!fimItens && reader.NodeType == XmlNodeType.Element)
                            {
                                if (reader.Name == "cProd")
                                {
                                    produtoNFe.cProd = reader.ReadElementContentAsString();
                                    produtoDataGrid.cProd = produtoNFe.cProd;
                                }

                                if (reader.Name == "cEAN")
                                {
                                    produtoNFe.cEAN = reader.ReadElementContentAsString();

                                    if (!string.IsNullOrEmpty(produtoNFe.cEAN) || !produtoNFe.cEAN.Equals(""))
                                    {
                                        bool isEAN = true;
                                        for (int i = 0; i < produtoNFe.cEAN.Length; i++)
                                        {
                                            char c = produtoNFe.cEAN[i];

                                            if (!DinnamuS_Desktop_2._0.Utils.ValidaNumero.Validar(c))
                                            {
                                                isEAN = false;
                                                break;
                                            }
                                        }

                                        if (isEAN)
                                        {
                                            produtoNFe.CodBarraForn = produtoNFe.cEAN;
                                            produtoNFe.XmlLink = produtoNFe.cEAN;
                                        }
                                        else
                                        {
                                            produtoNFe.XmlLink = produtoNFe.cProd;
                                        }
                                    }
                                    else
                                    {
                                        //produtoNFe.CodBarraForn = produtoNFe.cEAN;
                                        produtoNFe.XmlLink = produtoNFe.cProd;
                                    }

                                    produtoDataGrid.cEAN = produtoNFe.cEAN;
                                    intExiste = ExisteProdutoNFe(produtoNFe.XmlLink, produtoNFe.CodBarraForn);
                                }

                                if (reader.Name == "xProd")
                                {
                                    produtoNFe.xProd = reader.ReadElementContentAsString();
                                    produtoDataGrid.xProd = produtoNFe.xProd;
                                }

                                if (reader.Name == "uCom")
                                {
                                    produtoNFe.uCom = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "qCom")
                                {
                                    produtoNFe.qCom = reader.ReadElementContentAsDecimal();
                                    produtoDataGrid.qCom = produtoNFe.qCom.ToString();
                                }

                                if (reader.Name == "vUnCom")
                                {
                                    produtoNFe.vUnCom = reader.ReadElementContentAsDecimal();
                                    produtoDataGrid.vUnCom = produtoNFe.vUnCom.ToString();
                                }

                                if (reader.Name == "NCM")
                                {
                                    produtoNFe.NCM = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "CEST")
                                {
                                    produtoNFe.CEST = reader.ReadElementContentAsString();
                                }

                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ICMS")
                                {
                                    if (reader.ReadToDescendant("CST"))
                                    {
                                        produtoNFe.cstICMS = reader.ReadElementContentAsString();
                                    }

                                    if (reader.Name == "modBC")
                                    {
                                        produtoNFe.modBC = reader.ReadElementContentAsInt();
                                    }

                                    if (reader.Name == "vBC")
                                    {
                                        produtoNFe.vBC_ICMS = reader.ReadElementContentAsDecimal();
                                    }

                                    if (reader.Name == "pICMS")
                                    {
                                        produtoNFe.pICMS = reader.ReadElementContentAsDecimal();
                                    }

                                    if (reader.Name == "vICMS")
                                    {
                                        produtoNFe.vICMS = reader.ReadElementContentAsDecimal();
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "IPI")
                                {
                                    if (reader.ReadToDescendant("cEnq"))
                                    {
                                        produtoNFe.cEnq = reader.ReadElementContentAsInt();
                                    }

                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "IPITrib" || reader.NodeType == XmlNodeType.Element && reader.Name == "IPINT")
                                    {
                                        if (reader.ReadToDescendant("CST"))
                                        {
                                            produtoNFe.cstIPI = reader.ReadElementContentAsString();
                                        }

                                        if (reader.Name == "vBC")
                                        {
                                            produtoNFe.vBC_IPI = reader.ReadElementContentAsDecimal();
                                        }

                                        if (reader.Name == "pIPI")
                                        {
                                            produtoNFe.pIPI = reader.ReadElementContentAsDecimal();
                                        }

                                        if (reader.Name == "vIPI")
                                        {
                                            produtoNFe.vIPI = reader.ReadElementContentAsDecimal();
                                        }
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "PIS")
                                {
                                    if (reader.ReadToDescendant("CST"))
                                    {
                                        produtoNFe.cstPIS = reader.ReadElementContentAsInt();
                                    }

                                    if (reader.Name == "vBC")
                                    {
                                        produtoNFe.vBC_PIS = reader.ReadElementContentAsDecimal();
                                    }

                                    if (reader.Name == "pPIS")
                                    {
                                        produtoNFe.pPIS = reader.ReadElementContentAsDecimal();
                                    }

                                    if (reader.Name == "vPIS")
                                    {
                                        produtoNFe.vPIS = reader.ReadElementContentAsDecimal();
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "COFINS")
                                {

                                    if (reader.ReadToDescendant("CST"))
                                    {
                                        produtoNFe.cstCOFINS = reader.ReadElementContentAsInt();
                                    }

                                    if (reader.Name == "vBC")
                                    {
                                        produtoNFe.vBC_COFINS = reader.ReadElementContentAsDecimal();
                                    }

                                    if (reader.Name == "pCOFINS")
                                    {
                                        produtoNFe.pCOFINS = reader.ReadElementContentAsDecimal();
                                    }

                                    if (reader.Name == "vCOFINS")
                                    {
                                        produtoNFe.vCOFINS = reader.ReadElementContentAsDecimal();
                                    }
                                }

                                if (reader.Name == "vProd")
                                {
                                    produtoNFe.vProd = reader.ReadElementContentAsString();
                                    produtoDataGrid.vProd = produtoNFe.vProd;
                                    #region Codigo Legado DataRow
                                    //DataRow dataRow = tableProdutos.NewRow();
                                    //dataRow.SetField("cProd", produto.cProd);
                                    //dataRow.SetField("cEAN", produto.cEAN);
                                    //dataRow.SetField("xProd", produto.xProd);
                                    //dataRow.SetField("qCom", produto.qCom);
                                    //dataRow.SetField("vUnCom", produto.vUnCom);
                                    //dataRow.SetField("vProd", produto.vProd);


                                    //Icon _ok = new Icon("ok.ico");
                                    //Icon _not = new Icon("not.ico");
                                    //.SetField("Existe?", intExiste > 0 ? _ok : _not);
                                    // ? true : false;

                                    //Icon _shared = new Icon("shared.ico");
                                    //Icon _unshared = new Icon("unshared.ico");
                                    #endregion

                                    produtoNFe.isExiste = intExiste > 0 ? true : false;
                                    produtoDataGrid.IsExiste = produtoNFe.isExiste;

                                    produtoNFe.acaoProdNFe = intExiste > 0 ? TiposAcaoProdNFe.TiposAcoesNFe.Nenhum : TiposAcaoProdNFe.TiposAcoesNFe.Cadastrar;
                                    produtoDataGrid.acaoProdNFe = produtoNFe.acaoProdNFe;

                                    produtoNFe.emitenteNFe = emitenteNFe;
                                    produtoNFe.Loja = Program.Loja.Codigo;

                                    CarregaTabelaDePrecosProdutoNFe(produtoNFe);

                                    produtosNFE.Add(produtoNFe);
                                    produtosParaDataGridColumns.Add(produtoDataGrid);
                                }
                            }
                        }
                    }
                    //statusNota.status = StatusNota.Status.ARECEBER;
                    BindAcoes(produtosParaDataGridColumns);
                    FormataDataGridView();

                    DesabilitaRowParaProdutoRepetido(dgvProdutos);
                    
                        
                        //for (int i = 0; i < dgvProdutos.Rows.Count; i++)
                    //{
                    //    MessageBox.Show(((DataGridViewRow)dgvProdutos.Rows[i]).Cells[2].Value.ToString());
                    //}


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possível ler os produtos da Nota Fiscal \n" + ex.Message, "Exceção Expansiva", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormataDataGridView()
        {
            dgvProdutos.AllowUserToAddRows = false;
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

            cellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            dgvProdutos.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.BottomCenter };
            dgvProdutos.DefaultCellStyle = cellStyle;

            //DataGridViewImageColumn imgColumn = new DataGridViewImageColumn();
            //imgColumn.HeaderText = "Existe?";
            //imgColumn.Name = "imgColumn";

            //dgvProdutos.Columns.Add(imgColumn);



            dgvProdutos.Columns[1].DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.BottomLeft };

            //dgvProdutos.Columns[8].Width = 30;
            //dgvProdutos.Columns[9].Width = 30;
            //dgvProdutos.Columns[0].Width = 30;
            //dgvProdutos.Columns[1].Width = 30;

            //dgvProdutos.Columns[2].Width = 90;
            ////dgvProdutos.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; ;
            //dgvProdutos.Columns[3].Width = 110;
            //dgvProdutos.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dgvProdutos.Columns[5].Width = 90;
            //dgvProdutos.Columns[6].Width = 220;
            //dgvProdutos.Columns[7].Width = 220;


            dgvProdutos.RowTemplate.Height = 28;

            dgvProdutos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvProdutos.ColumnHeadersHeight = 40;
            dgvProdutos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            //dgvProdutos.Columns[10].Width = 30;
            //dgvProdutos.Columns[11].Width = 30;
        }

        private string AbrirArquivo(string tipo)
        {
            if (tipo == null)
                tipo = ".xml";

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.DefaultExt = tipo;
            openFile.Filter = "Arquivo (" + tipo + ")|*" + tipo;

            string[] resultadoParcial = null;

            string resultado = "";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                resultadoParcial = openFile.FileNames;

                if (resultadoParcial.Length <= 1)
                {
                    resultado = resultadoParcial[0];
                }
                else
                {
                    foreach (string caminho in resultadoParcial)
                    {
                        resultado += caminho + ";";
                    }
                } 
            }

            return resultado;
        }

        private int ExisteProdutoNFe(string xmlLink, string codbarraforn)
        {
            try
            {
                string strEANBusca = "SELECT xmlLink,codbarraforn FROM itensgradeproduto WHERE xmlLink = '" + xmlLink + "'" + (!(string.IsNullOrEmpty(codbarraforn)) ? "OR codbarraforn='" + codbarraforn + "'" : ""); //@cEAN";

                using (SqlCommand cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = strEANBusca;
                    //cmd.Connection = _connection;
                    cmd.Transaction = ConnectionSingleton.GetTransaction();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        int retorno = 0;
                        while (dr.Read())
                        {
                            retorno++;
                        }
                        return retorno;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro na chamada do método ExisteProdutoNFe, da classe frmLerXMLNFe \n" + ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
            
        }

        private void btnLerXML_Click(object sender, EventArgs e)
        {
            MessageBox.Show(((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).acaoProdNFe.ToString());
            
        }

        private void btnImportarXML_Click(object sender, EventArgs e) 
        {
            if (statusNota.status == StatusNota.Status.RECEBIDA)
            {
                MessageBox.Show("A Nota Fiscal de Nº " + dadosNFe.cNF + " já foi recebida em " + notaConsultada.Data.ToShortDateString(), "Recebimento de NFe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                return;
            }

            importarNFE();
        }

        private void importarNFE()
        {
           ConnectionSingleton.BeginTransaction(); //pesquisar como alterar o modo de lock da transaction (serializar)
            try
            {
                
                importarNFE_Action();

                ConnectionSingleton.Commit();

                if (isNotaRecebida(dadosNFe.IdNFe))
                {
                    notaConsultada = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(dadosNFe.IdNFe);
                    statusNota.status = StatusNota.Status.RECEBIDA;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível importar a Nota Fiscal! \n" + ex.Message, "Importação de Cadastros", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    ConnectionSingleton.RollBack();
                }
                catch (Exception exRollback)
                {
                    throw exRollback;
                }
            }
        }

        private void importarNFE_Action()
        {
            EstoqueFilialRepository estoqueFilialRepository = new EstoqueFilialRepository();
            try
            {
                #region Insere a Movimentação de Estoque (DadosMoviEstoque)
                DadosMoviEstoque dadosMoviEstoque = new DadosMoviEstoque()
                {
                    CodMov = "1",
                    NomeMov = "Entrada Por Compra",
                    TipoMov = "E",
                    Data = DateTime.Today,
                    Feito = "S",
                    Loja = Program.Loja.Codigo,
                    CodigoFilial = (int)cb_LocaisDeEstoque.SelectedValue,
                    Valor = dadosNFe.totalNFe,
                    ChaveNFe = dadosNFe.IdNFe

                };

                dadosMoviEstoqueRepository.InserirDadosMoviEstoque(dadosMoviEstoque);
                dadosMoviEstoque.Codigo = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(dadosNFe.IdNFe).Codigo;
                #endregion

                foreach (var _produtoNFe in produtosNFE)
                {
                    _produtoNFe.isExiste = ExisteProdutoNFe(_produtoNFe.ItensGradeProdutos[0].XmlLink, _produtoNFe.ItensGradeProdutos[0].CodBarraForn) > 0 ? true : false; //LINHA NOVA

                    CadProduto cadProd = ProdutoNFeMappings.MapToCadProduto(_produtoNFe);
                    cadProd.DataCadastro = DateTime.Today;
                    cadProd.Ativado = true;
                    cadProd.ItensGrade = new List<ItemGradeProduto>();
                    cadProd.ItensGrade.Add(ProdutoNFeMappings.MapToItemGradeProduto(_produtoNFe));
                    cadProd.ItensGrade[0].Precos = _produtoNFe.ItensGradeProdutos[0].Precos;
                    cadProd.ItensGrade[0].EstoqueNasFiliais = new List<EstoqueFilial>();
                   
                    if ((!_produtoNFe.isExiste) & (!_produtoNFe.isDuplicado) )
                    {
                        int chaveItem = produtoRepository.CadastraProdutoRetornaChaveItemGrade(cadProd);
                        _produtoNFe.ItensGradeProdutos[0].ChaveUnica = chaveItem;

                        ItemMovEstoque item = new ItemMovEstoque()
                        {
                            Codigo = dadosMoviEstoque.Codigo,
                            CodProd = chaveItem,
                            CodMov = dadosMoviEstoque.CodMov,
                            NomeMov = dadosMoviEstoque.NomeMov,
                            CodCor = string.Empty,
                            CodTam = string.Empty,
                            TipoMov = dadosMoviEstoque.TipoMov,
                            Data = dadosMoviEstoque.Data,
                            Quantidade = cadProd.ItensGrade[0].EstoqueInicial,
                            Preco = cadProd.ItensGrade[0].PrecoCompra,
                            Total = Decimal.Parse(_produtoNFe.vProd),
                            Ref = string.IsNullOrEmpty(cadProd.ItensGrade[0].Referencia) ? string.Empty : cadProd.ItensGrade[0].Referencia,
                            Loja = Program.Loja.Codigo,
                            EstoqueAnterior = 0.0F,
                            EstoqueAtual = float.Parse(cadProd.ItensGrade[0].EstoqueInicial.ToString())
                        };

                        dadosMoviEstoqueRepository.InserirItemMovEstoque(item);

                        foreach (var filial in Program.Loja.Filiais)
                        {
                            cadProd.ItensGrade[0].EstoqueNasFiliais.Add(
                               new EstoqueFilial
                               {
                                   CodigoLoja = filial.CodigoLoja,
                                   CodigoFilial = filial.CodigoFilial,
                                   CodigoProduto = chaveItem,
                                   Estoque = filial.CodigoFilial == (int)cb_LocaisDeEstoque.SelectedValue ? cadProd.ItensGrade[0].EstoqueInicial : 0
                               }
                                );
                        }
                        foreach (EstoqueFilial estoque in cadProd.ItensGrade[0].EstoqueNasFiliais)
                        {
                            estoqueFilialRepository.CriaRegistroNaEstoqueFilial(estoque);
                        }

                    }
                    else if ((!_produtoNFe.isExiste) & (_produtoNFe.isDuplicado)) // NAO ATUALIZA CADASTRO, APENAS MOVIMENTA ESTOQUE
                    {
                        //Chama o loop para calcular estoque anterior e quantidades na lista, pois ainda não constam no banco
                        float estoqueAnterior = 0;
                        int chaveItem = 0;
                        ProdutoNFe prod = produtosNFE.Find(p => p.Equals(_produtoNFe));
                        int indiceDoElementoAtual = produtosNFE.IndexOf(prod);

                        for (int i = 0; i <  indiceDoElementoAtual ; i++)
                        {
                            
                            if (produtosNFE[i].cProd == _produtoNFe.cProd)
                            {
                                chaveItem = produtosNFE[i].ItensGradeProdutos[0].ChaveUnica;
                                estoqueAnterior += (float)produtosNFE[i].ItensGradeProdutos[0].EstoqueInicial;
                            }
                        }

                        ItemMovEstoque item = new ItemMovEstoque()
                        {
                            Codigo = dadosMoviEstoque.Codigo,
                            CodProd = chaveItem,
                            CodMov = dadosMoviEstoque.CodMov,
                            NomeMov = dadosMoviEstoque.NomeMov,
                            CodCor = string.Empty,
                            CodTam = string.Empty,
                            TipoMov = dadosMoviEstoque.TipoMov,
                            Data = dadosMoviEstoque.Data,
                            Quantidade = cadProd.ItensGrade[0].EstoqueInicial,
                            Preco = cadProd.ItensGrade[0].PrecoCompra,
                            Total = Decimal.Parse(_produtoNFe.vProd),
                            Ref = string.IsNullOrEmpty(cadProd.ItensGrade[0].Referencia) ? string.Empty : cadProd.ItensGrade[0].Referencia,
                            Loja = Program.Loja.Codigo,
                            EstoqueAnterior = estoqueAnterior,
                            EstoqueAtual = estoqueAnterior + (float)cadProd.ItensGrade[0].EstoqueInicial
                        };
                        dadosMoviEstoqueRepository.InserirItemMovEstoque(item);

                        foreach (var filial in Program.Loja.Filiais)
                        {
                            cadProd.ItensGrade[0].EstoqueNasFiliais.Add(
                               new EstoqueFilial
                               {
                                   CodigoLoja = filial.CodigoLoja,
                                   CodigoFilial = filial.CodigoFilial,
                                   CodigoProduto = chaveItem,
                                   Estoque = filial.CodigoFilial == (int)cb_LocaisDeEstoque.SelectedValue ? (decimal)item.EstoqueAtual : 0
                               }
                                );
                        }
                        foreach (EstoqueFilial estoque in cadProd.ItensGrade[0].EstoqueNasFiliais)
                        {
                            estoqueFilialRepository.AtualizaEstoqueDoItem(estoque);
                        } 
                    }
                    else if ( (_produtoNFe.isExiste) & (!_produtoNFe.isDuplicado) ) //ATUALIZA CADASTRO E MOVIMENTA ESTOQUE
                    {
                        produtoRepository.Update(cadProd);
                        //movimentar o estoque
                    }
                }
                #region Pesquisa novamente, na base de dados, se cada produto existe(foi cadastrado), compara com os itens do DataGridView e atualiza a propriedade isExiste destes
                produtosNFE.ForEach(pNFe => pNFe.isExiste = (ExisteProdutoNFe(pNFe.XmlLink, pNFe.CodBarraForn) > 0 ? true : false));
                foreach (ProdutoNFeDataGridColumns pDataGrid in produtosParaDataGridColumns)
                {
                    foreach (ProdutoNFe p in produtosNFE)
                    {
                        if (p.cProd == pDataGrid.cProd)
                        {
                            pDataGrid.IsExiste = p.isExiste;
                        }
                    }
                }
                //Atualiza DataGridView, pois a Interface INotifyPropertyChanged apresentou erro
                BindAcoes(produtosParaDataGridColumns);
                #endregion
                statusNota.status = StatusNota.Status.RECEBIDA;
                DesabilitaRowParaProdutoRepetido(dgvProdutos);

                MessageBox.Show("Nota Fiscal importada com sucesso!", "Importação de Cadastros", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível importar a Nota Fiscal! \n" + ex.Message, "Importação de Cadastros", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void BindAcoes(ObservableCollection<ProdutoNFeDataGridColumns> lista)
        {

            bndSourceProdutosDataGrid = new BindingSource();

            List<AcoesProdutoNFe> acoes = new List<AcoesProdutoNFe>()
            {
                new AcoesProdutoNFe{Id = TiposAcaoProdNFe.TiposAcoesNFe.Nenhum, Descricao = ""},
                new AcoesProdutoNFe{Id = TiposAcaoProdNFe.TiposAcoesNFe.Cadastrar, Descricao = "Cadastrar"},
                new AcoesProdutoNFe{Id = TiposAcaoProdNFe.TiposAcoesNFe.Vincular, Descricao = "Vincular"}
            };

            var comboBox = (DataGridViewComboBoxColumn)dgvProdutos.Columns["acaoProdNFe"];
            comboBox.DisplayMember = "Descricao";
            comboBox.ValueMember = "Id";
            comboBox.DataSource = acoes;

            bndSourceProdutosDataGrid.DataSource = lista;
            dgvProdutos.DataSource = bndSourceProdutosDataGrid;

            colunaCodigo = dgvProdutos.Columns["cProd"].Index;
            colunaEAN = dgvProdutos.Columns["cEAN"].Index;
        }

        private void StatusNota_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MudaCorLabelStatusNota();
        }

        private void DesabilitaRowParaProdutoRepetido(DataGridView dgv)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if (dgvProdutos.Rows[i].ReadOnly)
                {
                    continue;
                }

                int increment = 1;
                while (dgv.HasNext(i, increment))
                {
                    DataGridViewRow linhaAtual = ((DataGridViewRow)dgv.Rows[i]);
                    DataGridViewRow ProximaLinha = ((DataGridViewRow)dgv.Rows[i + increment]);

                    var valorCelulaCodigoDaLinhaAtual = linhaAtual.Cells[colunaCodigo].Value.ToString();
                    var valorCelulaCodigoDaProximaLinha = ProximaLinha.Cells[colunaCodigo].Value.ToString();

                    var valorCelulaEanDaLinhaAtual = linhaAtual.Cells[colunaEAN].Value.ToString();
                    var valorCelulaEanDaProximaLinha = ProximaLinha.Cells[colunaEAN].Value.ToString();

                    if (valorCelulaCodigoDaLinhaAtual == valorCelulaCodigoDaProximaLinha && valorCelulaEanDaLinhaAtual == valorCelulaEanDaProximaLinha)
                    {
                        ProximaLinha.ReadOnly = true; //Seta a linha repetida como ReadOnly. Este será o parâmetro avaliado para permitir ou não alterar dados do produto

                        produtosNFE[ProximaLinha.Index].isDuplicado = true; //Na List de produtos que serão cadastrados, seta o seu item correspondente no DataGridView, como duplicado

                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = Color.WhiteSmoke;
                        ProximaLinha.DefaultCellStyle = style;
                    }

                    increment++;
                }
            }
        }

        private void CarregaComboLocaisDeEstoque()
        {
            try
            {
                cb_LocaisDeEstoque.ValueMember = "CodigoFilial";
                cb_LocaisDeEstoque.DisplayMember = "NomeFilial";
                cb_LocaisDeEstoque.SelectedValue = Program.Loja.FilialPadrao;
                cb_LocaisDeEstoque.DataSource = Program.Loja.Filiais;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetIconExist(bool _isExiste)
        {
            var ic = (DataGridViewImageColumn)dgvProdutos.Columns["imgColumn"];
            if (_isExiste)
            {
                ic.Image = Image.FromFile(@"ok.png"); ;
            }
            else
            {
                ic.Image = Image.FromFile(@"not.png");
            }

        }

        private void CarregaTabelaDePrecosProdutoNFe(ProdutoNFe prod)
        {
            if (prod.ItensGradeProdutos == null)
            {
                prod.ItensGradeProdutos = new List<ItemGradeProduto>();

                prod.ItensGradeProdutos.Add
                    (new ItemGradeProduto
                    {
                        Loja = Program.Loja.Codigo,
                        EstoqueInicial = prod.qCom
                    });

            }

            foreach (var item in prod.ItensGradeProdutos)
            {


                if (item.Precos == null)
                {
                    item.Precos = new List<ItemTabelaPreco>();

                    List<TabelaDePreco> tabelas = tabelaDePrecoRepository.RetornaTabelaDePrecos();

                    if (tabelas.Count > 0)
                    {
                        foreach (var tabela in tabelas)
                        {
                            item.Precos.Add(new ItemTabelaPreco()
                            {
                                //ChaveUnicaTabela = tabela.ChaveUnica,
                                CodigoTabela = tabela.ChaveUnica,
                                NomeTabela = tabela.Descricao,
                                PrecoVenda = default(decimal)
                            });
                        }
                    }
                }
            }
        }

        private void dgvProdutos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.ColumnIndex == 0)
            {
                if (((DataGridView)sender).Rows[e.RowIndex].ReadOnly)
                {
                    MessageBox.Show("Não é possível editar");
                }
                else
                {
                    frmCadProdutoXmlNfe formProdNFe = new frmCadProdutoXmlNfe();

                    formProdNFe.produtoNfe = produtosNFE.Find(p => p.cProd == ((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).cProd);

                    formProdNFe.ShowDialog();
                }




            }

        }

        private void MudaCorLabelStatusNota()
        {
            if (statusNota.status == StatusNota.Status.ARECEBER)
            {
                panelStatus.BackColor = Color.Green;
                lblMensagemStatus.ForeColor = Color.White;
                lblMensagemStatus.Text = "AGUARDANDO RECEBIMENTO";
            }

            if(statusNota.status == StatusNota.Status.RECEBIDA)
            {
                panelStatus.BackColor = Color.Yellow;
                lblMensagemStatus.ForeColor = Color.Black;
                lblMensagemStatus.Text = "NOTA RECEBIDA";
            }
            
            if (statusNota.status == StatusNota.Status.RECEBENDO)
            {
                panelStatus.BackColor = Color.GreenYellow;
                lblMensagemStatus.ForeColor = Color.Black;
                lblMensagemStatus.Text = "NOTA EM RECEBIMENTO...";
            }
            
            if (statusNota.status == StatusNota.Status.ALER)
            {
                panelStatus.BackColor = Color.Orange;
                lblMensagemStatus.Text = "LEIA UM ARQUIVO";
            }

            if (statusNota.status == StatusNota.Status.NAORECEBER)
            {
                panelStatus.BackColor = Color.Red;
                lblMensagemStatus.Text = "DESTINATÁRIO DESCONHECIDO";
            }

        }

        private void btnCancelarXML_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Função em Desenvolvimento", "Mensagem da DinnamuS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    /// <summary>
    /// Classe criada para carregar as colunas do DataGridView
    /// </summary>
    public class ProdutoNFeDataGridColumns : INotifyPropertyChanged , IEquatable<ProdutoNFeDataGridColumns>
    {
        private bool isExiste;
        public string cProd { get; set; }
        public string cEAN { get; set; }
        public string xProd { get; set; }
        public string qCom { get; set; }
        public string vUnCom { get; set; }
        public string vProd { get; set; }
        public bool IsExiste
        {
            get { return isExiste; }

            set
            {
                if (isExiste != value)
                {
                    isExiste = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsExiste"));
                    }

                }
            }
        }

        public TiposAcaoProdNFe.TiposAcoesNFe acaoProdNFe { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals(ProdutoNFeDataGridColumns other)
        {
            return other is ProdutoNFeDataGridColumns columns &&
                   cProd == columns.cProd &&
                   cEAN == columns.cEAN;
        }

        public override int GetHashCode()
        {
            int hashCode = 147002853;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(cProd);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(cEAN);
            return hashCode;
        }
    }

    public class StatusNota : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Status _status;

        public Status status
        {
            get { return _status; }

            set
            {
                if (_status != value)
                {
                    _status = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("status"));
                    }

                }
            }
        }

        public enum Status
        {
            
            ARECEBER,
            RECEBENDO,
            RECEBIDA,
            ALER,
            NAORECEBER
        }
    }

}
