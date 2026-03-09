using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;
using System.Text.Json;

namespace simur_backend.Controllers.Utils
{
    public class PaymentDetailsSwitcher
    {
        public static IPaymentDetails SetPaymentType(PaymentType paymentType)
        {
            switch (paymentType)
            {
                case Models.Constants.PaymentType.PIX:
                    return new PixDetails();
                default:
                    throw new NotImplementedException($"Payment of type {paymentType} is still not accepted");
            }
        }

        public static IPaymentDetails? SetPaymentDetails(PaymentDto payment)
        {
            switch(payment.MethodType)
            {
                case Models.Constants.PaymentType.PIX:
                    return JsonSerializer.Deserialize<PixDetails>(payment.Details.ToJson());
                case Models.Constants.PaymentType.CREDIT_CARD:
                    return JsonSerializer.Deserialize<CreditCardDetails>(payment.Details.ToJson());
                default:
                    throw new NotImplementedException($"Payment of type {payment.MethodType.ToString()} is still not accepted");
            }
        }
    }
}
