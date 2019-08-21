using LightLog;
using System;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Console.WriteLine("Hello World!");
            LogHelper.GetInstance().BuildLogDirectory();

            LogHelper.Info("Program", "info");
            LogHelper.Error("Program", "Error");
            LogHelper.Debug("Program", "Debug");

            Console.ReadKey();
        }
    }
}
