using DinnamuS_Desktop_2._0.Data.Persistence;
using DinnamuS_Desktop_2._0.Model.Infra;
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
            try
            {
                Loja = new LojaPersistence().RetornaLojaDeTrabalho();


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
