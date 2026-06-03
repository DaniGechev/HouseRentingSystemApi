using HouseRentingSystemApi.Models;

namespace HouseRentingSystemApi.Services
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseDetailModel>> GetAllAsync();

        Task<HouseDetailModel?> GetByIdAsync(int id);

        Task<HouseDetailModel> CreateAsync(HouseDetailModel model, string userId);

        Task<OperationStatus> EditAsync(int id, HouseEditModel model, string userId);

        Task<OperationStatus> DeleteAsync(int id, string userId);
    }
}
