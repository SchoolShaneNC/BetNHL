using BetNHL_Web_Api.Models;

namespace BetNHL_Web_Api.Helpers
{
    public static class DTOMapper
    {
        public static UserDTO MapUserDTO(ApplicationUser user)
        {
            return new UserDTO
            {
                ID = user.Id,
                Username = user.UserName ?? string.Empty,

                Balance = user.Balance,

                BetsWon = user.BetsWon,
                BetsLost = user.BetsLost,

                TotalMoneyBet = user.TotalMoneyBet,
                TotalMoneyWon = user.TotalMoneyWon,
                TotalMoneyLost = user.TotalMoneyLost,

                Parlays = user.Parlays?
                    .Select(MapParlayDTO)
                    .ToList() ?? new List<ParlayDTO>()
            };
        }

        public static ParlayDTO MapParlayDTO(Parlay parlay)
        {
            return new ParlayDTO
            {
                Id = parlay.Id,

                Username = parlay.User?.UserName ?? string.Empty, 

                DatePlaced = parlay.DatePlaced,

                Stake = parlay.Stake,
                CombinedOdds = parlay.CombinedOdds,
                PotentialPayout = parlay.PotentialPayout,

                Won = parlay.Won,

                Legs = parlay.Legs?
                    .Select(MapParlayLegDTO)
                    .ToList() ?? new List<ParlayLegDTO>()
            };
        }

        public static ParlayLegDTO MapParlayLegDTO(ParlayLeg leg)
        {
            return new ParlayLegDTO
            {
                ID = leg.ID,

                ParlayId = leg.ParlayId,

                Category = leg.Category,
                Metric = leg.Metric,
                Condition = leg.Condition,
                Context = leg.Context,

                GameId = leg.GameId,

                TeamPickedID = leg.TeamPickedID,
                PlayerPickedID = leg.PlayerPickedID,

                Odds = leg.Odds,
                Line = leg.Line,

                Won = leg.Won
            };
        }
    }
}