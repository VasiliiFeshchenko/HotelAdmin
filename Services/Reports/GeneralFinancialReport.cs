using HotelAdmin.Data.Models.Order;
using HotelAdmin.Services.IsProductionChecker;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MvcTest.Controllers;
using MvcTest.Data;
using MvcTest.Models;
using MvcTest.Sevices.MoscowTimeGetter;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace HotelAdmin.Services.Reports
{
    public class GeneralFinancialReport
    {
        private DateTime date;
        private Hotel hotel;
        private MvcTestContext context;
        public GeneralFinancialReport(DateTime date, Hotel hotel, MvcTestContext context)
        {
            this.date = date;
            this.hotel = hotel;
            this.context = context;
        }

        public async Task<ExcelPackage> GenerateExcelReportAsync()
        {
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

            GenerateHeaders(worksheet);

            ExcelData data = new ExcelData();
            await data.InitializeOrdersAsync(context, hotel.Id, date);

            int row = 3;

            SetSectionHeader(worksheet, ref row, "Заезд сегодня");
            row = GenerateOrders(worksheet, data.CheckInToday, row,true);

            SetSectionHeader(worksheet, ref row, "Забронировали сегодня");
            row = GenerateOrders(worksheet, data.BookToday, row);

            SetSectionHeader(worksheet, ref row, "Забронировали ранее, но оплатили сегодня");
            row = GenerateOrders(worksheet, data.PayedToday, row);

            SetSectionHeader(worksheet, ref row, "Отменили сегодня");
            row = GenerateCanceledOrders(worksheet, data.CanceledToday, row);

            SetSectionHeader(worksheet, ref row, "Вернули сегодня");
            row = GenerateCanceledOrders(worksheet, data.RefundedToday, row);

            StyleWorksheet(worksheet, row);
    
            await data.InitializeSummaryAsync(context, hotel.Id, date);

            row++;
            GenerateSummary(worksheet, data, row);
            return package;
        }

        private void GenerateHeaders(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].Value = "#";
            worksheet.Cells["B1"].Value = "Номер";
            worksheet.Cells["C1"].Value = "ФИО";
            worksheet.Cells["D1"].Value = "Заезд";
            worksheet.Cells["E1"].Value = "Дней";
            worksheet.Cells["F1"].Value = "Источник";
            worksheet.Cells["G1"].Value = "Создан";
            worksheet.Cells["H1"].Value = "Изменен";
            worksheet.Cells["I1"].Value = "Стоимость";
            worksheet.Cells["A1:A2"].Merge = true;
            worksheet.Cells["B1:B2"].Merge = true;
            worksheet.Cells["C1:C2"].Merge = true;
            worksheet.Cells["D1:D2"].Merge = true; 
            worksheet.Cells["E1:E2"].Merge = true;
            worksheet.Cells["F1:F2"].Merge = true;
            worksheet.Cells["G1:G2"].Merge = true;
            worksheet.Cells["H1:H2"].Merge = true;
            worksheet.Cells["I1:I2"].Merge = true;

            worksheet.Cells["J1"].Value = "Оплата сегодня";
            worksheet.Cells["J1:M1"].Merge = true;
            worksheet.Cells["J2"].Value = "Нал";
            worksheet.Cells["K2"].Value = "Карта";
            worksheet.Cells["L2"].Value = "Р/счёт";
            worksheet.Cells["M2"].Value = "Островок";

            worksheet.Cells["N1"].Value = "Оплата ранее/позднее";
            worksheet.Cells["N1:S1"].Merge = true;
            worksheet.Cells["N2"].Value = "Нал";
            worksheet.Cells["O2"].Value = "Дата";
            worksheet.Cells["P2"].Value = "Карта";
            worksheet.Cells["Q2"].Value = "Дата";
            worksheet.Cells["R2"].Value = "Р/счёт";
            worksheet.Cells["S2"].Value = "Дата";

            worksheet.Cells["T1"].Value = "Дата Отмены";
            worksheet.Cells["T1:T2"].Merge = true;


            worksheet.Cells["A1:T2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:T2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A1:T2"].Style.Font.Bold = true;

            worksheet.Cells["A1:T2"].AutoFitColumns();
        }
        private void SetSectionHeader(ExcelWorksheet worksheet, ref int row, string headerText)
        {
            var cellRange = worksheet.Cells["A" + row + ":T" + row];
            cellRange.Merge = true;
            cellRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cellRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cellRange.Style.Font.Size = 16;
            cellRange.Style.Font.Bold = true;
            cellRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["A" + row].Value = headerText;
            row++;
        }
        private int GenerateOrders(ExcelWorksheet worksheet, IEnumerable<Order> orders, int startingRow,bool checkInToday=false)
        {
            int row = startingRow;
            
            foreach (var order in orders)
            {
                worksheet.Cells["A" + row + ":T" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Cells["B" + row + ":T" + row].Style.WrapText=true;

                GenerateRow(worksheet, order, row);

                if (checkInToday)
                {
                    if (order.MoneyTransactions != null)
                    {
                        foreach (var transaction in order.MoneyTransactions.Where(t => t.TransactionMethod.Name == "Островок" && !t.IsRefund))
                        {
                            string cell = "M" + row;

                            if (worksheet.Cells[cell].Value != null)
                            {
                                worksheet.Cells[cell].Value += "\n" + transaction.Amount;
                            }
                            else
                            {
                                worksheet.Cells[cell].Value = transaction.Amount;
                            }
                        }
                    }
                }

                row++;
            }

            return row;
        }
        private int GenerateCanceledOrders(ExcelWorksheet worksheet, IEnumerable<Order> orders, int startingRow)
        {
            int row = startingRow;

            foreach (var order in orders)
            {
                worksheet.Cells["A" + row + ":T" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                worksheet.Cells["B" + row + ":T" + row].Style.WrapText = true;

                GenerateRow(worksheet, order, row);
                worksheet.Cells["T" + row].Value = ((DateTime)order.CancelationDate).ToString("dd.MM");
                row++;
            }

            return row;
        }
        private void StyleWorksheet(ExcelWorksheet worksheet, int row)
        {
            worksheet.View.FreezePanes(3, 21);
            worksheet.Cells["A0:A" + row].Style.WrapText = false;
            worksheet.Cells["A0:A" + row].AutoFitColumns();
            worksheet.Columns[2].Width = 12;
            worksheet.Columns[3].Width = 10;
            worksheet.Columns[4].Width = 6;
            worksheet.Columns[5].Width = 5;
            worksheet.Columns[7].Width = 11;
            worksheet.Columns[8].Width = 11;

            worksheet.Cells["I0:I" + row].Style.WrapText = false;
            worksheet.Cells["I0:I" + row].AutoFitColumns();
            worksheet.Columns[10].Width = 7;
            worksheet.Columns[11].Width = 11;
            worksheet.Columns[12].Width = 7;
            worksheet.Columns[13].Width = 7;
            worksheet.Columns[14].Width = 7;
            worksheet.Columns[15].Width = 6;
            worksheet.Columns[17].Width = 11;
            worksheet.Columns[19].Width = 11;
            worksheet.Columns[20].Width = 7;
            worksheet.Columns[21].Width = 6;

            worksheet.Cells["I1:I" + (row - 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["M1:M" + (row - 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["S1:S" + (row - 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            //for (int i = 4; i < row - 1; i=i+2)
            //{
            //    worksheet.Cells["A" + i.ToString() + ":Z" + i.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    worksheet.Cells["A" + i.ToString()+":Z" + i.ToString()].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //}
        }
        private void GenerateSummary(ExcelWorksheet worksheet, ExcelData data, int row)
        {
            var cellRange = worksheet.Cells["A" + row + ":T" + row];
            cellRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A" + row].Value = "Итого";
            worksheet.Cells["A" + row].Style.Font.Size = 26;
            worksheet.Cells["A" + row].Style.Font.Bold = true;

            worksheet.Cells["J" + row].Value = data.TodayCashSummary;
            worksheet.Cells["J" + row].Style.Font.Bold = true;
            worksheet.Cells["K" + row].Value = data.TodayCardSummary;
            worksheet.Cells["K" + row].Style.Font.Bold = true;
            worksheet.Cells["L" + row].Value = data.TodayBankAccountSummary;
            worksheet.Cells["L" + row].Style.Font.Bold = true;
            worksheet.Cells["M" + row].Value = data.TodayOstrovokSummary;
            worksheet.Cells["M" + row].Style.Font.Bold = true;
        }
               
        private void GenerateRow(ExcelWorksheet worksheet, Order order, int row)
        {
            char column = 'A';


            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string url = configuration["URL"];          
            var orderIdCell = worksheet.Cells[column + row.ToString()];
            orderIdCell.Value = order.Id; //A
            orderIdCell.Hyperlink = new ExcelHyperLink($"{url}/Orders/Edit?orderId={order.Id}&hotelId={order.HotelId}")
            {
                Display = order.Id.ToString()
            };
            column++;
            
            var roomsCell = worksheet.Cells[column + row.ToString()]; //B
            roomsCell.Value = string.Join("\n", order.Reservations.Select(reservation => reservation.BookableObject.Name));
            column++;

            worksheet.Cells[column + row.ToString()].Value = order.Client.Name; //C
            column++;
            //D
            worksheet.Cells[column + row.ToString()].Value = string.Join("\n", order.Reservations.Select(reservation => reservation.Start.ToString("dd.MM")));
            column++;
            //E
            worksheet.Cells[column + row.ToString()].Value = string.Join("\n", order.Reservations.Select(reservation => (reservation.End.Date - reservation.Start.Date).TotalDays + 1));
            column++;

            if (order.Source != null)
            {
                worksheet.Cells[column + row.ToString()].Value = order.Source.Name;
                worksheet.Cells[column + row.ToString()].Style.WrapText = false;                
            }
            column++;

            worksheet.Cells[column + row.ToString()].Value = order.CreationDate.ToString("dd.MM HH:mm");
            column++;
            worksheet.Cells[column + row.ToString()].Value = order.LastUpdateDate?.ToString("dd.MM HH:mm") ?? "--";
            column++;
            worksheet.Cells[column + row.ToString()].Value = order.Price;
            column++;

            var today = date;

            foreach (var transaction in order.MoneyTransactions)
            {
                if (!transaction.IsRefund)
                {
                    string transactionColumn = GetTransactionColumn(transaction, today);
                    if (!string.IsNullOrEmpty(transactionColumn))
                    {
                        var cell1 = worksheet.Cells[transactionColumn + row];                        
                        var cell2 = GetTransactionDateColumn(transaction, today).IsNullOrEmpty() ?
                            null: worksheet.Cells[GetTransactionDateColumn(transaction, today) + row];

                        if (cell1.Value != null)
                        {
                            cell1.Value += "\n";
                            cell1.Value = cell1.Value.ToString() + transaction.Amount;
                            if (transaction.TransactionMethod.Name == "Карта"&&transaction.Date.Date==today)
                            {
                                cell1.Value = cell1.Value.ToString() + transaction.Date.ToString("HH:mm");
                            }
                            if (cell2!=null)
                            {
                                cell2.Value += "\n";
                                if (transaction.TransactionMethod.Name=="Карта")
                                {
                                    cell2.Value = cell2.Value.ToString() + transaction.Date.ToString("dd.MM HH:mm");
                                }
                                else
                                {
                                    cell2.Value = cell2.Value.ToString() + transaction.Date.ToString("dd.MM");
                                }
                            }
                        }
                        else
                        {
                            cell1.Value = (int)transaction.Amount;
                            if (transaction.TransactionMethod.Name == "Карта" && transaction.Date.Date == today)
                            {
                                string time = transaction.Date.ToString("HH:mm");
                                cell1.Value = cell1.Value.ToString() +" "+ time;
                            }
                            if (cell2!=null)
                            {
                                if (transaction.TransactionMethod.Name == "Карта")
                                {
                                    cell2.Value = transaction.Date.ToString("dd.MM HH:mm");
                                }
                                else
                                {
                                    cell2.Value = transaction.Date.ToString("dd.MM");
                                }
                            }
                        }
                    }
                }
            }
        }

        private string GetTransactionColumn(MoneyTransaction transaction, DateTime today)
        {
            if (transaction.Date.Date == today)
            {
                return transaction.TransactionMethod.Name switch
                {
                    "Наличные" => "J",
                    "Карта" => "K",
                    "Расчётный счёт" => "L",
                    _ => null,
                };
            }
            else
            {
                return transaction.TransactionMethod.Name switch
                {
                    "Наличные" => "N",
                    "Карта" => "P",
                    "Расчётный счёт" => "R",
                    _ => null,
                };
            }
        }
        private string GetTransactionDateColumn(MoneyTransaction transaction, DateTime today)
        {
            if (transaction.Date.Date == today)
            {
                return null;
            }
            else
            {
                return transaction.TransactionMethod.Name switch
                {
                    "Наличные" => "O",
                    "Карта" => "Q",
                    "Расчётный счёт" => "S",
                    _ => null,
                };
            }
        }


    }
    public class ExcelData
    {
        public List<Order> CheckInToday { get; private set; }
        public List<Order> BookToday { get; private set; }
        public List<Order> PayedToday { get; private set; }
        public List<Order> CanceledToday { get; private set; }
        public List<Order> RefundedToday { get; private set; }

        public decimal TodayCashSummary { get; private set; }
        public decimal TodayCardSummary { get; private set; }
        public decimal TodayBankAccountSummary { get; private set; }
        public decimal TodayOstrovokSummary { get; private set; }


        public ExcelData()
        {
            // Default constructor
        }

        public async Task InitializeOrdersAsync(MvcTestContext context, int hotelId, DateTime date)
        {
            var today = date;

            // Use asynchronous queries with 'ToListAsync'
            CheckInToday = await context.Order
                .Include(order => order.Reservations)
                .ThenInclude(reservation => reservation.BookableObject)
                .Include(order => order.MoneyTransactions)
                .ThenInclude(transaction => transaction.TransactionMethod)
                .Include(order => order.Client)
                .Include(order => order.Source)
                .Where(order => order.Reservations.Any(reservation => reservation.Start.Date == today)
                               && order.HotelId == hotelId
                               && !order.IsCanceled)
                .ToListAsync();
            foreach (var item in CheckInToday)
            {
                if (item.Reservations[0].Start.Date!=today)
                {
                    CheckInToday.Remove(item);
                }
            }

            BookToday = await context.Order
                .Include(order => order.Reservations)
                .ThenInclude(reservation => reservation.BookableObject)
                .Include(order => order.MoneyTransactions)
                .ThenInclude(transaction => transaction.TransactionMethod)
                .Include(order => order.Client)
                .Include(order => order.Source)
                .Where(order => order.CreationDate.Date == today
                               && order.HotelId == hotelId
                               && !order.IsCanceled)
                .ToListAsync();

            PayedToday = await context.Order
                .Include(order => order.Reservations)
                .ThenInclude(reservation => reservation.BookableObject)
                .Include(order => order.MoneyTransactions)
                .ThenInclude(transaction => transaction.TransactionMethod)
                .Include(order => order.Client)
                .Include(order => order.Source)
                .Where(order => order.MoneyTransactions.Any(transaction => !transaction.IsRefund && transaction.Date.Date == today)
                               && order.HotelId == hotelId
                               && !order.IsCanceled)
                .ToListAsync();

            // Get the order IDs from CheckInToday and PayedToday
            List<int> checkInTodayOrderIds = CheckInToday.Select(order => order.Id).ToList();
            List<int> payedTodayOrderIds = PayedToday.Select(order => order.Id).ToList();

            // Exclude orders that are in both CheckInToday and PayedToday from BookToday
            BookToday = BookToday.Where(order => !checkInTodayOrderIds.Contains(order.Id)).ToList();

            PayedToday = PayedToday.Where(order => !checkInTodayOrderIds.Contains(order.Id) 
            && !payedTodayOrderIds.Contains(order.Id)).ToList();

            CanceledToday = await context.Order
                        .Include(order => order.Reservations)
                        .ThenInclude(reservation => reservation.BookableObject)
                        .Include(order => order.MoneyTransactions)
                        .ThenInclude(transaction => transaction.TransactionMethod)
                        .Include(order => order.Client)
                        .Include(order => order.Source)
                .Where(order => order.IsCanceled && ((DateTime)order.CancelationDate).Date == today 
                && order.HotelId == hotelId)
                .ToListAsync();

            RefundedToday = await context.Order
                        .Include(order => order.Reservations)
                        .ThenInclude(reservation => reservation.BookableObject)
                        .Include(order => order.MoneyTransactions)
                        .ThenInclude(transaction => transaction.TransactionMethod)
                        .Include(order => order.Client)
                        .Include(order => order.Source)
                .Where(order => order.MoneyTransactions.Any(transaction => transaction.IsRefund && transaction.Date.Date == today) 
                && order.HotelId == hotelId)
                .ToListAsync();
        }
        public async Task InitializeSummaryAsync(MvcTestContext context, int hotelId, DateTime date)
        {
            TodayCashSummary = (await context.Transaction.Where(t=>t.Date.Date==date
            && !t.IsRefund
            &&t.TransactionMethod.Id==1 
            && t.Order.HotelId==hotelId).ToListAsync()).Sum(t => t.Amount);

            TodayCardSummary = (await context.Transaction.Where(t=>t.Date.Date==date
            && !t.IsRefund 
            && t.TransactionMethod.Id == 2
            && t.Order.HotelId == hotelId).ToListAsync()).Sum(t => t.Amount);

            TodayBankAccountSummary = (await context.Transaction.Where(t => t.Date.Date == date 
            && !t.IsRefund 
            && t.TransactionMethod.Id == 3
            && t.Order.HotelId == hotelId).ToListAsync()).Sum(t => t.Amount);

            TodayOstrovokSummary = 0;
            foreach (var item in CheckInToday)
            {
                foreach (var transaction in item.MoneyTransactions)
                {
                    if (!transaction.IsRefund&&transaction.TransactionMethod.Id==4)
                    {
                        TodayOstrovokSummary += transaction.Amount;
                    }
                }
            }
        }
    }
}



