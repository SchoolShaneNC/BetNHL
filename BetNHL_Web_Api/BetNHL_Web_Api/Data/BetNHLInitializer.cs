//using Microsoft.EntityFrameworkCore;
//using BetNHL_Web_Api.Models;
//using System.Data.SqlTypes;
//using System.Diagnostics;
//using System.Numerics;
//using BetNHL_Web_Api.Data;

//namespace Project1API.Data
//{
//    public static class HackathonInitializer
//    {
//        /// <summary>
//        /// Prepares the Database and seeds data as required
//        /// </summary>
//        /// <param name="serviceProvider">DI Container</param>
//        /// <param name="DeleteDatabase">Delete the database and start from scratch</param>
//        /// <param name="UseMigrations">Use Migrations or EnsureCreated</param>
//        /// <param name="SeedSampleData">Add optional sample data</param>
//        public static void Initialize(IServiceProvider serviceProvider,
//            bool DeleteDatabase = true, bool UseMigrations = true, bool SeedSampleData = true)
//        {
//            using (var context = new BetNHLContext(
//                serviceProvider.GetRequiredService<DbContextOptions<BetNHLContext>>()))
//            {
//                //Refresh the database as per the parameter options
//                #region Prepare the Database
//                try
//                {
//                    //Note: .CanConnect() will return false if the database is not there!
//                    if (DeleteDatabase || !context.Database.CanConnect())
//                    {
//                        context.Database.EnsureDeleted(); //Delete the existing version 
//                        if (UseMigrations)
//                        {
//                            context.Database.Migrate(); //Create the Database and apply all migrations
//                        }
//                        else
//                        {
//                            context.Database.EnsureCreated(); //Create and update the database as per the Model
//                        }
//                        //Now create any additional database objects such as Triggers or Views
//                        //--------------------------------------------------------------------
//                        //Add triggers for concurrency control:

//                        // Member Table Triggers for Concurrency
//                        string sqlCmd = @"
//                            CREATE TRIGGER SetUserTimestampOnUpdate
//                            AFTER UPDATE ON Users
//                            BEGIN
//                                UPDATE Users
//                                SET RowVersion = randomblob(8)
//                                WHERE rowid = NEW.rowid;
//                            END;
//                        ";
//                        context.Database.ExecuteSqlRaw(sqlCmd);

//                        sqlCmd = @"
//                            CREATE TRIGGER SetBetTimestampOnInsert
//                            AFTER INSERT ON Bets
//                            BEGIN
//                                UPDATE Bets
//                                SET RowVersion = randomblob(8)
//                                WHERE rowid = NEW.rowid;
//                            END;
//                        ";
//                        context.Database.ExecuteSqlRaw(sqlCmd);

//                    }
//                    else //The database is already created
//                    {
//                        if (UseMigrations)
//                        {
//                            context.Database.Migrate(); //Apply all migrations
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine(ex.GetBaseException().Message);
//                }
//                #endregion

//                //Seed data needed for production and during development
//                #region Seed Required Data
//                try
//                {
//                    // Random random = new Random();
//                    if (!context.Users.Any())
//                    {
//                        var users = new List<User>
//                        {
//                            new User { Username = "Ghassan", PasswordHash = "Password1.", Balance = 10000M },
//                            new User { Code = "PE", Name = "Prince Edward Island" },
//                            new User { Code = "NB", Name = "New Brunswick" },
//                            new User { Code = "BC", Name = "British Columbia" },
//                            new User { Code = "NL", Name = "Newfoundland and Labrador" },
//                            new User { Code = "SK", Name = "Saskatchewan" },
//                            new User { Code = "NS", Name = "Nova Scotia" },
//                            new User { Code = "MB", Name = "Manitoba" },
//                            new User { Code = "QC", Name = "Quebec" },
//                            new User { Code = "YT", Name = "Yukon" },
//                            new User { Code = "NU", Name = "Nunavut" },
//                            new User { Code = "NT", Name = "Northwest Territories" },
//                            new User { Code = "AB", Name = "Alberta" }
//                        };

//                        context.Users.AddRange(Users);
//                        context.SaveChanges();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine(ex.GetBaseException().Message);
//                }
//                #endregion

//                //Seed meaningless data as sample data during development
//                #region Seed Sample Data 
//                if (SeedSampleData)
//                {
//                    try
//                    {
//                        // To randomly generate data
//                        Random random = new Random();

//                        // ------------------------------------
//                        // Regions
//                        // ------------------------------------


//                        // ------------------------------------
//                        // Challenges
//                        // ------------------------------------
//                        if (!context.Challenges.Any())
//                        {
//                            string[] challenges =
//                            {
//                            "Artificial Intelligence", "Web Development", "Cyber Security",
//                            "Cloud Computing", "Mobile Applications", "Data Analytics",
//                            "DevOps Automation", "Game Development", "Blockchain",
//                            "Internet of Things", "Machine Learning", "UI/UX Design",
//                            "AR/VR", "Embedded Systems", "FinTech", "HealthTech",
//                            "EdTech", "Green Tech", "Smart Cities", "Open Innovation"
//                        };

//                            string[] challengeCodes =
//                            {
//                            "AIH","WEB","CYB","CLD","MOB","DAT","DEV","GAM","BLC","IOT",
//                            "MLN","UIX","ARV","EMB","FIN","HLT","EDU","GRN","SMC","OPN"
//                        };

//                            for (int i = 0; i < challenges.Length; i++)
//                            {
//                                context.Challenges.Add(new Challenge
//                                {
//                                    Code = challengeCodes[i],
//                                    Name = challenges[i]
//                                });
//                            }

//                            context.SaveChanges();
//                        }

//                        // ------------------------------------
//                        // Primary Keys
//                        // ------------------------------------
//                        int[] challengeIDs = context.Challenges.Select(c => c.ID).ToArray();
//                        int[] regionIDs = context.Regions.Select(r => r.ID).ToArray();

//                        // ------------------------------------
//                        // Organizations
//                        // ------------------------------------
//                        string[] organizations =
//                        {
//                            "Ontario Tech Collective", "Atlantic Code Guild", "Prairie Dev Network",
//                            "Pacific AI Group", "Northern Innovators", "Maple Leaf Software",
//                            "Niagara Tech Hub", "Toronto Developers Association",
//                            "Canadian Cyber Alliance", "Cloud North", "Startup Ontario",
//                            "Women in Tech Canada", "Open Source Canada", "Future Coders",
//                            "Hack the North", "Code for Canada", "Data Science Society",
//                            "AI Canada", "DevOps Nation", "Blockchain Toronto",
//                            "Green Tech Canada", "Digital Health Alliance",
//                            "EdTech Canada", "FinTech North"
//                        };

//                        // ------------------------------------
//                        // Names
//                        // ------------------------------------
//                        string[] firstNames =
//                        {
//                            "Lyric","Antoinette","Kendal","Vivian","Ruth","Jamison","Emilia",
//                            "Natalee","Yadiel","Jakayla","Lukas","Moses","Kyler","Karla",
//                            "Chanel","Tyler","Camilla","Quintin","Braden","Clarence"
//                        };

//                        string[] lastNames = { "Watts", "Randall", "Arias", "Weber", "Stone" };

//                        // ------------------------------------
//                        // Members
//                        // ------------------------------------
//                        if (!context.Members.Any())
//                        {
//                            int memberCounter = 1;

//                            foreach (string lastName in lastNames)
//                            {
//                                HashSet<string> selectedFirstNames = new HashSet<string>();

//                                while (selectedFirstNames.Count < 2)
//                                {
//                                    selectedFirstNames.Add(firstNames[random.Next(firstNames.Length)]);
//                                }

//                                foreach (string firstName in selectedFirstNames)
//                                {
//                                    // Age 12–30 guaranteed
//                                    int age = random.Next(12, 31);
//                                    DateTime dob = DateTime.Today.AddYears(-age);

//                                    Member member = new Member
//                                    {
//                                        FirstName = firstName,
//                                        LastName = lastName,
//                                        MiddleName = lastName[1].ToString().ToUpper(),

//                                        // Guaranteed unique & valid
//                                        MemberCode = (1000000 + memberCounter).ToString(),

//                                        DOB = dob,

//                                        // Valid ranges
//                                        SkillRating = random.Next(1, 11), // 1–10
//                                        YearsExperience = random.Next(0, Math.Min(age, 21)),

//                                        Category = (memberCounter % 2 == 0) ? "J" : "A",
//                                        Organization = organizations[random.Next(organizations.Length)],
//                                        RegionID = regionIDs[random.Next(regionIDs.Length)],
//                                        ChallengeID = challengeIDs[random.Next(challengeIDs.Length)]
//                                    };

//                                    memberCounter++;

//                                    context.Members.Add(member);
//                                }
//                            }

//                            context.SaveChanges();
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.WriteLine(ex.GetBaseException().Message);
//                    }
//                }

//                #endregion

//            }

//        }
//    }
//}
