public static class DataVerseHelper
{
  static string dataverseenvurl = Environment.GetEnvironmentVariable("dataverseenvurl");
  static string dataverseusername = Environment.GetEnvironmentVariable("dataverseusername");
  static string dataversepassword = Environment.GetEnvironmentVariable("dataversepassword");
  static string table_prefix = Environment.GetEnvironmentVariable("dataverse_table_prefix");
  public static string connectionString = $@"
    AuthType = OAuth;
    Url = {dataverseenvurl};
    UserName = {dataverseusername};
    Password = {dataversepassword};
    LoginPrompt=Auto;
    RequireNewInstance = True";
  static ServiceClient serviceClient = new(connectionString);
  public static void SaveData(IOrganizationService serviceClient, string content, string fileName, ILogger log)
        {
            int count = 0;
            JArray jArray = (JArray)JsonConvert.DeserializeObject(content);
            List<Entity> ls = new List<Entity>();

            foreach (var item in jArray)
            {
                Entity lateBoundAccount = new(table_prefix + "_dataversetable");
                lateBoundAccount[table_prefix + "_pid"] = item["PID"] != null ? item["PID"].ToString() : string.Empty; 
                ls.Add(lateBoundAccount);
            }
            CreateMultipleExample(service, ls);
        }
    static Guid[] CreateMultipleExample(IOrganizationService service, List<Entity> recordsToCreate)
        {
                // Create an EntityCollection populated with the list of entities.
            EntityCollection entities = new(recordsToCreate)
            {
                // All the records must be for the same table.
                EntityName = recordsToCreate[0].LogicalName
            };

            // Instantiate CreateMultipleRequest
            CreateMultipleRequest createMultipleRequest = new()
            {
                Targets = entities,
            };

            // Send the request
            CreateMultipleResponse createMultipleResponse =
                        (CreateMultipleResponse)service.Execute(createMultipleRequest);

            // Return the Ids of the records created.
            return createMultipleResponse.Ids;
        }
}
