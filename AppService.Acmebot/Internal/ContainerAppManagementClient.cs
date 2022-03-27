using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.Rest;

namespace AppService.Acmebot
{
    public class ContainerAppManagementClient
    {
        private Uri _uri;
        private TokenCredentials _tokenCredentials;

        public ContainerAppManagementClient(Uri uri, TokenCredentials tokenCredentials)
        {
            _uri = uri;
            _tokenCredentials = tokenCredentials;
        }

        public string SubscriptionId { get; set; }

        internal async Task<ContainerApp> GetContainerAppsAsync(string resourceGroupName, string appName) => throw new NotImplementedException();
        internal async Task<IEnumerable<ContainerApp>> ListByResourceGroupAllContainerAppsAsync(string resourceGroupName) => throw new NotImplementedException();
        internal async Task<IReadOnlyList<Certificate>> ListAllCertificatesAsync() => throw new NotImplementedException();
        internal async Task GetContainerAppsConfigurationAsync(ContainerApp app) => throw new NotImplementedException();
        internal async Task<Certificate> CreateOrUpdateCertificateAsync(string resourceGroup, string certificateName, Certificate certificate) => throw new NotImplementedException();
        internal async Task CreateOrUpdateContainerAppAsync(ContainerApp app) => throw new NotImplementedException();
        internal async Task GetContainerAppConfigurationAsync(ContainerApp app) => throw new NotImplementedException();
        internal async Task UpdateContainerAppConfigurationAsync(ContainerApp app, object config) => throw new NotImplementedException();
        internal async Task DeleteCertificateAsync(string resourceGroup, string name) => throw new NotImplementedException();
    }

    public class ContainerApp
    {
        internal string State;
        internal string Name;
        public IEnumerable<HostNameSslState> HostNameSslStates;

        public class HostNameSslState
        {
            public HostNameSslState()
            {
            }
            public string Name { get; set; }
            public string Thumbprint { get; set; }
            public bool ToUpdate { get; set; }
        }
    }
}