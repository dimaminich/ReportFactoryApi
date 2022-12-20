using ReportFactoryApi.Models;
using ReportFactoryApi.Provider;

namespace ReportFactoryApi.Services
{
    public class DataService
    {
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger)
        {
            _logger = logger;
        }

        public async Task<ReportData> TakeData(ReportRequest request, IConfiguration config)
        {
            ReportData reportData = new ReportData();

            DatabaseProvider dbDataProvider = new DatabaseProvider(request, config);
            reportData = await dbDataProvider.GetReportData(reportData);

            UserDataProvider userDataProvider = new UserDataProvider(request, config);
            reportData = userDataProvider.GetReportData(reportData);

            return reportData;
        }
    }
}
