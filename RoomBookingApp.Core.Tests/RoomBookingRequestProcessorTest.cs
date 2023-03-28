using Shouldly;
using Xunit;

namespace RoomBookingApp.Core.Tests;

public class RoomBookingRequestProcessorTest
{
    [Fact]
    public void Should_Return_Room_Booking_Response_With_Request_Values()
    {
        // Arrange
        var request = new RoomBookingRequest
        {
            FullName = "Test Full Name",
            Email = "test@test.com",
            Date = new DateTime(2023, 03, 27),
        };

        var processor = new RoomBookingRequestProcessor();

        // Act
        RoomBookingResult result = processor.BookRoom(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Date, result.Date);

        result.ShouldNotBeNull();
        result.FullName.ShouldBe(request.FullName);
        result.Email.ShouldBe(request.Email);
        result.Date.ShouldBe(request.Date);
    }
}
