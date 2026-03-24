using MongoDB.Bson.Serialization.Attributes;
using QRCoder;
using simur_backend.Models.DTO.V1;
using simur_backend.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    public class PixStaticPayload
    {
        public string Txid { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string MerchantDocument { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    [BsonDiscriminator("PixStaticDetails")]
    public class PixStaticDetails : IPaymentDetails
    {
        //[Required]
        //public string PayerName { get; init; }

        //[Required]
        //[JsonIgnore]
        //public string PayerDocument { get; init; }

        //[Required]
        //public string PayerEmail { get; init; }

        [Required]
        public string Description { get; init; }

        [JsonIgnore]
        public Guid PaymentId { get; set; }

        [JsonIgnore]
        public decimal Amount { get; set; }

        [JsonIgnore]
        public string PixKey { get; set; }

        public string QrCodeBase64 { get; set; }
        public string PaymentUri { get; set; }

        [StringLength(35, MinimumLength = 1)]
        public string TXID { get; set; }
        public DateTime ExpiresAt { get; set; }

        public PixStaticDetails() {
            base.PaymentType = Constants.PaymentType.PIX_DYNAMIC;
        }

        public PixStaticDetails GenerateStaticPixPayment(Guid paymentId, MerchantDto merchant, decimal amount, HttpRequest httpRequest)
        {
            PaymentId = paymentId;
            Amount = amount;
            PixKey = merchant.PixKey;
            ExpiresAt = DateTimeOffset.MaxValue.DateTime;
            TXID = StringUtilities.GetAlphanumericCode(35);

            PixStaticPayload payload = new()
            {
                Txid = TXID,
                Amount = amount,
                Currency = "BRL",
                MerchantDocument = merchant.Document,
                CreatedAt = DateTime.Now
            };

            //Json web token with value and TXID
            string pixkeyLength = (PixKey.Length < 10) ? "0" + PixKey.Length : "" + PixKey.Length;
            string amountTextValue = Amount.ToString().Replace(",", ".");
            string amountLength = (amountTextValue.Length < 10) ? "0" + amountTextValue.Length : "" + amountTextValue.Length;
            string cityLength = (merchant.Address.City.Length < 10) ? "0" + merchant.Address.City.Length : "" + merchant.Address.City.Length;
            string companyNameLength = (merchant.CompanyName.Length < 10) ? "0" + merchant.CompanyName.Length : "" + merchant.CompanyName.Length;
            string descriptionLength = (Description.Length < 10) ? "0" + Description.Length : "" + Description.Length;
            string field26Length = "" + (PixKey.Length + 14);
            string txid = Guid.NewGuid().ToString("N")[..25];

            StringBuilder urlBuilder = new StringBuilder()
                .Append("00").Append("02").Append("01") //Payload Format Indicator
                .Append("01").Append("02").Append("11")  //Point of Initiation Method: 11 static; 12 dynamic
                .Append("26").Append(field26Length).Append("0014BR.GOV.BCB.PIX").Append("01").Append(pixkeyLength).Append(PixKey)  //PIX Key
                .Append("52").Append("04").Append(merchant.MCC)  //Merchant Category Code: MCC
                .Append("53").Append("03986")  //Currency: static field BRL 5303986
                .Append("58").Append("02BR")  //Country Code: static field BRL 5802BR
                .Append("59").Append(companyNameLength).Append(StringUtilities.ReplaceAccents(merchant.CompanyName))  //Merchant Name
                .Append("60").Append(cityLength).Append(StringUtilities.ReplaceAccents(merchant.Address.City))  //Merchant City
                .Append("62").Append("07").Append("0525").Append(txid)
                .Append("6304");  //CRC (Cyclic Redundancy Check)

            string URL = urlBuilder.ToString();
            string crcCode = AlgorithmCRC16.EncodeString(URL);
            URL += crcCode;
            PaymentUri = URL;
            QRCodeData qrCodeData = new QRCodeGenerator().CreateQrCode(PaymentUri, QRCodeGenerator.ECCLevel.Q);
            byte[] qrCodeImage = new PngByteQRCode(qrCodeData).GetGraphic(20);
            QrCodeBase64 = Convert.ToBase64String(qrCodeImage);
            return this;
        }
    }
}
