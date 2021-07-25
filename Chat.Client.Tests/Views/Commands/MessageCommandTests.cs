using Chat.Client.Views.Commands;
using Chat.Core.Models;
using System;
using Xunit;
using Assert = Xunit.Assert;

namespace Chat.Client.Tests.Views.Commands
{
    [Collection("Group 1")]
    public class MessageCommandTests : BaseClassTests
    {
        [Fact]
        [BaseClassTests]
        public void ExecuteTest()
        {
            var messageCommand = new MessageCommand();

            var complement = "Olá, galera";
            var message = new Message();

            var result = messageCommand.Execute(complement, ref message, null);

            Assert.True(result);
            Assert.Equal(complement, message.Text);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNullComplement()
        {
            var messageCommand = new MessageCommand();

            var message = new Message();

            var result = messageCommand.Execute(null, ref message, null);

            Assert.False(result);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNullMessage()
        {
            var messageCommand = new MessageCommand();

            var complement = "Carlo";
            Message message = null;

            Assert.Throws<ArgumentNullException>(() => messageCommand.Execute(complement, ref message, null));
        }

    }
}