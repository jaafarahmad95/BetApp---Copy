using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using testtest.dto;
using testtest.dto.Payment;
using testtest.dto.Site;
using testtest.Models;
using testtest.Models.Deposit;
using testtest.Models.Site;
using testtestdbcontext.testtest.dto.Site;
using testtestdbcontext.testtest.dto.Withdrow;
using testtestdbcontext.testtest.Models;

namespace testtest.MappingProfile
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<RegisterEntity, AppUser>().ReverseMap();
            CreateMap<UserResponse, AppUser>().ReverseMap();
            CreateMap<UpdatePasswordDto, AppUser>().ReverseMap();
            CreateMap<UserListViewDto, AppUser>().ReverseMap();
            CreateMap<RegisterAdmin, AppUser>().ReverseMap();

            CreateMap<LayOffer, OfferResultDto>();
            CreateMap<LayOffer, Bet>()
               .ForMember(dest => dest.RemainingStake
               , m => m.MapFrom(src => src.Liquidity));
            CreateMap<BackOffer, OfferResultDto>();
            CreateMap<BackOffer, Bet>()
               .ForMember(dest => dest.RemainingStake
               , m => m.MapFrom(src => src.Liquidity));
            CreateMap<OfferDto ,Bet >()
                .ForMember(dest => dest.RemainingStake
               , m => m.MapFrom(src => src.Stake));
            CreateMap<OfferDto, LayOffer>()     
                .ForMember(dest => dest.Liquidity,
                m => m.MapFrom(src => src.Stake));
            CreateMap<OfferDto, BackOffer>()     
                .ForMember(dest => dest.Liquidity,
                m => m.MapFrom(src => src.Stake));


           

            CreateMap<Bet, InvolvedLayBets>()
                .ForMember(des => des.BetId, m => m.MapFrom(src => src.Id))
                .ForMember(des => des.RemainingStake, m => m.MapFrom(src => src.RemainingStake));
            CreateMap<Bet, InvolvedBackBets>()
                .ForMember(des => des.BetId, m => m.MapFrom(src => src.Id))
                .ForMember(des => des.RemainingStake, m => m.MapFrom(src => src.RemainingStake));
            CreateMap<InvolvedLayBets, Bet>()
                .ForMember(des => des.RemainingStake, m => m.MapFrom(src => src.RemainingStake))
                .ForMember(des => des.Id, m => m.MapFrom(src => src.BetId));
            CreateMap<InvolvedBackBets, Bet>()
               .ForMember(des => des.RemainingStake, m => m.MapFrom(src => src.RemainingStake))
                .ForMember(des => des.Id, m => m.MapFrom(src => src.BetId));
            CreateMap<Bet, BetViewDto>()
                .ForMember(des => des.stake, m => m.MapFrom(src => src.InStake));
            CreateMap<Bet,BetSlipDTO>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(des => des.Stake,m =>m.MapFrom(src =>src.RemainingStake))
                .ForSourceMember(m => m.Lalaibelty,opt => opt.DoNotValidate())
                .ForSourceMember(m => m.Profit,opt => opt.DoNotValidate())
                .ForSourceMember(m => m.CreationDate,opt => opt.DoNotValidate());


            CreateMap<Event, EventViewDto>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(des=>  des.EventId,  m=>  m.MapFrom(src => src.Fixture.EventId))
                .ForMember(des => des.HomeTeam, m => m.MapFrom(src => src.Fixture.participants.first_name))
                .ForMember(des => des.AwayTeam, m => m.MapFrom(src => src.Fixture.participants.second_name))
                .ForMember(des => des.Score,    m => m.MapFrom(src => src.Fixture.scoreboard.score))
                .ForMember(des=>des.startDate,  m=>m.MapFrom(src=>src.Fixture.startDate))
                .ForMember(des=>des.scoreboard, m=>m.Ignore());
                CreateMap<Event, SearchViewDto>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(des=>  des.EventId,  m=>  m.MapFrom(src => src.Fixture.EventId))
                .ForMember(des => des.HomeTeam, m => m.MapFrom(src => src.Fixture.participants.first_name))
                .ForMember(des => des.AwayTeam, m => m.MapFrom(src => src.Fixture.participants.second_name))
                .ForMember(des => des.Score,    m => m.MapFrom(src => src.Fixture.scoreboard.score))
                .ForMember(des=>des.startDate,  m=>m.MapFrom(src=>src.Fixture.startDate))
                .ForMember(des=>des.Stage,  m=>m.MapFrom(src=>src.Fixture.stage));



            CreateMap<Results, LayOffer>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(des => des.RunnerName, m => m.MapFrom(src => src.name.value));
                
            CreateMap<Results, BackOffer>()
               .IgnoreAllPropertiesWithAnInaccessibleSetter()
               .ForMember(des => des.RunnerName, m => m.MapFrom(src => src.name.value));
           CreateMap<Results, MarketViewDto>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(des => des.Name, m => m.MapFrom(src => src.name.value));

          
                

            CreateMap<Currency, UpdateCurrencyDto>().ReverseMap();
            CreateMap<Currency, CurrencyDto>().ReverseMap();


            CreateMap<Deposit , DepositViewDto>()
                .ForMember(dest => dest.Method,
                  m => m.MapFrom(src => src.depositMethod.Name))
               .ForMember(dest => dest.Currency,
                  m => m.MapFrom(src => src.Currency.Code))
               .ForMember(dest => dest.CurrencyValue,
                  m => m.MapFrom(src => src.CurrencyRate));
            CreateMap<DepositCreationDto, Deposit >()
                .ForMember(des=>des.Date, m=>m.Ignore());
            CreateMap<DepositMethod, DepositMethodsDto>();


            CreateMap<WithdrowreqDto, Withdrow >()
                .ForMember(des=>des.Date, m=>m.Ignore());
             CreateMap<Withdrow , WithdrowViewDto>();


            CreateMap<ScoreBoared, ScoreBoardDTO>()
                .ForMember(des => des.penalties1, m => m.MapFrom(src => src.penalties.player1.five))
                .ForMember(des => des.penalties2, m => m.MapFrom(src => src.penalties.player2.five))
                .ForMember(des => des.redCards1, m => m.MapFrom(src => src.redCards.player1.five))
                .ForMember(des => des.redCards2, m => m.MapFrom(src => src.redCards.player2.five))
                .ForMember(des => des.yellowCards1, m => m.MapFrom(src => src.yellowCards.player1.five))
                .ForMember(des => des.yellowCards2, m => m.MapFrom(src => src.yellowCards.player2.five))
                .ForMember(des => des.shirtColor1, m => m.MapFrom(src => src.playerInfo.one.shirtColor))
                .ForMember(des => des.shirtColor2, m => m.MapFrom(src => src.playerInfo.two.shirtColor))
                .ForMember(des => des.shortsColor1, m => m.MapFrom(src => src.playerInfo.one.shortsColor))
                .ForMember(des => des.shortsColor2, m => m.MapFrom(src => src.playerInfo.two.shortsColor))
                .ForMember(des => des.teamName1, m => m.MapFrom(src => src.playerInfo.one.teamName))
                .ForMember(des => des.teamName2, m => m.MapFrom(src => src.playerInfo.two.teamName))
                .ForMember(des => des.firsthalf1teamscore, m => m.MapFrom(src => src.scoreDetailed.player1.one))
                .ForMember(des => des.firsthalf2teamscore, m => m.MapFrom(src => src.scoreDetailed.player2.one))
                .ForMember(des => des.firstEhalf1teamscore, m => m.MapFrom(src => src.scoreDetailed.player1.six))
                .ForMember(des => des.firstEhalf2teamscore, m => m.MapFrom(src => src.scoreDetailed.player2.six))
                .ForMember(des => des.seconedEhalf1teamscore, m => m.MapFrom(src => src.scoreDetailed.player1.seven))
                .ForMember(des => des.seconedEhalf2teamscore, m => m.MapFrom(src => src.scoreDetailed.player2.seven));
        }
    }
}
