using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using SPS.AzureFunction.Model;
using SPS.AzureFunction.Service;
using System;
using System.IO;
using System.Threading.Tasks;
namespace AzureCodeSample.Service
{
    public class BoxService
    {
      public string clientId =Environment.GetEnvironmentVariable("BoxClient");
      public string clientSecret = Environment.GetEnvironmentVariable("BoxSecret");
      public string enterpriseId =Environment.GetEnvironmentVariable("BoxEnterpriseId");
      public string userId = Environment.GetEnvironmentVariable("BoxAdminId");
      //get with CCG way
      public async Task<IBoxClient> GetAdminClientAsync(ILogger logger)
        {
            try
            {
                var boxConfig = new BoxConfigBuilder(clientId, clientSecret)
                    .SetEnterpriseId(enterpriseId)
                    .Build();

                var boxCCG = new BoxCCGAuth(boxConfig);
                var adminToken = await boxCCG.AdminTokenAsync();

                IBoxClient adminClient = boxCCG.AdminClient(adminToken, userId, false);
                adminClient.Auth.SessionAuthenticated += (o, e) =>
                {
                    string newAccessToken = e.Session.AccessToken;
                    logger.LogInformation($"Box Access Token{newAccessToken}");
                };
                return adminClient;
            }
            catch (BoxException ex)
            {
                logger.LogInformation($"Error getting Box client: {ex.Message}");
                return null;
            }
        }
      //Get with JWT way
      public async Task<IBoxClient> GetAdminClientAsync(ILogger logger)
        {
            try
            {
                var privateKey = "privateKey";
                var privateKeypassword="";
                var publickeyID="";
                var boxConfig = new BoxConfigBuilder(clientId, clientSecret, enterpriseId, privateKey, privateKeypassword, publickeyID).Build();
                var boxJWT = new BoxJWTAuth(boxConfig);
                var adminToken = await boxJWT.AdminTokenAsync(); //valid for 60 minutes so should be cached and re-used
                BoxClient adminClient = boxJWT.AdminClient(adminToken);
                adminClient.Auth.SessionAuthenticated += delegate (object o, SessionAuthenticatedEventArgs e)
                {
                    string newAccessToken = e.Session.AccessToken;
                    // cache the new access token
                };
                return adminClient;
            }
            catch (BoxException ex)
            {
                logger.LogInformation($"Error getting Box client: {ex.Message}");
                return null;
            }
        }
      public async Task<string> GetFolderIdByName(string rootFolderId, string folderName)
        {
            try
            {
                string targetFolderId = "";

                var root = new List<string>() { rootFolderId };
              //模糊 query 
                var items = await adminClient.SearchManager.QueryAsync(folderName, type: "folder", ancestorFolderIds: root, limit: 1000);
              //精确 query
                folderName="\""+folderName+"\"";
                var items = await adminClient.SearchManager.QueryAsync(folderName, type: "folder", ancestorFolderIds: root, limit: 1000);
                var targetFolder = items.Entries
                    .OfType<BoxFolder>()
                    .FirstOrDefault(f => f.Name == folderName);

                if (targetFolder != null)
                {
                    targetFolderId = targetFolder.Id;
                    //break;
                }
                return targetFolderId;
            }
            catch(Exception ex)
            {
                logger.LogInformation($"Error occured: {ex.Message}");
                return "";
            }
        }
      public async Task<string> UploadToBoxAsync(string folderId, ILogger loggerh)
        {
            try
                {
                  string fileName="my file name";
                  string fileId="my file id";
                  string folderId="decide your file will existed in which folder";
                    //Load file content and upload to BOX
                    using (MemoryStream memoryStream = new MemoryStream(mybyte[]content))
                    {
                        memoryStream.Position = 0;
                        //upload file to BOX
                      //Method1 update a file
                        var uploadedFile = await adminClient.FilesManager.UploadNewVersionAsync(fileName, fileId, memoryStream);

                      
                      //Method2 upload a file
                        BoxFileRequest requestParams = new BoxFileRequest()
                        {
                            Name = fileName,
                            Parent = new BoxRequestEntity()
                            {
                                Id = folderId
                            }
                        };
                        var uploadedFile = await adminClient.FilesManager.UploadAsync(requestParams, memoryStream);

                      //return file's id, the sting:file/uploadedFile.Id  is the file path
                        return "file/" + uploadedFile.Id;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error occured..{ex.message}");
                    return "";
                }
        }
      
}
