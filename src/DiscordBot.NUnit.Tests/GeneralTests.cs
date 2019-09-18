using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Core;
using NUnit.Framework;
using Moq;
using DiscordBot.NUnit.Tests;
using Discord.WebSocket;
using DiscordBot;
using Discord;

namespace Tests
{
    public class GeneralTests
    {
        [Test]
        public void GetUserWithSimilarNames_Test()
        {
            // Not a viable solution until it is possible to mock a SocketUser

            Mock<IGuildUser>[] testAccounts = new Mock<IGuildUser>[3];
            testAccounts[0] = MockData.CreateMockIGuildUser("test", 1);
            testAccounts[1] = MockData.CreateMockIGuildUser("tester", 2);
            testAccounts[2] = MockData.CreateMockIGuildUser("testerr", 3);
            UserAccounts.GetAccount((SocketUser)testAccounts[0].Object);
            UserAccounts.GetAccount((SocketUser)testAccounts[1].Object);
            UserAccounts.GetAccount((SocketUser)testAccounts[2].Object);

            SocketUser actual = Functions.GetUserWithSimilarName("tester");
            SocketUser expected = (SocketUser)testAccounts[1].Object;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetWhitelist_Test()
        {
            string path = @"Q:\Desktop\Programming\OVER_W4TCHER 2.0\src\DiscordBot\SystemLang\testwhitelist.txt";
            List<ulong> actual = DataStorage.LoadWhitelist(path);
            List<ulong> expected = new List<ulong>()
            {
                462346036594212874,
                186202035220250624
            };

            Assert.AreEqual(expected, actual);
        }
    }
}
