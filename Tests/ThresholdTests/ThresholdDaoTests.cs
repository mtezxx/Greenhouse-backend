using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Tests.Utils;
using Xunit;

public class ThresholdDaoTest : DbTestBase
{
    private IThresholdDao _thresholdDao;

    public ThresholdDaoTest()
    {
        TestInit();
        TestInitialize();
    }

    public void TestInitialize()
    {
        _thresholdDao = new ThresholdDao(DbContext);
    }

    [Fact]
    public async Task GetAsync_ReturnsListOfThresholds()
    {
        var thresholds = new List<Threshold>
        {
            new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 },
            new Threshold { Type = "Humidity", minValue = 20, maxValue = 80 }
        };
        DbContext.Thresholds.AddRange(thresholds);
        await DbContext.SaveChangesAsync();

        var result = await _thresholdDao.GetAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Temperature", result[0].Type);
        Assert.Equal(0, result[0].minValue);
        Assert.Equal(50, result[0].maxValue);
        Assert.Equal("Humidity", result[1].Type);
        Assert.Equal(20, result[1].minValue);
        Assert.Equal(80, result[1].maxValue);
    }

    [Fact]
    public async Task GetByLatestAsync_ReturnsLatestThreshold()
    {
        var threshold1 = new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 };
        var threshold2 = new Threshold { Type = "Temperature", minValue = 5, maxValue = 55 };
        DbContext.Thresholds.AddRange(threshold1, threshold2);
        await DbContext.SaveChangesAsync();

        var result = await _thresholdDao.GetByLatestAsync();

        Assert.NotNull(result);
        Assert.Equal(threshold2.Type, result.Type);
        Assert.Equal(threshold2.minValue, result.minValue);
        Assert.Equal(threshold2.maxValue, result.maxValue);
    }

    [Fact]
    public async Task GetByLatestAsync_ThrowsException_WhenNoThresholdFound()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdDao.GetByLatestAsync());
        Assert.Equal("No latest threshold to be found.", exception.Message);
    }

    [Fact]
    public async Task GetByTypeAsync_ReturnsThreshold_WhenTypeExists()
    {
        var threshold = new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 };
        DbContext.Thresholds.Add(threshold);
        await DbContext.SaveChangesAsync();

        var result = await _thresholdDao.GetByTypeAsync("Temperature");

        Assert.NotNull(result);
        Assert.Equal(threshold.Type, result.Type);
        Assert.Equal(threshold.minValue, result.minValue);
        Assert.Equal(threshold.maxValue, result.maxValue);
    }

    [Fact]
    public async Task GetByTypeAsync_ThrowsException_WhenTypeNotFound()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => _thresholdDao.GetByTypeAsync("NonExistentType"));
        Assert.Equal("No results.", exception.Message);
    }

    [Fact]
    public async Task AddAsync_AddsThresholdSuccessfully()
    {
        var threshold = new Threshold { Type = "Temperature", minValue = 0, maxValue = 50 };

        var result = await _thresholdDao.AddAsync(threshold);
        await DbContext.SaveChangesAsync();

        Assert.NotNull(result);
        Assert.Equal(threshold.Type, result.Type);
        Assert.Equal(threshold.minValue, result.MinValue);
        Assert.Equal(threshold.maxValue, result.MaxValue);

        var thresholdInDb = await DbContext.Thresholds.FirstOrDefaultAsync();
        Assert.NotNull(thresholdInDb);
        Assert.Equal(threshold.Type, thresholdInDb.Type);
        Assert.Equal(threshold.minValue, thresholdInDb.minValue);
        Assert.Equal(threshold.maxValue, thresholdInDb.maxValue);
    }
}
