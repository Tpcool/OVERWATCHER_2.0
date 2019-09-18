using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using Moq;

namespace DiscordBot.NUnit.Tests
{
    class MockData
    {
        public static Mock<IGuildUser> CreateMockIGuildUser(string username)
        {
            var user = new Mock<IGuildUser>();
            user.Setup(u => u.Username).Returns(username);
            return user;
        }

        public static Mock<IGuildUser> CreateMockIGuildUser(string username, ulong id)
        {
            var user = new Mock<IGuildUser>();
            user.Setup(u => u.Username).Returns(username);
            user.Setup(u => u.Id).Returns(id);
            return user;
        }

        public static Mock<UserAccount> CreateMockUserAccount(ulong id, int currency = 0, bool imaginary = false)
        {
            var user = new Mock<UserAccount>();
            user.Setup(u => u.Id).Returns(id);
            user.Setup(u => u.Currency).Returns(currency);
            user.Setup(u => u.IsCurrencyImaginary).Returns(imaginary);
            return user;
        }
    }
}
