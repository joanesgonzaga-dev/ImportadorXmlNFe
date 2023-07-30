using DinnamuS_Desktop_2._0.Data;
using DinnamuS_Desktop_2._0.Data.Persistence;
using DinnamuS_Desktop_2._0.Model;
using DinnamuS_Desktop_2._0.Model.Estoque;
using DinnamuS_Desktop_2._0.Model.Fiscal;
using DinnamuS_Desktop_2._0.Model.Infra;
using DinnamuS_Desktop_2._0.Model.NFe;
using DinnamuS_Desktop_2._0.Model.Produto;
using DinnamuS_Desktop_2._0.Model.Produto.ProdutoXMLNFe;
using DinnamuS_Desktop_2._0.Utils;
using DinnamuS_Desktop_2._0.Utils.Mapping;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System.Data;
using System.Globalization;

namespace ImportadorXmlNFe
{
    public partial class FrmLerXMLNFe : Form
    {
        #region Variaveis
        readonly SqlConnection _connection;
        TabelaDePrecoRepository tabelaDePrecoRepository;
        StatusNota statusNota;
        string caminhoArquivo;

        /// <summary>
        /// ChaveUnica do ItemGradeProduto retornada quando o item da NFe pré-existe no BD
        /// </summary>
        int chaveItemPreExistente = 0;

        #region Inteiros representam os ídices das Columns do DataGridView de Produtos
        private int colunaCodigoIndex;
        private int colunaEANIndex;
        private int colunaReceberIndex;

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
        public FrmLerXMLNFe()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                loja = Program.Loja;
                _connection = ConnectionSingleton.GetInstance();

                produtoRepository = new CadProdutoRepository();
                tabelaDePrecoRepository = new TabelaDePrecoRepository(Program.Loja);
                dadosMoviEstoqueRepository = new DadosMoviEstoqueRepository();
                InitializeComponent();
                CarregaComboLocaisDeEstoque();
                tiposMovEstoque = dadosMoviEstoqueRepository.RetornaTiposMovEstoque();
                produtosNFE = new List<ProdutoNFe>();
                BindDataGridProdutos(new ObservableCollection<ProdutoNFeDataGridColumns>());
                FormataDataGridView();

                statusNota = new StatusNota();
                statusNota.PropertyChanged += StatusNota_PropertyChanged;
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ocorreu um erro ao construir o aplicativo \n" + "new frmLerXMLNFe()" + ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show("Ocorreu um erro ao construir o aplicativo \n" + "new frmLerXMLNFe()" + ex.Message + "\n" + ex.StackTrace);
                Log.CloseAndFlush();
            }
        }
        private void frmLerXMLNFe_Load(object sender, EventArgs e)
        {
            statusNota.status = StatusNota.Status.ALER;
            cadProdutos = new List<CadProduto>();
            produtoDataGrid = new ProdutoNFeDataGridColumns();
            BindDataGridProdutos(new ObservableCollection<ProdutoNFeDataGridColumns>());
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

            try
            {
                caminhoArquivo = new Uri(caminhoArquivo, UriKind.Absolute).AbsoluteUri;
                if (!Uri.IsWellFormedUriString(caminhoArquivo, UriKind.Absolute))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                LimpaComponentes();
                return;
            }

            LerDadosXmlNFe();
            LerDadosEmitenteXmlNFe();
            LerDadosDestinatarioNFe();
            LerDadosProdutosXmlNFe();

            if (isNotaRecebida(TiposMovEstoque.Tipo.EntradaPorCompra, dadosNFe.IdNFe))
            {
                notaConsultada = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(TiposMovEstoque.Tipo.EntradaPorCompra, dadosNFe.IdNFe);
                statusNota.status = StatusNota.Status.RECEBIDA;
            }
            else
            {
                statusNota.status = StatusNota.Status.ARECEBER;
            }

            //if (!ValidaDestinatario(destinatarioNFe, loja))
            //{
            //    statusNota.status = StatusNota.Status.NAO_RECEBER;
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
        private bool isNotaRecebida(TiposMovEstoque.Tipo tipo, string chaveDeAcesso)
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
                    return dadosMoviEstoqueRepository.ConsultaLancamentoNFe(tipo, chaveDeAcesso);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Não foi possível consultar se a nota já foi recebida. Método isNotaRecebida(chaveDeAcesso)\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
            }
        }
        private void LerDadosXmlNFe()
        {
            try
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
            catch (Exception ex)
            {
                Log.Error(ex, $"{ex.Message}\n {ex.StackTrace}");
                MessageBox.Show("Ocorreu um erro ao ler o cabeçalho da NFe \n" + "frmLerXMLNFe.LerDadosXmlNFe()" + ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void LerDadosEmitenteXmlNFe()
        {
            try
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
            catch (Exception ex)
            {
                Log.Error(ex, $"Não foi possível ler os dados do emitente. Método: LerDadosEmitenteXmlNFe().\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
            }
        }
        private void LerDadosDestinatarioNFe()
        {
            try
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

                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "autXML")
                        {
                            break;
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
                            //    destinatarioNFe.CRT = reader.ReadElementContentAsInt();
                            //}

                            
                            destinatarioNFe.CRT = Program.Loja.CodigoRegimeTributario;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Não foi possível ler os dados do destinatário. Método: LerDadosDestinatarioNFe().\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
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

            emitenteNFe = null;
            dadosNFe = null;
            destinatarioNFe = null;
            dgvProdutos.Rows.Clear();
            statusNota.status = StatusNota.Status.ALER;
        }

        private void LerDadosProdutosXmlNFe()
        {
            produtosParaDataGridColumns = new ObservableCollection<ProdutoNFeDataGridColumns>();
            produtosNFE = new List<ProdutoNFe>();
            dgvProdutos.Rows.Clear();
            try
            {
                using (XmlReader reader = XmlReader.Create(caminhoArquivo))
                {
                    var fimItens = false;
                    chaveItemPreExistente = 0;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "det")
                        {
                            produtoNFe = new ProdutoNFe();
                            produtoDataGrid = new ProdutoNFeDataGridColumns();
                            produtoNFe.PadroesFiscaisProdutoOrigem = new Dictionary<Impostos.TipoDeImposto, PadraoFiscalProdutoOrigem>();
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

                                        if (!ValidaNumero.Validar(c))
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
                                chaveItemPreExistente = ExisteProdutoNFe(produtoNFe.XmlLink, produtoNFe.CodBarraForn);

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
                                produtoDataGrid.qCom = (int)produtoNFe.qCom;
                            }

                            if (reader.Name == "vUnCom")
                            {
                                produtoNFe.vUnCom = reader.ReadElementContentAsDecimal();
                                produtoDataGrid.vUnCom = produtoNFe.vUnCom;
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
                                reader.Read(); //Avança para o próximo elemento nó: <ICMS????>

                                if (reader.ReadToDescendant("orig"))
                                {
                                    produtoNFe.OrigemDoProduto = reader.ReadElementContentAsInt().ToString();
                                }

                                if (reader.Name == "CSOSN")
                                {
                                    produtoNFe.cstICMS = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "CST")
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
                                
                                produtoNFe.PadroesFiscaisProdutoOrigem.Add
                                    (
                                    Impostos.TipoDeImposto.ICMS,
                                    new PadraoFiscalProdutoOrigem
                                    {
                                        Imposto = Impostos.TipoDeImposto.ICMS,
                                        CodigoRegimeTributarioOrigem = emitenteNFe.CRT,
                                        CodigoRegimeTributarioDestino = destinatarioNFe.CRT,
                                        CstImposto = produtoNFe.cstICMS
                                    });
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

                                    produtoNFe.PadroesFiscaisProdutoOrigem.Add
                                        (
                                            Impostos.TipoDeImposto.IPI,
                                            new PadraoFiscalProdutoOrigem()
                                            {
                                                CstImposto = produtoNFe.cstIPI,
                                                CodigoRegimeTributarioOrigem = emitenteNFe.CRT,
                                                CodigoRegimeTributarioDestino = destinatarioNFe.CRT,
                                                Imposto = Impostos.TipoDeImposto.IPI
                                            }
                                        );
                                }
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "PIS")
                            {
                                if (reader.ReadToDescendant("CST"))
                                {
                                    produtoNFe.cstPIS = reader.ReadElementContentAsString();
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

                                produtoNFe.PadroesFiscaisProdutoOrigem.Add
                                    (
                                        Impostos.TipoDeImposto.PIS,
                                        new PadraoFiscalProdutoOrigem
                                        {
                                            Imposto = Impostos.TipoDeImposto.PIS,
                                            CodigoRegimeTributarioOrigem = emitenteNFe.CRT,
                                            CodigoRegimeTributarioDestino = destinatarioNFe.CRT,
                                            CstImposto = produtoNFe.cstPIS
                                        }
                                    );
                            }

                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "COFINS")
                            {
                                if (reader.ReadToDescendant("CST"))
                                {
                                    produtoNFe.cstCOFINS = reader.ReadElementContentAsString();
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

                                produtoNFe.PadroesFiscaisProdutoOrigem.Add
                                    (
                                        Impostos.TipoDeImposto.COFINS,
                                        new PadraoFiscalProdutoOrigem
                                        {
                                            Imposto = Impostos.TipoDeImposto.COFINS,
                                            CodigoRegimeTributarioOrigem = emitenteNFe.CRT,
                                            CodigoRegimeTributarioDestino = destinatarioNFe.CRT,
                                            CstImposto = produtoNFe.cstCOFINS
                                        }
                                    );
                            }

                            if (reader.Name == "vProd")
                            {
                                produtoNFe.vProd = reader.ReadElementContentAsDecimal();
                                produtoDataGrid.vProd = produtoNFe.vProd;
                               
                                produtoNFe.ChaveDeCriacao = dadosNFe.IdNFe;

                                produtoNFe.isExiste = chaveItemPreExistente > 0 ? true : false;
                                produtoDataGrid.IsExiste = produtoNFe.isExiste;

                                produtoNFe.AcaoProdNFe = chaveItemPreExistente > 0 ? TiposAcaoProdNFe.TiposAcoesNFe.Nenhum : TiposAcaoProdNFe.TiposAcoesNFe.Cadastrar;
                                produtoDataGrid.acaoProdNFe = produtoNFe.AcaoProdNFe;

                                produtoNFe.isReceber = produtoNFe.isExiste ? false : true;
                                produtoDataGrid.isReceber = produtoNFe.isReceber;

                                produtoNFe.emitenteNFe = emitenteNFe;
                                produtoNFe.Loja = Program.Loja.Codigo;

                                CarregaTabelaDePrecosProdutoNFe(ref produtoNFe, chaveItemPreExistente);
                                produtoNFe.isAlteraPreco = false;

                                produtosNFE.Add(produtoNFe);
                                produtosParaDataGridColumns.Add(produtoDataGrid);
                            }
                        }
                    }
                }
                //statusNota.status = StatusNota.Status.ARECEBER;
                BindDataGridProdutos(produtosParaDataGridColumns);
                FormataDataGridView();

                DesabilitaRowParaProdutoRepetido(dgvProdutos);
                //for (int i = 0; i < dgvProdutos.Rows.Count; i++)
                //{
                //    MessageBox.Show(((DataGridViewRow)dgvProdutos.Rows[i]).Cells[2].Value.ToString());
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Não foi possível ler os produtos da Nota Fiscal.\n{ex.Message}\n{ex.StackTrace}");
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
            
            
            
            //dgvProdutos.Columns[2].DefaultCellStyle.Format = "c2";
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


            //dgvProdutos.Columns[5].DefaultCellStyle.Format = "N2";
            //dgvProdutos.Columns[6].DefaultCellStyle.Format = "c2";
            //dgvProdutos.Columns[7].DefaultCellStyle.Format = "C";

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
                int retorno = 0;
                string strEANBusca = "SELECT chaveunica, xmlLink,codbarraforn FROM itensgradeproduto WHERE xmlLink = '" + xmlLink + "'" + (!(string.IsNullOrEmpty(codbarraforn)) ? "OR codbarraforn='" + codbarraforn + "'" : ""); //@cEAN";

                using (SqlCommand cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = strEANBusca;
                    //cmd.Connection = _connection;
                    cmd.Transaction = ConnectionSingleton.GetTransaction();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dr.Read();
                            retorno = Convert.IsDBNull(dr.GetInt32(0)) ? retorno : dr.GetInt32(0);
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
            if (dadosNFe == null)
            {
                return;
            }
            ConnectionSingleton.BeginTransaction(); //pesquisar como alterar o modo de lock da transaction (serializar)
            try
            {
                importarNFE_Action();

                ConnectionSingleton.Commit();

                if (isNotaRecebida(TiposMovEstoque.Tipo.EntradaPorCompra, dadosNFe.IdNFe))
                {
                    notaConsultada = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(TiposMovEstoque.Tipo.EntradaPorCompra, dadosNFe.IdNFe);
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
                dadosMoviEstoque.Codigo = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(TiposMovEstoque.Tipo.EntradaPorCompra, dadosNFe.IdNFe).Codigo;
                #endregion

                foreach (var _produtoNFe in produtosNFE)
                {
                    if (!_produtoNFe.isReceber)
                    {
                        continue;
                    }

                    CadProduto cadProd = ProdutoNFeMappings.MapToCadProduto(_produtoNFe);
                    cadProd.ItensGrade = new List<ItemGradeProduto>();
                    cadProd.ItensGrade.Add(ProdutoNFeMappings.MapToItemGradeProduto(_produtoNFe));
                    int chaveItem = ExisteProdutoNFe(_produtoNFe.XmlLink, _produtoNFe.ItensGradeProdutos[0].CodBarraForn);
                    _produtoNFe.ItensGradeProdutos[0].ChaveUnica = chaveItem;
                    _produtoNFe.isExiste = chaveItem > 0 ? true : false; //LINHA NOVA

                    AliquotaICMSPersistence iCMSPersistence = new AliquotaICMSPersistence();
                    AliquotaICMS icms = iCMSPersistence.RetornaICMSPadrao();
                    cadProd.CodAliquota = icms.Codigo;
                    cadProd.PercentualDeICMS = (float)icms.ICMS;

                    cadProd.DataCadastro = DateTime.Today;
                    cadProd.Codigo = produtoRepository.RetornaCodigoProdutoPelaChaveUnicaDoItemGrade(chaveItem);
                    cadProd.Ativado = true;
                    cadProd.Feito = 1;
                    cadProd.ItensGrade[0].Precos = new List<ItemTabelaPreco>();

                    foreach (var itemPreco in _produtoNFe.ItensGradeProdutos[0].Precos)
                    {
                        cadProd.ItensGrade[0].Precos.Add(
                            new ItemTabelaPreco()
                            {
                                ChaveUnica = itemPreco.ChaveUnica,
                                ChaveUnicaTabela = itemPreco.ChaveUnicaTabela,
                                CodigoProduto = itemPreco.CodigoProduto,
                                CodigoTabela = itemPreco.CodigoTabela,
                                NomeTabela = itemPreco.NomeTabela,
                                PrecoVenda = itemPreco.PrecoVenda
                            }
                            );
                    }

                    cadProd.ItensGrade[0].EstoqueNasFiliais = new List<EstoqueFilial>();

                    if ((!_produtoNFe.isExiste) & (!_produtoNFe.isDuplicado))
                    {

                        chaveItem = produtoRepository.CadastraProdutoRetornaChaveItemGrade(cadProd);
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
                            Total = _produtoNFe.vProd,
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
                        float estoqueAnterior = 0;
                        ProdutoNFe prod = produtosNFE.Find(p => p.Equals(_produtoNFe));
                        int indiceDoElementoAtual = produtosNFE.IndexOf(prod);

                        for (int i = 0; i < indiceDoElementoAtual; i++)
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
                            Total = _produtoNFe.vProd,
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

                    else if ((_produtoNFe.isExiste) & (!_produtoNFe.isDuplicado))
                    {
                        #region Nota
                        /*
                         * //ATUALIZA(RIA) CADASTRO E MOVIMENTA ESTOQUE
                         * Em tese, esta condição deveria permitir atualizar os dados do produto da nota já existente no cadastro,
                         * porém, na prática, esta operação altera de forma inadvertida os dados do produto existente.
                         * O bloco condicional será mantido por precaução
                         */
                        #endregion
                        MovimentaEstoqueProdutosExistentes(TiposMovEstoque.Tipo.EntradaPorCompra, ref cadProd, chaveItem, estoqueFilialRepository, dadosMoviEstoque, _produtoNFe);
                        produtoRepository.UpdatePelaNFe(cadProd);
                    }

                    else if ((_produtoNFe.isExiste) & (_produtoNFe.isDuplicado))
                    {
                        //ATUALIZA(RIA) CADASTRO E MOVIMENTA ESTOQUE
                        MovimentaEstoqueProdutosExistentes(TiposMovEstoque.Tipo.EntradaPorCompra, ref cadProd, chaveItem, estoqueFilialRepository, dadosMoviEstoque, _produtoNFe);
                        produtoRepository.UpdatePelaNFe(cadProd);
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
                BindDataGridProdutos(produtosParaDataGridColumns);
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

        private void MovimentaEstoqueProdutosExistentes(TiposMovEstoque.Tipo tipoMov, ref CadProduto cadProd, int chaveItem, EstoqueFilialRepository estoqueFilialRepository, DadosMoviEstoque dadosMoviEstoqueAtual, ProdutoNFe _produtoNFe)
        {
            try
            {
                cadProd.Chaveunica = produtoRepository.RetornaChaveUnicaProdutoPelaChaveDoItemGrade(chaveItem);

                foreach (var item in cadProd.ItensGrade)
                {
                    item.ChaveUnica = chaveItem;
                    item.EstoqueNasFiliais = new List<EstoqueFilial>();
                    item.EstoqueNasFiliais.Add
                        (
                            new EstoqueFilial()
                            {
                                CodigoProduto = chaveItem,
                                CodigoFilial = (int)cb_LocaisDeEstoque.SelectedValue,
                                CodigoLoja = loja.Codigo
                                //Estoque     = item.EstoqueInicial
                            }
                        );
                    decimal estoqueAnterior = estoqueFilialRepository.ConsultaEstoqueDoItem(item.EstoqueNasFiliais[0]);

                    string _nomeMov = string.Empty;
                    string _tipoMov = string.Empty;
                    decimal _estoqueAtual = default(decimal);
                    int _codMov = (int)tipoMov;
                    switch (tipoMov)
                    {
                        case TiposMovEstoque.Tipo.ZeroBase:
                            break;

                        case TiposMovEstoque.Tipo.EntradaPorCompra:
                            _nomeMov = "Entrada por Compra";
                            _tipoMov = "E";
                            _estoqueAtual = estoqueAnterior + cadProd.ItensGrade[0].EstoqueInicial;
                            break;

                        case TiposMovEstoque.Tipo.SaidaPorVenda:
                            _nomeMov = "Saída por Venda";
                            _tipoMov = "S";
                            _estoqueAtual = estoqueAnterior - cadProd.ItensGrade[0].EstoqueInicial;
                            break;

                        case TiposMovEstoque.Tipo.AjusteDeEntrada:
                            _nomeMov = "Ajuste de Entrada";
                            _tipoMov = "E";
                            _estoqueAtual = estoqueAnterior + cadProd.ItensGrade[0].EstoqueInicial;
                            break;

                        case TiposMovEstoque.Tipo.AjusteDeSaida:
                            _nomeMov = "Ajuste de Saída";
                            _tipoMov = "S";
                            _estoqueAtual = estoqueAnterior - cadProd.ItensGrade[0].EstoqueInicial;
                            break;

                        default:
                            break;
                    }

                    ItemMovEstoque itemMovEstoque = new ItemMovEstoque()
                    {
                        Codigo = dadosMoviEstoqueAtual.Codigo,
                        CodProd = item.ChaveUnica,
                        CodMov = _codMov.ToString(),
                        NomeMov = _nomeMov,
                        TipoMov = _tipoMov,
                        CodCor = string.Empty,
                        CodTam = string.Empty,
                        Data = dadosMoviEstoqueAtual.Data,
                        Quantidade = cadProd.ItensGrade[0].EstoqueInicial,
                        Preco = cadProd.ItensGrade[0].PrecoCompra,
                        Total = _produtoNFe.vProd,
                        Ref = string.IsNullOrEmpty(cadProd.ItensGrade[0].Referencia) ? string.Empty : cadProd.ItensGrade[0].Referencia,
                        Loja = loja.Codigo,
                        EstoqueAnterior = (float)estoqueAnterior,
                        EstoqueAtual = (float)_estoqueAtual
                    };

                    dadosMoviEstoqueRepository.InserirItemMovEstoque(itemMovEstoque);

                    foreach (var filial in Program.Loja.Filiais)
                    {
                        item.EstoqueNasFiliais.Add(
                           new EstoqueFilial
                           {
                               CodigoLoja = filial.CodigoLoja,
                               CodigoFilial = filial.CodigoFilial,
                               CodigoProduto = item.ChaveUnica,
                               Estoque = filial.CodigoFilial == (int)cb_LocaisDeEstoque.SelectedValue ? (decimal)itemMovEstoque.EstoqueAtual : 0
                           }
                            );
                    }
                    foreach (EstoqueFilial estoque in cadProd.ItensGrade[0].EstoqueNasFiliais)
                    {
                        estoqueFilialRepository.AtualizaEstoqueDoItem(estoque);
                    }

                    if (!produtoNFe.isAlteraPreco)
                    {
                        cadProd.ItensGrade[0].Precos.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Não foi possível movimentar o estoque dos produtos existentes. Método: MovimentaEstoqueProdutosExistentes(...).\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
            }

        }

        private void BindDataGridProdutos(ObservableCollection<ProdutoNFeDataGridColumns> lista)
        {
            bndSourceProdutosDataGrid = new BindingSource();

            //List<AcoesProdutoNFe> acoes = new List<AcoesProdutoNFe>()
            //{
            //    new AcoesProdutoNFe{Id = TiposAcaoProdNFe.TiposAcoesNFe.Nenhum, Descricao = ""},
            //    new AcoesProdutoNFe{Id = TiposAcaoProdNFe.TiposAcoesNFe.Cadastrar, Descricao = "Cadastrar"},
            //    new AcoesProdutoNFe{Id = TiposAcaoProdNFe.TiposAcoesNFe.Vincular, Descricao = "Vincular"}
            //};

            //var comboBox = (DataGridViewComboBoxColumn)dgvProdutos.Columns["acaoProdNFe"];
            //comboBox.DisplayMember = "Descricao";
            //comboBox.ValueMember = "Id";
            //comboBox.DataSource = acoes;

            

            bndSourceProdutosDataGrid.DataSource = lista;
            dgvProdutos.DataSource = bndSourceProdutosDataGrid;

            colunaCodigoIndex = dgvProdutos.Columns["cProd"].Index;
            colunaEANIndex = dgvProdutos.Columns["cEAN"].Index;
            colunaReceberIndex = dgvProdutos.Columns["isReceber"].Index;

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

                    var valorCelulaCodigoDaLinhaAtual = linhaAtual.Cells[colunaCodigoIndex].Value.ToString();
                    var valorCelulaCodigoDaProximaLinha = ProximaLinha.Cells[colunaCodigoIndex].Value.ToString();

                    var valorCelulaEanDaLinhaAtual = linhaAtual.Cells[colunaEANIndex].Value.ToString();
                    var valorCelulaEanDaProximaLinha = ProximaLinha.Cells[colunaEANIndex].Value.ToString();

                    if (valorCelulaCodigoDaLinhaAtual == valorCelulaCodigoDaProximaLinha && valorCelulaEanDaLinhaAtual == valorCelulaEanDaProximaLinha)
                    {
                        ProximaLinha.ReadOnly = true; //Seta a linha repetida como ReadOnly. Este será o parâmetro avaliado para permitir ou não alterar dados do produto

                        produtosNFE[ProximaLinha.Index].isDuplicado = true; //Na List de produtos que serão cadastrados, seta o seu item correspondente no DataGridView, como duplicado

                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = Color.LightGray;
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
                Log.Error(ex, $"Erro ao popular o combo Locais de Estoque. Método: CarregaComboLocaisDeEstoque\n{ex.Message}.\n{ex.StackTrace}");
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
        private void CarregaTabelaDePrecosProdutoNFe(ref ProdutoNFe prodNFe, int chaveUnicaItem)
        {
            if (prodNFe.ItensGradeProdutos == null)
            {
                prodNFe.ItensGradeProdutos = new List<ItemGradeProduto>();
                prodNFe.ItensGradeProdutos.Add
                    (new ItemGradeProduto
                    {
                        ChaveUnica = chaveUnicaItem,
                        Loja = Program.Loja.Codigo,
                        CodBarraForn = prodNFe.CodBarraForn,
                        EstoqueInicial = prodNFe.qCom,
                        ChaveDeCriacao = prodNFe.ChaveDeCriacao
                    });

                long codProd = produtoRepository.RetornaCodigoProdutoPelaChaveUnicaDoItemGrade(chaveUnicaItem);
                CadProduto produtoVinculado = produtoRepository.RetornaProduto<long>(TiposParametrosConsultaProdutos.Parametro.Codigo, codProd);

                prodNFe.ProdutoVinculado = produtoVinculado;
            }

            foreach (var item in prodNFe.ItensGradeProdutos)
            {
                if (item.Precos == null)
                {
                    item.Precos = new List<ItemTabelaPreco>();
                    List<TabelaDePreco> tabelas = tabelaDePrecoRepository.RetornaTabelasDePrecos();

                    if (tabelas.Count > 0)
                    {
                        foreach (var tabela in tabelas)
                        {
                            
                            item.Precos.Add(new ItemTabelaPreco()
                            {
                                //ChaveUnicaTabela = tabela.ChaveUnica,
                                CodigoTabela = tabela.ChaveUnica,
                                NomeTabela = tabela.Descricao,
                                PrecoVenda = tabela.CalculaPreco(prodNFe.vUnCom)
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
                    MessageBox.Show("Não é possível editar um item duplicado", "Edição de item da Nota Fiscal", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    FrmCadProdutoXmlNfe formProdNFe = new FrmCadProdutoXmlNfe();

                    formProdNFe.produtoNfe = produtosNFE.Find(p => p.cProd == ((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).cProd);

                    formProdNFe.ShowDialog();

                    //(Desvincula) Atualiza isExiste de produtoNFe desvinculado e atualiza DataGridView
                    foreach (var _produtoNFe in produtosNFE)
                    {
                        if (!(_produtoNFe.ProdutoVinculado is null) & !(formProdNFe.produtoNfe.ProdutoVinculado is null))
                        {
                            if (formProdNFe.produtoNfe.ProdutoVinculado.Codigo == _produtoNFe.ProdutoVinculado.Codigo & (!formProdNFe.produtoNfe.Equals(_produtoNFe)))
                            {
                                _produtoNFe.isExiste = false;
                                produtosParaDataGridColumns[produtosNFE.IndexOf(_produtoNFe)].IsExiste = false;
                            }
                        }

                    }

                    //Atualiza isExiste no DataGridView para produto recém vinculado
                    foreach (var produtoDataGrid in produtosParaDataGridColumns)
                    {
                        if (produtoDataGrid.cProd == formProdNFe.produtoNfe.cProd)
                        {
                            produtoDataGrid.IsExiste = formProdNFe.produtoNfe.isExiste;
                        }
                    }

                    ((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).IsExiste = formProdNFe.produtoNfe.isExiste;

                    //(Vincula) Atualiza isExiste de ProdutoNFe recém vinculado
                    foreach (var produtoNFe in produtosNFE)
                    {
                        if (produtoNFe.cProd == ((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).cProd)
                        {
                            produtoNFe.isExiste = formProdNFe.produtoNfe.isExiste;
                        }
                    }

                    dgvProdutos.Refresh();
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

            if (statusNota.status == StatusNota.Status.RECEBIDA)
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

            if (statusNota.status == StatusNota.Status.NAO_RECEBER)
            {
                panelStatus.BackColor = Color.Red;
                lblMensagemStatus.Text = "DESTINATÁRIO NÃO PERMITIDO";
            }

        }

        private void btnCancelarXML_Click(object sender, EventArgs e)
        {
            if (dadosNFe != null)
            {
                if (statusNota.status != StatusNota.Status.RECEBIDA)
                {
                    MessageBox.Show("Não é possível excluir a Nota Fiscal nº " + dadosNFe.cNF + ", pois ela não foi recebida", "Estorno de NotaFiscal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Confirma o cancelamento da importação da NFe de nº?: " + dadosNFe.cNF, "Estorno de Recebimento de NFe", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        ConnectionSingleton.BeginTransaction();
                        EstoqueFilialRepository estoqueFilialRepository = new EstoqueFilialRepository();
                        #region Insere a Movimentação de Estoque (DadosMoviEstoque)
                        int _codMov = (int)TiposMovEstoque.Tipo.AjusteDeSaida;

                        DadosMoviEstoque movimentacaoAtual = new DadosMoviEstoque()
                        {
                            CodMov = _codMov.ToString(),
                            NomeMov = "Ajuste de Saida",
                            TipoMov = "S",
                            Data = DateTime.Today,
                            Feito = "S",
                            Loja = Program.Loja.Codigo,
                            CodigoFilial = (int)cb_LocaisDeEstoque.SelectedValue,
                            Valor = dadosNFe.totalNFe,
                            ChaveNFe = dadosNFe.IdNFe

                        };

                        movimentacaoAtual.Codigo = dadosMoviEstoqueRepository.InserirDadosMoviEstoque(movimentacaoAtual);
                        DadosMoviEstoque movimentacaoAnterior = dadosMoviEstoqueRepository.RetornaDadosMoviEstoque(TiposMovEstoque.Tipo.EntradaPorCompra, dadosNFe.IdNFe);
                        dadosMoviEstoqueRepository.DesvinculaMoviEstoqueDaNFe(movimentacaoAnterior.Codigo, dadosNFe.IdNFe);
                        #endregion

                        #region Registra movimento de saída na ItensMovEstoque
                        foreach (var _produtoNFe in produtosNFE)
                        {
                            if (_produtoNFe.isExiste)
                            {
                                CadProduto cadProd = new CadProduto();
                                cadProd.Codigo = produtoRepository.RetornaCodigoProdutoPelaChaveUnicaDoItemGrade(_produtoNFe.ItensGradeProdutos[0].ChaveUnica);
                                bool excluirDaCadProduto = true;
                                if (produtoRepository.RetornaProduto<long>(TiposParametrosConsultaProdutos.Parametro.Codigo, cadProd.Codigo).ChaveDeCriacao == dadosNFe.IdNFe)
                                {
                                    foreach (var item in _produtoNFe.ItensGradeProdutos)
                                    {
                                        ItemMovEstoque itemMov = new ItemMovEstoque();
                                        itemMov.CodProd = item.ChaveUnica;
                                        itemMov.Codigo = movimentacaoAnterior.Codigo;
                                        itemMov.Data = movimentacaoAnterior.Data;

                                        if (dadosMoviEstoqueRepository.ExisteMovimentacaoPosterior(itemMov))
                                        {
                                            excluirDaCadProduto = false;
                                            MovimentaEstoqueProdutosExistentes(TiposMovEstoque.Tipo.AjusteDeSaida, ref cadProd, item.ChaveUnica, estoqueFilialRepository, movimentacaoAtual, _produtoNFe);
                                        }

                                        else
                                        {
                                            produtoRepository.DeleteItemGradeProduto(item.ChaveUnica);
                                        }
                                    }

                                    if (excluirDaCadProduto)
                                    {
                                        produtoRepository.Delete(cadProd.Codigo);
                                        _produtoNFe.isExiste = false;

                                        foreach (ProdutoNFe _prod in produtosNFE)
                                        {
                                            if (_produtoNFe.XmlLink == _prod.XmlLink)
                                            {
                                                _prod.isExiste = false;
                                            }
                                        }

                                    }
                                }

                                else
                                {
                                    int chaveUnicaItem = _produtoNFe.ItensGradeProdutos[0].ChaveUnica;
                                    cadProd.ItensGrade = _produtoNFe.ItensGradeProdutos;
                                    MovimentaEstoqueProdutosExistentes(TiposMovEstoque.Tipo.AjusteDeSaida, ref cadProd, chaveUnicaItem, estoqueFilialRepository, movimentacaoAtual, _produtoNFe);
                                }
                            }
                        }
                        #endregion

                        ConnectionSingleton.Commit();

                        LerDadosXmlNFe();
                        LerDadosEmitenteXmlNFe();
                        LerDadosDestinatarioNFe();
                        LerDadosProdutosXmlNFe();
                        statusNota.status = StatusNota.Status.ARECEBER;

                        MessageBox.Show($"A Nota Fiscal nº {dadosNFe.cNF} foi estornada com sucesso!", "Estorno de NotaFiscal", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Não foi possível cancelar a NFe.\n{ex.Message}\n{ex.StackTrace}");

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
            }
        }

        private void dgvProdutos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var checkBox = (bool)((DataGridViewRow)this.dgvProdutos.Rows[e.RowIndex]).Cells[colunaReceberIndex].Value;
                ((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).isReceber = checkBox;

                produtosNFE.Find(p => p.cProd == ((ProdutoNFeDataGridColumns)bndSourceProdutosDataGrid.Current).cProd).isReceber = checkBox;
            }
            catch (Exception)
            {
                //throw;
            }       
        }
    }
    /// <summary>
    /// Classe criada para carregar as colunas do DataGridView
    /// </summary>
    public class ProdutoNFeDataGridColumns : INotifyPropertyChanged, IEquatable<ProdutoNFeDataGridColumns>
    {
        private bool isExiste;
        public string cProd { get; set; }
        public string cEAN { get; set; }
        public string xProd { get; set; }
        public int qCom { get; set; }
        public decimal vUnCom { get; set; }
        public decimal vProd { get; set; }
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

        public bool isReceber { get; set; } = true;

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
            NAO_RECEBER,
            NOTA_ESTORNADA
        }

    }

}
