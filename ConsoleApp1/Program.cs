using System;
using System.Threading.Tasks;
using ConsoleApp;
using ServiceLayer;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            StartApp();

            Console.ReadKey();
        }

        static async void StartApp()
        {
            IGetData _dataService = AppConfig.getDataService;
            ISendReport _reportService = AppConfig.sendReportService;

            await Task.Factory.StartNew(
                () =>
                {
                    while (AppConfig.electricityIsFine)
                    {
                        try
                        {
                            _reportService.SendReport(_dataService.GetData().ToReport());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Some app error happens: {ex.ToString()}");
                        }
                        

                        Task.WaitAll(Task.Delay(AppConfig.delay_ms));
                    }
                });
        }
    }
}
