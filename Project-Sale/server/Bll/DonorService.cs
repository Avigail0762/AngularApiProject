using AutoMapper;
using server.Bll.Interfaces;
using server.Dal.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Bll
{
    public class DonorService : IDonorService
    {
        private readonly IDonorDal donorDal;
        private readonly IMapper mapper;
        private readonly ILogger<DonorService> logger;

        public DonorService(IDonorDal donorDal, IMapper mapper, ILogger<DonorService> logger)
        {
            this.donorDal = donorDal;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Donor> Add(DonorDTO donorDto)
        {
            logger.LogInformation("Add donor started. Email={Email}", donorDto.Email);

            try
            {
                var donor = mapper.Map<Donor>(donorDto);
                await donorDal.Add(donor);

                logger.LogInformation("Donor added successfully. Id={Id}, Email={Email}", donor.Id, donor.Email);
                return donor;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding donor. Email={Email}", donorDto.Email);
                throw;
            }
        }

        public async Task<List<Donor>> Get()
        {
            logger.LogInformation("Get all donors started");

            var donors = await donorDal.Get();
            logger.LogInformation("Get all donors finished. Count={Count}", donors.Count);
            return donors;
        }

        public async Task<Donor?> GetByEmail(string email)
        {
            logger.LogInformation("Get donor by email started. Email={Email}", email);

            var donor = await donorDal.GetByEmail(email);
            if (donor == null)
            {
                logger.LogWarning("Donor not found by email. Email={Email}", email);
            }
            else
            {
                logger.LogInformation("Donor found. Id={Id}, Email={Email}", donor.Id, donor.Email);
            }

            return donor;
        }

        public async Task<Donor?> GetByGift(Gift gift)
        {
            logger.LogInformation("Get donor by gift started. GiftId={GiftId}", gift.Id);

            var donor = await donorDal.GetByGift(gift);
            if (donor == null)
            {
                logger.LogWarning("Donor not found for gift. GiftId={GiftId}", gift.Id);
            }
            else
            {
                logger.LogInformation("Donor found for gift. DonorId={DonorId}, GiftId={GiftId}", donor.Id, gift.Id);
            }

            return donor;
        }

        public async Task<Donor?> GetById(int id)
        {
            logger.LogInformation("Get donor by id started. Id={Id}", id);

            var donor = await donorDal.GetById(id);
            if (donor == null)
            {
                logger.LogWarning("Donor not found. Id={Id}", id);
            }
            else
            {
                logger.LogInformation("Donor found. Id={Id}, Email={Email}", donor.Id, donor.Email);
            }

            return donor;
        }

        public async Task<Donor?> GetByName(string firstName, string lastName)
        {
            logger.LogInformation("Get donor by name started. FirstName={FirstName}, LastName={LastName}", firstName, lastName);

            var donor = await donorDal.GetByName(firstName, lastName);
            if (donor == null)
            {
                logger.LogWarning("Donor not found by name. FirstName={FirstName}, LastName={LastName}", firstName, lastName);
            }
            else
            {
                logger.LogInformation("Donor found. Id={Id}, Name={FirstName} {LastName}", donor.Id, donor.FirstName, donor.LastName);
            }

            return donor;
        }

        public async Task<bool> Remove(int id)
        {
            logger.LogInformation("Remove donor started. Id={Id}", id);

            try
            {
                await donorDal.Remove(id);
                logger.LogInformation("Donor removed successfully. Id={Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing donor. Id={Id}", id);
                return false;
            }
        }

        public async Task Update(int id, DonorDTO updateDonor)
        {
            logger.LogInformation("Update donor started. Id={Id}", id);

            try
            {
                await donorDal.Update(id, updateDonor);
                logger.LogInformation("Donor updated successfully. Id={Id}", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating donor. Id={Id}", id);
                throw;
            }
        }
    }
}
