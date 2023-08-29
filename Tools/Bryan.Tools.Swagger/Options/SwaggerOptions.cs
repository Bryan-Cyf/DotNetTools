using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tools.Swagger
{
    public class SwaggerOptions
    {
        public const string SectionName = "Swagger";

        /// <summary>
        /// 文档的前缀,默认是swagger
        /// </summary>
        public string RoutePrefix { get; set; } = "swagger";

        public bool IsOpen { get; set; }

        public List<string> IncludeXmlComments { get; set; }

        public List<OpenApiInfo> Infos { get; set; }
    }
}
