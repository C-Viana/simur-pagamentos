using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Messaging
{
    public interface IMessageBusService
    {
        Task PublishPaymentStatus(PaymentStatusHistory data);
    }
}
