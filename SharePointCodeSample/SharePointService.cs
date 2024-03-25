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
        
        public async Task<List<MyModel>> GetItemsToBeSynced(ILogger log)
        {
            var result = new List<MyModel>();
            var camlQuery = new CamlQuery();
            camlQuery.ViewXml = String.Format(@"
                    <View Scope='RecursiveAll'><Query><Where>
                        <And>
                            <Eq><FieldRef Name='attr'/><Value Type='Text'>{0}</Value></Eq>
                            <Leq><FieldRef Name='attr2'/><Value Type='DateTime' IncludeTimeValue='FALSE'>{1}</Value></Leq>
                        </And>
                    </Where></Query>
                    <ViewFields>
                        <FieldRef Name='attr3'/>
                    </ViewFields></View>
                ", statusBeforeSending, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            var listItems = await this.FetchData(ListName, camlQuery, log);
            foreach (var item in listItems)
            {
                result.Add(new PtdrData
                {
                    Id = item["Id"] != null ? item["Id"].ToString() : string.Empty,
                    Code = item["Code"] != null ? item["Code"].ToString() : string.Empty,
                    Term = item["Term"] != null ? item["Term"].ToString() : string.Empty
                });
            }

            return result;
        }
    }
}
