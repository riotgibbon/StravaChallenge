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

    public class WhenGettingClubSegmentAttempts : GetClub
    {
        private static int segmentId = 782179;
        private static DateTime startTime = new DateTime(2014,7,5);
        private static DateTime endTime = new DateTime(2014,7,27,23,59,59);
        private static List<SegmentEffortDTO.SegmentEffort> segmentEfforts;

        private Because of = () => segmentEfforts = Segment.Efforts(accessToken, segmentId, startTime, endTime).Result;

        private It should_have_segmentEfforts = () => segmentEfforts.ShouldNotBeNull();
        private It should_have_min_237_segmentEfforts = () => segmentEfforts.Count.ShouldBeGreaterThanOrEqualTo(237);
    }
}