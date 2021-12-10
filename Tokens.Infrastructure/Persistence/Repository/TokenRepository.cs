﻿using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Tokens.Application.Infrastructure;
using Tokens.Domain.Entities;
using Tokens.Infrastructure.Persistence.Database;

namespace Tokens.Infrastructure.Persistence.Repository;

public class TokenRepository : ITokenRepository
{
    private readonly IBlobStorage _blobStorage;
    private readonly IQueryable<Token> _readonlyTokensDbSet;
    private readonly DbSet<Token> _tokensDbSet;

    public TokenRepository(ApplicationDbContext dbContext, IBlobStorage blobStorage)
    {
        _blobStorage = blobStorage;
        _tokensDbSet = dbContext.Tokens;
        _readonlyTokensDbSet = dbContext.Tokens.AsNoTracking();
    }

    public async Task<Token> Find(TokenId id)
    {
        var getMetadata = _readonlyTokensDbSet
            .Where(Token.IsNotExpired)
            .FirstWithId(id);

        var getContent = _blobStorage.FindAsync(id);

        await Task.WhenAll(getMetadata, getContent);

        var token = await getMetadata;

        if (token == null)
            throw new NotFoundException(nameof(Token));

        token.Content = await getContent;

        return token;
    }

    public async Task<FindTokensResult> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter)
    {
        return await Find(null, ids, paginationFilter);
    }

    public async Task<FindTokensResult> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter)
    {
        return await Find(owner, Array.Empty<TokenId>(), paginationFilter);
    }

    public async Task<IdentityAddress> GetOwner(TokenId tokenId)
    {
        var result = await _readonlyTokensDbSet.Select(t => new {t.CreatedBy, t.Id}).FirstOrDefaultAsync(t => t.Id == tokenId);

        if (result == null)
            throw new NotFoundException(nameof(Token));

        return result.CreatedBy;
    }

    private async Task<FindTokensResult> Find(IdentityAddress owner, IEnumerable<TokenId> ids, PaginationFilter paginationFilter)
    {
        if (paginationFilter == null)
            throw new Exception("A pagination filter has to be provided.");

        var query = _readonlyTokensDbSet.Where(Token.IsNotExpired);

        if (ids.Any())
            query = query.Where(t => ids.Contains(t.Id));

        if (owner != null)
            query = query.Where(t => t.CreatedBy == owner);

        var totalNumberOfItems = await query.CountAsync();

        var tokens = await query
            .OrderBy(t => t.CreatedAt)
            .Paged(paginationFilter)
            .ToListAsync();

        await FillContent(tokens);

        return new FindTokensResult(totalNumberOfItems, tokens);
    }

    public async Task<int> GetNumberOfTokensOfOwner(IdentityAddress owner)
    {
        var numberOfTokens = await _readonlyTokensDbSet
            .Where(t => t.CreatedBy == owner)
            .CountAsync();

        return numberOfTokens;
    }

    private async Task FillContent(IEnumerable<Token> tokens)
    {
        var fillContentTasks = tokens.Select(FillContent);
        await Task.WhenAll(fillContentTasks);
    }

    private async Task FillContent(Token token)
    {
        token.Content = await _blobStorage.FindAsync(token.Id);
    }

    #region Write

    public void Add(Token token)
    {
        _tokensDbSet.Add(token);
        _blobStorage.Add(token.Id, token.Content);
    }

    public void AddRange(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            Add(token);
        }
    }

    public void Remove(TokenId id)
    {
        throw new NotImplementedException();
    }

    public void Remove(Token token)
    {
        _tokensDbSet.Remove(token);
        _blobStorage.Remove(token.Id);
    }

    public void RemoveRange(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            Remove(token);
        }
    }

    public void Update(Token entity)
    {
        _tokensDbSet.Update(entity);
    }

    #endregion
}

public static class IDbSetExtensions
{
    public static async Task<Token> FirstWithId(this IQueryable<Token> query, TokenId id)
    {
        var entity = await query.FirstOrDefaultAsync(t => t.Id == id);

        if (entity == null)
            throw new NotFoundException(nameof(Token));

        return entity;
    }
}