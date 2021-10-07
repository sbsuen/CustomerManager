using System;
using Xunit;
using CustomerManager.Models;
using CustomerManager.DTO;
using CustomerManager.Exceptions;
using CustomerManager.Utilities;

namespace CustomerManagerTest
{
    public class CardFormatValidatorTest
    {
        //CardType Enum Values:
        // Unknown      = 0
        // Amex         = 1
        // Visa         = 2
        // Mastercard   = 3

        private readonly CardDTO mockUnknownCardDTO = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "1234",
            CVV = "123",
            TypeId = 0,
            ExpiryDate = "1234"
        };

        private readonly CardDTO mockBadTypeCardDTO = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "1234",
            CVV = "123",
            TypeId = 100,
            ExpiryDate = "1234"
        };

        private readonly CardDTO mockGoodVisaDTO = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "4242424242424242",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 2,
        };

        private readonly CardDTO mockGoodAmexDTO = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "378282246310005",
            CVV = "1234",
            ExpiryDate = "1230",
            TypeId = 1,
        };

        private readonly CardDTO mockGoodMastercardDTO = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "5555555555554444",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 3,
        };

        private readonly CardDTO mockBadVisaDTO_BadNumber_One = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "42424242424242421",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 2,
        };

        private readonly CardDTO mockBadVisaDTO_BadNumber_Two = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "abc4242424242424242",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 2,
        };

        private readonly CardDTO mockBadVisaDTO_BadCVV = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "42424242424242421",
            CVV = "13",
            ExpiryDate = "1230",
            TypeId = 2,
        };

        private readonly CardDTO mockBadVisaDTO_BadDate = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "42424242424242421",
            CVV = "123",
            ExpiryDate = "December 2030",
            TypeId = 2,
        };

        private readonly CardDTO mockBadAmexDTO_BadNumber_One = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "3782822463100054",
            CVV = "1234",
            ExpiryDate = "1230",
            TypeId = 1,
        };

        private readonly  CardDTO mockBadAmexDTO_BadNumber_Two = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "378282246310005",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 1,
        };

        private readonly CardDTO mockBadAmexDTO_BadCVV = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "378282246310005",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 1,
        };

        private readonly CardDTO mockBadAmexDTO_BadDate = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "378282246310005",
            CVV = "1234",
            ExpiryDate = "123",
            TypeId = 1,
        };

        private readonly CardDTO mockBadMastercardDTO_BadNumber = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "5555555555554444%",
            CVV = "123",
            ExpiryDate = "1230",
            TypeId = 3,
        };

        private readonly CardDTO mockBadMastercardDTO_BadCVV = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "5555555555554444",
            CVV = "1s23",
            ExpiryDate = "1230",
            TypeId = 3,
        };

        private readonly CardDTO mockBadMastercardDTO_BadDate = new CardDTO
        {
            Id = 1,
            CustomerId = 1,
            CardNumber = "5555555555554444",
            CVV = "123",
            ExpiryDate = "1$30",
            TypeId = 3,
        };

        [Fact]
        public void TestUnknownCardTypeException_ShouldThrowException()
        {
            Assert.Throws<UnknownCardTypeException>(() => CardFormatValidator.ValidateCardFormat(mockUnknownCardDTO));
            Assert.Throws<UnknownCardTypeException>(() => CardFormatValidator.ValidateCardFormat(mockBadTypeCardDTO));
        }

        [Fact]
        public void TestGoodCardDTOs_Ok_Test()
        {
            Assert.True(CardFormatValidator.ValidateCardFormat(mockGoodAmexDTO));
            Assert.True(CardFormatValidator.ValidateCardFormat(mockGoodMastercardDTO));
            Assert.True(CardFormatValidator.ValidateCardFormat(mockGoodVisaDTO));
        }

        [Fact]
        public void TestBadCardNumberFormatting_ShouldThrowException()
        {
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadAmexDTO_BadNumber_One));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadAmexDTO_BadNumber_Two));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadMastercardDTO_BadNumber));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadVisaDTO_BadNumber_One));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadVisaDTO_BadNumber_Two));
        }

        [Fact]
        public void TestBadCardCVVFormatting_ShouldThrowException()
        {
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadMastercardDTO_BadCVV));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadAmexDTO_BadCVV));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadVisaDTO_BadCVV));
        }

        [Fact]
        public void TestBadCardExpiryDateFormatting_ShouldThrowException()
        {
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadMastercardDTO_BadDate));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadAmexDTO_BadDate));
            Assert.Throws<InvalidCardFormatException>(() => CardFormatValidator.ValidateCardFormat(mockBadVisaDTO_BadDate));
        }
    }
}
