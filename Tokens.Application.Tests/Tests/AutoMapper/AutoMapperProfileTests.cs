﻿using AutoMapper;
using Tokens.Application.AutoMapper;
using Xunit;

namespace Tokens.Application.Tests.Tests.AutoMapper
{
    public class AutoMapperProfileTests
    {
        [Fact]
        public void ProfileIsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
