using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Courses
{
    class TextFieldValidation
    {
        public static bool IsValidEmail(string email)
        {
            // Early return for an empty email field
            if (string.IsNullOrWhiteSpace(email))
            {
                App.Current.MainPage.DisplayAlert("Email Validation", "Email cannot be empty.", "OK");
                return false;
            }

            try // Preparing the email for Validation
            {
                // Normalizes the email domain
                string domain(Match match)
                {
                    // Convert Unicode domain names to ASCII | ArgumentException on invalid domain
                    IdnMapping idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName; // Returns normalized domain
                }

                // Reassigns to email with the normalized domain for Regex Validation
                email = Regex.Replace(email, @"(@)(.+)$", 
                            domain,
                            RegexOptions.None, 
                            TimeSpan.FromMilliseconds(200));
            }
            catch (ArgumentException e)
            {
                App.Current.MainPage.DisplayAlert("Invalid Argument Exception", $"{e} was invalid.", "OK");
                return false;
            }
            catch (RegexMatchTimeoutException e)
            {
                App.Current.MainPage.DisplayAlert("Timeout Exception", $"Validation for {e} timed out.", "OK");
                return false;
            }

            try
            {
                // Returns false with a mismatch of alphanumerics in the proper pattern, otherwise true
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException e)
            {
                App.Current.MainPage.DisplayAlert("Timeout Exception", $"Validation for {e} timed out.", "OK");
                return false;
            }
        }
    }
}
