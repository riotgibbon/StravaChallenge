namespace StravaChallenge.DTO
{
    public class ClubMember
    {
       
            public int id { get; set; }
            public int resource_state { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string profile_medium { get; set; }
            public string profile { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string sex { get; set; }
            public object friend { get; set; }
            public object follower { get; set; }
            public bool premium { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public bool approve_followers { get; set; }
       
    }
}