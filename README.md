# Report-DB-Sizes
.NET Console App: Check PostgreSQL databases disk space sizes by connection strings and send report to Google Sheets

Required:
1. Configured PostgreSQL data base.
2. Provided connection string to config.json. The value can be **multiple**.

3. Configured project [Google API Console](https://console.developers.google.com/apis/). In particular: 
  * added [Sheets API](https://console.developers.google.com/apis/library/sheets.googleapis.com); 
  * created [credentials for OAuth 2.0](https://console.developers.google.com/apis/credentials/oauthclient) (need chosen *web app*);
  * provided *ClientId* and *ClientSecret* values to config.json;

4. Optional: for calculating free disk space for each server it also expected to be provided to config.json common disk space value in GigaBytes with accordings server name. The value can be **multiple**.
