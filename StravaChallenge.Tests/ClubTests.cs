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


    public class SegmentAttempts : GetClub
    {
        protected static int segmentId = 782179;
        protected static DateTime startTime = new DateTime(2014, 7, 5);
        protected static DateTime endTime = new DateTime(2014, 7, 27, 23, 59, 59);
        protected static List<SegmentEffortDTO.SegmentEffort> segmentEfforts;
        private Establish context = () =>
        {
            segmentEfforts = Segment.Efforts(accessToken, segmentId, startTime, endTime).Result;
        };
        protected static List<SegmentEffortDTO.SegmentEffort> clubSegmentEfforts;
        private Because of = () => clubSegmentEfforts = Segment.ClubEfforts(clubMembers, segmentEfforts);

        private It should_have_segmentEfforts = () => segmentEfforts.ShouldNotBeNull();
        private It should_have_min_237_segmentEfforts = () => segmentEfforts.Count.ShouldBeGreaterThanOrEqualTo(237);

        private It should_have_club_segment_efforts = () => clubSegmentEfforts.ShouldNotBeNull();

        private It should_have_min_27_club_segment_efforts =
            () => clubSegmentEfforts.Count.ShouldBeGreaterThanOrEqualTo(27);
    }

  

}