using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Alura_AspNet_Identity.Identity
{
    public class UserPasswordValidator : IIdentityValidator<string>
    {
        public int SizeRequired { get; set; }
        public bool SpecialCharRequired { get; set; }
        public bool LowerCaseRequired { get; set; }
        public bool UpperCaseRequired { get; set; }
        public bool DigitsRequired { get; set; }

        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var errors = new List<string>();

            if (!VerifySizeRequired(item))
                errors.Add($"A senha deve conter no mínimo {SizeRequired} caracteres");
            if (SpecialCharRequired && !VerifySpecialChar(item))
                errors.Add("A senha deve conter caracteres especiais");
            if (LowerCaseRequired && !VerifyLowerCase(item))
                errors.Add("A senha deve conter no mínimo uma letra minúscula");
            if (UpperCaseRequired && !VerifyUpperCase(item))
                errors.Add("A senha deve conter no mínimo uma letra minúscula");
            if (DigitsRequired && !VerifyDigits(item))
                errors.Add("A senha deve conter no mínimo um dígito");

            if (errors.Any())
                return IdentityResult.Failed(errors.ToArray());
            else
                return IdentityResult.Success;
        }

        private bool VerifySizeRequired(string password) =>
            password?.Length >= SizeRequired;

        private bool VerifySpecialChar(string password) =>
            Regex.IsMatch(password, @"\|!@#$%&/()=?»«@£§€{}.-;'<>_,");

        private bool VerifyLowerCase(string password) =>
            password.Any(char.IsLower);

        private bool VerifyUpperCase(string password) =>
            password.Any(char.IsUpper);

        private bool VerifyDigits(string password) =>
            password.Any(char.IsDigit);
    }
}