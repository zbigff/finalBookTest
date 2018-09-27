
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class RecOrderValidator : IRecOrderValidator
    {
        private static readonly Regex PhoneRegex = new Regex(@"^((00[0-9]{2})|(\+[0-9]{2}))([0-9]{9})$", RegexOptions.Compiled);
        private static readonly EmailAddressAttribute EmailAddressAttribute = new EmailAddressAttribute();

        public bool IsValid(RecognitionOrder recognitionOrder)
        {
            if (string.IsNullOrWhiteSpace(recognitionOrder.DestinationFolder))
                return false;
            if (string.IsNullOrWhiteSpace(recognitionOrder.SourcePath))
                return false;
            if (!recognitionOrder.PatternFaces.Any())
                return false;
            if (string.IsNullOrWhiteSpace(recognitionOrder.EmailAddress) || !EmailAddressAttribute.IsValid(recognitionOrder.EmailAddress))
                return false;
            if (!PhoneRegex.Match(recognitionOrder.PhoneNumber).Success)
                return false;
            return true;
        }
    }
}

