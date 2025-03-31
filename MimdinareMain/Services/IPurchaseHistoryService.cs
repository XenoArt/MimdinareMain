// Services/IPurchaseHistoryService.cs
using Mimdinare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MimdinareMain.Services
{
    public interface IPurchaseHistoryService
    {
        Task<Purchase> AddPurchaseAsync(Purchase purchase);
        Task<List<Purchase>> GetAllPurchasesAsync();
        Task<List<Purchase>> GetPurchasesByProductNameAsync(string productName);
        Task<Purchase?> GetPurchaseByIdAsync(int id);
        Task<Purchase> UpdatePurchaseAsync(int id, Purchase purchase);
        Task<bool> DeletePurchaseAsync(int id);
    }
}