namespace RoomBookingApp.Core.Tests
{
    public class RoomBookingRequestProcessor
    {
        internal RoomBookingResult BookRoom(RoomBookingRequest bookingRequest)
        {
            return new RoomBookingResult
            {
                FullName = bookingRequest.FullName,
                Email = bookingRequest.Email,
                Date = bookingRequest.Date
            };
        }
    }
}