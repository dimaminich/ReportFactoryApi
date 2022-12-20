using ReportBuilder;
using ReportFactoryApi.Data;
using ReportFactoryApi.Models;


namespace ReportFactoryApi.Provider
{
    public class UserDataProvider: Provider
    {
        public UserDataProvider(ReportRequest request, IConfiguration config):
            base(request, config)
        {}

        public ReportData GetReportData(ReportData reportData)
        {
            if (_request.UserValueItems == null || _request.UserValueItems.Count == 0) return reportData;

            foreach (UserValueItem userValueItem in _request.UserValueItems)
            {
                UserDataInstance userdataInstance = new UserDataInstance(userValueItem);
                DataSetIdentifier dataSetIdentifier = GetDataSetIdentifier(userValueItem.DataSetName!);
                reportData = userdataInstance.AddDatasetToReportData(reportData, dataSetIdentifier);
            }

            return reportData;
        }
    }
}
