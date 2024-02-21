using HotelAdmin.Data.Models.BookableObjects;
using HotelAdmin.Data.Models.RoomStateSetters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers;
using MvcTest.Data;
using MvcTest.Models;

namespace MvcTest.Sevices
{
    public class RoomsReservationsTableDataGetter
    {
        private readonly Hotel hotel;
        private readonly MvcTestContext _context;
        private readonly DateTime start;
        private readonly DateTime end;
        private readonly int totalDays;

        public RoomsReservationsTableDataGetter(Hotel hotel, MvcTestContext context, DateTime start, DateTime end)
        {
            this.hotel = hotel;
            _context = context;
            this.start = start;
            this.end = end;
            totalDays = (int)(this.end - this.start).TotalDays;
        }

        public async Task<(List<TableRowData>, List<DateTime>)> Get(BookableObject bookableObject=null)
        {

            List<BookableObject> bookableObjects = (bookableObject == null ? 
                await BookableObjectsGetter.GetBookableObjects(_context, hotel.Id) : 
                new List<BookableObject> { bookableObject});
            List<BookableObjectStateSetter> stateSetters = await _context.BookableObjectStateSetter
                .Where(stateSetter =>
                    stateSetter.Start <= end && stateSetter.End >= start &&
                    stateSetter.BookableObject.HotelId == hotel.Id && 
                    (!(stateSetter is Reservation)||!(stateSetter as Reservation).Order.IsCanceled))
                .Include(s => (s as Reservation).Order.Client)
                .Include(s=>(s as Reservation).Order.MoneyTransactions)
                .ToListAsync();
            List<TableRowData> tableRowsData = InitializeTableRowsData(bookableObjects);

            for (int i = 0; i < totalDays; i++)
            {
                for (int j = 0; j < tableRowsData.Count; j++)
                {
                    var cell = tableRowsData[j].DateCells[i];
                    var rowBookableObject = bookableObjects[j];
                    var isClosedCombined = cell != null && cell.StateSetter != null && cell.StateSetter is ClosureSet closureSet && closureSet.Type == ClosureType.combined;
                    if (cell == null || isClosedCombined)
                    {
                        var cellDate = start.AddDays(i);
                        cell = CreateTableCellData(cell, cellDate, tableRowsData[j].DateCells[i]);
                        SetStateSetter(cell, stateSetters, cellDate, rowBookableObject, isClosedCombined);
                        tableRowsData[j].DateCells[i] = cell;
                        if (rowBookableObject is Room rowRoom && rowRoom.RoomCombination != null && !(cell.StateSetter is PriceSet))
                        {
                            HandleEmbeddedRoomClosure(cell, tableRowsData, rowRoom,i);
                        }
                        else if (rowBookableObject is RoomCombination rowRoomCombination && cell.StateSetter is Reservation)
                        {
                            HandleCombinedRoomClosure(cell, tableRowsData, rowRoomCombination, i);
                        }
                    }
                }
            }

            var dates = GenerateDatesList();
            return (tableRowsData, dates);
        }

        private List<TableRowData> InitializeTableRowsData(List<BookableObject> bookableObjects)
        {
            var tableRowsData = new List<TableRowData>();
            for (int i = 0; i < bookableObjects.Count; i++)
            {
                tableRowsData.Add(new TableRowData(totalDays) { BookableObject = bookableObjects[i] });
            }
            return tableRowsData;
        }

        private TableCellData CreateTableCellData(TableCellData cell, DateTime cellDate, TableCellData existingCellData)
        {
            cell = new TableCellData { Date = cellDate };
            if (existingCellData != null && existingCellData.StateSetter != null)
            {
                cell.StateSetter = existingCellData.StateSetter;
            }
            return cell;
        }

        private void SetStateSetter(TableCellData cell, List<BookableObjectStateSetter> stateSetters, DateTime cellDate, BookableObject rowBookableObject, bool isClosedCombined)
        {
            List<BookableObjectStateSetter> setters = stateSetters
                .Where(item =>
                    item.Start <= cellDate.Date &&
                    item.End >= cellDate.Date &&
                    item.BookableObject == rowBookableObject)
                .ToList();
            BookableObjectStateSetter newState = setters.Where(setter => setter is Reservation).FirstOrDefault();
            if (newState == null)
            {
                newState = setters.Where(setter => setter is ClosureSet).FirstOrDefault();
            }
            if (newState == null && !isClosedCombined)
            {
                newState = setters.Where(setter => setter is PriceSet).FirstOrDefault();
            }
            if (newState != null)
            {
                cell.StateSetter = newState;
            }
        }

        private void HandleEmbeddedRoomClosure(TableCellData cell, List<TableRowData> tableRowsData, Room rowRoom, int currentDateIndex)
        {
            if (cell.StateSetter is Reservation)
            {
                var combinedRoomTableRowData = tableRowsData
                    .FirstOrDefault(tableRowData => tableRowData.BookableObject.Id == rowRoom.RoomCombination.Id);

                if (combinedRoomTableRowData != null)
                {
                    var combinedCell = combinedRoomTableRowData.DateCells[currentDateIndex];
                    if (combinedCell == null || !(combinedCell.StateSetter is Reservation || combinedCell.StateSetter is ClosureSet))
                    {
                        combinedRoomTableRowData.DateCells[currentDateIndex] = new TableCellData
                        {
                            Date = cell.Date,
                            StateSetter = new ClosureSet { Comment = "Объект нельзя забронировать", Type = ClosureType.combined }
                        };
                    }
                }
            }
            else
            {
                // Code that handles situations where some rooms included in combinedRoom are closed (e.g., maintenance)
            }
        }

        private void HandleCombinedRoomClosure(TableCellData cell, List<TableRowData> tableRowsData, RoomCombination rowRoomCombination, int index)
        {
            foreach (var tableRowData in tableRowsData)
            {
                if (rowRoomCombination.Rooms.Contains(tableRowData.BookableObject))
                {
                    var embededCell = tableRowData.DateCells[index];
                    if (embededCell == null || !(embededCell.StateSetter is Reservation || embededCell.StateSetter is ClosureSet))
                    {
                        tableRowData.DateCells[index] = new TableCellData
                        {
                            Date = cell.Date,
                            StateSetter = new ClosureSet { Comment = "Объект нельзя забронировать" }
                        };
                    }
                }
            }
        }

        private List<DateTime> GenerateDatesList()
        {
            var dates = new List<DateTime>();
            for (var date = start; date < end; date = date.AddDays(1))
            {
                dates.Add(date);
            }
            return dates;
        }
    }

    public class TableRowData
    {
        public BookableObject BookableObject { get; set; }
        public List<TableCellData> DateCells { get; set; }

        public TableRowData(int datesNumber)
        {
            DateCells = Enumerable.Repeat<TableCellData>(null, datesNumber).ToList();
        }
    }

    public class TableCellData
    {
        public DateTime Date { get; set; }
        public BookableObjectStateSetter? StateSetter { get; set; }
    }
}
