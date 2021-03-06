﻿using System;
using System.Diagnostics;
using Castle.Core.Internal;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.Configuration;

namespace TestStack.Seleno.AcceptanceTests.Configuration
{
    [TestFixture]
    class ProcessLifecycleTests
    {
        private const string Chrome = "chromedriver";
        private const string IE = "IEDriverServer";
        private const string Phantom = "phantomjs";
        private const string Firefox = "firefox";
        private const string IisExpress = "iisexpress";

        [TestCase(Chrome)]
        [TestCase(IE)]
        [TestCase(Phantom)]
        [TestCase(Firefox)]
        public void Closing_SelenoHost_should_close_child_browser(string driverName)
        {
            Process.GetProcessesByName(driverName).ForEach(StopProcess);
            var selenoHost = new SelenoHost();
            Func<RemoteWebDriver> driver = GetBrowserFactory(driverName);
            selenoHost.Run("TestStack.Seleno.AcceptanceTests.Web", 12346,
                c => c.WithRemoteWebDriver(driver));
            Process.GetProcessesByName(driverName).Length.Should().Be(1);

            selenoHost.Dispose();

            Process.GetProcessesByName(driverName).Should().BeEmpty();
        }

        [Test]
        public void Closing_SelenoHost_should_close_Iis_Express()
        {
            Process.GetProcessesByName(IisExpress).ForEach(StopProcess);
            Process.GetProcessesByName("chromedriver").ForEach(StopProcess);

            var selenoHost = new SelenoHost();
            selenoHost.Run("TestStack.Seleno.AcceptanceTests.Web", 12346,
                c => c.WithRemoteWebDriver(BrowserFactory.Chrome));
            Process.GetProcessesByName(IisExpress).Length.Should().Be(1);

            selenoHost.Dispose();

            Process.GetProcessesByName("chromedriver").Should().BeEmpty();
            Process.GetProcessesByName(IisExpress).Should().BeEmpty();
        }

        private void StopProcess(Process process)
        {
            if (process == null)
                return;
            if (!process.HasExited)
                process.Kill();
            process.Dispose();
        }

        private Func<RemoteWebDriver> GetBrowserFactory(string browser)
        {
            switch (browser)
            {
                case Chrome:
                    return BrowserFactory.Chrome;
                case IE:
                    return BrowserFactory.InternetExplorer;
                case Phantom:
                    return BrowserFactory.PhantomJS;
                case Firefox:
                    return BrowserFactory.FireFox;
            }
            return null;
        }
    }
}
