using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public class CurrencyUpdateList
    {
        public string userId { get; set; }
       

        public ICollection<UpdateCurrencyDto> CurrencyList { get; set; }
            = new List<UpdateCurrencyDto>();

    }
}
