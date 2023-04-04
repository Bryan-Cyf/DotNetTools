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
        private string _pathBase = null;

        public const string SectionName = "Swagger";

        /// <summary>
        /// 是否开启
        /// </summary>
        public bool IsOpen { get; set; } = true;

        /// <summary>
        /// 文档的前缀,默认是swagger
        /// </summary>
        public string RoutePrefix { get; set; } = "swagger";

        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string PathBase { get => _pathBase?.TrimStart('/'); set => _pathBase = value; }

        public Action<Swashbuckle.AspNetCore.Swagger.SwaggerOptions> ConfigareSwaggerOptions { get; set; }

        public Action<SwaggerUIOptions> ConfigareSwaggerUIOptions { get; set; }

        /// <summary>
        /// 添加注释文档,例如xxx.xml,入口程序的文档会自动添加
        /// </summary>
        public IList<string> IncludeXmlComments { get; set; }
    }
}
