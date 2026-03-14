using System.Text;

namespace simur_backend.Controllers.Utils
{
    public class BoletoUtilities
    {
        private static StringBuilder _BarcodeDigits;
        private static int _VerificationCode = -1;
        static DateTime _DueDateFactorRef = DateTime.Parse("1997-10-07");

        public BoletoUtilities()
        {
            _BarcodeDigits = new();
        }

        private static string FillWithZerosAtLeft(int totalDigits, decimal docValue)
        {
            int ZerosToFill = 0;
            string AmountAsStringWithDigitsOnly = docValue.ToString().Replace(".", "").Replace(",", "");

            if (AmountAsStringWithDigitsOnly.Length > 10)
                AmountAsStringWithDigitsOnly = AmountAsStringWithDigitsOnly.Substring(0, 9);
            else
            {
                ZerosToFill = 10 - AmountAsStringWithDigitsOnly.Length;
                for (int i = 0; i < ZerosToFill; i++)
                {
                    AmountAsStringWithDigitsOnly = ("0" + AmountAsStringWithDigitsOnly);
                }
            }

            return AmountAsStringWithDigitsOnly;
        }

        private static int ApplyVerificationDigitForReadableLine(string moduleWithDigits)
        {
            int sum = 0;
            int auxInt = 0;
            string auxString = "";
            int multiplier = 1;

            for (int i = moduleWithDigits.Length-1; i >= 0; i--)
            {
                auxInt = (int)(char.GetNumericValue(moduleWithDigits[i]) * multiplier);
                if (auxInt > 9)
                {
                    auxString = auxInt.ToString();
                    sum += (int)(char.GetNumericValue(auxString[0]) + char.GetNumericValue(auxString[1]));
                }
                else
                    sum += auxInt;
                if (multiplier == 1) multiplier = 2;
                else multiplier = 1;
            }
            return sum % 10;
        }

        public BoletoUtilities SetBankId(string bankId)
        {
            //Up to 3 digits
            _BarcodeDigits.Append(bankId);
            return this;
        }

        public BoletoUtilities SetCurrencyId(string currencyId)
        {
            //One single digit
            _BarcodeDigits.Append(currencyId);
            return this;
        }

        public BoletoUtilities SetDueDateFactor(DateTime dueDate)
        {
            //Up to 4 digits
            if (dueDate < _DueDateFactorRef)
                _BarcodeDigits.Append("0");
            else
            {
                int DaysDiff = (dueDate - _DueDateFactorRef).Days;
                if (DaysDiff > 9999)
                {
                    int Year = dueDate.Year;
                    string Month = (dueDate.Month < 10) ? "0" + dueDate.Month : "" + dueDate.Month;
                    string Day = (dueDate.Day < 10) ? "0" + dueDate.Day : "" + dueDate.Day;
                    _BarcodeDigits.Append($"{Year}");
                }
                else
                    _BarcodeDigits.Append(DaysDiff);
            }
            
            return this;
        }

        public BoletoUtilities SetDocumentValue(decimal docValue)
        {
            //Up to 10 digits. Here code must reach 18 digits + 1 digit verification code
            _BarcodeDigits.Append(FillWithZerosAtLeft(10, docValue));
            return this;
        }

        public BoletoUtilities SetBeneficiary(string beneficiaryId)
        {
            //FIELD DEFINED BY DOC EMITTER. CHOSED TO USE 7 DIGITS
            _BarcodeDigits.Append(beneficiaryId);
            return this;
        }

        public BoletoUtilities SetOurNumber(string ourNumber)
        {
            //FIELD DEFINED BY DOC EMITTER. CHOSED TO USE 14 DIGITS
            _BarcodeDigits.Append(ourNumber);
            return this;
        }

        public BoletoUtilities SetModality(string documentModality)
        {
            //FIELD DEFINED BY DOC EMITTER. CHOSED TO USE 4 DIGITS
            _BarcodeDigits.Append('0').Append(documentModality);
            return this;
        }

        public BoletoUtilities ApplyVerificationDigitForBarcode()
        {
            int sum = 0;
            int multiplier = 2;
            List<char> digits = [.. _BarcodeDigits.ToString()];
            for (int i = _BarcodeDigits.Length-1; i >= 0 ; i--)
            {
                sum += (int)(char.GetNumericValue(digits[i]) * multiplier);
                multiplier++;
                if (multiplier > 9) multiplier = 2;
            }
            sum *= 10;
            _VerificationCode = (sum % 11);

            _BarcodeDigits.Insert(5, string.Empty + _VerificationCode);

            return this;
        }

        public string GetBarcode()
        {
            return _BarcodeDigits.ToString();
        }

        public static string CreateBarcodeNumber(string bankId, string currencyId, DateTime dueDate, decimal docValue, string beneficiaryId, string ourNumber, string documentModality)
        {
            return new BoletoUtilities()
                .SetBankId(bankId)
                .SetCurrencyId(currencyId)
                .SetDueDateFactor(dueDate)
                .SetDocumentValue(docValue)
                .SetBeneficiary(beneficiaryId)
                .SetOurNumber(ourNumber)
                .SetModality(documentModality)
                .ApplyVerificationDigitForBarcode() //CODE MUST REACH 44 DIGITS AT THIS POINT
                .GetBarcode();
        }

        public static string CreateReadableLine(string bankId, string currencyId, decimal docValue, string barCodeVerificationDigit, string beneficiaryId, string ourNumber, string documentModality)
        {
            /**
             * bankId:  3
             * currencyId:  1
             * docValue:    10
             * barCodeVerificationDigit:    1
             * beneficiaryId: 7  
             * ourNumber:   14
             * documentModality:    3
             * 
             * PARCIAL: 38 digits
             * EXPECTED: 47 or 48 total digits
             * */
            
            Dictionary<string, string> modules = new Dictionary<string, string>();
            modules.Add("module1", $"0{bankId}{currencyId}0000"); //9 digits + 1 VC
            modules.Add("module2", $"{beneficiaryId}{documentModality}"); //10 digits + 1 VC
            modules.Add("module3", $"{FillWithZerosAtLeft(10, docValue)}"); //10 digits + 1 VC
            modules.Add("module4", barCodeVerificationDigit); //1 digit
            modules.Add("module5", $"{ourNumber}"); //14 digits

            return $"{modules.GetValueOrDefault("module1")}{ApplyVerificationDigitForReadableLine(modules.GetValueOrDefault("module1"))}" +
                $"{modules.GetValueOrDefault("module2")}{ApplyVerificationDigitForReadableLine(modules.GetValueOrDefault("module2"))}" +
                $"{modules.GetValueOrDefault("module3")}{ApplyVerificationDigitForReadableLine(modules.GetValueOrDefault("module3"))}" +
                $"{modules.GetValueOrDefault("module4")}" +
                $"{modules.GetValueOrDefault("module5")}";
        }
    }
}
