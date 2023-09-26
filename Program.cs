namespace FormsLearning
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                LoadProgram();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void LoadProgram()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}