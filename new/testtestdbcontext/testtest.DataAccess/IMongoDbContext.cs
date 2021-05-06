using MongoDB.Driver;

namespace testtest.DataAccess
{
    public interface IMongoDbContext
    {
        IClientSessionHandle Session { get; set; }

        IMongoCollection<T> GetCollection<T>(string name);
        public  void Dropdatabase(string dbname);
    }
}