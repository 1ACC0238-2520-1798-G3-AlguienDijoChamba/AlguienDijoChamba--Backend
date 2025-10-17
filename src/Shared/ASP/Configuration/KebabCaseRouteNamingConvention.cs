using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AlguienDijoChamba.Api.Shared.ASP.Configuration;

public class KebabCaseRouteNamingConvention : IControllerModelConvention
{
    private static string ToKebabCase(string text) => Regex.Replace(text, "([a-z])([A-Z])", "$1-$2").ToLower();
    public void Apply(ControllerModel controller)
    {
        controller.ControllerName = ToKebabCase(controller.ControllerName);
        foreach (var selector in controller.Selectors)
        {
            if (selector.AttributeRouteModel != null)
            {
                var template = selector.AttributeRouteModel.Template;
                if (!string.IsNullOrEmpty(template))
                {
                    selector.AttributeRouteModel.Template = ToKebabCase(template);
                }
            }
        }
    }
}