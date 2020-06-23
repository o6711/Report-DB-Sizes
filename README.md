# Report-DB-Sizes
.NET Console App: Check PostgreSQL databases disk space sizes by connection strings and send report to Google Sheets

Required:
1. Configured PostgreSQL data base.

2. Provided connection string to config.json. The value can be **multiple**.

3. Configured project on [Google API Console](https://console.developers.google.com/apis/). In particular: 
  * added [Sheets API](https://console.developers.google.com/apis/library/sheets.googleapis.com); 
  * created [credentials for OAuth 2.0](https://console.developers.google.com/apis/credentials/oauthclient) (need chosen *web app*);
  * provided *ClientId* and *ClientSecret* values to config.json;

Optional: 
1. For calculating free disk space for each server it also expected to be provided to config.json common disk space value in GigaBytes with accordings server name. The value can be **multiple**.

2. For cyclic call configure AppConfig.cs -> delay_ms. The delay represents time after which the app re-read data and re-send report.


