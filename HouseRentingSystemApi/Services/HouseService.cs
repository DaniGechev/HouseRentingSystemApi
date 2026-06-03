using HouseRentingSystemApi.Data;
using HouseRentingSystemApi.Data.Entities;
using HouseRentingSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystemApi.Services
{
    public class HouseService : IHouseService
    {
        private readonly AppDbContext context;

        public HouseService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<HouseDetailModel>> GetAllAsync()
        {
            return await context.Houses
                .AsNoTracking()
                .Select(h => new HouseDetailModel()
                {
                    Title = h.Title,
                    Address = h.Address,
                    ImageUrl = h.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<HouseDetailModel?> GetByIdAsync(int id)
        {
            var house = await context.Houses.FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return null;
            }

            return new HouseDetailModel()
            {
                Title = house.Title,
                Address = house.Address,
                ImageUrl = house.ImageUrl
            };
        }

        public async Task<HouseDetailModel> CreateAsync(HouseDetailModel model, string userId)
        {
            var newHouse = new House()
            {
                Description = model.Description,
                PricePerMonth = model.PricePerMonth,
                Address = model.Address,
                Title = model.Title,
                ImageUrl = model.ImageUrl,
                UserId = userId,
                CategoryId = await EnsureCategoryAsync(model.Category.ToString())
            };

            context.Houses.Add(newHouse);
            await context.SaveChangesAsync();

            return new HouseDetailModel()
            {
                Address = newHouse.Address,
                ImageUrl = newHouse.ImageUrl,
                Title = newHouse.Title,
                Description = newHouse.Description,
                PricePerMonth = newHouse.PricePerMonth,
                Category = model.Category
            };
        }

        public async Task<OperationStatus> EditAsync(int id, HouseEditModel model, string userId)
        {
            var house = await context.Houses.FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return OperationStatus.NotFound;
            }

            if (house.UserId != userId)
            {
                return OperationStatus.Forbidden;
            }

            house.Title = model.Title;
            house.Address = model.Address;
            house.Description = model.Description;
            house.ImageUrl = model.ImageUrl;
            house.PricePerMonth = model.PricePerMonth;
            house.CategoryId = await EnsureCategoryAsync(model.Category.ToString());

            await context.SaveChangesAsync();

            return OperationStatus.Success;
        }

        public async Task<OperationStatus> DeleteAsync(int id, string userId)
        {
            var house = await context.Houses.FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return OperationStatus.NotFound;
            }

            if (house.UserId != userId)
            {
                return OperationStatus.Forbidden;
            }

            context.Houses.Remove(house);
            await context.SaveChangesAsync();

            return OperationStatus.Success;
        }

        private async Task<int> EnsureCategoryAsync(string categoryName)
        {
            var category = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category == null)
            {
                category = new Category() { Name = categoryName };
                context.Categories.Add(category);
                await context.SaveChangesAsync();
            }

            return category.Id;
        }
    }
}
