using DinnamuS_Desktop_2._0.Data.Persistence;
using DinnamuS_Desktop_2._0.Model.Infra;
using Serilog;
using System;
using System.Windows.Forms;

namespace ImportadorXmlNFe
{
    static class Program
    {
        public static Loja Loja;
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("ImportadorLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                DinRegistry dinRegistry = new DinRegistry();
                dinRegistry.LerRegedit();

                Loja = new Loja();
                int codLoja = int.Parse(dinRegistry.CONFIG.LojaAtiva);
                Loja.Codigo = codLoja;
                int codFilial = int.Parse(dinRegistry.CONFIG.FilialAtiva);
                Loja.FilialPadrao = codFilial;
                Loja.CNPJ = dinRegistry.CONFIG.CnpjLojaAtiva;
                Loja.CodigoRegimeTributario = Int32.Parse(dinRegistry.CONFIG.CodigoRegimeTributario);

                Loja.Filiais = new LojaPersistence().RetornaFiliais(int.Parse(dinRegistry.CONFIG.LojaAtiva));
                 
                //Loja = new LojaPersistence().RetornaLojaDeTrabalho();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Erro ocorrido: \n" + ex.Message);

            }

            finally
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmLerXMLNFe());
            }
        }
    }
}
