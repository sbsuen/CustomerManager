using CustomerManager.DBContexts;
using CustomerManager.DTO;
using CustomerManager.Models;
using CustomerManager.Repositories;
using CustomerManager.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManager.Controllers
{
    [Route("api/cards")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        private readonly CardRepository _cardRepository;

        public CardsController(CustomerManagerContext context)
        {
            _customerRepository = new CustomerRepository(context);
            _cardRepository = new CardRepository(context);
        }

        // GET: api/Cards/customer/5
        /// <summary>
        /// Gets a list of cards linked to an existing customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<CardDisplayDTO>>> GetCardsByCustomer(long customerId)
        {
            if (!_customerRepository.Exists(customerId))
                return NotFound($"Customer with id {customerId} not found. Cannot complete request.");
            var result = await _cardRepository.GetCardsByCustomerId(customerId);

            var cards = new List<CardDisplayDTO>();

            foreach (Card card in result)
            {
                cards.Add(CreateDTOFromCard(card));
            }

            return new ActionResult<IEnumerable<CardDisplayDTO>>(cards);
        }

        // GET: api/Cards/5
        /// <summary>
        /// Gets a specific card by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CardDisplayDTO>> GetCardById(long id)
        {
            var card = await _cardRepository.GetById(id);
            if (card == null)
            {
                return NotFound();
            }
            var customer = await _customerRepository.GetById(card.CustomerId);
            if (customer != null)
            {
                card.Customer = customer;
            }

            return CreateDTOFromCard(card);
        }

        // HEAD: api/Cards/validatecard
        /// <summary>
        /// Accepts a customer's credit card information, and validates the customer has the credit card on file.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="expiryDate"></param>
        /// <param name="cvv"></param>
        /// <returns></returns>
        [HttpHead("validatecard/{customerId}/{cardNumber}/{expirydate}/{cvv}")]
        public async Task<ActionResult> ValidateCustomerCard(long customerId, string cardNumber, string expiryDate, string cvv)
        {
            try
            {
                if (string.IsNullOrEmpty(cardNumber))
                {
                    throw new ArgumentException($"'{nameof(cardNumber)}' cannot be null or empty.", nameof(cardNumber));
                }

                if (string.IsNullOrEmpty(expiryDate))
                {
                    throw new ArgumentException($"'{nameof(expiryDate)}' cannot be null or empty.", nameof(expiryDate));
                }

                if (string.IsNullOrEmpty(cvv))
                {
                    throw new ArgumentException($"'{nameof(cvv)}' cannot be null or empty.", nameof(cvv));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var customer = await _customerRepository.GetById(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            var customerCards = await _cardRepository.GetCardsByCustomerId(customerId);
            if (customerCards == null)
            {
                return NotFound();
            }

            bool matchFound = MatchingCardExists(customerCards, cardNumber, expiryDate, cvv);

            return matchFound ? StatusCode(200) : NotFound();
        }


        // PUT: api/Cards/5
        /// <summary>
        /// Edits an existing credit card
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(long id, CardDTO cardDTO)
        {
            if (id != cardDTO.Id)
            {
                return BadRequest();
            }
            if (!_customerRepository.Exists(cardDTO.CustomerId))
            {
                return BadRequest($"Cannot modify card: Customer with id {cardDTO.CustomerId} does not exist.");
            }
            try
            {
                CardFormatValidator.ValidateCardFormat(cardDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            var card = await _cardRepository.GetById(id);
            ModifyCard(card, cardDTO);
            
            try
            {
                await _cardRepository.Update(card);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cards
        /// <summary>
        /// Creates a new card, must be tied to a customer
        /// </summary>
        /// <param name="cardDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Card>> PostCard(CardDTO cardDTO)
        {
            if ( !_customerRepository.Exists(cardDTO.CustomerId) )
            {
                return BadRequest($"Cannot add card: Customer with id {cardDTO.CustomerId} does not exist.");
            }

            try
            {
                CardFormatValidator.ValidateCardFormat(cardDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            Card card = CreateCardFromDTO(cardDTO);

            await _cardRepository.Add(card);

            CardDisplayDTO cardDisplayDTO = CreateDTOFromCard(card);

            return CreatedAtAction("GetCardById", new { id = cardDisplayDTO.Id }, cardDisplayDTO);
        }

        // DELETE: api/Cards/5
        /// <summary>
        /// Deletes a card if it exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Card>> DeleteCard(long id)
        {
            var card = await _cardRepository.GetById(id);
            if (card == null)
            {
                return NotFound();
            }

            await _cardRepository.Delete(card);

            return card;
        }

        private CardDisplayDTO CreateDTOFromCard(Card card)
        {
            return new CardDisplayDTO
            {
                Id = card.Id,
                CustomerId = card.CustomerId,
                Type = card.Type,
                LastFourDigits = card.LastFourDigits,
                ExpiryDate = card.ExpiryDate
            };
        }

        private Card CreateCardFromDTO(CardDTO cardDTO)
        {
            byte[] ccSalt = HashUtility.GenerateSalt();

            byte[] cardNumberHashed = HashUtility.ComputeHashedString(cardDTO.CardNumber + ccSalt);
            byte[] cvvHashed = HashUtility.ComputeHashedString(cardDTO.CVV + ccSalt);

            return new Card
            {
                Id = cardDTO.Id,
                CustomerId = cardDTO.CustomerId,
                Type = (CardType)cardDTO.TypeId,
                ExpiryDate = DateTime.ParseExact(cardDTO.ExpiryDate, "MMyy", CultureInfo.InvariantCulture),
                CardNumberHash = cardNumberHashed,
                CVVHash = cvvHashed,
                CCSalt = ccSalt,
                LastFourDigits = cardDTO.CardNumber.Substring(cardDTO.CardNumber.Length - 4),
                CreateDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };
        }

        private void ModifyCard(Card card, CardDTO cardDTO)
        {
            byte[] currentSalt = card.CCSalt;

            byte[] inputCCVHashed = HashUtility.ComputeHashedString(cardDTO.CVV + Convert.ToBase64String(currentSalt));
            byte[] inputCardNumberHashed = HashUtility.ComputeHashedString(cardDTO.CardNumber + Convert.ToBase64String(currentSalt));

            if (!card.CardNumberHash.SequenceEqual(inputCardNumberHashed) || !card.CVVHash.SequenceEqual(inputCCVHashed))
            {
                card.CCSalt = HashUtility.GenerateSalt();
                card.CardNumberHash = HashUtility.ComputeHashedString(cardDTO.CardNumber + Convert.ToBase64String(card.CCSalt));
                card.CVVHash = HashUtility.ComputeHashedString(cardDTO.CVV + Convert.ToBase64String(card.CCSalt));
            }
            else
            {
                card.CCSalt = card.CCSalt;
                card.CardNumberHash = card.CardNumberHash;
                card.CVVHash = card.CVVHash;
            }
            card.Id = cardDTO.Id;
            card.CustomerId = cardDTO.Id;
            card.Type = (CardType)cardDTO.TypeId;
            card.ExpiryDate = DateTime.ParseExact(cardDTO.ExpiryDate, "MMyy", CultureInfo.InvariantCulture);
            card.LastFourDigits = cardDTO.CardNumber.Substring(cardDTO.CardNumber.Length - 4);
            card.LastUpdatedDate = DateTime.Now;
        }

        private bool MatchingCardExists(IEnumerable<Card> cards, string cardNumber, string expiryDate, string cvv)
        {
            System.Diagnostics.Debug.WriteLine($"Checking for matching cards...");
            foreach (var card in cards)
            {
                byte[] inputCardHash = HashUtility.ComputeHashedString(cardNumber + card.CCSalt);
                byte[] inputCVVHash = HashUtility.ComputeHashedString(cvv + card.CCSalt);

                System.Diagnostics.Debug.WriteLine($"Salt value: {Convert.ToBase64String(card.CCSalt)}");
                System.Diagnostics.Debug.WriteLine($"Input Card Hash: {Convert.ToBase64String(inputCardHash)}");
                System.Diagnostics.Debug.WriteLine($"Input CVV Hash: {Convert.ToBase64String(inputCVVHash)}");
                System.Diagnostics.Debug.WriteLine($"Current Card Hash: {Convert.ToBase64String(card.CardNumberHash)}");
                System.Diagnostics.Debug.WriteLine($"Current CVV Hash: {Convert.ToBase64String(card.CVVHash)}");

                if (!card.CardNumberHash.SequenceEqual(inputCardHash)) continue;
                if (!card.ExpiryDate.ToString("MMyy").Equals(expiryDate)) continue;
                if (!card.CVVHash.SequenceEqual(inputCVVHash)) continue;

                System.Diagnostics.Debug.WriteLine("Matching card exists...");

                return true;
            }
            return false;
        }

        private bool CardExists(long id)
        {
            return _cardRepository.Exists(id);
        }
    }
}
