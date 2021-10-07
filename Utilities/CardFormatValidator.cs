using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CustomerManager.Models;
using CustomerManager.DTO;
using CustomerManager.Exceptions;

namespace CustomerManager.Utilities
{
    public class CardFormatValidator
    {
        private static readonly Regex AmexNumberFormat = new Regex(@"^[0-9]{15}$");
        private static readonly Regex AmexCVVFormat = new Regex(@"^[0-9]{4}$");

        private static readonly Regex VisaNumberFormat = new Regex(@"^[0-9]{16}$");
        private static readonly Regex VisaCVVFormat = new Regex(@"^[0-9]{3}$");

        private static readonly Regex MastercardNumberFormat = new Regex(@"^[0-9]{16}$");
        private static readonly Regex MastercardCVVFormat = new Regex(@"^[0-9]{3}$");

        private static readonly Regex ExpiryDateFormat = new Regex(@"^[0-9]{4}$");

        public static bool ValidateCardFormat(CardDTO cardDTO)
        {
            if (cardDTO.TypeId < 0 || cardDTO.TypeId > 3) cardDTO.TypeId = 0;
            CardType type = (CardType)cardDTO.TypeId;
            if (type == CardType.Unknown)
                throw new UnknownCardTypeException("Invalid card type. Card type is unknown.");

            if (!isGoodCardNumberFormat(cardDTO.CardNumber, type))
                throw new InvalidCardFormatException($"Invalid card number format for card type {type}");
            if (!isGoodCVVFormat(cardDTO.CVV, type))
                throw new InvalidCardFormatException($"Invalid CVV format for card type {type}");
            if (!ExpiryDateFormat.IsMatch(cardDTO.ExpiryDate))
                throw new InvalidCardFormatException($"Invalid expiry date format for card type {type}");

            return true;
        }
        private static bool isGoodCardNumberFormat(string cardNumber, CardType type)
        {
            if (type == CardType.Amex)
            {
                return AmexNumberFormat.IsMatch(cardNumber);
            }
            else if (type == CardType.Visa)
            {
                return VisaNumberFormat.IsMatch(cardNumber);
            }
            else if (type == CardType.MasterCard)
            {
                return MastercardNumberFormat.IsMatch(cardNumber);
            }
            return false;
        }

        private static bool isGoodCVVFormat(string cvv, CardType type)
        {
            if (type == CardType.Amex)
            {
                return AmexCVVFormat.IsMatch(cvv);
            }
            else if (type == CardType.Visa)
            {
                return VisaCVVFormat.IsMatch(cvv);
            }
            else if (type == CardType.MasterCard)
            {
                return MastercardCVVFormat.IsMatch(cvv);
            }
            return false;
        }
    }
}
