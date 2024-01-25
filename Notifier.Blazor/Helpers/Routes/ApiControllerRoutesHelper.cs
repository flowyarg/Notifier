using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Reflection;

namespace Notifier.Blazor.Helpers.Routes
{
    public static class ApiControllerRoutesHelper
    {
        private static TTargetExpression? To<TTargetExpression>(this Expression? source) where TTargetExpression : Expression => (TTargetExpression?)source;

        public static string GetActionUrl<TController>(Expression<Func<TController, Delegate>> actionExpression)
            where TController : Controller
        {
            var methodInfo = actionExpression
                .To<LambdaExpression>()?.Body
                .To<UnaryExpression>()?.Operand
                .To<MethodCallExpression>()?.Object
                .To<ConstantExpression>()?.Value
            as MethodInfo;

            if (methodInfo == null)
            {
                throw new Exception("Can't find method by expression");
            }

            var controllerUrl = typeof(TController)
               .GetCustomAttribute<RouteAttribute>()
               ?.Template?.Replace("[controller]", typeof(TController).Name.Replace("Controller", ""));

            if (string.IsNullOrEmpty(controllerUrl))
            {
                throw new Exception("Can't resolve controller route");
            }           

            var actionUrl = methodInfo
                .GetCustomAttributes<RouteAttribute>()
                ?.FirstOrDefault()
                ?.Template;

            if (string.IsNullOrEmpty(actionUrl))
            {
                throw new Exception("Can't resolve action route");
            }

            return $"{controllerUrl}/{actionUrl}";
        }
    }
}
