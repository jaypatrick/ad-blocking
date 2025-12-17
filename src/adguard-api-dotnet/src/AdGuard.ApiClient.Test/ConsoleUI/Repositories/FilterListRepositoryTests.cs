using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.Repositories.Implementations;
using AdGuard.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="FilterListRepository"/>.
/// </summary>
public class FilterListRepositoryTests : RepositoryTestBase<FilterListRepository>
{
    /// <inheritdoc />
    protected override FilterListRepository CreateRepository()
    {
        return new FilterListRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override FilterListRepository CreateRepositoryWithNullFactory()
    {
        return new FilterListRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override FilterListRepository CreateRepositoryWithNullLogger()
    {
        return new FilterListRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetAllAsync Tests

    #endregion
}
