using Microsoft.EntityFrameworkCore;
using server.Dal.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Dal
{
    public class DonorDal : IDonorDal
    {
        private readonly SaleContext saleContext;

        public DonorDal (SaleContext saleContext)
        {
            this.saleContext = saleContext;
        }

        public async Task<Donor> Add(Donor donor)
        {
            if (donor == null)
                throw new ArgumentNullException(nameof(donor));

            await saleContext.Donors.AddAsync(donor);
            await saleContext.SaveChangesAsync();
            return donor;
        }

        public async Task<bool> Remove(int id)
        {
            var donor = await saleContext.Donors.FindAsync(id);
            if (donor == null)
                return false;

            saleContext.Donors.Remove(donor);
            await saleContext.SaveChangesAsync();
            return true;
        }

        public async Task Update(int id, DonorDTO updateDonor)
        {
            var donor = await saleContext.Donors.FindAsync(id);
            if (donor == null)
                throw new Exception("Donor לא נמצא");

            donor.FirstName = updateDonor.FirstName;
            donor.LastName = updateDonor.LastName;
            donor.Email = updateDonor.Email;
            donor.Address = updateDonor.Address;
            donor.PhoneNumber = updateDonor.PhoneNumber;

            await saleContext.SaveChangesAsync();
        }

        public async Task<List<Donor>> Get()
        {
            return await saleContext.Donors.AsNoTracking().Include(d => d.Gifts).ToListAsync();
        }

        public async Task<Donor?> GetByEmail(string email)
        {
            return await saleContext.Donors.AsNoTracking().SingleOrDefaultAsync(d => d.Email == email);
        }

        public async Task<Donor?> GetByGift(Gift gift)
        {
            return await saleContext.Donors.AsNoTracking().FirstOrDefaultAsync(d => d.Gifts.Contains(gift));
        }
        
        public async Task<Donor?> GetByName(string firstName, string lastName)
        {
            return await saleContext.Donors.AsNoTracking()
                .FirstOrDefaultAsync(d => d.FirstName == firstName && d.LastName == lastName);
        }

        public async Task<Donor?> GetById(int id)
        {
            return await saleContext.Donors.FindAsync(id);
        }
    }
}
