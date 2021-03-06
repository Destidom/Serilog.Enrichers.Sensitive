﻿using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using Xunit;

namespace Serilog.Enrichers.Sensitive.Tests.Unit
{
    public class WhenMaskingLogEventPropertyGlobally
    {
        [Fact]
        public void GivenNotInSensitiveArea_EmailAddressIsNotMasked()
        {
            _logger.Information("{Prop}","test@email.com");

            InMemorySink.Instance
                .Should()
                .HaveMessage("{Prop}")
                .Appearing().Once()
                .WithProperty("Prop")
                .WithValue("***MASKED***");
        }

        [Fact]
        public void GivenNotInSensitiveArea_IbanIsNotMasked()
        {
            _logger.Information("{Prop}", "NL02ABNA0123456789");

            InMemorySink.Instance
                .Should()
                .HaveMessage("{Prop}")
                .Appearing().Once()
                .WithProperty("Prop")
                .WithValue("***MASKED***");
        }

        [Fact]
        public void GivenInSensitiveArea_EmailAddressIsMasked()
        {
            using (_logger.EnterSensitiveArea())
            {
                _logger.Information("{Prop}", "test@email.com");
            }

            InMemorySink.Instance
                .Should()
                .HaveMessage("{Prop}")
                .Appearing().Once()
                .WithProperty("Prop")
                .WithValue("***MASKED***");
        }

        [Fact]
        public void GivenInSensitiveArea_IbanIsMasked()
        {
            using (_logger.EnterSensitiveArea())
            {
                _logger.Information("{Prop}", "NL02ABNA0123456789");
            }

            InMemorySink.Instance
                .Should()
                .HaveMessage("{Prop}")
                .Appearing().Once()
                .WithProperty("Prop")
                .WithValue("***MASKED***");
        }

        [Fact]
        public void GivenMultiplePropertiesToMask_AllPropertiesAreMasked()
        {
            _logger.Information("{PropA}, {PropB}, {PropC}", "foo@bar.net", "foo@bar.net", "foo@bar.nets");

            InMemorySink.Instance
                .Should()
                .HaveMessage("{PropA}, {PropB}, {PropC}")
                .Appearing().Once()
                .WithProperty("PropA").WithValue("***MASKED***");

            InMemorySink.Instance
                .Should()
                .HaveMessage("{PropA}, {PropB}, {PropC}")
                .Appearing().Once()
                .WithProperty("PropB").WithValue("***MASKED***");

            InMemorySink.Instance
                .Should()
                .HaveMessage("{PropA}, {PropB}, {PropC}")
                .Appearing().Once()
                .WithProperty("PropC").WithValue("***MASKED***");
        }

        private readonly ILogger _logger;

        public WhenMaskingLogEventPropertyGlobally()
        {
            _logger = new LoggerConfiguration()
                .Enrich.WithSensitiveDataMasking()
                .WriteTo.InMemory()
                .CreateLogger();
        }
    }
}
