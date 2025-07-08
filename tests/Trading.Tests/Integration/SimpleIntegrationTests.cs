using FluentAssertions;
using Trading.Api.Models;
using Trading.Application.Commands;
using Trading.Application.DTOs;
using Trading.Application.Interfaces;
using Trading.Messaging.Contracts;
using Trading.Api.Mappers;
using Trading.Domain;
using Trading.Infrastructure.Data;
using Trading.Infrastructure.Repositories;
using Trading.Messaging.Service;
using Moq;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Trading.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class SimpleIntegrationTests
    {
        [Fact]
        public async Task ExecuteTradeCommandHandler_WithValidCommand_ShouldCreateTrade()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new TradingDbContext(options);
            var repository = new TradeRepository(context);
            
            var mockProducer = new Mock<ITradeMessageProducer>();
            mockProducer.Setup(x => x.PublishTradeExecutedAsync(It.IsAny<TradeExecutedMessage>()))
                .Returns(Task.CompletedTask);

            var handler = new ExecuteTradeCommandHandler(repository, mockProducer.Object);

            var command = new ExecuteTradeCommand
            {
                UserId = Guid.NewGuid().ToString(),
                Asset = "AAPL",
                Quantity = 100,
                Price = 150.50m,
                TradeType = "Buy"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Asset.Should().Be("AAPL");
            result.Quantity.Should().Be(100);
            result.Price.Should().Be(150.50m);
            result.TradeType.Should().Be("Buy");
            result.Status.Should().Be("Pending");

            // Verify trade was saved to database
            var savedTrade = await context.Trades.FirstOrDefaultAsync(t => t.Asset == "AAPL");
            savedTrade.Should().NotBeNull();
            savedTrade!.Quantity.Should().Be(100);
            savedTrade.Price.Should().Be(150.50m);
            savedTrade.TradeType.Should().Be(TradeType.Buy);

            // Verify message was published
            mockProducer.Verify(x => x.PublishTradeExecutedAsync(It.IsAny<TradeExecutedMessage>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteTradeCommandHandler_WithInvalidUserId_ShouldThrowFormatException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new TradingDbContext(options);
            var repository = new TradeRepository(context);
            
            var mockProducer = new Mock<ITradeMessageProducer>();
            var handler = new ExecuteTradeCommandHandler(repository, mockProducer.Object);

            var command = new ExecuteTradeCommand
            {
                UserId = "invalid-guid",
                Asset = "AAPL",
                Quantity = 100,
                Price = 150.50m,
                TradeType = "Buy"
            };

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(() => 
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteTradeCommandHandler_WithInvalidTradeType_ShouldThrowArgumentException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new TradingDbContext(options);
            var repository = new TradeRepository(context);
            
            var mockProducer = new Mock<ITradeMessageProducer>();
            var handler = new ExecuteTradeCommandHandler(repository, mockProducer.Object);

            var command = new ExecuteTradeCommand
            {
                UserId = Guid.NewGuid().ToString(),
                Asset = "AAPL",
                Quantity = 100,
                Price = 150.50m,
                TradeType = "InvalidType"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TradeRepository_ShouldSaveAndRetrieveTrades()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new TradingDbContext(options);
            var repository = new TradeRepository(context);

            var trade = Trade.Create(
                Guid.NewGuid(),
                "MSFT",
                200,
                300.00m,
                TradeType.Sell
            );

            // Act
            var savedTrade = await repository.AddAsync(trade);
            var allTrades = await repository.GetAllAsync();

            // Assert
            savedTrade.Should().NotBeNull();
            savedTrade.Asset.Should().Be("MSFT");
            savedTrade.Quantity.Should().Be(200);
            savedTrade.Price.Should().Be(300.00m);
            savedTrade.TradeType.Should().Be(TradeType.Sell);

            allTrades.Should().NotBeNull();
            allTrades.Count.Should().Be(1);
            allTrades[0].Asset.Should().Be("MSFT");
        }

        [Fact]
        public void TradeApiValidator_WithValidRequest_ShouldNotThrow()
        {
            // Arrange
            var request = new ExecuteTradeRequest
            {
                UserId = Guid.NewGuid().ToString(),
                Asset = "AAPL",
                Quantity = 100,
                Price = 150.50m,
                TradeType = "Buy"
            };

            // Act & Assert
            var action = () => TradeApiValidator.Validate(request);
            action.Should().NotThrow();
        }

        [Theory]
        [InlineData("", "Asset", 100, 150.50, "Buy", "UserId is required.")]
        [InlineData("valid-guid", "", 100, 150.50, "Buy", "Asset is required.")]
        [InlineData("valid-guid", "AAPL", 0, 150.50, "Buy", "Quantity must be positive.")]
        [InlineData("valid-guid", "AAPL", 100, -10, "Buy", "Price must be positive.")]
        [InlineData("valid-guid", "AAPL", 100, 150.50, "", "TradeType is required.")]
        public void TradeApiValidator_WithInvalidRequest_ShouldThrowArgumentException(
            string userId, string asset, decimal quantity, decimal price, string tradeType, string expectedError)
        {
            // Arrange
            var request = new ExecuteTradeRequest
            {
                UserId = userId,
                Asset = asset,
                Quantity = quantity,
                Price = price,
                TradeType = tradeType
            };

            // Act & Assert
            var action = () => TradeApiValidator.Validate(request);
            action.Should().Throw<ArgumentException>().WithMessage(expectedError);
        }
    }
} 