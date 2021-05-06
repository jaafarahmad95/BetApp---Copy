using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace testtest.ModelValidator
{
    public class UnprocessableEntityObjectResult:ObjectResult
    {
        public UnprocessableEntityObjectResult(ModelStateDictionary modelstate)
    : base(new SerializableError(modelstate))
        {
            if (modelstate == null)
            {
                throw new ArgumentNullException(nameof(modelstate));
            }

            StatusCode = 422;
        }
    }
}
