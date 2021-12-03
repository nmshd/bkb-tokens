using System;
using AutoFixture;
using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Moq;
using Tokens.Application.AutoMapper;
using Tokens.Application.Infrastructure;
using Tokens.Application.Tests.AutoFixture;
using Tokens.Infrastructure.Persistence.Database;

namespace Tokens.Application.Tests.Tests.Tokens
{
    public abstract class HandlerTestsBase
    {
        protected static readonly IdentityAddress ActiveIdentity = IdentityAddress.Parse("activeidentity");
        protected static readonly DateTime DateTimeNow = DateTime.UtcNow;
        protected readonly ApplicationDbContext _actContext;

        protected readonly ApplicationDbContext _arrangeContext;
        protected readonly ApplicationDbContext _assertionContext;
        protected readonly IMapper _mapper;
        protected readonly Mock<IUserContext> _userContextMock;
        protected Fixture _fixture;
        protected Mock<ITokenRepository> _tokenRepositoryMock;
        protected IUnitOfWork _unitOfWork;

        protected HandlerTestsBase()
        {
            SystemTime.Set(DateTimeNow);

            _fixture = new CustomFixture();

            SetupUnitOfWork();

            _userContextMock = new Mock<IUserContext>();
            _userContextMock.Setup(s => s.GetAddress()).Returns(ActiveIdentity);

            _mapper = AutoMapperProfile.CreateMapper();
        }

        private void SetupUnitOfWork()
        {
            _tokenRepositoryMock = new Mock<ITokenRepository>();

            _unitOfWork = new FakeUnitOfWork(_tokenRepositoryMock.Object);
        }
    }
}
