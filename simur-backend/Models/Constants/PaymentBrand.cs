using System.ComponentModel;

namespace simur_backend.Models.Constants
{
    public enum PaymentBrand
    {
        [Description("Visa")] VISA,
        [Description("Mastercard")] MASTERCARD,
        [Description("American Express")] AMEX,
        [Description("Elo")] ELO,
        [Description("Hipercard")] HIPERCARD,
        [Description("Alelo")] ALELO
    }
}
