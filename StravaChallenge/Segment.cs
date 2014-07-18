using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using StravaChallenge.DTO;

namespace StravaChallenge
{
    public class Segment
    {
        public static async Task<List<SegmentEffortDTO.SegmentEffort>> Efforts(string accessToken, int segmentId, DateTime startTime, DateTime endTime)
        {
            var pageSize = 200;
            var page = 1;
            var allSegments = new  List<SegmentEffortDTO.SegmentEffort>();
            bool gotAllSegments = false;
            while (!gotAllSegments)
            {
                var segments = await GetSegmentsPage(accessToken, pageSize, page);
                allSegments = allSegments.Union(segments).ToList();
                gotAllSegments = segments.Count < pageSize;
                page++;
            }


            return allSegments;
        }

        private static async Task<List<SegmentEffortDTO.SegmentEffort>> GetSegmentsPage(string accessToken, int pageSize, int page)
        {
            var segments =
                await
                    HttpTools.GetHttpResponseAsync<List<SegmentEffortDTO.SegmentEffort>>(
                        string.Format(
                            "https://www.strava.com/api/v3/segments/782179/all_efforts?start_date_local=2014-07-05T00:00:00Z&end_date_local=2014-07-27T23:59:59Z&per_page={0}&page={1}",
                            pageSize, page), accessToken);
            return segments;
        }

        public static List<SegmentEffortClubMember> ClubEfforts(IEnumerable<ClubMember> clubMembers, IEnumerable<SegmentEffortDTO.SegmentEffort> clubSegmentEfforts)
        {
            var clubEfforts = from e in clubSegmentEfforts
                join m in clubMembers on e.athlete.id equals m.id
                select new SegmentEffortClubMember
                {
                    ClubMember = m,
                    SegmentEffort = e
                };
            return clubEfforts.ToList();


        }

        public static List<SegmentEffortClubMember> BestClubSegmentEfforts(IEnumerable<ClubMember> clubMembers, List<SegmentEffortClubMember> clubSegmentEfforts)
        {
            var efforts = clubSegmentEfforts.Select(s => s.SegmentEffort);
          
            var bestEfforts =
                efforts.GroupBy(e => e.athlete.id).Select(e => e.OrderBy(f => f.elapsed_time)).Select(e => e.First()).OrderBy(e=>e.elapsed_time);
           
            return ClubEfforts(clubMembers, bestEfforts);
        }

        public static List<SegmentEffortClubMember> GenderSegmentEfforts(List<SegmentEffortClubMember> bestClubSegmentEfforts, string s)
        {
            return bestClubSegmentEfforts.Where(e => e.ClubMember.sex == s).ToList();
        }
    }
}
