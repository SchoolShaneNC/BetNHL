namespace BetNHL_Web_Api.Models
{
    public class NhlPlayerDTO    // week 1
    {
        public int ID { get; set; }            
        public string FullName => FirstName +" "+ LastName;
        public string Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int a = today.Year - DOB.Year
                    - ((today.Month < DOB.Month ||
                        (today.Month == DOB.Month && today.Day < DOB.Day)) ? 1 : 0);
                return a.ToString();
            }
        }
        public DateTime DOB { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }   
        public int JerseyNumber { get; set; }         
        public string Headshot { get; set; }

        public string HeroImage { get; set; }
        public int TeamID { get; set; }         
    }
}
