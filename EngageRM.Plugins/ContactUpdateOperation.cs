using Microsoft.Xrm.Sdk;
using System;

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

            tracingService.Trace("updating contact...");
            // The InputParameters collection contains all the data passed in the message request.  
            if ((context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
                )
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];
                Entity preEntity = null;

                if(context.PreEntityImages.Contains("PreEntityImage") &&
                context.PreEntityImages["PreEntityImage"] is Entity)
                {
                    tracingService.Trace("updating contact with PreEntity image...");
                    preEntity = (Entity)context.PreEntityImages["PreEntityImage"];
                }

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

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
                        if(preEntity != null && preEntity.Attributes.Contains("firstname") == true)
                        {
                            firstname = (string)preEntity["firstname"];
                        }

                        string lastname = "";
                        if(preEntity?.Attributes.Contains("lastname") == true)
                        {
                            lastname = (string)preEntity["lastname"];
                        }

                        var hash = EngageRMPasswordService.GetPassword(email, firstname, lastname);
                        tracingService.Trace($"passwordhash {hash} - {email} {firstname} {lastname}");

                        //if (email != "")
                        if(!entity.Attributes.Contains("new_passwordhash")) { 
                            entity.Attributes.Add("new_passwordhash", hash);
                        } else
                        {
                            entity["new_passwordhash"] = hash;
                        }
                        tracingService.Trace("Hash Added");
                        tracingService.Trace("Updated contact {0}", entity["new_passwordhash"]);
                    }
                    tracingService.Trace("contact updated/skipped");
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
