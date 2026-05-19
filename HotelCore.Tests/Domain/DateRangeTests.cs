using System;
using HotelCore.Domain.Exceptions;
using HotelCore.Domain.ValueObjects;
using Xunit;

namespace HotelCore.Tests.Domain
{
    /// <summary>
    /// Các bài kiểm thử đơn vị cho Value Object DateRange.
    /// Kiểm tra các ràng buộc ngày, thời gian lưu trú và logic chồng lấn lịch.
    /// </summary>
    public class DateRangeTests
    {
        [Fact]
        public void Constructor_ValidDates_ShouldCreateInstance()
        {
            // Arrange
            var checkIn = DateTime.Today.AddDays(1);
            var checkOut = DateTime.Today.AddDays(5);

            // Act
            var range = new DateRange(checkIn, checkOut);

            // Assert
            Assert.Equal(checkIn.Date, range.CheckInDate);
            Assert.Equal(checkOut.Date, range.CheckOutDate);
            Assert.Equal(4, range.DurationInDays);
        }

        [Fact]
        public void Constructor_CheckInInPast_ShouldThrowHotelDomainException()
        {
            // Arrange
            var checkIn = DateTime.Today.AddDays(-1);
            var checkOut = DateTime.Today.AddDays(2);

            // Act & Assert
            var exception = Assert.Throws<HotelDomainException>(() => new DateRange(checkIn, checkOut));
            Assert.Contains("không thể nằm trong quá khứ", exception.Message);
        }

        [Fact]
        public void Constructor_CheckOutBeforeCheckIn_ShouldThrowHotelDomainException()
        {
            // Arrange
            var checkIn = DateTime.Today.AddDays(2);
            var checkOut = DateTime.Today.AddDays(1);

            // Act & Assert
            var exception = Assert.Throws<HotelDomainException>(() => new DateRange(checkIn, checkOut));
            Assert.Contains("phải diễn ra sau ngày nhận phòng", exception.Message);
        }

        [Fact]
        public void Constructor_CheckOutEqualToCheckIn_ShouldThrowHotelDomainException()
        {
            // Arrange
            var checkIn = DateTime.Today.AddDays(2);
            var checkOut = DateTime.Today.AddDays(2);

            // Act & Assert
            var exception = Assert.Throws<HotelDomainException>(() => new DateRange(checkIn, checkOut));
            Assert.Contains("phải diễn ra sau ngày nhận phòng", exception.Message);
        }

        [Theory]
        [InlineData(1, 3, 2, 4, true)]   // Chồng lấn giữa (StartB ở giữa A)
        [InlineData(2, 4, 1, 3, true)]   // Chồng lấn đầu (StartA ở giữa B)
        [InlineData(1, 5, 2, 4, true)]   // B nằm hoàn toàn trong A
        [InlineData(2, 4, 1, 5, true)]   // A nằm hoàn toàn trong B
        [InlineData(1, 3, 3, 5, false)]  // B bắt đầu ngay khi A kết thúc (Tiếp giáp - không chồng lấn)
        [InlineData(3, 5, 1, 3, false)]  // A bắt đầu ngay khi B kết thúc (Tiếp giáp - không chồng lấn)
        [InlineData(1, 2, 4, 5, false)]  // Hai khoảng cách xa nhau hoàn toàn
        public void OverlapsWith_VariousRanges_ShouldReturnExpectedResult(
            int offsetStartA, int offsetEndA, 
            int offsetStartB, int offsetEndB, 
            bool expectedResult)
        {
            // Arrange
            var baseDate = DateTime.Today;
            var rangeA = new DateRange(baseDate.AddDays(offsetStartA), baseDate.AddDays(offsetEndA));
            var rangeB = new DateRange(baseDate.AddDays(offsetStartB), baseDate.AddDays(offsetEndB));

            // Act
            var result1 = rangeA.OverlapsWith(rangeB);
            var result2 = rangeB.OverlapsWith(rangeA);

            // Assert
            Assert.Equal(expectedResult, result1);
            Assert.Equal(expectedResult, result2);
        }

        [Fact]
        public void Contains_DateInsideRange_ShouldReturnTrue()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(3));
            var targetDate = DateTime.Today.AddDays(1);

            // Act
            var contains = range.Contains(targetDate);

            // Assert
            Assert.True(contains);
        }

        [Fact]
        public void Contains_DateOutsideRange_ShouldReturnFalse()
        {
            // Arrange
            var range = new DateRange(DateTime.Today, DateTime.Today.AddDays(3));
            var targetDate = DateTime.Today.AddDays(4);

            // Act
            var contains = range.Contains(targetDate);

            // Assert
            Assert.False(contains);
        }
    }
}
