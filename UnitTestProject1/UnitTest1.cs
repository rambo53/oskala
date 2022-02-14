using Microsoft.VisualStudio.TestTools.UnitTesting;
using OskalaAbo.Controllers;
using OskalaAbo.Models;
using System;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var User = new AContact();
            User.mail = "stef@gmail.com";

            var UserSubscriber = new UserSubscriberController();
            var result = UserSubscriber.validMail(User.mail);

            Assert.IsFalse(result.Result.isSuccess);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var User = new AContact();
            User.mail = "eni@gmail.com";

            var UserSubscriber = new UserSubscriberController();
            var result = UserSubscriber.validMail(User.mail);

            Assert.IsTrue(result.Result.isSuccess);
        }
    }
}
