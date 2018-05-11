using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabricContrib
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly ServiceContext _serviceContext;

        /// <summary>
        /// OWIN server handle.
        /// </summary>
        private IDisposable _serverHandle;

        private readonly IOwinAppBuilder _startup;
        private string _publishAddress;
        private string _listeningAddress;
        private readonly string _appRoot;

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup, ServiceContext serviceContext)
        {
            _startup = startup;
            _appRoot = appRoot;
            _serviceContext = serviceContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Trace.TraceInformation("Initialize OwinCommunicationListener");

            var serviceEndpoint = this._serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var port = serviceEndpoint.Port;

            var statefulServiceContext = _serviceContext as StatefulServiceContext;
            if (statefulServiceContext != null)
            {
                var statefulInitParams = statefulServiceContext;

                _listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}/{2}/{3}/",
                    port,
                    statefulInitParams.PartitionId,
                    statefulInitParams.ReplicaId,
                    Guid.NewGuid());
            }
            else
            {
                var statelessServiceContext = _serviceContext as StatelessServiceContext;
                if (statelessServiceContext != null)
                {
                    _listeningAddress = string.Format(
                        CultureInfo.InvariantCulture,
                        "http://+:{0}/{1}",
                        port,
                        string.IsNullOrWhiteSpace(_appRoot)
                            ? ""
                            : _appRoot.TrimEnd('/') + '/');
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            _publishAddress = _listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            Trace.TraceInformation($"Opening on {_publishAddress}");

            try
            {
                Trace.TraceInformation($"Starting web server on {_listeningAddress}");

                _serverHandle = WebApp.Start(_listeningAddress, _startup.Configuration);

                return Task.FromResult(_publishAddress);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);

                StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Trace.TraceInformation("Close OwinCommunicationListener");

            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            Trace.TraceInformation("Abort OwinCommunicationListener");

            StopWebServer();
        }

        private void StopWebServer()
        {
            if (_serverHandle != null)
            {
                try
                {
                    _serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
