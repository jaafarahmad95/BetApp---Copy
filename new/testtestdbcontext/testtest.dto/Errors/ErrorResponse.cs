using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public class ErrorResponse
    {
        public ErrorResponse() { }

        public ErrorResponse(ErrorModelDto error)
        {
            Errors.Add(error);
        }
        public List<ErrorModelDto> Errors { get; set; } = new List<ErrorModelDto>();
    }
}
