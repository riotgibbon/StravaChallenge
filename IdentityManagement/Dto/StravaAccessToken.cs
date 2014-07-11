﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Dto
{
    public class StravaAccessToken
    {
        public string access_token { get; set; }
        public Athlete athlete { get; set; }
    }

    public class Athlete
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
        public int follower_count { get; set; }
        public int friend_count { get; set; }
        public int mutual_friend_count { get; set; }
        public string date_preference { get; set; }
        public string measurement_preference { get; set; }
        public string email { get; set; }
        public List<object> clubs { get; set; }
        public List<object> bikes { get; set; }
        public List<object> shoes { get; set; }
    }

    public class StravaAccessTokenInfo : StravaAccessToken
    {
        public string mail { get; set; }
        public string uid { get; set; }
        public string cn { get; set; }
    }

}
