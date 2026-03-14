using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Controllers.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("BoletoDetails")]
    public class BoletoDetails : IPaymentDetails
    {
        [Required]
        public string BeneficiaryName { get; init; }

        [Required]
        [StringLength(7)]
        public string BeneficiaryId { get; init; }

        // Código do banco (3 dígitos)
        // ex: "341" (Itaú), "237" (Bradesco), "104" (Caixa), "001" (Banco do Brasil)
        [Required]
        [StringLength(3)]
        public string BankCode { get; init; }

        // Agência e conta do beneficiário (loja que recebe)
        [Required]
        public string Agency { get; init; }

        [Required]
        public string Account { get; init; }

        [Required]
        [StringLength(3)]
        [AllowedValues("101", "104")]
        public string DocumentModality { get; init; } //101 (Cobrança Rápida COM Registro), 104 (Cobrança Eletrônica COM Registro)


        [StringLength(44, MinimumLength = 44)]
        public string Barcode { get; set; }

        [StringLength(48, MinimumLength = 47)]
        public string DigitableLine { get; set; }

        public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.Now;
        public DateOnly DueDate { get; set; }


        [StringLength(13)]
        public string OurNumber { get; set; } = $"{DateTime.UtcNow:yyMMddHHmmssff}";
        public string BoletoUrl { get; set; }               // Link simulado para PDF
        public int ExpirationDays { get; set; } = 3;
        public string Instructions { get; set; }

        [JsonIgnore]
        public Guid PaymentId { get; set; }

        [JsonIgnore]
        public decimal Amount { get; set; }

        public BoletoDetails() {
            base.PaymentType = Constants.PaymentType.BOLETO;
            DueDate = DateOnly.FromDateTime(GeneratedAt.DateTime).AddDays(3);
        }

        public BoletoDetails GenerateSlipCodes(Guid paymentId, decimal amount, HttpRequest httpRequest)
        {
            PaymentId = paymentId;
            Amount = amount;

            Barcode = BoletoUtilities.CreateBarcodeNumber(
                BankCode,
                "9",
                DueDate.ToDateTime(TimeOnly.MinValue),
                Amount,
                BeneficiaryId,
                OurNumber,
                DocumentModality
            );

            DigitableLine = BoletoUtilities.CreateReadableLine(BankCode, "9", Amount, $"{Barcode[5]}", BeneficiaryId, OurNumber, DocumentModality);

            Instructions = $"Não receber após vencimento.\nBoleto referente ao pagamento {PaymentId} de {GeneratedAt}";
            BoletoUrl = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/boleto/{PaymentId}";
            return this;
        }
    }
}
