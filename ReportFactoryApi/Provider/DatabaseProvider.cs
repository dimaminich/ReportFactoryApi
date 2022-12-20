using ReportBuilder;
using ReportFactoryApi.Data;
using ReportFactoryApi.Models;

namespace ReportFactoryApi.Provider
{
    public class DatabaseProvider: Provider
    {
        public DatabaseProvider(ReportRequest request, IConfiguration config) :
            base(request, config)
        { }

        public async Task<ReportData> GetReportData(ReportData reportData)
        {
            if (_request.QueryItems == null || _request.QueryItems.Count == 0) return reportData;

            foreach (QueryItem queryItem in _request.QueryItems!)
            {
                DatabaseInstance databaseInstance = new DatabaseInstance(queryItem, _config);
                DataSetIdentifier dataSetIdentifier = GetDataSetIdentifier(queryItem.DataSetName!);
                reportData = await databaseInstance.AddDatasetToReportData(reportData, dataSetIdentifier);
            }

            return reportData;
        }
    }
}
