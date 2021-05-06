using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public class UserListViewDto
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The Name of the User 
        /// </summary>
        public string Name { get; set; }

       public string LastName { get; set; }
       public string  Email { get; set; }
       public string  City { get; set; }
       public string  Country { get; set; }
       public string  Currency { get; set; } ="TL" ;
       public string  PersonalID { get; set; }
       public DateTime  birthdate { get; set; }
       
       
       
       
    }
}
