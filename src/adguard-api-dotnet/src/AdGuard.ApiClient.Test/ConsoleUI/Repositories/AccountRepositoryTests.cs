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
/// Unit tests for <see cref="AccountRepository"/>.
/// </summary>
public class AccountRepositoryTests : RepositoryTestBase<AccountRepository>
{
    /// <inheritdoc />
    protected override AccountRepository CreateRepository()
    {
        return new AccountRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override AccountRepository CreateRepositoryWithNullFactory()
    {
        return new AccountRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override AccountRepository CreateRepositoryWithNullLogger()
    {
        return new AccountRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetLimitsAsync Tests

    #endregion
}
