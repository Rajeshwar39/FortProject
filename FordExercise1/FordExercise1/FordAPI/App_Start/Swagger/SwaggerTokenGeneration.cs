using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace FordAPI.App_Start.Swagger
{
    public class SwaggerTokenGeneration : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/Token", new PathItem()
            {
                post = new Operation()
                {
                    tags = new List<string>() { "OAuth" },
                    consumes = new List<string>() { "application/x-www-form-urlencoded" },
                    parameters = new List<Parameter>() {
                        new Parameter()
                        {
                            type = "string",
                            name = "grant_type",
                            required = true,
                            @in = "formData",
                            description = "Refers to the way an application gets an access token, default value is 'password'"
                        },
                        new Parameter()
                        {
                            type = "string",
                            name = "username",
                            required = true,
                            @in = "formData",
                            description = "Please input given username"
                        },
                         new Parameter()
                        {
                            type = "string",
                            format = "password",
                            name = "password",
                            required = true,
                            @in = "formData",
                            description = "Please input given password"
                        }
                    }
                    //description = @"OAuth grant types.
                    //Authorization Code,
                    //Implicit,
                    //Password,
                    //Client Credentials,
                    //Device Code,
                    //Refresh Token.
                    //Here we are using Password grant type",
                    //summary= "Available grant type are:"
                }
            });
        }
    }
}