using System.ComponentModel.DataAnnotations;

namespace testtestdbcontext.testtest.dto.Withdrow
{
    public class WithdrowreqDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string BankAccountNo {get;set;}
        
    }
}