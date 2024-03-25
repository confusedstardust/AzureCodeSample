using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;

namespace AzureCodeSample.Service
{
    public static class GraphService
    {
        public static readonly string clientId = Environment.GetEnvironmentVariable("ClientId");
        public static readonly string clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        public static readonly string tenantId = Environment.GetEnvironmentVariable("TenantId");



        public static GraphServiceClient GetClient(string token, ILogger logger)
        {
            try
            {
// specify scope
                var scopes = new[] { "User.Read.All" };
//get credential
                var credential = new OnBehalfOfCredential(tenantId, clientId, clientSecret, token);
//get client
                var client = new GraphServiceClient(credential, scopes);
                return client;
            }catch(Exception ex)
            {
                logger.LogCritical($"Error occured:{ex.Message}");
                throw;
            }
        }
//query user securityid and groups
        public static async GetUserInfoAsync(string token,ILogger log)
        {
            var client = GetClient(token, log);
            var usergroup = await client.Users[userEmail].MemberOf.GetAsync();
            var groups = userinfo.Groups;
            var userSecurityID = (await client.Users[userEmail].GetAsync(requestConfiguration =>
            requestConfiguration.QueryParameters.Select = new string[] {
                "MailNickName"})).MailNickname;
        }

    }
}
