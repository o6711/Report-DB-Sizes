using Newtonsoft.Json;
using ServiceLayer;
using ServiceLayer.GetDataSizeService;
using ServiceLayer.SendReportService;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp
{
    static class AppConfig
    {
        #region Services declaration
        public static readonly IGetData getDataService = new ByDb();
        public static readonly ISendReport sendReportService = new ToGoogleSheets();
        #endregion

        #region Other declaration
        public const int delay_ms = 60000;
        public const bool electricityIsFine = true;
        private const string _configurationFileSource = "config.json";
        #endregion

        static AppConfig()
        {
            ReadConfig(out Config config);

            if (getDataService is ByDb)
            {
                ByDb.DBConnectionsList = config.DBConnectionStrings;
            }

            if (sendReportService is ToGoogleSheets)
            {
                ToGoogleSheets.GoogleSheets_ClientId = config.GoogleSheetsClientId;
                ToGoogleSheets.GoogleSheets_ClientSecret = config.GoogleSheetsClientSecret;
                ToGoogleSheets.DisksSpacesToServers = config.DisksSpacesToServers_ParsedData;
            }
        }

        #region The class helpers
        private static void ReadConfig(out Config config)
        {
            try
            {
                using StreamReader reader = new StreamReader(_configurationFileSource);
                string json = reader.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
                config.DisksSpacesToServers_ParsedData = Parse_DisksSpacesToServers(config.DisksSpacesToServers_PureData);
            }
            catch
            {
                throw new Exception("Error of parsing config.json. Check the file filled correct way");
            }
        }

        private static Dictionary<string, double> Parse_DisksSpacesToServers(List<string> pureData)
        {
            Dictionary<string, double> parsedData = new Dictionary<string, double>();
            foreach (var item in pureData)
            {
                var subitems = item.Split('=');
                parsedData.Add(subitems[0], Convert.ToDouble(subitems[1]));
            }

            return parsedData;
        }
        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    struct Config
    {
        [JsonProperty]
        public List<string> DBConnectionStrings;
        [JsonProperty]
        public string GoogleSheetsClientId;
        [JsonProperty]
        public string GoogleSheetsClientSecret;

        /// <summary>
        /// It is supposing Disk Space must be declared in GigaBytes
        /// </summary>
        [JsonProperty]
        public List<string> DisksSpacesToServers_PureData;

        public Dictionary<string, double> DisksSpacesToServers_ParsedData { get; set; }
    }
}
