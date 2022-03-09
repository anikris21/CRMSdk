using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngageRM.Plugins
{
    public class ContactUpdateOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
(ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                tracingService.Trace("updating contact");
                try
                {
                    // Plug-in business logic goes here.
                    if (entity.Attributes.Contains("emailaddress1") == true ||
                        entity.Attributes.Contains("emailaddress2") == true ||
                        entity.Attributes.Contains("emailaddress3") == true)
                    {

                        string email = "";
                        if (entity.Attributes.Contains("emailaddress1") == true)
                        {
                            email = (string)entity.Attributes["emailaddress1"];
                        }
                        else if (entity.Attributes.Contains("emailaddress2") == true)
                        {
                            email = (string)entity.Attributes["emailaddress2"];
                        }
                        else if (entity.Attributes.Contains("emailaddress3") == true)
                        {
                            email = (string)entity.Attributes["emailaddress3"];
                        }

                        string firstname = "";
                        if(entity.Attributes.Contains("firstname"))
                        {
                            firstname = (string)entity["firstname"];
                        }

                        string lastname = "";
                        if(entity.Attributes.Contains("lastname"))
                        {
                            lastname = (string)entity["lastname"];
                        }

                        //if (email != "")
                        {
                            entity["new_passwordhash"] = email + firstname + lastname;
                        }
                    }
                    tracingService.Trace("Updated contact {0}", entity["new_passwordhash"]);
                }

                //catch (OrganizationServiceFault ex)
                //{
                //    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                //}

                catch (Exception ex)
                {
                    tracingService.Trace("ContactUpdateOperationPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
