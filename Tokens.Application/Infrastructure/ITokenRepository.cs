using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Tokens.Domain.Entities;

namespace Tokens.Application.Infrastructure;

public interface ITokenRepository : IRepository<Token, TokenId>
{
    Task<FindTokensResult> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter);
    Task<FindTokensResult> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter);
    Task<IdentityAddress> GetOwner(TokenId tokenId);
}

public record FindTokensResult(int TotalRecords, IEnumerable<Token> Tokens);