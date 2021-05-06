using System.Threading.Tasks;
using MongoDB.Driver;
using testtest.DataAccess;
using testtestdbcontext.testtest.Models.Site;

namespace testtestdbcontext.AppUserService.Site
{
    public class BetCounterService : IBetCounterService
    {
         protected IMongoCollection<BetCounter> _betC;
        private readonly IMongoDbContext _context;
       
        public BetCounterService(IMongoDbContext context)
        {
            _context = context;
            _betC= context.GetCollection<BetCounter>(typeof(BetCounter).Name);
            

        }


        public async Task<bool>  addtoCounter(string EventId)
        {

            var filter = Builders<BetCounter>.Filter.Eq("EventId",EventId);
            var x= _betC.Find(filter).FirstOrDefaultAsync().Result;
            if(x!=null)
            {
             var currentcount =  _betC.Distinct<int>("betcounter",filter).FirstOrDefault();
             var newcounter = currentcount+1;
             var update = Builders<BetCounter>.Update.Set("betcounter",newcounter);
             await _betC.UpdateOneAsync(filter,update);
           return true;
            }
            else
             {
                BetCounter newcount= new BetCounter()
                {
                    EventId=EventId,
                    
                    betcounter=1
                };
                await _betC.InsertOneAsync(newcount);
                return true ;
             }
                 
        }

        public async Task<bool>  takefromCounter(string EventId)
        {

            var filter = Builders<BetCounter>.Filter.Eq("EventId",EventId);
            var currentcount =  _betC.Distinct<int>("betcounter",filter).FirstOrDefault();
            var newcounter = currentcount-1;
            var update = Builders<BetCounter>.Update.Set("betcounter",newcounter);
            await _betC.UpdateOneAsync(filter,update);
           return true;     
        }
    }
}