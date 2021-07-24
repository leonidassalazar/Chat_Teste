using Chat.Client.BL;
using Chat.Client.Views.Commands;
using Chat.Core.Enum;
using Chat.Core.Models;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using Xunit;
using Assert = Xunit.Assert;

namespace Chat.Client.Tests.Views.Commands
{
    public class ToCommandTests : BaseClassTests
    {
        [Fact]
        public void ExecuteTest()
        {
            var toCommand = new ToCommand();

            var complement = "Carlo";
            var message = new Message();

            var result = toCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(complement, message.UserDest);
        }

        [Fact]
        public void ExecuteTestNullComplement()
        {
            var toCommand = new ToCommand();

            var message = new Message();

            var result = toCommand.Execute(null, ref message, null);

            Assert.False(result);
            Assert.Null(message.UserDest);
        }

        [Fact]
        public void ExecuteTestNullMessage()
        {
            var toCommand = new ToCommand();

            var complement = "Carlo";
            Message message = null;

            Assert.Throws<ArgumentNullException>(() => toCommand.Execute(complement, ref message, null));
        }

    }
}