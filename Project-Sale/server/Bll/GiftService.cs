using AutoMapper;
using server.Bll.Interfaces;
using server.Dal.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Bll
{
    public class GiftService : IGiftService
    {
        private readonly IGiftDal giftDal;
        private readonly IDonorDal donorDal;
        private readonly IMapper mapper;
        private readonly ILogger<GiftService> logger;

        public GiftService(IGiftDal giftDal, IDonorDal donorDal, IMapper mapper, ILogger<GiftService> logger)
        {
            this.giftDal = giftDal;
            this.donorDal = donorDal;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Gift> Add(GiftDTO gifted)
        {
            logger.LogInformation("Add gift started. Name={GiftName}", gifted.Name);

            var gift = mapper.Map<Gift>(gifted);
            var donor = await donorDal.GetById(gifted.DonorId);
            if (donor == null)
            {
                logger.LogWarning("Add gift failed - donor not found. DonorId={DonorId}", gifted.DonorId);
                throw new Exception("Donor not found");
            }

            gift.Donor = donor;
            var newGift = await giftDal.Add(gift);
            logger.LogInformation("Gift added successfully. Id={GiftId}, Name={GiftName}", newGift.Id, newGift.Name);
            return newGift;
        }

        public async Task<List<Gift>> Get()
        {
            logger.LogInformation("Get all gifts started");
            var gifts = await giftDal.Get();
            logger.LogInformation("Get all gifts finished. Count={Count}", gifts.Count);
            return gifts;
        }

        public async Task<List<Gift>?> GetByBuyersNumber(int number)
        {
            logger.LogInformation("Get gifts by buyers number started. Number={Number}", number);
            var gifts = await giftDal.GetByBuyersNumber(number);
            logger.LogInformation("Get gifts by buyers number finished. Count={Count}", gifts?.Count ?? 0);
            return gifts;
        }

        public async Task<List<Gift>> GetByCategory(string category)
        {
            logger.LogInformation("Get gifts by category started. Category={Category}", category);
            var gifts = await giftDal.GetByCategory(category);
            logger.LogInformation("Get gifts by category finished. Count={Count}", gifts.Count);
            return gifts;
        }

        public async Task<List<Gift>?> GetByDonorName(string firstName, string lastName)
        {
            logger.LogInformation("Get gifts by donor started. FirstName={FirstName}, LastName={LastName}", firstName, lastName);
            var gifts = await giftDal.GetByDonorName(firstName, lastName);
            logger.LogInformation("Get gifts by donor finished. Count={Count}", gifts?.Count ?? 0);
            return gifts;
        }

        public async Task<Gift?> GetById(int id)
        {
            logger.LogInformation("Get gift by id started. Id={Id}", id);
            var gift = await giftDal.GetById(id);
            if (gift == null)
                logger.LogWarning("Gift not found by id. Id={Id}", id);
            else
                logger.LogInformation("Get gift by id finished successfully. Id={Id}, Name={Name}", id, gift.Name);
            return gift;
        }

        public async Task<Gift?> GetByName(string firstname)
        {
            logger.LogInformation("Get gift by name started. Name={Name}", firstname);
            var gift = await giftDal.GetByName(firstname);
            if (gift == null)
                logger.LogWarning("Gift not found by name. Name={Name}", firstname);
            else
                logger.LogInformation("Get gift by name finished successfully. Id={Id}, Name={Name}", gift.Id, gift.Name);
            return gift;
        }

        public async Task<List<Gift>> GetByPrice(bool ascending = true)
        {
            logger.LogInformation("Get gifts by price started. Ascending={Ascending}", ascending);
            var gifts = await giftDal.GetByPrice(ascending);
            logger.LogInformation("Get gifts by price finished. Count={Count}", gifts.Count);
            return gifts;
        }

        public async Task<bool> Remove(int id)
        {
            logger.LogInformation("Remove gift started. Id={Id}", id);
            var result = await giftDal.Remove(id);
            if (result)
                logger.LogInformation("Gift removed successfully. Id={Id}", id);
            else
                logger.LogWarning("Gift not found for remove. Id={Id}", id);
            return result;
        }

        public async Task Update(int id, GiftDTO updateGift)
        {
            logger.LogInformation("Update gift started. Id={Id}", id);

            try
            {
                await giftDal.Update(id, updateGift);
                logger.LogInformation("Gift updated successfully. Id={Id}, Name={GiftName}", id, updateGift.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gift update failed. Id={Id}", id);
                throw;
            }
        }
    }
}
