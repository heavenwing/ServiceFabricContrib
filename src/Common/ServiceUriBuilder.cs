// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ServiceFabricContrib
{
    using System;
    using System.Fabric;

    /// <summary>
    /// ServiceUriBuilder
    /// </summary>
    public class ServiceUriBuilder
    {
        /// <summary>
        /// actor with serviceInstance
        /// </summary>
        /// <param name="serviceInstance"></param>
        public ServiceUriBuilder(string serviceInstance)
        {
            this.ActivationContext = FabricRuntime.GetActivationContext();
            this.ServiceInstance = serviceInstance;
        }

        /// <summary>
        /// actor with applicationInstance and serviceInstance
        /// </summary>
        /// <param name="applicationInstance"></param>
        /// <param name="serviceInstance"></param>
        public ServiceUriBuilder(string applicationInstance, string serviceInstance)
        {
            this.ActivationContext = FabricRuntime.GetActivationContext();
            this.ApplicationInstance = applicationInstance;
            this.ServiceInstance = serviceInstance;
        }

        /// <summary>
        /// actor with context and serviceInstance
        /// </summary>
        /// <param name="context"></param>
        /// <param name="serviceInstance"></param>
        public ServiceUriBuilder(ICodePackageActivationContext context, string serviceInstance)
        {
            this.ActivationContext = context;
            this.ServiceInstance = serviceInstance;
        }

        /// <summary>
        /// actor with context, applicationInstance and serviceInstance
        /// </summary>
        /// <param name="context"></param>
        /// <param name="applicationInstance"></param>
        /// <param name="serviceInstance"></param>
        public ServiceUriBuilder(ICodePackageActivationContext context, string applicationInstance, string serviceInstance)
        {
            this.ActivationContext = context;
            this.ApplicationInstance = applicationInstance;
            this.ServiceInstance = serviceInstance;
        }

        /// <summary>
        /// The name of the application instance that contains he service.
        /// </summary>
        public string ApplicationInstance { get; set; }

        /// <summary>
        /// The name of the service instance.
        /// </summary>
        public string ServiceInstance { get; set; }

        /// <summary>
        /// The local activation context
        /// </summary>
        public ICodePackageActivationContext ActivationContext { get; set; }

        /// <summary>
        /// Get uri for service address
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            string applicationInstance = this.ApplicationInstance;

            if (String.IsNullOrEmpty(applicationInstance))
            {
                // the ApplicationName property here automatically prepends "fabric:/" for us
                applicationInstance = this.ActivationContext.ApplicationName.Replace("fabric:/", String.Empty);
            }

            return new Uri("fabric:/" + applicationInstance + "/" + this.ServiceInstance);
        }
    }
}