using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppResizer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Если это приложение уже запущено, не загружаем его.
            if (System.Diagnostics.Process.GetProcessesByName(
                System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)
                ).Count() > 1)
            {
                Console.Beep(600, 300);
                Console.Beep(400, 200);
                MessageBox.Show("Application is already running.", "AppResizer");
                return;
            }
            // Detect Errors
            AppDomain currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            System.Windows.Forms.Application.ThreadException += GlobalThreadExceptionHandler;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        
        // Detect Errors --------------------------------------------------------------------------
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            MessageBox.Show("Error: "+ ex.Message + "\n\n-------------\n\n" + ex.StackTrace, "Application Resizer Crashed =(");
            Application.Exit();
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = e.Exception;
            MessageBox.Show("Error: " + ex.Message + "\n\n-------------\n\n" + ex.StackTrace, "Application Resizer Crashed =(");
            Application.Exit();
        }
        // -----------------------------------------------------------------------------------------
        
    }
}
