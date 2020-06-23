using ServiceLayer.DataBox;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using System.Runtime.CompilerServices;
using System.Linq;

namespace ServiceLayer.SendReportService
{
    public class ToGoogleSheets : ISendReport
    {
        // -> https://console.developers.google.com/apis/credentials - Идентификаторы клиентов OAuth 2.0
        public static string GoogleSheets_ClientId;
        public static string GoogleSheets_ClientSecret;
        public static Dictionary<string, double> DisksSpacesToServers;

        private static Spreadsheet _googleSheets_Spreadsheet;
        public void SendReport(IEnumerable<Report> reports)
        {
            SendReportsToSpreadsheet(reports);
        }

        #region The class helpers
        private void SendReportsToSpreadsheet(IEnumerable<Report> reports)
        {
            //each Allocation (Server) represents by individual sheet, so group it
            var groupedReports = reports.GroupBy(x => x.Data.Allocation);

            SheetsService service = PrepareSheetsService();

            if (_googleSheets_Spreadsheet == null)
            {
                //create new one

                #region new Sheet creation
                string sheetName = "DB size checker";
                var newSheet = new Spreadsheet();
                newSheet.Properties = new SpreadsheetProperties();
                newSheet.Properties.Title = sheetName;
                newSheet.Sheets = new List<Sheet>();
                int id = 0;
                foreach (var groupByAllocation in groupedReports)
                {
                    var sheet = new Sheet();
                    sheet.Properties = new SheetProperties();
                    sheet.Properties.Title = groupByAllocation.Key;
                    sheet.Properties.SheetId = ++id;

                    sheet.Properties.GridProperties = new GridProperties();
                    sheet.Properties.GridProperties.ColumnCount = 5;
                    sheet.Properties.GridProperties.RowCount = groupByAllocation.Count() + 3;
                    newSheet.Sheets.Add(sheet);
                }
                #endregion

                _googleSheets_Spreadsheet = service.Spreadsheets.Create(newSheet).Execute();
                Console.WriteLine($"https://docs.google.com/spreadsheets/d/{_googleSheets_Spreadsheet.SpreadsheetId}/edit");
                //now check -> https://drive.google.com/drive/u/0/my-drive
                //it should be created spreadsheet "DB size checker"

            }
            
            foreach (var groupedReport in groupedReports)
            {
                var dataForSheet = GetDataForSheet(groupedReport);
                UpdateSheet(ref service, sheetName: groupedReport.Key, ref dataForSheet);
            }
        }

        private SheetsService PrepareSheetsService()
        {
            string[] scopes = { SheetsService.Scope.Spreadsheets };

            var credential = GoogleWebAuthorizationBroker
                .AuthorizeAsync(new ClientSecrets
                {
                    ClientId = GoogleSheets_ClientId,
                    ClientSecret = GoogleSheets_ClientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,
                new FileDataStore("MyAppsToken")).Result;

            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DB size checker",

            });
        }

        private IList<IList<Object>> GetDataForSheet(IGrouping<string, Report> groupedReports)
        {
            IList<IList<Object>> resultsList = new List<IList<Object>>();
            resultsList.Add(new List<Object>()
                                    {
                                        "Сервер",
                                        "База данных",
                                        "Размер в ГБ",
                                        "Дата обновления"
                                    });
            double occupiedSize = 0;
            foreach (var report in groupedReports)
            {
                resultsList.Add( new List<Object>()
                                    {
                                        report.Data.Allocation,
                                        report.Data.SourceFileName,
                                        report.Data.Size,
                                        report.Date
                                    });

                occupiedSize += Convert.ToDouble(report.Data.Size);
            }
            resultsList.Add(new List<Object>(){"","","",""});
            resultsList.Add(new List<Object>()
                                    {
                                        $"{groupedReports.Key}",
                                        "Свободно",
                                        (DisksSpacesToServers[groupedReports.Key] - occupiedSize).ToString(),
                                        DateTime.Now.ToString()
                                    });
            return resultsList;
        }

        private static void UpdateSheet(ref SheetsService service, string sheetName, ref IList<IList<Object>> values)
        {
            SpreadsheetsResource.ValuesResource.ClearRequest clearRequest = 
                new SpreadsheetsResource.ValuesResource.ClearRequest(service, new ClearValuesRequest(), _googleSheets_Spreadsheet.SpreadsheetId, range: $"{sheetName}!A:D");
            //preliminary cleanse the sheet
            clearRequest.Execute();

            SpreadsheetsResource.ValuesResource.AppendRequest updateRequest =
               service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, _googleSheets_Spreadsheet.SpreadsheetId, range: $"{sheetName}!A:A");
            updateRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.OVERWRITE;
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            
            updateRequest.Execute();
        }

        #endregion
    }
}
