using System;
using testtest.Models.Deposit;


namespace testtest.Models.Site
{
    public class UserState
    {
        public string UserName { get; set; }
        public string Country { get; set; }
        public DateTime birthdate { get; set; }
        public string Currency { get; set; } 

        public int numberofPlacedBets {get;set;}

        public int NoOfWonBet { get; set; }

        public double AvgStake { get; set; }

        public double highistStake {get;set;}

        public double CurrentBalance {get;set;}
        public testtest.Models.Deposit.Deposit LatestDeposit {get;set;}

        public testtest.Models.Deposit.Deposit HighistDeposit {get;set;}

    }
}
