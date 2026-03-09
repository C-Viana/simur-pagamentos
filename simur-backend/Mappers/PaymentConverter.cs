using rest_with_asp_net_10_cviana.Data.Converter.Contract;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Mappers
{
    public class PaymentConverter : IParser<Payment, PaymentDto>, IParser<PaymentDto, Payment>
    {
        public PaymentDto Parse(Payment origin)
        {
            if(origin == null) return null;
            return new PaymentDto()
            {
                Id = origin.Id,
                ExternalOrderId = origin.ExternalOrderId,
                Amount = origin.Amount,
                Currency = origin.Currency,
                PayerDocument = origin.CustomerDocument,
                SellerDocument = origin.MerchantDocument,
                Status = origin.Status,
                CreatedAt = origin.CreatedAt
            };
        }

        public List<PaymentDto> ParseList(List<Payment> origin)
        {
            if (origin == null) return [null];
            return [.. origin.Select(Parse)];
        }

        public Payment Parse(PaymentDto origin)
        {
            if (origin == null) return null;
            return new Payment(
                origin.Id,
                origin.ExternalOrderId,
                origin.Amount,
                origin.Currency,
                origin.Status,
                origin.CreatedAt,
                origin.SellerDocument,
                origin.PayerDocument
            );
        }

        public List<Payment> ParseList(List<PaymentDto> origin)
        {
            if (origin == null) return [null];
            return [.. origin.Select(Parse)];
        }
    }
}
