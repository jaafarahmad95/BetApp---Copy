using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public const string BaseAPIAddress = "http://localhost:58212/";
        //public const string BaseAPIAddress= "http://193.140.43.22:8080/";
        //test
        public static class Authentication
        {
            public const string Login = Base + "/auth/login";

            public const string Refresh = Base + "/auth/refresh";
        }
        public static class SystemUser
        {
            public const string userstat = Base+"/user/analyze/{UserName}";
            public const string GetUsers = Base + "/users/all";
           
            public const string GetUserByuserName = Base + "/users/{UserName}";
            public const string GetUser = Base + "/user/{id}";
            public const string CreateAdmin = Base + "/users/Admin";
            public const string CreateClient = Base + "/users/client";
            public const string UpdatePassword = Base + "/users/Password/{id}";
            public const string DeleteUser = Base + "/users/delete/{id}";
            public const string Guest = Base + "/users/guests";

       
        }

        public static class Accounts
        {
            public const string GetAccount = Base + "/Account/{username}";
            public const string GetBalancebyUsername = Base + "/Account/balanceByName/{username}";
            public const string GetBalancebyid = Base + "/Account/balanceById/{id}";
            public const string GetBetHistory = Base + "/History/Bet";
            public const string Withdrow = Base + "/Account/withdrow";
            public const string GetWithdrowHistory = Base + "/History/withdrow";

        }

        public static class Offers
        {
            
            public const string GetLayOffers = Base + "/offers/lay/{EventId}/{MarketName}";
            public const string GetBackOffers = Base + "/offers/back/{EventId}/{MarketName}";
            public const string PlaceLaybet = Base + "/bet/lay";
            public const string PlaceBackbet = Base + "/bet/back";
            public const string Removeoffers = Base + "/bet/remove/{userid}";
            public const string getoffers = Base + "/offers/{userId}/{EventId}/{MarketName}";

            public const string getUnmatched = Base +"/offers/UserBets";

           
        }
        public static class connmanager
        {
         public const string ManageHomeUserConn= Base + "/con/home";
         public const string ManageLiveUserConn= Base + "/con/live";
         public const string ManageScoreUserConn= Base + "/con/score";

        }

        public static class Events
        {
            public const string GetRegion = Base + "/Events";
            public const string GetCompInRegion = Base + "/events/{region}";
            public const string GetEventincomp = Base + "/Events/{region}/{comp}";
            public const string GetLiveEvents = Base + "/events/live";
            public const string GetEventmarkets = Base + "/event/{eventId}";
            public const string GetMarketsOdds = Base + "/event/{eventId}/{MarketName}";
            public const string GetScoreBoared = Base + "/event/score/{userId}/{eventId}";
            public const string GetRegAndComp = Base + "/event/all";
            public const string search = Base + "/event/search/{SearchTearm}";

        }

        public static class Deposits
        {
            public const string GetDeposits = Base + "/Deposits/History";
            public const string GetDepositMethods = Base + "/Deposits/methods";
            public const string CreateDeposit  = Base + "/Deposits";
           
        }

        public static class Currency
        {
            public const string GetCurrencies = Base + "/currency";
            public const string GetCurrency = Base + "/currency/currencyId";
            public const string GetDefaultCurrency = Base + "/currency/default";
            public const string SetDefaultCurrency = Base + "/currency/{currencyId}/default";
            public const string UpdateCurrencyPut = Base + "/currency/update";
        }
    }
}
