using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Core;
using NUnit.Framework;
using Moq;
using DiscordBot.NUnit.Tests;
using Discord.WebSocket;
using DiscordBot;

namespace Tests
{
    public class GeneralTests
    {
        [Test]
        public void GetUserWithSimilarNames_Test()
        {
            // Not a viable solution until it is possible to mock a SocketUser
            //Mock<SocketUser>[] testAccounts = new Mock<SocketUser>[3];
            //testAccounts[0] = MockData.CreateMockSocketUser("test", 1);
            //testAccounts[1] = MockData.CreateMockSocketUser("tester", 2);
            //testAccounts[2] = MockData.CreateMockSocketUser("testerr", 3);
            //UserAccounts.GetAccount(testAccounts[0].Object);
            //UserAccounts.GetAccount(testAccounts[1].Object);
            //UserAccounts.GetAccount(testAccounts[2].Object);

            //SocketUser actual = Functions.GetUserWithSimilarName("tester");
            //SocketUser expected = testAccounts[1].Object;

            //Assert.AreEqual(expected, actual);
        }
    }
}
