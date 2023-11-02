using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores;

public class MyExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<MyExceptionFilter> logger;

    public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
    {
        this.logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, context.Exception.Message);
        base.OnException(context);
    }
}

// In C#, the 'base' keyword is used to access members of the base class from within a derived class. In this case, base.OnException(context); is calling the OnException method of the base class, passing in the context parameter.
// By calling base.OnException(context);, the custom filter is ensuring that the base class's exception handling logic is still executed.
// The OnException method is a part of the ExceptionFilterAttribute class in ASP.NET Core. It's a method that gets called when an exception occurs in the method that the attribute is applied to. The context parameter is an ExceptionContext object that encapsulates information about the exception.
