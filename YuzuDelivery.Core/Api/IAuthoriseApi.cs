namespace YuzuDelivery.Core
{
    public interface IAuthoriseApi
    {
        bool Authorise(string username, string password, string role);
    }
}