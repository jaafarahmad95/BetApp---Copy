using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto.Payment
{
    public  class DepositViewDto
    {
        /// <summary>
        /// Payment Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Payment Method
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Payment Date
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Payment Currency
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Currency Value
        /// </summary>
        public decimal CurrencyValue { get; set; }
        
        /// <summary>
        /// Payment Staff
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Payment Amount
        /// </summary>
        public double Amount { get; set; }
        
       
       

    }
}
