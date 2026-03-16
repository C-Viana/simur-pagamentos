using System.ComponentModel;

namespace simur_backend.Models.Constants
{
    public enum PaymentType
    {
        [Description("Boleto Bancário")] BOLETO,
        [Description("Transferência Bancária")] BANK_TRANSFER,
        [Description("Dinheiro")] CASH,
        [Description("Cartão de Crédito")] CREDIT_CARD,
        [Description("Cartão de Débito")] DEBIT_CARD,
        [Description("Carteira Digital")] DIGITAL_WALLET,
        [Description("Pix dinâmico")] PIX_DYNAMIC,
        [Description("Pix estático")] PIX_STATIC,
        [Description("Criptomoeda")] CRYPTO,
        [Description("Vale Presente/Refeição/Alimentação")] VOUCHER
    }
}
