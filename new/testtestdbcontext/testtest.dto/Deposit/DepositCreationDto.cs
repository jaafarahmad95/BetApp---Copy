using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace testtest.dto.Payment
{
    public class DepositCreationDto
    {

        /// <summary>
        /// Payment Method Id
        /// </summary>
        public int MethodId { get; set; } = 2;
        /// <summary>
        /// Installment Id
        /// </summary>
        //public DateTime Date { get; set; } 
        /// <summary>
        /// Payment Currency Id
        /// </summary>
        public int CurrencyId { get; set; }
        /// <summary>
        /// Payment Staff
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// Payment Amount
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// Payment Note
        /// </summary>
       public string cardNumber { get; set; }
       public string CVV { get; set; }
       


    }
}
