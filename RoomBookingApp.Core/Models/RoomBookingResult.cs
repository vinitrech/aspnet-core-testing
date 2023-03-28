using RoomBookingApp.Core.Enums;

namespace RoomBookingApp.Core.Models;

public class RoomBookingResult : RoomBookingBase
{
    public BookingResultFlag Flag { get; set; }
    public int? RoomBookingId { get; set; }
}