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
    public class MessageCommandTests : BaseClassTests
    {
        [Fact]
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
        public void ExecuteTestNullComplement()
        {
            var messageCommand = new MessageCommand();
            
            var message = new Message();

            var result = messageCommand.Execute(null, ref message, null);

            Assert.False(result);
        }

        [Fact]
        public void ExecuteTestNullMessage()
        {
            var messageCommand = new MessageCommand();

            var complement = "Carlo";
            Message message = null;

            Assert.Throws<ArgumentNullException>(() => messageCommand.Execute(complement, ref message, null));
        }

    }
}