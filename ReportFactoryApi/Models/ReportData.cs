using ReportBuilder;

namespace ReportFactoryApi.Models
{
    public class ReportData
    {
        public List<ReportDataSet01> DataSet01 { get; set; } = new List<ReportDataSet01>();
        public List<ReportDataSet02> DataSet02 { get; set; } = new List<ReportDataSet02>();
        public List<ReportDataSet03> DataSet03 { get; set; } = new List<ReportDataSet03>();
        public List<ReportDataSet04> DataSet04 { get; set; } = new List<ReportDataSet04>();

        public void AddToDataSet(DataSetIdentifier dataSetName, IReportDataSet dataSet)
        {
            switch (dataSetName)
            {
                case DataSetIdentifier.DataSet1:
                    DataSet01.Add((ReportDataSet01)dataSet);
                    break;
                case DataSetIdentifier.DataSet2:
                    DataSet02.Add((ReportDataSet02)dataSet);
                    break;
                case DataSetIdentifier.DataSet3:
                    DataSet03.Add((ReportDataSet03)dataSet);
                    break;
                case DataSetIdentifier.DataSet4:
                    DataSet04.Add((ReportDataSet04)dataSet);
                    break;
                default:
                    break;
            }
        }

        public static IReportDataSet? GetDataSetInstance(DataSetIdentifier dataSetName)
        {
            IReportDataSet? reportDataSet = null;

            switch (dataSetName)
            {
                case DataSetIdentifier.DataSet1:
                    reportDataSet = new ReportDataSet01();
                    break;
                case DataSetIdentifier.DataSet2:
                    reportDataSet = new ReportDataSet02();
                    break;
                case DataSetIdentifier.DataSet3:
                    reportDataSet = new ReportDataSet03();
                    break;
                case DataSetIdentifier.DataSet4:
                    reportDataSet = new ReportDataSet04();
                    break;
                default:
                    break;
            }

            return reportDataSet;
        }
    }
}
