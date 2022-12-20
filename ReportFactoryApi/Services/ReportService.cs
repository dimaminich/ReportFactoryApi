using ReportBuilder;
using ReportFactoryApi.Models;
using ReportFactoryApi.Provider;
using ReportFactoryApi.Utilities;

namespace ReportFactoryApi.Services
{

    public class ReportService
    {
        private readonly ILogger<ReportService> _logger;

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
        }

        public async Task<string> CreateReport(ReportData reportData, string reportTemplate, string reportFilename, IConfiguration config)
        {
            ReportDataBundler dataBundler = BundleData(reportData);
            ReportParameterBundler parameterBundler = new ReportParameterBundler();

            var section = config.GetSection(nameof(DocPaths));
            DocPaths docPaths = section.Get<DocPaths>();
            string reportTemplatePath = docPaths.ReportTemplatePath;
            string reportPath = docPaths.ReportPath;

            string reportFullFileName = String.Empty;

            await Task.Run(() =>
            {
                Builder builder = new Builder();
                reportFullFileName = builder.BuildPdfReport(
                    Path.Combine(reportTemplatePath, reportTemplate),
                    Path.Combine(reportPath, FileUtility.AddTimeStampToFilename(reportFilename)),
                    dataBundler,
                    parameterBundler
                    );
            });

            return reportFullFileName;
        }

        public async Task<byte[]> CreateReportBytes(ReportData reportData, string reportTemplate, IConfiguration config)
        {
            ReportDataBundler dataBundler = BundleData(reportData);
            ReportParameterBundler parameterBundler = new ReportParameterBundler();

            var section = config.GetSection(nameof(DocPaths));
            DocPaths docPaths = section.Get<DocPaths>();
            string reportTemplatePath = docPaths.ReportTemplatePath;

            List<byte> reportBytes = new List<byte>();

            await Task.Run(() =>
            {
                Builder builder = new Builder();
                
                byte[] bytes = builder.BuildReportBytes(
                    Path.Combine(reportTemplatePath, reportTemplate),
                    dataBundler,
                    parameterBundler
                    );

                reportBytes.AddRange(bytes.ToList());
            });

            return reportBytes.ToArray();
        }

        private ReportDataBundler BundleData(ReportData reportData)
        {
            ReportDataBundler dataBundler = new ReportDataBundler();

            reportData.DataSet01.ForEach(data => dataBundler.AddReportData(data));
            reportData.DataSet02.ForEach(data => dataBundler.AddReportData(data));
            reportData.DataSet03.ForEach(data => dataBundler.AddReportData(data));
            reportData.DataSet04.ForEach(data => dataBundler.AddReportData(data));

            return dataBundler;
        }
    }
}
