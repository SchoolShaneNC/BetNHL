namespace BetNHL.Models
{ // week 1
    public class NhlTeamDTO
    {
        public int Id { get; set; }             
        public string Name { get; set; }           
        public string Abbreviation { get; set; }   
        public string LogoUrl { get; set; }    

        public static string GenerateLogoUrl(string abbreviation, int season)
        {
            return $"https://assets.nhle.com/logos/nhl/svg/{abbreviation}_light.svg?season={season}";
        }
    }
}
