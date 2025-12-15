/*
 * AdGuard DNS API - Unit Tests
 *
 * Tests for DateTimeExtensions class
 */

using System;
using AdGuard.ApiClient.Helpers;
using Xunit;

namespace AdGuard.ApiClient.Test.Helpers
{
    /// <summary>
    /// Unit tests for <see cref="DateTimeExtensions"/> class.
    /// </summary>
    public class DateTimeExtensionsTests
    {
        #region ToUnixMilliseconds (DateTime) Tests

        [Fact]
        public void ToUnixMilliseconds_UnixEpoch_ReturnsZero()
        {
            // Arrange
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = epoch.ToUnixMilliseconds();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ToUnixMilliseconds_KnownDate_ReturnsCorrectTimestamp()
        {
            // Arrange - January 1, 2024, 00:00:00 UTC
            var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedTimestamp = 1704067200000L; // Known value

            // Act
            var result = date.ToUnixMilliseconds();

            // Assert
            Assert.Equal(expectedTimestamp, result);
        }

        [Fact]
        public void ToUnixMilliseconds_LocalTime_ConvertsToUtc()
        {
            // Arrange
            var utcDate = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);
            var expectedTimestamp = utcDate.ToUnixMilliseconds();

            // Act - Create same moment in local time (this test is timezone-dependent but verifies conversion)
            var localDate = utcDate.ToLocalTime();
            var localResult = localDate.ToUnixMilliseconds();

            // Assert - Both should produce same timestamp
            Assert.Equal(expectedTimestamp, localResult);
        }

        [Fact]
        public void ToUnixMilliseconds_WithMilliseconds_IncludesMilliseconds()
        {
            // Arrange
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 500, DateTimeKind.Utc); // 500ms after epoch

            // Act
            var result = date.ToUnixMilliseconds();

            // Assert
            Assert.Equal(500, result);
        }

        #endregion

        #region ToUnixMilliseconds (DateTimeOffset) Tests

        [Fact]
        public void ToUnixMilliseconds_DateTimeOffset_UnixEpoch_ReturnsZero()
        {
            // Arrange
            var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = epoch.ToUnixMilliseconds();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ToUnixMilliseconds_DateTimeOffset_WithOffset_ReturnsCorrectTimestamp()
        {
            // Arrange - Same moment expressed with different offsets
            var utcMoment = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
            var offsetMoment = new DateTimeOffset(2024, 1, 1, 7, 0, 0, TimeSpan.FromHours(-5));

            // Act
            var utcResult = utcMoment.ToUnixMilliseconds();
            var offsetResult = offsetMoment.ToUnixMilliseconds();

            // Assert - Same moment should have same timestamp
            Assert.Equal(utcResult, offsetResult);
        }

        #endregion

        #region FromUnixMilliseconds Tests

        [Fact]
        public void FromUnixMilliseconds_Zero_ReturnsUnixEpoch()
        {
            // Arrange
            var expectedEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = DateTimeExtensions.FromUnixMilliseconds(0);

            // Assert
            Assert.Equal(expectedEpoch, result);
        }

        [Fact]
        public void FromUnixMilliseconds_KnownTimestamp_ReturnsCorrectDate()
        {
            // Arrange
            var timestamp = 1704067200000L; // January 1, 2024, 00:00:00 UTC
            var expectedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = DateTimeExtensions.FromUnixMilliseconds(timestamp);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void FromUnixMilliseconds_RoundTrip_PreservesValue()
        {
            // Arrange
            var originalDate = new DateTime(2024, 6, 15, 14, 30, 45, 123, DateTimeKind.Utc);

            // Act
            var timestamp = originalDate.ToUnixMilliseconds();
            var roundTripDate = DateTimeExtensions.FromUnixMilliseconds(timestamp);

            // Assert
            Assert.Equal(originalDate, roundTripDate);
        }

        #endregion

        #region FromUnixMillisecondsToOffset Tests

        [Fact]
        public void FromUnixMillisecondsToOffset_Zero_ReturnsUnixEpoch()
        {
            // Arrange
            var expectedEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = DateTimeExtensions.FromUnixMillisecondsToOffset(0);

            // Assert
            Assert.Equal(expectedEpoch, result);
        }

        [Fact]
        public void FromUnixMillisecondsToOffset_ReturnsUtcOffset()
        {
            // Act
            var result = DateTimeExtensions.FromUnixMillisecondsToOffset(1000);

            // Assert
            Assert.Equal(TimeSpan.Zero, result.Offset);
        }

        #endregion

        #region Now Tests

        [Fact]
        public void Now_ReturnsCurrentTime()
        {
            // Arrange
            var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Act
            var result = DateTimeExtensions.Now();

            // Assert
            var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Assert.InRange(result, before, after);
        }

        [Fact]
        public void Now_IsPositive()
        {
            // Act
            var result = DateTimeExtensions.Now();

            // Assert
            Assert.True(result > 0);
        }

        #endregion

        #region FromNow Tests

        [Fact]
        public void FromNow_PositiveTimeSpan_ReturnsFutureTimestamp()
        {
            // Arrange
            var now = DateTimeExtensions.Now();
            var oneHour = TimeSpan.FromHours(1);

            // Act
            var result = DateTimeExtensions.FromNow(oneHour);

            // Assert
            Assert.True(result > now);
            // Allow for small timing differences
            var expectedDiff = oneHour.TotalMilliseconds;
            Assert.InRange(result - now, expectedDiff - 1000, expectedDiff + 1000);
        }

        [Fact]
        public void FromNow_NegativeTimeSpan_ReturnsPastTimestamp()
        {
            // Arrange
            var now = DateTimeExtensions.Now();
            var oneHourAgo = TimeSpan.FromHours(-1);

            // Act
            var result = DateTimeExtensions.FromNow(oneHourAgo);

            // Assert
            Assert.True(result < now);
        }

        [Fact]
        public void FromNow_ZeroTimeSpan_ReturnsApproximatelyNow()
        {
            // Arrange
            var now = DateTimeExtensions.Now();

            // Act
            var result = DateTimeExtensions.FromNow(TimeSpan.Zero);

            // Assert - Should be within 100ms of now
            Assert.InRange(Math.Abs(result - now), 0, 100);
        }

        #endregion

        #region DaysAgo Tests

        [Fact]
        public void DaysAgo_PositiveValue_ReturnsPastTimestamp()
        {
            // Arrange
            var now = DateTimeExtensions.Now();
            var expectedDiff = TimeSpan.FromDays(7).TotalMilliseconds;

            // Act
            var result = DateTimeExtensions.DaysAgo(7);

            // Assert
            Assert.True(result < now);
            Assert.InRange(now - result, expectedDiff - 1000, expectedDiff + 1000);
        }

        [Fact]
        public void DaysAgo_NegativeValue_TreatedAsPositive()
        {
            // Act
            var positiveResult = DateTimeExtensions.DaysAgo(7);
            var negativeResult = DateTimeExtensions.DaysAgo(-7);

            // Assert - Both should return the same value
            Assert.InRange(Math.Abs(positiveResult - negativeResult), 0, 100);
        }

        [Fact]
        public void DaysAgo_Zero_ReturnsApproximatelyNow()
        {
            // Arrange
            var now = DateTimeExtensions.Now();

            // Act
            var result = DateTimeExtensions.DaysAgo(0);

            // Assert
            Assert.InRange(Math.Abs(result - now), 0, 100);
        }

        #endregion

        #region HoursAgo Tests

        [Fact]
        public void HoursAgo_PositiveValue_ReturnsPastTimestamp()
        {
            // Arrange
            var now = DateTimeExtensions.Now();
            var expectedDiff = TimeSpan.FromHours(24).TotalMilliseconds;

            // Act
            var result = DateTimeExtensions.HoursAgo(24);

            // Assert
            Assert.True(result < now);
            Assert.InRange(now - result, expectedDiff - 1000, expectedDiff + 1000);
        }

        [Fact]
        public void HoursAgo_NegativeValue_TreatedAsPositive()
        {
            // Act
            var positiveResult = DateTimeExtensions.HoursAgo(6);
            var negativeResult = DateTimeExtensions.HoursAgo(-6);

            // Assert
            Assert.InRange(Math.Abs(positiveResult - negativeResult), 0, 100);
        }

        #endregion

        #region MinutesAgo Tests

        [Fact]
        public void MinutesAgo_PositiveValue_ReturnsPastTimestamp()
        {
            // Arrange
            var now = DateTimeExtensions.Now();
            var expectedDiff = TimeSpan.FromMinutes(30).TotalMilliseconds;

            // Act
            var result = DateTimeExtensions.MinutesAgo(30);

            // Assert
            Assert.True(result < now);
            Assert.InRange(now - result, expectedDiff - 1000, expectedDiff + 1000);
        }

        [Fact]
        public void MinutesAgo_NegativeValue_TreatedAsPositive()
        {
            // Act
            var positiveResult = DateTimeExtensions.MinutesAgo(15);
            var negativeResult = DateTimeExtensions.MinutesAgo(-15);

            // Assert
            Assert.InRange(Math.Abs(positiveResult - negativeResult), 0, 100);
        }

        #endregion

        #region StartOfToday Tests

        [Fact]
        public void StartOfToday_ReturnsStartOfUtcDay()
        {
            // Act
            var result = DateTimeExtensions.StartOfToday();
            var resultDateTime = DateTimeExtensions.FromUnixMilliseconds(result);

            // Assert
            Assert.Equal(0, resultDateTime.Hour);
            Assert.Equal(0, resultDateTime.Minute);
            Assert.Equal(0, resultDateTime.Second);
            Assert.Equal(0, resultDateTime.Millisecond);
        }

        [Fact]
        public void StartOfToday_IsBeforeNow()
        {
            // Arrange
            var now = DateTimeExtensions.Now();

            // Act
            var result = DateTimeExtensions.StartOfToday();

            // Assert
            Assert.True(result <= now);
        }

        #endregion

        #region EndOfToday Tests

        [Fact]
        public void EndOfToday_ReturnsEndOfUtcDay()
        {
            // Act
            var result = DateTimeExtensions.EndOfToday();
            var resultDateTime = DateTimeExtensions.FromUnixMilliseconds(result);

            // Assert
            Assert.Equal(23, resultDateTime.Hour);
            Assert.Equal(59, resultDateTime.Minute);
            Assert.Equal(59, resultDateTime.Second);
            Assert.Equal(999, resultDateTime.Millisecond);
        }

        [Fact]
        public void EndOfToday_IsAfterStartOfToday()
        {
            // Act
            var start = DateTimeExtensions.StartOfToday();
            var end = DateTimeExtensions.EndOfToday();

            // Assert
            Assert.True(end > start);
        }

        [Fact]
        public void EndOfToday_DifferenceFromStartIsOneDayMinusOneMs()
        {
            // Arrange
            var expectedDiff = TimeSpan.FromDays(1).TotalMilliseconds - 1;

            // Act
            var start = DateTimeExtensions.StartOfToday();
            var end = DateTimeExtensions.EndOfToday();

            // Assert
            Assert.Equal(expectedDiff, end - start);
        }

        #endregion

        #region StartOfDay Tests

        [Fact]
        public void StartOfDay_SpecificDate_ReturnsCorrectTimestamp()
        {
            // Arrange
            var date = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc);
            var expectedStart = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = DateTimeExtensions.StartOfDay(date);
            var resultDateTime = DateTimeExtensions.FromUnixMilliseconds(result);

            // Assert
            Assert.Equal(expectedStart.Year, resultDateTime.Year);
            Assert.Equal(expectedStart.Month, resultDateTime.Month);
            Assert.Equal(expectedStart.Day, resultDateTime.Day);
            Assert.Equal(0, resultDateTime.Hour);
            Assert.Equal(0, resultDateTime.Minute);
        }

        #endregion

        #region EndOfDay Tests

        [Fact]
        public void EndOfDay_SpecificDate_ReturnsCorrectTimestamp()
        {
            // Arrange
            var date = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc);

            // Act
            var result = DateTimeExtensions.EndOfDay(date);
            var resultDateTime = DateTimeExtensions.FromUnixMilliseconds(result);

            // Assert
            Assert.Equal(2024, resultDateTime.Year);
            Assert.Equal(6, resultDateTime.Month);
            Assert.Equal(15, resultDateTime.Day);
            Assert.Equal(23, resultDateTime.Hour);
            Assert.Equal(59, resultDateTime.Minute);
            Assert.Equal(59, resultDateTime.Second);
            Assert.Equal(999, resultDateTime.Millisecond);
        }

        #endregion

        #region FormatAsIso8601 Tests

        [Fact]
        public void FormatAsIso8601_KnownTimestamp_ReturnsCorrectFormat()
        {
            // Arrange
            var date = new DateTime(2024, 1, 15, 12, 30, 45, 123, DateTimeKind.Utc);
            var timestamp = date.ToUnixMilliseconds();

            // Act
            var result = DateTimeExtensions.FormatAsIso8601(timestamp);

            // Assert
            Assert.Equal("2024-01-15T12:30:45.123Z", result);
        }

        [Fact]
        public void FormatAsIso8601_UnixEpoch_ReturnsEpochFormat()
        {
            // Act
            var result = DateTimeExtensions.FormatAsIso8601(0);

            // Assert
            Assert.Equal("1970-01-01T00:00:00.000Z", result);
        }

        #endregion

        #region Format Tests

        [Fact]
        public void Format_DefaultFormat_ReturnsExpectedFormat()
        {
            // Arrange
            var date = new DateTime(2024, 1, 15, 12, 30, 45, DateTimeKind.Utc);
            var timestamp = date.ToUnixMilliseconds();

            // Act
            var result = DateTimeExtensions.Format(timestamp);

            // Assert
            Assert.Equal("2024-01-15 12:30:45", result);
        }

        [Fact]
        public void Format_CustomFormat_ReturnsCustomFormat()
        {
            // Arrange
            var date = new DateTime(2024, 1, 15, 12, 30, 45, DateTimeKind.Utc);
            var timestamp = date.ToUnixMilliseconds();

            // Act
            var result = DateTimeExtensions.Format(timestamp, "dd/MM/yyyy");

            // Assert
            Assert.Equal("15/01/2024", result);
        }

        [Fact]
        public void Format_TimeOnlyFormat_ReturnsTimeOnly()
        {
            // Arrange
            var date = new DateTime(2024, 1, 15, 14, 45, 30, DateTimeKind.Utc);
            var timestamp = date.ToUnixMilliseconds();

            // Act
            var result = DateTimeExtensions.Format(timestamp, "HH:mm:ss");

            // Assert
            Assert.Equal("14:45:30", result);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Timestamps_BeforeEpoch_AreNegative()
        {
            // Arrange
            var beforeEpoch = new DateTime(1969, 12, 31, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = beforeEpoch.ToUnixMilliseconds();

            // Assert
            Assert.True(result < 0);
        }

        [Fact]
        public void LargeTimestamp_HandledCorrectly()
        {
            // Arrange - Year 3000
            var futureDate = new DateTime(3000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var timestamp = futureDate.ToUnixMilliseconds();
            var roundTrip = DateTimeExtensions.FromUnixMilliseconds(timestamp);

            // Assert
            Assert.Equal(futureDate.Year, roundTrip.Year);
            Assert.Equal(futureDate.Month, roundTrip.Month);
            Assert.Equal(futureDate.Day, roundTrip.Day);
        }

        #endregion
    }
}
