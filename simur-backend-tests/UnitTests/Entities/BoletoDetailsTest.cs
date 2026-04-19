using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Asn1.Ocsp;
using simur_backend.Controllers.Utils;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace simur_backend_tests.UnitTests.Entities
{
    public record BoletoMock()
    {
        public string PayerName { get; init; }

        public string BeneficiaryName { get; init; }

        public string BeneficiaryId { get; init; }

        public string BankCode { get; init; }

        [Required]
        public string Agency { get; init; }

        [Required]
        public string Account { get; init; }

        public string DocumentModality { get; init; }
    }

    public class BoletoDetailsTest
    {
        [Fact(DisplayName = "Testando a geração de detalhes do boleto")]
        public void GenerateBoletoDetails()
        {
            decimal Amount = decimal.Parse("4057.80");
            BoletoMock boleto = new() {
                BankCode = "0237",
                Agency = "3051",
                Account = "0790052-9",
                DocumentModality = "101",
                BeneficiaryId = "53178402000147",
                BeneficiaryName = "Parceiro Ferramentas Ltda",
                PayerName = "Bruna Medel Brum"
            };

            string OurNumber = $"{DateTime.UtcNow:yyMMddHHmmssff}";

            string barcode = BoletoUtilities.CreateBarcodeNumber(
                boleto.BankCode, 
                "9", 
                DateTimeOffset.Now.AddDays(3).DateTime,
                Amount,
                boleto.BeneficiaryId,
                OurNumber.Remove(OurNumber.Length - 1),
                boleto.DocumentModality
            );
            Assert.Equal(44, barcode.Length);
        }
    }
}
