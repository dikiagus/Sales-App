using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesApp.Models;
using SalesApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using X.PagedList;
using X.PagedList.Extensions;
using System.ComponentModel;
using OfficeOpenXml.Style;

namespace SalesApp.Controllers
{
    public class SalesOrderController : Controller
    {
        private readonly SalesDbContext _context;

        public SalesOrderController(SalesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SOOrderIndexViewModel filter, int page = 1)
        {
            var query = _context.SO_ORDER
                .Include(x => x.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x =>
                    x.SO_ORDER_ID.ToString().Contains(filter.Keyword) ||
                    x.Customer.CustomerName.Contains(filter.Keyword));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(x => x.OrderDate >= filter.StartDate.Value && x.OrderDate <= DateTime.Today);
            }

            int pageSize = 5; // bisa kamu ubah sesuai kebutuhan

            var orders = await query
                        .OrderByDescending(x => x.OrderDate)
                        .ToListAsync();

            var pagedOrders = orders.ToPagedList(page, pageSize);


            var result = new SOOrderIndexViewModel
            {
                Keyword = filter.Keyword,
                StartDate = filter.StartDate,
                Orders = pagedOrders,
                PageIndex = page,
                PageSize = pageSize
            };

            return View(result);
        }

        // GET: /SalesOrder/Create
        public IActionResult Create()
        {
            var viewModel = new SalesOrderViewModel
            {
                Customers = _context.COM_CUSTOMER
                    .Select(c => new SelectListItem
                    {
                        Value = c.COM_CUSTOMER_ID.ToString(),
                        Text = c.CustomerName
                    }).ToList(),

                Items = new List<SalesOrderItemViewModel>() // Kosong dulu
            };

            return View(viewModel);
        }

        // POST: /SalesOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesOrderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdown
                viewModel.Customers = _context.COM_CUSTOMER
                    .Select(c => new SelectListItem
                    {
                        Value = c.COM_CUSTOMER_ID.ToString(),
                        Text = c.CustomerName
                    }).ToList();

                return View(viewModel);
            }

            // 1. Simpan header ke SO_ORDER
            var order = new SO_ORDER
            {
                OrderNo = viewModel.SalesOrderNumber,
                OrderDate = viewModel.OrderDate,
                COM_CUSTOMER_ID = viewModel.CustomerId,
                Address = viewModel.Address
            };

            _context.SO_ORDER.Add(order);
            await _context.SaveChangesAsync(); // Supaya ID-nya kebentuk

            // 2. Simpan detail ke SO_ITEM
            foreach (var item in viewModel.Items)
            {
                var detail = new SO_ITEM
                {
                    SO_ORDER_ID = order.SO_ORDER_ID,
                    ItemName = item.ItemName,
                    Quantity = item.Qty,
                    Price = item.Price
                };

                _context.SO_ITEM.Add(detail);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id, int page = 1)
        {
            var order = await _context.SO_ORDER
                .Include(o => o.SO_ITEMS)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order == null)
                return NotFound();

            var viewModel = new SalesOrderViewModel
            {
                SalesOrderNumber = order.OrderNo,
                OrderDate = order.OrderDate,
                CustomerId = order.COM_CUSTOMER_ID,
                Address = order.Address,
                Items = order.SO_ITEMS.Select(i => new SalesOrderItemViewModel
                {
                    ItemName = i.ItemName,
                    Qty = i.Quantity,
                    Price = i.Price
                }).ToList(),
                Customers = _context.COM_CUSTOMER.Select(c => new SelectListItem
                {
                    Value = c.COM_CUSTOMER_ID.ToString(),
                    Text = c.CustomerName
                }).ToList()
            };

            ViewBag.Page = page; // ⬅️ simpan info halaman
            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SalesOrderViewModel viewModel, int Page = 1)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Customers = _context.COM_CUSTOMER.Select(c => new SelectListItem
                {
                    Value = c.COM_CUSTOMER_ID.ToString(),
                    Text = c.CustomerName
                }).ToList();
                ViewBag.Page = Page;
                return View(viewModel);
            }

            var order = await _context.SO_ORDER
                .Include(o => o.SO_ITEMS)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order == null)
                return NotFound();

            // Update header
            order.OrderNo = viewModel.SalesOrderNumber;
            order.OrderDate = viewModel.OrderDate;
            order.COM_CUSTOMER_ID = viewModel.CustomerId;
            order.Address = viewModel.Address;

            // Hapus item lama
            _context.SO_ITEM.RemoveRange(order.SO_ITEMS);

            // Tambah item baru
            foreach (var item in viewModel.Items)
            {
                _context.SO_ITEM.Add(new SO_ITEM
                {
                    SO_ORDER_ID = order.SO_ORDER_ID,
                    ItemName = item.ItemName,
                    Quantity = item.Qty,
                    Price = item.Price
                });
            }

            await _context.SaveChangesAsync();

            // ⬅️ Redirect ke halaman sebelumnya
            return RedirectToAction(nameof(Index), new { page = Page });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var order = await _context.SO_ORDER
                .Include(o => o.SO_ITEMS)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order != null)
            {
                _context.SO_ITEM.RemoveRange(order.SO_ITEMS);
                _context.SO_ORDER.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(); // Supaya AJAX tahu berhasil
            }

            return NotFound();
        }


        public IActionResult ExportToExcel(string? keyword, DateTime? startDate)
        {
            OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Diki Agus");

            // Query data from the database
            var query = _context.SO_ORDER.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(o =>
                    o.SO_ORDER_ID.ToString().Contains(keyword) ||
                    o.Customer.CustomerName.Contains(keyword));
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            var orders = query.ToList();

            // Create the Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sales Orders");

            // Load data into the worksheet
            worksheet.Cells.LoadFromCollection(orders.Select(o => new
            {
                o.SO_ORDER_ID,
                OrderDate = o.OrderDate.ToString("dd-MM-yyyy"),
                Customer = o.Customer?.CustomerName ?? "N/A", // Handle null customer names
                o.Address
            }), true);

            // Set horizontal alignment to center for all cells
            var totalRows = worksheet.Dimension.End.Row;
            var totalCols = worksheet.Dimension.End.Column;
            worksheet.Cells[1, 1, totalRows, totalCols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Auto-fit columns to adjust width based on content
            worksheet.Cells.AutoFitColumns();

            // Save the Excel file into a memory stream
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0; // Reset the stream position to the beginning

            // Return the file as a download response
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesOrders.xlsx");
        }

    }
}
