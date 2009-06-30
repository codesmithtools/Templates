using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PLINQO.Mvc.UI.Helpers;

namespace PLINQO.Mvc.UI.Models
{
    public class JsonResult : System.Web.Mvc.JsonResult
    {
        public static readonly JsonSerializerSettings DefaultSettings;
        public const string JsonDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

        static JsonResult()
        {
            DefaultSettings = new JsonSerializerSettings();

            var timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeStyles = DateTimeStyles.AssumeLocal;
            timeConverter.DateTimeFormat = JsonDateFormat;

            DefaultSettings.Converters.Add(timeConverter);
            DefaultSettings.Converters.Add(new ByteArrayConverter());

            DefaultSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public JsonResult()
        {
            SerializerSettings = DefaultSettings;
#if DEBUG
            Formatting = Formatting.Indented;
#endif
        }

        public JsonSerializerSettings SerializerSettings { get; set; }

        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                var writer = new JsonTextWriter(response.Output) { Formatting = Formatting };
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);

                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}
