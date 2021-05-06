using System.Collections.Generic;
using System.Threading.Tasks;

namespace testtestdbcontext.AppUserService.Site
{
    public interface IConnectionService 
    {
       
        Task manageHUserCon (string username,string conId);
        Task<string> getHGroups(string ConId);
        Task<bool> removeHgroup(string ConId, string GroupID);
        Task<bool> addHgroup(string ConId, string GroupID);
        Task RemoveHConn (string conId);
        Task<List<string>> getHconnectionId (string username);
        Task RemoveHConGroup(string conId);
        Task<string> gethuserId (string conId);




        Task manageLUserCon (string username,string conId);
        Task<List<string>> getLGroups(string ConId);
        Task<bool> removeLgroup(string ConId, string GroupID);
       
        Task<bool> addLgroup(string ConId, string GroupID);
        Task RemoveLConn (string conId);
        Task<List<string>> getLconnectionId (string username);
        Task RemoveLConGroup(string conId);
        Task<string> getLuserId (string conId);



        Task manageSUserCon (string username,string conId);
        Task<string> getSGroups(string ConId);
        Task<bool> removeSgroup(string ConId, string GroupID);
        Task<bool> addSgroup(string ConId, string GroupID);
        Task RemoveSConn (string conId);
        Task<List<string>> getSconnectionId (string username);
        Task RemoveSConGroup(string conId);
        Task<string> getSuserId (string conId);

        
       
        

    }

}