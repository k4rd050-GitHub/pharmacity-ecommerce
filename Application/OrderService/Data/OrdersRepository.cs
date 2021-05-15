﻿namespace OrderService.Data
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Crosscutting.Helpers;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using OrderService.Data.Dto;
    using OrderService.Data.Fakes;

    public class OrdersRepository
    {
        private readonly IMongoCollection<OrderDto> _ordersCollection;

        public OrdersRepository(IMongoCollection<OrderDto> ordersCollection)
        {
            _ordersCollection = ordersCollection
                                ?? throw new ArgumentNullException(nameof(ordersCollection));
            if (_ordersCollection.EstimatedDocumentCount() == 0)
            {
                _ordersCollection.InsertMany(FakeOrders.Data);
            }
        }

        public async Task<OrderDto> GetOrderAsync(string orderId, CancellationToken cancellationToken)
        {
            return await _ordersCollection.Find(o => o.OrderId == orderId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<OrderDto> GetOrders(string ids)
        {
            var idsList = DataTransferHelper.ProductIdsFromString(ids);
            return _ordersCollection.AsQueryable().Where(o => idsList.Contains(o.OrderId));
        }
    }
}