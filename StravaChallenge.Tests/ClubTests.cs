using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Machine.Specifications;
using NUnit.Core;
using NUnit.Framework;
using StravaChallenge.DTO;
using Machine.Specifications;

namespace StravaChallenge.Tests
{

    public abstract class GetClub
    {
        protected static int clubId = 67033;
        protected static string accessToken = ConfigurationManager.AppSettings["acccessToken"];
        protected static List<ClubMember> clubMembers;

        private Establish context = () =>
        {
            clubMembers = Club.GetMembers(clubId, accessToken).Result;
        };

        

    }

    [Subject("Club")]
    public class WhenCallingAClub:GetClub
    {
        
        private It should_have_members = () => clubMembers.ShouldNotBeNull();
        private It should_have_at_least_51_members = () => clubMembers.Count.ShouldBeGreaterThanOrEqualTo(51);
        private It should_have_first_member_Rob = () => clubMembers[0].firstname.ShouldStartWith("Rob");
        private It should_have_first_member_Id_13187 = () => clubMembers[0].id.ShouldEqual(13187);
    }


    public class ClubSegmentEfforts : GetClub
    {
        protected static int segmentId = 782179;
        protected static DateTime startTime = new DateTime(2014, 7, 5);
        protected static DateTime endTime = new DateTime(2014, 7, 27, 23, 59, 59);
        protected static List<SegmentEffortDTO.SegmentEffort> segmentEfforts;

        protected static List<SegmentEffortClubMember> clubSegmentEfforts;

        private Establish context = () =>
        {
            segmentEfforts = Segment.Efforts(accessToken, segmentId, startTime, endTime).Result;
            clubSegmentEfforts = Segment.ClubEfforts(clubMembers, segmentEfforts);
        };
    }
    public class ClubSegmentClubEfforts: ClubSegmentEfforts
    {
        private It should_have_segmentEfforts = () => segmentEfforts.ShouldNotBeNull();
        private It should_have_min_237_segmentEfforts = () => segmentEfforts.Count.ShouldBeGreaterThanOrEqualTo(237);

        private It should_have_club_segment_efforts = () => clubSegmentEfforts.ShouldNotBeNull();

        private It should_have_min_27_club_segment_efforts =
            () => clubSegmentEfforts.Count.ShouldBeGreaterThanOrEqualTo(27);

        private It should_have_Someone_as_first_attempt =
            () => clubSegmentEfforts[0].ClubMember.firstname.ShouldEqual("Rob");
    }

    public class BestClubSegmentClubEfforts : ClubSegmentEfforts
    {
        protected static List<SegmentEffortClubMember> bestClubSegmentEfforts;
        protected static List<SegmentEffortClubMember> maleEfforts;
        protected static List<SegmentEffortClubMember> femaleEfforts;

        private Because of = () =>
        {
            bestClubSegmentEfforts = Segment.BestClubSegmentEfforts(clubMembers, clubSegmentEfforts);
            maleEfforts = Segment.GenderSegmentEfforts(bestClubSegmentEfforts, "M");
            femaleEfforts = Segment.GenderSegmentEfforts(bestClubSegmentEfforts, "F");
        };

        private It should_have_minimum_time_of_74_seconds =
            () => bestClubSegmentEfforts[0].SegmentEffort.elapsed_time.ShouldBeLessThanOrEqualTo(74);

        private It should_have_5_total_efforts = () => bestClubSegmentEfforts.Count.ShouldEqual(15);

        private It should_have_a_lot_of_blokes = () => maleEfforts.Count.ShouldBeGreaterThanOrEqualTo(15);
        private It should_have_less_women = () => femaleEfforts.Count.ShouldBeGreaterThanOrEqualTo(0);
    }

}