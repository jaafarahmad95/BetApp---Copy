using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using testtest.DataAccess;
using testtest.Models;
using testtest.Models.SignalRConnection;

namespace testtestdbcontext.AppUserService.Site
{
    
    public class ConnectionService : IConnectionService
    {
        protected IMongoCollection<HomeGroup> _hCon;
         protected IMongoCollection<LiveGroupConnection> _lCon;
         protected IMongoCollection<SBoardGroups> _SBGroup; 
        private readonly IMongoDbContext _context;
        protected IMongoCollection<HomeUserConn> _hUsrCon;
        protected IMongoCollection<LiveUserConn> _lUsrCon;
        protected IMongoCollection<SBUserConn> _sUsrCon;
        protected IMongoCollection<AppUser> _appuser;
        
        public ConnectionService(IMongoDbContext context)
        {
            _context = context;
            _hCon = _context.GetCollection<HomeGroup>(typeof(HomeGroup).Name);
            _lCon= _context.GetCollection<LiveGroupConnection>(typeof(LiveGroupConnection).Name);
            _SBGroup= _context.GetCollection<SBoardGroups>(typeof(SBoardGroups).Name);
            _hUsrCon = _context.GetCollection<HomeUserConn>(typeof(HomeUserConn).Name);
            _lUsrCon = _context.GetCollection<LiveUserConn>(typeof(LiveUserConn).Name);
            _sUsrCon = _context.GetCollection<SBUserConn>(typeof(SBUserConn).Name);
           _appuser= context.GetCollection<AppUser>(typeof(AppUser).Name);
        }

        public async Task manageHUserCon(string username, string conID)
        {
           var filter = Builders<HomeUserConn>.Filter.Eq("Username",username);
            var conn =  _hUsrCon.Find(filter).SingleOrDefaultAsync().Result;
            if(conn == null)
                {   List<string> v = new List<string>();
                    v.Add(conID); 
                    HomeUserConn x = new HomeUserConn()
                    {
                        Username= username,
                        ConnId= v 
                    };
                 await   _hUsrCon.InsertOneAsync(x);
                }
            else
            { 
              var oldconlist = conn.ConnId;
              oldconlist.Add(conID);
              var update = Builders<HomeUserConn>.Update.Set("ConnId",oldconlist);
              await  _hUsrCon.UpdateOneAsync(filter,update);
            }
        }

        public async Task<string> getHGroups(string ConId)
        {
          
            var filter = Builders<HomeGroup>.Filter.Eq("ConnectionId", ConId);
            var x = await _hCon.Distinct<string>("ConnectionGroups", filter).FirstOrDefaultAsync();
            return x ;

           
        }

        public async Task<bool> removeHgroup(string ConId, string GroupName)
        {
            var x = await _hCon.Find(z => z.ConnectionId == ConId).FirstOrDefaultAsync();
            if (x != null)
            {
                
                var filter = Builders<HomeGroup>.Filter.Eq("ConnectionId", ConId);
                var update = Builders<HomeGroup>.Update.Set("ConnectionGroups", "");
                await _hCon.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {

                return true;
            }
        }

        public async Task<bool> addHgroup(string ConId, string GroupName)
        {
           var x = await _hCon.Find(z => z.ConnectionId == ConId).FirstOrDefaultAsync();
            if (x != null)
            {

                
                var filter = Builders<HomeGroup>.Filter.Eq("ConnectionId", ConId);
                var update = Builders<HomeGroup>.Update.Set("ConnectionGroups", GroupName);
                await _hCon.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {
                
                HomeGroup newcon = new HomeGroup()
                {
                    ConnectionId = ConId,
                    ConnectionGroups = GroupName
                };
                await _hCon.InsertOneAsync(newcon);
                return true;
            }
        }

        public async Task RemoveHConn(string conId)
        {
            var conn =  _hUsrCon.Find(x => x.ConnId.Contains(conId)).SingleOrDefaultAsync().Result;
            var filter = Builders<HomeUserConn>.Filter.Eq("Username",conn.Username);
            List<string> newconnIds = new List<string>();
            if(conn.ConnId.Count ==1)
            {
              await  _hUsrCon.DeleteOneAsync(x => x.Username==conn.Username);
            }
           else
            {
               foreach (var item in conn.ConnId)
            {
                if(item != conId)
                    newconnIds.Add(item);

            }
            var update = Builders<HomeUserConn>.Update.Set("ConnId",newconnIds);
            await  _hUsrCon.UpdateOneAsync(filter,update);
           } 
        }

        public async Task<List<string>> getHconnectionId(string username)
        {
            var con = await _hUsrCon.Find(x => x.Username== username).FirstOrDefaultAsync();
             List<string> conid = new List<string>();
            if(con != null)
            {
                 conid = con.ConnId;
                return conid;
            }
            else 
            {
                var emptystring = "";
                conid.Add(emptystring);
                return conid;
            }
           
        }
         public async Task RemoveHConGroup(string conId)
        {
            await _hCon.DeleteOneAsync(z => z.ConnectionId == conId);

        }

        public async  Task<string> gethuserId (string conId)
        {

            var f = Builders<HomeUserConn>.Filter.Eq("ConnId",conId);
            var username = await _hUsrCon.Distinct<string>("Username",f).FirstOrDefaultAsync();
            var f2 = Builders<AppUser>.Filter.Eq("UserName",username);
            var userId= await _appuser.Distinct<string>("Id",f2).FirstOrDefaultAsync();
            return userId;

        }



        public async Task manageLUserCon(string username, string conID)
        {
            var filter = Builders<LiveUserConn>.Filter.Eq("UserName",username);
            var conn =  _lUsrCon.Find(filter).SingleOrDefaultAsync().Result;
            if(conn == null)
                {   List<string> v = new List<string>();
                    v.Add(conID); 
                    LiveUserConn x = new LiveUserConn()
                    {
                        UserName= username,
                        ConnId= v 
                    };
                 await   _lUsrCon.InsertOneAsync(x);
                }
            else
            { 
              var oldconlist = conn.ConnId;
              oldconlist.Add(conID);
              var update = Builders<LiveUserConn>.Update.Set("ConnId",oldconlist);
              await  _lUsrCon.UpdateOneAsync(filter,update);
            }
        }

        public async Task<List<string>> getLGroups(string ConId)
        {
            List<string> listtoret = new List<string>();
            var filter = Builders<LiveGroupConnection>.Filter.Eq("ConnectionId", ConId);
            var x = await _lCon.Distinct<string>("ConnectionGroups", filter).ToListAsync();
            foreach (var item in x)
            {
               if(item != "LiveGameGroup")
                listtoret.Add(item);

            }
            return listtoret;
        }

        public async Task<bool> removeLgroup(string ConId, string GroupID)
        {
            var x = await _lCon.Find(z => z.ConnectionId == ConId).FirstOrDefaultAsync();
            if (x != null)
            {
                List<string> newgroup = new List<string>();
                var oldgroups = x.ConnectionGroups;
                foreach (var item in oldgroups)
                {
                    if(item!= GroupID)
                        newgroup.Add(GroupID);

                }
                var filter = Builders<LiveGroupConnection>.Filter.Eq("ConnectionId", ConId);
                var update = Builders<LiveGroupConnection>.Update.Set("ConnectionGroups", newgroup);
                await _lCon.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {

                return true;
            }

        }

       

        public async Task<bool> addLgroup(string ConId, string GroupName)
        {
             var x = await _lCon.Find(z => z.ConnectionId == ConId).FirstOrDefaultAsync();
            if (x != null)
            {

                var oldgroups = x.ConnectionGroups;
                oldgroups.Add(GroupName);
                var filter = Builders<LiveGroupConnection>.Filter.Eq("ConnectionId", ConId);
                var update = Builders<LiveGroupConnection>.Update.Set("ConnectionGroups", oldgroups);
                await _lCon.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {
                List<string> newgroup = new List<string>(){GroupName};
              
                LiveGroupConnection newcon = new LiveGroupConnection()
                {
                    ConnectionId = ConId,
                    ConnectionGroups = newgroup
                };
                await _lCon.InsertOneAsync(newcon);
                return true;
            }
        }

        public async Task RemoveLConn(string conId)
        {
           var conn =  _lUsrCon.Find(x => x.ConnId.Contains(conId)).SingleOrDefaultAsync().Result;
            var filter = Builders<LiveUserConn>.Filter.Eq("UserName",conn.UserName);
            List<string> newconnIds = new List<string>();
            if(conn.ConnId.Count ==1)
            {
              await  _lUsrCon.DeleteOneAsync(x => x.UserName==conn.UserName);
            }else {
                foreach (var item in conn.ConnId)
                {
                if(item != conId)
                    newconnIds.Add(item);
                }
                var update = Builders<LiveUserConn>.Update.Set("ConnId",newconnIds);
                await  _lUsrCon.UpdateOneAsync(filter,update);
            }
            
        }
        public async Task RemoveLConGroup(string conId)
        {
            await _lCon.DeleteOneAsync(x => x.ConnectionId== conId);
        }

        public async Task<List<string>> getLconnectionId(string username)
        {
           var con = await _lUsrCon.Find(x => x.UserName== username).FirstOrDefaultAsync();
            List<string> conid = new List<string>();
            if  (con != null) {
                 conid = con.ConnId;
                return conid;
            }else {
                var emptystring = "";
                conid.Add(emptystring);
                return conid;
            }
        }

        public async  Task<string> getLuserId (string conId)
        {

            var f = Builders<LiveUserConn>.Filter.Eq("ConnId",conId);
            var username = await _lUsrCon.Distinct<string>("Username",f).FirstOrDefaultAsync();
            var f2 = Builders<AppUser>.Filter.Eq("UserName",username);
            var userId= await _appuser.Distinct<string>("Id",f2).FirstOrDefaultAsync();
            return userId;

        }





        public async Task manageSUserCon(string username, string conID)
        {
           var filter = Builders<SBUserConn>.Filter.Eq("Username",username);
            var conn =  _sUsrCon.Find(filter).SingleOrDefaultAsync().Result;
            if(conn == null)
                {   List<string> v = new List<string>();
                    v.Add(conID); 
                    SBUserConn x = new SBUserConn()
                    {
                        Username= username,
                        ConnId= v 
                    };
                 await   _sUsrCon.InsertOneAsync(x);
                }
            else
            { 
              var oldconlist = conn.ConnId;
              oldconlist.Add(conID);
              var update = Builders<SBUserConn>.Update.Set("ConnId",oldconlist);
              await  _sUsrCon.UpdateOneAsync(filter,update);
            }
        }

        public async Task<string> getSGroups(string ConId)
        {
             var filter = Builders<SBoardGroups>.Filter.Eq("ConnectionId", ConId);
            var x = await _SBGroup.Distinct<string>("ConnectionGroups", filter).FirstOrDefaultAsync();
            return x ;
        }

        public async Task<bool> removeSgroup(string ConId, string GroupID)
        {
            var x = await _SBGroup.Find(z => z.ConnectionId == ConId).FirstOrDefaultAsync();
            if (x != null)
            {
                
                var filter = Builders<HomeGroup>.Filter.Eq("ConnectionId", ConId);
                var update = Builders<HomeGroup>.Update.Set("ConnectionGroups", "");
                await _hCon.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {

                return true;
            }
        }

        public async Task<bool> addSgroup(string ConId, string GroupName)
        {
           var x = await _SBGroup.Find(z => z.ConnectionId == ConId).FirstOrDefaultAsync();
            if (x != null)
            {

                
                var filter = Builders<SBoardGroups>.Filter.Eq("ConnectionId", ConId);
                var update = Builders<SBoardGroups>.Update.Set("ConnectionGroups", GroupName);
                await _SBGroup.UpdateOneAsync(filter, update);
                return true;
            }
            else
            {
                
                SBoardGroups newcon = new SBoardGroups()
                {
                    ConnectionId = ConId,
                    ConnectionGroups = GroupName
                };
                await _SBGroup.InsertOneAsync(newcon);
                return true;
            }
        }

        public async Task RemoveSConn(string conId)
        {
            var conn =  _sUsrCon.Find(x => x.ConnId.Contains(conId)).SingleOrDefaultAsync().Result;
            var filter = Builders<SBUserConn>.Filter.Eq("Username",conn.Username);
            List<string> newconnIds = new List<string>();
            if(conn.ConnId.Count ==1)
            {
              await  _sUsrCon.DeleteOneAsync(x => x.Username==conn.Username);
            }
            foreach (var item in conn.ConnId)
            {
                if(item != conId)
                    newconnIds.Add(item);

            }
            var update = Builders<SBUserConn>.Update.Set("ConnId",newconnIds);
            await  _sUsrCon.UpdateOneAsync(filter,update);
        }

        public async Task<List<string>> getSconnectionId(string username)
        {
            var con = await _sUsrCon.Find(x => x.Username== username).FirstOrDefaultAsync();
             List<string> conid = new List<string>();
            if (con != null)
            { conid = con.ConnId;
            return conid;}
            else
             {
                var emptystring = "";
                conid.Add(emptystring);
                return conid;
            }
        }

        public async Task RemoveSConGroup(string conId)
        {
            await _SBGroup.DeleteOneAsync(x => x.ConnectionId==conId);
            
        }

        public async  Task<string> getSuserId (string conId)
        {

            var f = Builders<SBUserConn>.Filter.Eq("ConnId",conId);
            var username = await _sUsrCon.Distinct<string>("Username",f).FirstOrDefaultAsync();
            var f2 = Builders<AppUser>.Filter.Eq("UserName",username);
            var userId= await _appuser.Distinct<string>("Id",f2).FirstOrDefaultAsync();
            return userId;

        }

       
    }
}