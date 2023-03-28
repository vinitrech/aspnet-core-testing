using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;
using Xunit;

namespace RoomBookingApp.Core.Tests;

public class RoomBookingRequestProcessorTest
{
    private readonly RoomBookingRequestProcessor _processor;
    private RoomBookingRequest _request;
    private Mock<IRoomBookingService> _roomBookingServiceMock;
    private List<Room> _availableRooms;

    public RoomBookingRequestProcessorTest()
    {

        // Arrange

        _request = new RoomBookingRequest
        {
            FullName = "Test Full Name",
            Email = "test@test.com",
            Date = new DateTime(2023, 03, 27),
        };
        _availableRooms = new List<Room>() { new Room() {
            Id = 1
        } };

        _roomBookingServiceMock = new Mock<IRoomBookingService>();
        _roomBookingServiceMock.Setup(q => q.GetAvailableRooms(_request.Date))
        .Returns(_availableRooms);

        _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
    }

    // Some places use different syntaxes for the same tests, just for comparison reasons

    [Fact]
    public void Should_Return_Room_Booking_Response_With_Request_Values()
    {
        // Act
        RoomBookingResult result = _processor.BookRoom(_request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_request.FullName, result.FullName);
        Assert.Equal(_request.Email, result.Email);
        Assert.Equal(_request.Date, result.Date);

        result.ShouldNotBeNull();
        result.FullName.ShouldBe(_request.FullName);
        result.Email.ShouldBe(_request.Email);
        result.Date.ShouldBe(_request.Date);
    }

    [Fact]
    public void Should_Throw_Exception_For_Null_Request()
    {
        var exception = Should.Throw<ArgumentNullException>(() =>
         {
             _processor.BookRoom(null);
         });

        exception.ParamName.ShouldBe("bookingRequest");
    }

    [Fact]
    public void Should_Save_RoomBooking_Request()
    {
        RoomBooking savedBooking = null;

        _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
        .Callback<RoomBooking>(booking =>
        {
            savedBooking = booking;
        });

        _processor.BookRoom(_request);

        _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Once());

        savedBooking.ShouldNotBeNull();
        savedBooking.FullName.ShouldBe(_request.FullName);
        savedBooking.Email.ShouldBe(_request.Email);
        savedBooking.Date.ShouldBe(_request.Date);
        savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
    }

    [Fact]
    public void Should_Not_Save_Room_Booking_Request_If_None_Available()
    {
        _availableRooms.Clear();
        _processor.BookRoom(_request);

        _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never());
    }

    // Data Driven Test
    [Theory]
    [InlineData(BookingResultFlag.Failure, false)]
    [InlineData(BookingResultFlag.Success, true)]
    public void Should_Return_SuccessOrFailure_Flag_In_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
    {
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }

        var result = _processor.BookRoom(_request);
        bookingSuccessFlag.ShouldBe(result.Flag);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(null, false)]
    public void Should_Return_RoomBookingId_In_Result(int? roomBookingId, bool isAvailable)
    {
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }
        else
        {
            _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    booking.Id = roomBookingId;
                });
        }

        var result = _processor.BookRoom(_request);
        result.RoomBookingId.ShouldBe(roomBookingId);
    }
}
