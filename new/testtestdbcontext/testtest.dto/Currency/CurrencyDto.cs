using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public class CurrencyDto
    {
        /// <summary>
        /// Currency Id
        /// </summary>
        public int CurrenceyId { get; set; }
        /// <summary>
        /// Currency Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Currency Symbol
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// Currency Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Currency Rate
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// Is Default Currency
        /// </summary>
        public bool IsDefault { get; set; }

    }
}
