using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using testtest.Models.Site;

namespace testtest.Service.Site
{
     public interface IBetServices
    {
        
        Task<bool> SattelBets();
    }
}
