using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using StravaChallenge.DTO;

namespace StravaChallenge
{
    public class Club
    {
        public static async Task<List<ClubMember>> GetMembers(int clubId, string accessToken)
        {
            var members =
                await
                    HttpTools.GetHttpResponseAsync<List<ClubMember>>(
                        GetClubRequestUri(clubId), accessToken);
            return members;
        }

        private static string GetClubRequestUri(int clubId)
        {
            return string.Format("https://www.strava.com/api/v3/clubs/{0}/members?per_page=200", clubId);
        }
    }
}