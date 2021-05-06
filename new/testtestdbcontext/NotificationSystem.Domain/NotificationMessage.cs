using System;
using System.ComponentModel.DataAnnotations;

namespace NotificationSystem.Domain
{
    [Serializable]
    public class NotificationMessage : INotificationMessage
    {
        [Required]
        [MaxLength(100)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Body { get; set; }
        public MessageType MessageType { get; set; }
        [MaxLength(50)]
        public string Group { get; set; }
        [MaxLength(50)]
        public string User { get; set; }
        [MaxLength(50)]
        public string Server { get; set; }

        public DateTime MessageDate { get; set; }

        [MaxLength(20)]
        public string UserRole { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }
       
        public string UriImage { get; set; }
        
        [MaxLength(50)]
        public string Amount { get; set; }
        public string NotificationId { get; set; }
        
    }

}



