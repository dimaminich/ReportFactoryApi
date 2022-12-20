using Microsoft.Data.SqlClient;
using ReportBuilder;
using ReportFactoryApi.Models;
using ReportFactoryApi.Utilities;
using System.Data;
using System.Text.Json;

namespace ReportFactoryApi.Data
{
    public class DatabaseInstance
    {
        private QueryItem _queryItem;
        private IConfiguration _config;

        public DatabaseInstance(QueryItem queryItem, IConfiguration config)
        {
            _queryItem = queryItem;
            _config = config;
        }

        public async Task<ReportData> AddDatasetToReportData(ReportData reportData, DataSetIdentifier dataSetIdentifier)
        {
            if (_queryItem.ResultMappings == null || _queryItem.ResultMappings.Count == 0)
                throw new NullReferenceException("No result mappings found in request!");

            DatabaseInfo db = _config
                    .GetSection("DbConnections")
                    .Get<List<DatabaseInfo>>()
                    .FirstOrDefault(db => db.DbConnection.Equals(_queryItem.DbConnection))
                    ?? throw new NullReferenceException("No database information found according given key!");

            string connString = db.DbConnectionString;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                string filename = GetFileName(_queryItem.QueryName!, _config)
                            ?? throw new NullReferenceException("No given query found in query pool");
                string commandText = GetCommandText(filename);
                SqlCommand command = new SqlCommand(commandText, connection);
                command.CommandTimeout = 10000;
                command = AddParametersToCommand(command, _queryItem.QueryParameters!);

                await command.Connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        IReportDataSet reportDataSet = ReportData.GetDataSetInstance(dataSetIdentifier)
                            ?? throw new NullReferenceException("No dataset found according given identifier!");

                        foreach (ResultMapping resultMapping in _queryItem.ResultMappings!)
                        {
                            string sourceKey = resultMapping.Source!;
                            string targetKey = resultMapping.Target!;

                            try
                            {
                                var targetValue = dataRow[sourceKey];
                                Type type = targetValue.GetType();
                                reportDataSet.SetValueByKey(targetKey, targetValue);
                            }
                            catch (Exception)
                            {
                                await connection.CloseAsync();
                                throw;
                            }
                        }

                        reportData.AddToDataSet(dataSetIdentifier, reportDataSet);
                    }
                }
                await command.Connection.CloseAsync();
            }
            return reportData;
        }


        public string? GetFileName(string queryName, IConfiguration config)
        {
            var section = config.GetSection(nameof(DocPaths));
            DocPaths docPaths = section.Get<DocPaths>();
            string queriesPath = docPaths.QueriesPath;
            return FileUtility.FindFileInDirectory(queriesPath, queryName);
        }

        private string GetCommandText(string filename)
        {
            string commandText = String.Empty;

            try
            {
                commandText = System.IO.File.ReadAllText(filename).Trim();

            }
            catch (Exception)
            {

                throw;
            }

            if (commandText == String.Empty) throw new NullReferenceException("Command content is empty!");

            return commandText;
        }

        private SqlCommand AddParametersToCommand(SqlCommand command, List<QueryParameter> parameters)
        {
            foreach (QueryParameter parameter in parameters)
            {
                command = AddParameterToCommand(command, parameter)
                    ?? throw new NullReferenceException("Can't set parameter to given query.");
            }

            return command;
        }

        private SqlCommand AddParameterToCommand(SqlCommand command, QueryParameter parameter)
        {
            string parameterName = $"@{parameter.ParameterPlaceholder}";
            command.Parameters.Add(parameterName, parameter.ParameterType);

            switch (parameter.ParameterType)
            {
                case System.Data.SqlDbType.BigInt: //parameterType: 0
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Int64>();
                    break;
                case System.Data.SqlDbType.Binary: //parameterType: 1
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Byte[]>();
                    break;
                case System.Data.SqlDbType.Bit: //parameterType: 2
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Boolean>();
                    break;
                case System.Data.SqlDbType.Char: //parameterType: 3
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<String>();
                    break;
                case System.Data.SqlDbType.DateTime: //parameterType: 4
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<DateTime>();
                    break;
                case System.Data.SqlDbType.Decimal: //parameterType: 5
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Decimal>();
                    break;
                case System.Data.SqlDbType.Float: //parameterType: 6
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Double>();
                    break;
                case System.Data.SqlDbType.Image: //parameterType: 7
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Byte[]>();
                    break;
                case System.Data.SqlDbType.Int: //parameterType: 8
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Int32>();
                    break;
                case System.Data.SqlDbType.Money: //parameterType: 9
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Decimal>();
                    break;
                case System.Data.SqlDbType.NChar: //parameterType: 10
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<String>();
                    break;
                case System.Data.SqlDbType.NText: //parameterType: 11
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<String>();
                    break;
                case System.Data.SqlDbType.NVarChar: //parameterType: 12
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<String>();
                    break;
                case System.Data.SqlDbType.Real: //parameterType: 13
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Single>();
                    break;
                case System.Data.SqlDbType.UniqueIdentifier: //parameterType: 14
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Guid>();
                    break;
                case System.Data.SqlDbType.SmallDateTime: //parameterType: 15
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<DateTime>();
                    break;
                case System.Data.SqlDbType.SmallInt: //parameterType: 16
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Int16>();
                    break;
                case System.Data.SqlDbType.SmallMoney: //parameterType: 17
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Decimal>();
                    break;
                case System.Data.SqlDbType.Text: //parameterType: 18
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<String>();
                    break;
                case System.Data.SqlDbType.Timestamp: //parameterType: 19
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Byte[]>();
                    break;
                case System.Data.SqlDbType.TinyInt: //parameterType: 20
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Byte>();
                    break;
                case System.Data.SqlDbType.VarBinary: //parameterType: 21
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<Byte[]>();
                    break;
                case System.Data.SqlDbType.VarChar: //parameterType: 22
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<String>();
                    break;
                case System.Data.SqlDbType.Variant: //parameterType: 23
                    command.Parameters[parameterName].Value = parameter.ParameterValue.Deserialize<object>();
                    break;
                case System.Data.SqlDbType.Xml: //parameterType: 25
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.Xml Is Not Implemented.");
                case System.Data.SqlDbType.Udt: //parameterType: 29
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.Udt Is Not Implemented.");
                case System.Data.SqlDbType.Structured: //parameterType: 30
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.Structured Is Not Implemented.");
                case System.Data.SqlDbType.Date: //parameterType: 31
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.Date Is Not Implemented.");
                case System.Data.SqlDbType.Time: //parameterType: 32
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.Time Is Not Implemented.");
                case System.Data.SqlDbType.DateTime2: //parameterType: 33
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.DateTime2 Is Not Implemented.");
                case System.Data.SqlDbType.DateTimeOffset: //parameterType: 34
                    throw new ArgumentException("Conversion Of Type System.Data.SqlDbType.DateTimeOffset Is Not Implemented.");
                default:
                    throw new ArgumentException("Conversion Of This Type Is Not Implemented.");
            }

            return command;
        }
    }
}
