using System.Net.Http.Headers;
using System.Web.Http;

namespace ValidationServer.Bootstrap
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "ValidationServer/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "ValidationServer/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );
        }
    }

}
