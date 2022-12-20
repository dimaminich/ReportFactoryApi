using ReportBuilder;
using ReportFactoryApi.Models;

namespace ReportFactoryApi.Provider
{
    public abstract class Provider
    {
        protected ReportRequest _request { get; set; }
        protected IConfiguration _config { get; set; }

        public Provider(ReportRequest request, IConfiguration config)
        {
            _request = request;
            _config = config;
        }

        public DataSetIdentifier GetDataSetIdentifier(string dataSetName)
        {
            DataSetIdentifier dataSetIdentifier;
            if (!DataSetIdentifier.TryParse(dataSetName, out dataSetIdentifier))
            {
                throw new NullReferenceException("Given dataset identifier not found!");
            }

            return dataSetIdentifier;
        }
    }
}
