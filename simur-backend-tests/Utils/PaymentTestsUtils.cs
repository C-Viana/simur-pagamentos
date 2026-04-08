using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;

namespace simur_backend_tests.Utils
{
    public static class PaymentTestsUtils
    {
        public static Payment getPayment()
        {
            return new Payment(
                Guid.NewGuid(),
                "PF-20260403165626-9853",
                decimal.Parse("1452.79"),
                "BRL",
                simur_backend.Models.Constants.PaymentStatus.CREATED,
                DateTimeOffset.Now,
                "53178402000147",
                "26499704102"
            );
        }

        public static PaymentDto getPaymentDto()
        {
            PaymentDto p = new()
            {
                ExternalOrderId = "PF-20260403165626-9853",
                Amount = decimal.Parse("1452.79"),
                Currency = "BRL",
                SellerDocument = "53178402000147",
                PayerDocument = "26499704102"
            };

            return p;
        }

        public static BoletoDetails getBoletoDetails()
        {
            return new()
            {
                PaymentType = simur_backend.Models.Constants.PaymentType.BOLETO,
                BankCode = "1051",
                Agency = "3051",
                Account = "0790052-9",
                DocumentModality = "101",
                BeneficiaryId = "1000375",
                BeneficiaryName = "Parceiro Ferramentas Ltda"
            };
        }
    }
}
