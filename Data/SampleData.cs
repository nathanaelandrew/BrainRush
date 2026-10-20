using BrainRush.Models.Entities;

namespace BrainRush.Data;

public static class SampleData
{
    static Question Q(int id, Category t, string prompt, int correct, params string[] opts) =>
        new() { Id = id, Topic = t, Prompt = prompt, CorrectIndex = correct, Options = opts.ToList() };

    static Player P(int id, string name, string handle, bool friend, bool you, int streak, int best, int acc,
        int mov, int mus, int spo, int gam, int gen) => new()
    {
        Id = id, Name = name, Handle = handle, IsFriend = friend, IsYou = you,
        Streak = streak, BestStreak = best, Accuracy = acc,
        Scores = new()
        {
            [Category.Movies] = mov, [Category.Music] = mus, [Category.Sports] = spo,
            [Category.Gaming] = gam, [Category.General] = gen,
        },
    };

    public static readonly List<Question> Questions = new()
    {
        Q(1, Category.Movies, "Which film was the first non-English-language winner of Best Picture?", 0, "Parasite", "Roma", "Amélie", "Life Is Beautiful"),
        Q(2, Category.Movies, "Who directed both Inception and Interstellar?", 0, "Christopher Nolan", "Denis Villeneuve", "Ridley Scott", "James Cameron"),
        Q(3, Category.Movies, "Which studio produced Toy Story, the first fully computer-animated feature?", 1, "DreamWorks", "Pixar", "Illumination", "Blue Sky"),
        Q(4, Category.Movies, "In The Matrix, which pill does Neo choose?", 2, "Blue", "Green", "Red", "Yellow"),

        Q(5, Category.Music, "Which band recorded the album Abbey Road?", 1, "The Rolling Stones", "The Beatles", "Pink Floyd", "Led Zeppelin"),
        Q(6, Category.Music, "How many strings does a standard violin have?", 1, "3", "4", "5", "6"),
        Q(7, Category.Music, "How many keys does a standard full-size piano have?", 1, "76", "88", "92", "100"),
        Q(8, Category.Music, "Who released the 2022 album Midnights?", 2, "Adele", "Billie Eilish", "Taylor Swift", "Dua Lipa"),

        Q(9, Category.Sports, "How many players per team are on court in basketball?", 1, "4", "5", "6", "7"),
        Q(10, Category.Sports, "Which country won the 2022 FIFA World Cup?", 3, "France", "Brazil", "Croatia", "Argentina"),
        Q(11, Category.Sports, "In tennis, what is a score of zero called?", 1, "Nil", "Love", "Duck", "Zip"),
        Q(12, Category.Sports, "How many rings are on the Olympic flag?", 1, "4", "5", "6", "7"),

        Q(13, Category.Gaming, "Which company created Mario?", 2, "Sega", "Sony", "Nintendo", "Atari"),
        Q(14, Category.Gaming, "Which block do you need to build a Nether portal in Minecraft?", 0, "Obsidian", "Bedrock", "Netherrack", "Diamond"),
        Q(15, Category.Gaming, "Los Santos is the main city of which game?", 1, "Watch Dogs", "Grand Theft Auto V", "Saints Row", "Mafia"),
        Q(16, Category.Gaming, "Who is the playable hero of The Legend of Zelda?", 2, "Zelda", "Ganon", "Link", "Epona"),

        Q(17, Category.General, "Which planet is known as the Red Planet?", 1, "Venus", "Mars", "Mercury", "Jupiter"),
        Q(18, Category.General, "What is the chemical symbol for gold?", 2, "Ag", "Gd", "Au", "Go"),
        Q(19, Category.General, "Which is the largest ocean on Earth?", 3, "Atlantic", "Indian", "Arctic", "Pacific"),
        Q(20, Category.General, "How many continents are there?", 2, "5", "6", "7", "8"),
    };

    public static readonly List<Player> Players = new()
    {
        P(1, "You", "you", false, true, 4, 9, 71, 820, 640, 510, 1100, 760),
        P(2, "Marisol Vega", "marisol", true, false, 7, 14, 84, 1210, 980, 450, 700, 1100),
        P(3, "Dmitri Okafor", "dmitri", true, false, 2, 8, 69, 600, 540, 1500, 620, 700),
        P(4, "Kaya Tanaka", "kaya", true, false, 11, 17, 88, 900, 1300, 380, 1200, 640),
        P(5, "Jun Reyes", "junreyes", true, false, 0, 6, 66, 700, 720, 820, 1400, 500),
        P(6, "Priya Nair", "priyan", false, false, 15, 21, 91, 1500, 1100, 900, 880, 1300),
        P(7, "Tobias Lindqvist", "tobi_l", false, false, 3, 10, 74, 800, 600, 1700, 500, 900),
        P(8, "Amara Diallo", "amara_d", false, false, 5, 12, 80, 1000, 1250, 400, 950, 1050),
        P(9, "Ren Matsuda", "renmatsuda", false, false, 1, 5, 62, 400, 300, 200, 1900, 350),
        P(10, "Lucía Ferrer", "lucia_f", false, false, 8, 13, 86, 1100, 700, 1000, 600, 1500),
    };

    public static readonly List<DailyChallenge> Challenges = new()
    {
        new() { Id = 1, Title = "Lunch-break film quiz", Topic = Category.Movies, Host = "kaya", Questions = 4, Seconds = 15,
            Invited = new() { "kaya", "marisol", "dmitri", "you" }, Finished = new() { "kaya", "marisol" } },
        new() { Id = 2, Title = "Boss-fight brain teasers", Topic = Category.Gaming, Host = "junreyes", Questions = 4, Seconds = 12,
            Invited = new() { "junreyes", "you", "renmatsuda" }, Finished = new() { "junreyes" } },
        new() { Id = 3, Title = "Mixed bag Tuesday", Topic = Category.General, Host = "marisol", Questions = 4, Seconds = 20,
            Invited = new() { "marisol", "you", "dmitri", "kaya", "junreyes" }, Finished = new() { "marisol", "dmitri", "kaya" } },
    };

    public static Player? Find(string handle) => Players.FirstOrDefault(p => p.Handle == handle);

    public static string UntilReset()
    {
        var t = DateTime.Today.AddDays(1) - DateTime.Now;
        return $"{(int)t.TotalHours}h {t.Minutes:00}m";
    }
}