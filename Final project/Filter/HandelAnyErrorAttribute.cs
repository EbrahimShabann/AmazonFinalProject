using Final_project.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Final_project.Filter
{
    public class HandelAnyErrorAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            ViewResult view = new ViewResult();
            view.ViewName = "_ErrorLayout";

            context.Result = view;

            context.ExceptionHandled = true;
        }
    }
}
