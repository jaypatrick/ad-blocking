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
/// Unit tests for <see cref="StatisticsRepository"/>.
/// </summary>
public class StatisticsRepositoryTests : RepositoryTestBase<StatisticsRepository>
{
    /// <inheritdoc />
    protected override StatisticsRepository CreateRepository()
    {
        return new StatisticsRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override StatisticsRepository CreateRepositoryWithNullFactory()
    {
        return new StatisticsRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override StatisticsRepository CreateRepositoryWithNullLogger()
    {
        return new StatisticsRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetTimeQueriesStatsAsync Tests

    #endregion
}
