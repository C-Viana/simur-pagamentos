using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities.Payments;

namespace simur_backend.Utilities
{
    public class BoletoPdfGenerator
    {
        public byte[] Create(PaymentDto payment)
        {
            var boleto = (BoletoDetails) payment.PaymentDetails;
            var barcode = BarcodeHelper.CreateBarcode(boleto.Barcode);
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Content().Column(col =>
                    {
                        col.Item().Text("BOLETO BANCÁRIO")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        col.Item().LineHorizontal(1);

                        col.Item().Text($"Beneficiário: {boleto.BeneficiaryName}");
                        col.Item().Text($"Sacado: {boleto.PayerName}");
                        col.Item().Text($"Valor: R$ {boleto.Amount}");
                        col.Item().Text($"Vencimento: {boleto.DueDate:dd/MM/yyyy}");

                        col.Item().PaddingTop(10).Text("Linha Digitável:")
                            .Bold();

                        col.Item().Text(boleto.DigitableLine)
                            .FontSize(14);

                        col.Item().PaddingTop(20)
                            .Text("Este é um boleto simulado para fins de teste.")
                            .FontSize(10)
                            .Italic();

                        col.Item().PaddingTop(20)
                            .Text($"Instruções de pagamento: {boleto.Instructions}")
                            .FontSize(10)
                            .Italic();

                        col.Item().PaddingTop(20)
                            .Image(barcode);

                        col.Item().AlignCenter()
                            .Text(boleto.Barcode)
                            .FontSize(10);
                    });
                });
            });
            QuestPDF.Settings.License = LicenseType.Community;
            return document.GeneratePdf();
        }
    }
}
