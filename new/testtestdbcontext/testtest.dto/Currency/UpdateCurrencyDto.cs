using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public class UpdateCurrencyDto
    {
        /// <summary>
        /// Currency ID
        /// </summary>
        public int CurrenceyId { get; set; }

        /// <summary>
        /// Currency Rate Value
        /// </summary>
        public decimal value { get; set; }
    }
}
