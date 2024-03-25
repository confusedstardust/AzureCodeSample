using Microsoft.Extensions.Logging;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointCodeSample.Services
{
    public class SPFileHelper
    {
        private ClientContext context;
        private ILogger log;
        private string clientId;
        private string clientSecret;
        private ClientContext context=GetSpContextBasedOnClientIdAndSecret(mySPurl,clientId,clientSecret);
//get file content form SharePoint
        public async Task<ClientResult<Stream>> GetFileStream(string filePath)
        {
            context.Load(context.Web, w => w.ServerRelativeUrl);
            await context.ExecuteQueryAsync();
            var fileUrl = $"{context.Web.ServerRelativeUrl}{filePath}";
            var file = context.Web.GetFileByServerRelativeUrl(fileUrl);
            var fileStream = file.OpenBinaryStream();
            context.Load(file);
            await context.ExecuteQueryAsync();
            return fileStream;
        }

        public ClientContext GetSpContextBasedOnClientIdAndSecret(string siteUrl, string clientId, string clientSecret)
        {
            var clientContext = new AuthenticationManager().GetACSAppOnlyContext(
                siteUrl, clientId, clientSecret);
            return clientContext;
        }
    }
}
