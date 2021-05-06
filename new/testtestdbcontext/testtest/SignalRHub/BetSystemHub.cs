
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using NotificationSystem.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using testtest.DataAccess;
using testtest.dto;
using testtest.dto.Site;
using testtest.Service.Site;
using testtestdbcontext.AppUserService.Site;
using testtest.Models.SignalRConnection;
using Quartz;

namespace testtest.SignalRHub
{
     
    public class ClientResponse
    {
        public bool IsAddedToGroup { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; }
    }
    public class BetSystemHub : Hub
    {
       IScheduler _scheduler;
        public readonly IOfferService _offerservice;
        public readonly IConnectionService _ConSer ;
       
       
      
        public  BetSystemHub( IScheduler scheduler,IOfferService offerService,IConnectionService ConSer )
     {
         _offerservice = offerService;
         _ConSer =ConSer ;
         _scheduler =scheduler;
        
     }
        
        public override  Task OnConnectedAsync()
        {  

            return base.OnConnectedAsync();
        }

        public async Task<ClientResponse> AssignToGroup(string groupName)
        {       
                
                        try
                    {
                
                        await  Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                        await _ConSer.addHgroup(Context.ConnectionId, groupName);
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
           var list = await _ConSer.getHGroups(Context.ConnectionId);
           if(list!=null)
           {
               
                     try
                         {
                
                            await  Groups.RemoveFromGroupAsync(Context.ConnectionId, list);
                            await _ConSer.removeHgroup(Context.ConnectionId, list);
                            await   AssignToGroup(groupName);
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
                 await   AssignToGroup(groupName);
                 return new ClientResponse
            {
                    IsAddedToGroup = true
            };

           }
                       
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
           
             var conID = Context.ConnectionId;
             _ConSer.RemoveHConn(conID);
             _ConSer.RemoveHConGroup(conID);
            var userId = _ConSer.gethuserId(conID);
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
    }






   

}

   

