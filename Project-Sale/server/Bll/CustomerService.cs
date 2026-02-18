using AutoMapper;
using server.Bll.Interfaces;
using server.Dal;
using server.Dal.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Bll
{
    // במקום שבכל פונקציה נצטרך לשלוח את הuserId
    // בשביל לדעת באיזה user אנחנו עובדים
    // נשלוף מהטוקן
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDal _customerDal;
        private readonly IGiftService _giftService;
        private readonly IPurchasesService _purchasesService;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerDal customerDal,
            IGiftService giftService,
            IPurchasesService purchasesService,
            IMapper mapper,
            ILogger<CustomerService> logger)
        {
            _customerDal = customerDal;
            _giftService = giftService;
            _purchasesService = purchasesService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<User> Register(UserDTO dto)
        {
            _logger.LogInformation("Register started. Email={Email}", dto.Email);

            if (await _customerDal.GetUserByEmail(dto.Email) != null)
            {
                _logger.LogWarning("Register failed - email already exists. Email={Email}", dto.Email);
                throw new Exception("Email already exists");
            }

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var createdUser = await _customerDal.AddUser(user);

            _logger.LogInformation("Register completed successfully. UserId={UserId}", createdUser.Id);
            return createdUser;
        }

        public async Task<List<Gift>> GetCart(int userId)
        {
            _logger.LogInformation("GetCart started. UserId={UserId}", userId);

            var user = await _customerDal.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("GetCart failed - user not found. UserId={UserId}", userId);
                throw new Exception("User not found");
            }

            if (user.ShoppingCart == null || !user.ShoppingCart.Any())
            {
                _logger.LogInformation("GetCart completed - shopping cart is empty. UserId={UserId}", userId);
                return new List<Gift>();
            }

            var result = new List<Gift>();

            foreach (var giftId in user.ShoppingCart)
            {
                var gift = await _giftService.GetById(giftId);
                if (gift == null)
                {
                    _logger.LogWarning("GetCart - gift not found. UserId={UserId}, GiftId={GiftId}", userId, giftId);
                    continue;
                }
                result.Add(gift);
            }

            _logger.LogInformation("GetCart completed successfully. UserId={UserId}, Count={Count}", userId, result.Count);
            return result;
        }



        public async Task<List<Gift>> GetGifts(string? category, bool? sortPriceAsc)
        {
            _logger.LogInformation("GetGifts called. Category={Category}, SortPriceAsc={SortPriceAsc}", category, sortPriceAsc);

            List<Gift> gifts;

            if (category != null)
            {
                gifts = await _giftService.GetByCategory(category);
                _logger.LogInformation("Returned {Count} gifts for category {Category}", gifts.Count, category);
                return gifts;
            }

            if (sortPriceAsc != null)
            {
                gifts = await _giftService.GetByPrice(sortPriceAsc.Value);
                _logger.LogInformation("Returned {Count} gifts sorted by price ascending={Ascending}", gifts.Count, sortPriceAsc.Value);
                return gifts;
            }

            gifts = await _giftService.Get();
            _logger.LogInformation("Returned all gifts. Count={Count}", gifts.Count);
            return gifts;
        }

        public async Task AddToCart(int userId, int giftId)
        {
            _logger.LogInformation("AddToCart started. UserId={UserId}, GiftId={GiftId}", userId, giftId);
            var user = await _customerDal.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("AddToCart failed - user not found. UserId={UserId}", userId);
                throw new Exception("User not found");
            }

            var gift = await _giftService.GetById(giftId);
            if (gift == null)
            {
                _logger.LogWarning("AddToCart failed - gift not found. GiftId={GiftId}", giftId);
                throw new Exception("Gift not found");
            }
            if (gift.IsDrawn)
            {
                _logger.LogWarning("AddToCart failed - gift already drawn. GiftId={GiftId}", giftId);
                throw new Exception("Cannot add drawn gift to cart");
            }

            if (user.ShoppingCart == null) user.ShoppingCart = new List<int>();

            user.ShoppingCart.Add(giftId);
            await _customerDal.UpdateUser(user);
            _logger.LogInformation("Gift added to cart successfully. UserId={UserId}, GiftId={GiftId}", userId, giftId);
        }


        public async Task RemoveFromCart(int userId, int giftId)
        {
            _logger.LogInformation("RemoveFromCart started. UserId={UserId}, GiftId={GiftId}", userId, giftId);

            var user = await _customerDal.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("RemoveFromCart failed - user not found. UserId={UserId}", userId);
                throw new Exception("User not found");
            }

            if (user.ShoppingCart != null && user.ShoppingCart.Contains(giftId))
            {
                user.ShoppingCart.Remove(giftId);
                await _customerDal.UpdateUser(user);
                _logger.LogInformation("Gift removed from cart successfully. UserId={UserId}, GiftId={GiftId}", userId, giftId);
            }
            else
            {
                _logger.LogWarning("Gift not found in cart. UserId={UserId}, GiftId={GiftId}", userId, giftId);
            }
        }
          
        public async Task Purchase(int userId)
        {
            _logger.LogInformation("Purchase started. UserId={UserId}", userId);
            var user = await _customerDal.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("Purchase failed - user not found. UserId={UserId}", userId);
                throw new Exception("User not found");
            }

            if (user.ShoppingCart == null || !user.ShoppingCart.Any())
            {
                _logger.LogInformation("Purchase aborted - shopping cart is empty. UserId={UserId}", userId);
                return;
            }

            var cartItems = user.ShoppingCart.ToList();

            foreach (var giftId in cartItems)
            {
                var gift = await _giftService.GetById(giftId);
                if (gift == null)
                {
                    _logger.LogWarning("Gift not found during purchase. GiftId={GiftId}", giftId);
                    continue;
                }

                var ticketsForGift = await _purchasesService.GetTicketsByGiftId(giftId);

                var existingTicket = ticketsForGift.FirstOrDefault(t => t.UserId == userId);

                if (existingTicket != null)
                {
                    gift.BuyersNumber += 1;
                    var giftDto = _mapper.Map<GiftDTO>(gift);
                    await _giftService.Update(gift.Id, giftDto);

                    existingTicket.Quantity += 1;
                    existingTicket.TicketNumberForGift = gift.BuyersNumber;

                    await _customerDal.UpdateTicket(existingTicket);

                    _logger.LogInformation("Existing ticket quantity incremented. UserId={UserId}, GiftId={GiftId}, NewQuantity={Quantity}", userId, giftId, existingTicket.Quantity);
                }
                else
                {
                    gift.BuyersNumber += 1;
                    var giftDto = _mapper.Map<GiftDTO>(gift);
                    await _giftService.Update(gift.Id, giftDto);

                    var ticket = new Ticket
                    {
                        UserId = userId,
                        GiftId = giftId,
                        TicketNumberForGift = gift.BuyersNumber,
                        Quantity = 1
                    };

                    await _customerDal.AddTicket(ticket);

                    _logger.LogInformation("New ticket added. UserId={UserId}, GiftId={GiftId}, TicketNumber={TicketNumber}", userId, giftId, ticket.TicketNumberForGift);
                }
            }

            user.ShoppingCart.Clear();
            await _customerDal.UpdateUser(user);
            _logger.LogInformation("Purchase completed successfully. UserId={UserId}", userId);
        }
    }
}




