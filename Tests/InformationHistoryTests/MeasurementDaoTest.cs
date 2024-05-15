using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;

public class MeasurementDaoTest
{
    private EfcContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EfcContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure a unique name
            .Options;
        return new EfcContext(options);
    }
/*
    [Fact]
    public async Task AddAsync_AddsTemperatureSuccessfully()
    {
        using (var context = CreateContext())
        {
            var dao = new MeasurementDao<Temperature>(context);
            var temperature = new Temperature { Value = 25.5, Time = DateTime.Now };

            var result = await dao.AddAsync(temperature);
            await context.SaveChangesAsync();

            Assert.Equal(25.5, result.Value);
            Assert.NotEqual(default, result.Id); // ID should be set by EF Core
        }
    }
*/
    [Fact]
    public async Task GetAllAsync_ReturnsAllTemperatures()
    {
        using (var context = CreateContext())
        {
            var dao = new MeasurementDao<Temperature>(context);
            context.Add(new Temperature { Value = 22.1, Time = DateTime.Now });
            context.Add(new Temperature { Value = 23.5, Time = DateTime.Now });
            context.SaveChanges();

            var results = await dao.GetAllAsync();

            Assert.Equal(2, results.Count);
            Assert.All(results, item => Assert.IsType<Temperature>(item));
        }
    }
}