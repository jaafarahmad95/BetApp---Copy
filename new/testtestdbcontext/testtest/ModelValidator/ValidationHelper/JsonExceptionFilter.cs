using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testtest.ModelValidator
{
    public class JsonExceptionFilter:IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
            var error = new ApiErrors();
            error.Message = "A Server error occurd";
            error.Detail = context.Exception.Message;
            context.Result = new ObjectResult(error) {StatusCode = 500 };
        }
    }
}
