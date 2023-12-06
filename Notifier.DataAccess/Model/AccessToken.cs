namespace Notifier.DataAccess.Model
{
    public class AccessToken
    {
        public int Id { get; set; }
        public string Token {  get; set; }
        public string IV { get; set; }
        public DateTimeOffset ValidThrough {  get; set; }
    }
}
