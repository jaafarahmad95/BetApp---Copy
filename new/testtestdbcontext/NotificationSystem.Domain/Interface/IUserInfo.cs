namespace NotificationSystem.Domain
{
    public interface IUserInfo
    {
        string Group { get; set; }
        string User { get; set; }
        string Server { get; set; }
        string UserRole { get; set; }
        string UserName { get; set; }
       
    }
}