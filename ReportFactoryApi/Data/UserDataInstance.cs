using ReportBuilder;
using ReportFactoryApi.Models;

namespace ReportFactoryApi.Data
{
    public class UserDataInstance
    {
        private UserValueItem _userValueItem;

        public UserDataInstance(UserValueItem userValueItem)
        {
            this._userValueItem = userValueItem;
        }

        public ReportData AddDatasetToReportData(ReportData reportData, DataSetIdentifier dataSetIdentifier)
        {
            IReportDataSet reportDataSet = ReportData.GetDataSetInstance(dataSetIdentifier)
                            ?? throw new NullReferenceException("No dataset found according given identifier!");

            foreach (ValueItem valueItem in _userValueItem.ValueItems!)
            {
                string targetKey = valueItem.Target!;
                string targetValue = valueItem.TargetValue.GetRawText();
                Type type = targetValue.GetType();
                reportDataSet.SetValueByKey(targetKey, targetValue);
            }

            reportData.AddToDataSet(dataSetIdentifier, reportDataSet);

            return reportData;
        }



    }
}
