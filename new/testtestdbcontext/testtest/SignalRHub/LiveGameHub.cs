using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using testtest.SignalRHub;
using testtestdbcontext.AppUserService.Site;
using Quartz;

namespace testtest.SignalRHub
{
    public class LiveGameHub : Hub
    { 
         IScheduler _scheduler;
        public readonly IConnectionService _ConSer ;
        public LiveGameHub(IScheduler scheduler,IConnectionService ConSer )
        {
              _ConSer =ConSer ;
              _scheduler =scheduler;
        }

        public override  Task OnConnectedAsync()
        {  

            return base.OnConnectedAsync();
        }

        public  override Task OnDisconnectedAsync(Exception exception)
        {
           
            var conID = Context.ConnectionId;
            _ConSer.RemoveLConn(conID);
            _ConSer.RemoveLConGroup(conID);
            var userId = _ConSer.getLuserId(conID);
             var trigforusr="foruser"+userId;
             var grpnameuser = "quartzUpdateOddsTriggers"+userId ;
             var names = _scheduler.GetTriggerGroupNames().Result;
             foreach(var item in names)
             {
                    if (item.ToString()==grpnameuser)
                         _scheduler.UnscheduleJob(new TriggerKey(trigforusr,grpnameuser));
                        
             }
             
            var jobid =   "foruser"+userId;      
            var jobgroup = "quartzUpdateOdds";
            JobKey jk = new Quartz.JobKey (jobid, jobgroup);
             _scheduler.DeleteJob(jk);
            return base.OnDisconnectedAsync(exception);
        }


        public async Task<ClientResponse> AssignToGroupz(string groupName)
        {       
               
                try
                    {
                
                        await  Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                        await _ConSer.addLgroup(Context.ConnectionId, groupName);
                        return new ClientResponse
                        {
                            IsAddedToGroup = true
                        };
                    }
                catch (Exception exception)
                    {
                        return new ClientResponse
                        {
                        IsAddedToGroup = false,
                        ErrorMessages = new List<string>() { exception.Message }
                        };
                    } 
              
  
        }

        public async Task<ClientResponse> RemoveFromGroup(string groupName)
        {
           var list = await _ConSer.getLGroups(Context.ConnectionId);
           if(list!=null)
           {
             foreach (var item in list)
                {
                    {
               
                     try
                         {
                
                            await  Groups.RemoveFromGroupAsync(Context.ConnectionId, item);
                            await _ConSer.removeLgroup(Context.ConnectionId, item);
                         }
                     catch (Exception exception)
                         {
                             return new ClientResponse
                              {
                                 IsAddedToGroup = false,
                                ErrorMessages = new List<string>() { exception.Message }
                             };
                         }
                    }  
                }
            await   AssignToGroupz(groupName);
                      return new ClientResponse
                             {
                                 IsAddedToGroup = true
                             };
           }

           else 
           {
                 await   AssignToGroupz(groupName);
                 return new ClientResponse
            {
                    IsAddedToGroup = true
            };

           }
        }
    }
}