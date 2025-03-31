using Microsoft.EntityFrameworkCore;
using Mimdinare.Models;
using MimdinareMain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimdinareMain.Services
{
    public class PurchaseHistoryService : IPurchaseHistoryService
    {
        private readonly AppDbContext _context;

        public PurchaseHistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Purchase> AddPurchaseAsync(Purchase purchase)
        {
            // Validate duration format
            if (!IsValidDurationFormat(purchase.Duration))
            {
                throw new ArgumentException("Duration must be in mm:ss format (e.g., 30:45)");
            }

            // Set default Products if null
            purchase.Products ??= string.Empty;

            // Auto-set timestamps
            var now = DateTime.Now;
            purchase.FormattedDate = now.ToString("dd/MM/yyyy");
            purchase.FormattedTime = now.ToString("HH:mm:ss");

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<List<Purchase>> GetAllPurchasesAsync()
        {
            return await _context.Purchases
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<Purchase>> GetPurchasesByProductNameAsync(string productName)
        {
            return await _context.Purchases
                .Where(p => p.ProductName.Contains(productName) || p.Products.Contains(productName))
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<Purchase?> GetPurchaseByIdAsync(int id)
        {
            return await _context.Purchases.FindAsync(id);
        }

        public async Task<Purchase> UpdatePurchaseAsync(int id, Purchase purchase)
        {
            // Validate duration format
            if (!IsValidDurationFormat(purchase.Duration))
            {
                throw new ArgumentException("Duration must be in mm:ss format (e.g., 30:45)");
            }

            var existing = await _context.Purchases.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Purchase not found");

            existing.ProductName = purchase.ProductName;
            existing.Products = purchase.Products ?? string.Empty;
            existing.Quantity = purchase.Quantity;
            existing.Duration = purchase.Duration;
            existing.Price = purchase.Price;
            // UnitPriceValue removed

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeletePurchaseAsync(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return false;

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper Methods
        private bool IsValidDurationFormat(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return false;

            var parts = duration.Split(':');
            if (parts.Length != 2) return false;

            return int.TryParse(parts[0], out int minutes) &&
                   int.TryParse(parts[1], out int seconds) &&
                   seconds >= 0 && seconds < 60;
        }

        public string FormatDuration(int totalSeconds)
        {
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }

        public int GetTotalSeconds(string duration)
        {
            if (!IsValidDurationFormat(duration)) return 0;

            var parts = duration.Split(':');
            return int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
        }
    }
}