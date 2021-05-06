using System;
using System.ComponentModel.DataAnnotations;

namespace NotificationSystem.Domain
{
    [Serializable]
    public class UserInfo : IUserInfo
    {
        [MaxLength(50)]
        public string Group { get; set; }

        [MaxLength(50)]
        public string User { get; set; }

        [MaxLength(20)]
        public string UserRole { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }
        public string Company { get; set; }

        [MaxLength(50)]
        public string Server { get; set; }
    }



}



