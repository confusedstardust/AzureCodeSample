
//using ExcelDataReader to solve Excel file
public static async Task<List<MyModel>> SolveExcel( Stream excelStream, string worksheetName, ILogger log)
        {
            List<MyModel> result = new List<MyModel>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Stream stream=excelStream;
            using (var excelReader = ExcelReaderFactory.CreateReader(stream))
            {
                var columnsIndexes = new Dictionary<string, int>();
                var dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true,
                        ReadHeaderRow = rowReader =>
                        {
                            for (int i = 0; i < rowReader.FieldCount; i++)
                            {
                                string columnName = rowReader.GetString(i);
                                if (Constants.excelColumns.Contains(columnName))
                                    columnsIndexes.Add(columnName, i);
                            }
                        }
                    },
                    FilterSheet = (tableReader, sheetIndex) =>tableReader.Name.Contains(worksheetName),

                });
                
                try
                {
                    var table = dataSet.Tables[0];
                    var reader = table.CreateDataReader();
                    while (reader.Read())
                    {
                        MyModel model = new MyModel
                        {
                            attr1 = Convert.ToString(reader.GetValue(columnsIndexes["RU ID"])),
                            attr2 = columnsIndexes.TryGetValue("Server Type", out int serverTypeIndex) ? Convert.ToString(reader.GetValue(columnsIndexes["Server Type"])) : ""
                        };
                        result.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation("no such worksheet"+ex.StackTrace+ex.Message);
                   return null ;
                }
            }
            return result;
        }
