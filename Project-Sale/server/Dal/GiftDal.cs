using Microsoft.EntityFrameworkCore;
using server.Dal.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Dal
{
    public class GiftDal : IGiftDal
    {
        private readonly SaleContext saleContext;

        public GiftDal(SaleContext saleContext, IDonorDal donorDal)
        {
            this.saleContext = saleContext;
        }

        public async Task<Gift> Add(Gift gift)
        {
            await saleContext.Gifts.AddAsync(gift);
            await saleContext.SaveChangesAsync();
            return gift;
        }

        public async Task<List<Gift>> Get()
        {
            return await saleContext.Gifts
                       .AsNoTracking()
                       .Include(g => g.Donor)  // טוען את ה-Donor
                       .ToListAsync();
        }


        public async Task<List<Gift>> GetByCategory(string category)
        {
            return await saleContext.Gifts
                           .AsNoTracking()
                           .Include(g => g.Donor)
                           .Where(g => g.Category == category)
                           .ToListAsync();
        }

        public async Task<List<Gift>?> GetByDonorName(string firstName, string lastName)
        {
            return await saleContext.Gifts.AsNoTracking()
                              .Include(g => g.Donor)
                              .Where(g => g.Donor.FirstName == firstName &&
                                          g.Donor.LastName == lastName)
                              .ToListAsync();
        }

        public async Task<List<Gift>?> GetByBuyersNumber(int number)
        {
            return await saleContext.Gifts
                              .AsNoTracking()
                              .Include(g => g.Donor)
                              .Where(g => g.BuyersNumber == number)
                              .ToListAsync();
        }

        public async Task<Gift?> GetById(int id)
        {
            return await saleContext.Gifts.AsNoTracking()
                           .Include(g => g.Donor)
                           .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Gift?> GetByName(string name)
        {
            return await saleContext.Gifts.AsNoTracking()
                           .Include(g => g.Donor)
                           .FirstOrDefaultAsync(g => g.Name == name);
        }

        public async Task<bool> Remove(int id)
        {
            var gift = await saleContext.Gifts.FirstOrDefaultAsync(g => g.Id == id);
            if (gift == null)
                return false;

            saleContext.Gifts.Remove(gift);
            await saleContext.SaveChangesAsync();
            return true;
        }

        public async Task Update(int id, GiftDTO updateGift)
        {
            var gift = await saleContext.Gifts.FirstOrDefaultAsync(g => g.Id == id);
            if (gift == null)
                return;
            
            gift.Name = updateGift.Name;
            gift.Category = updateGift.Category;
            gift.Price = updateGift.Price;
            gift.BuyersNumber = updateGift.BuyersNumber;
            gift.DonorId = updateGift.DonorId;
            gift.WinnerTicketId = updateGift.WinnerTicketId;
            gift.IsDrawn = updateGift.IsDrawn;

            await saleContext.SaveChangesAsync();
        }

        public async Task<List<Gift>> GetByPrice(bool ascending = true)
        {
            if (ascending)
            {
                return await saleContext.Gifts.AsNoTracking()
                    .OrderBy(g => g.Price)
                    .ToListAsync();
            }
            else
            {
                return await saleContext.Gifts.AsNoTracking()
                    .OrderByDescending(g => g.Price)
                    .ToListAsync();
            }
        }
    }
}
