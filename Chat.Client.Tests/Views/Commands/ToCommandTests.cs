using Chat.Client.Views.Commands;
using Chat.Core.Models;
using System;
using Xunit;
using Assert = Xunit.Assert;

namespace Chat.Client.Tests.Views.Commands
{
    [Collection("Group 1")]
    public class ToCommandTests : BaseClassTests
    {
        [Fact]
        [BaseClassTests]
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
        [BaseClassTests]
        public void ExecuteTestNullComplement()
        {
            var toCommand = new ToCommand();

            var message = new Message();

            var result = toCommand.Execute(null, ref message, null);

            Assert.False(result);
            Assert.Null(message.UserDest);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNullMessage()
        {
            var toCommand = new ToCommand();

            var complement = "Carlo";
            Message message = null;

            Assert.Throws<ArgumentNullException>(() => toCommand.Execute(complement, ref message, null));
        }

    }
}