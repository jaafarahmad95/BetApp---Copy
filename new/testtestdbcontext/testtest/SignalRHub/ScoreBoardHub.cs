using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using testtest.SignalRHub;
using testtestdbcontext.AppUserService.Site;
using Quartz;

namespace testtestdbcontext.testtest.SignalRHub
{
    public class ScoreBoardHub : Hub
    {
         IScheduler _scheduler;

        public readonly IConnectionService _ConSer ;
        public ScoreBoardHub(IScheduler scheduler,IConnectionService ConSer )
        {
              _ConSer =ConSer ;
              _scheduler =scheduler;
        }

        public  override Task OnDisconnectedAsync(Exception exception)
        {
           
            var conID = Context.ConnectionId;
            _ConSer.RemoveSConn(conID);
            _ConSer.RemoveSConGroup(conID);
            var userId = _ConSer.getSuserId(conID);
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

        public override  Task OnConnectedAsync()
        {  

            return base.OnConnectedAsync();
        }

        public async Task<ClientResponse> RemoveFromGroup(string groupName)
        {
               var group = await _ConSer.getSGroups(Context.ConnectionId);
                if(group != null)
                {
                    try
                         {
                
                            await  Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
                            await _ConSer.removeSgroup(Context.ConnectionId,group);
                            await   AssignToSBGroupz(groupName);
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
                else
                {
                    try
                    {
                            await   AssignToSBGroupz(groupName);
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
                  

                     

        }

        public async Task<ClientResponse> AssignToSBGroupz(string groupName)
        {       
               
                try
                    {
                
                        await  Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                        await _ConSer.addSgroup(Context.ConnectionId, groupName);
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
        
        
    }
}