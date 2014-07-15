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
    [Subject("Club")]
    public class WhenCallingAClub
    {
        static int clubId = 67033;
        static string accessToken = ConfigurationManager.AppSettings["acccessToken"];
        private static List<ClubMember> clubMembers;

        private Establish context = () =>
        {

        };

        private Because of =  () =>
        {
            clubMembers = Club.GetMembers(clubId, accessToken).Result;
        };

        private It should_have_members = () => clubMembers.ShouldNotBeNull();
        private It should_have_at_least_51_members = () => clubMembers.Count.ShouldBeGreaterThanOrEqualTo(51);
        private It should_have_first_member_Rob = () => clubMembers[0].firstname.ShouldStartWith("Rob");
        private It should_have_first_member_Id_13187 = () => clubMembers[0].id.ShouldEqual(13187);
    }
}