using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using testtest.dto;
using testtest.Models.SignalRConnection;
using testtestdbcontext.AppUserService.Site;

namespace testtestdbcontext.testtest.Controllers
{
    public class ConnectionController  : ControllerBase
    {
         private readonly IConnectionService _conser;
        
        public ConnectionController(IConnectionService conser)
        {
         _conser=conser;   
        }


      [HttpPost(ApiRoutes.connmanager.ManageHomeUserConn, Name = "ManageHomeUserConn")]

      public async  Task<IActionResult> ManageHomeUserConn([FromBody] HomeUserConn usercone)
      {

            var username = usercone.Username; 
            var conID = usercone.ConnId;
            
            foreach (var item in conID)
            {
               await _conser.manageHUserCon(username,item); 
            }
            
            return Ok();

      }

       [HttpPost(ApiRoutes.connmanager.ManageLiveUserConn, Name = "ManageLiveUserConn")]

      public async  Task<IActionResult> ManageLiveUserConn([FromBody] LiveUserConn usercone)
      {

            var username = usercone.UserName; 
            var conID = usercone.ConnId;
           
             foreach (var item in conID)
            {
               await _conser.manageLUserCon(username,item); 
            } 
            
            return Ok();

      }

       [HttpPost(ApiRoutes.connmanager.ManageScoreUserConn, Name = "ManageScoreUserConn")]

      public async  Task<IActionResult> ManageScoreUserConn([FromBody] SBUserConn usercone)
      {

            var username = usercone.Username; 
            var conID = usercone.ConnId;
            foreach (var item in conID)
            {
               await _conser.manageSUserCon(username,item); 
            }
            
            return Ok();

      }
    }
}