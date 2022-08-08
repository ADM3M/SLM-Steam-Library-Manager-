using System;
using System.Collections.Generic;
using System.Linq;
using api.Application.Interfaces;
using api.Common.DTO;
using api.Core.Entities;
using api.Infrastructure.Data;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace api.Tests.Unit;

public class UserRepoTests
{
    private readonly UserRepository _sut;
    private readonly Mock<DataContext> _contextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IGamesRepository> _gamesRepoMock = new();

    private readonly List<Games> DbGames = new List<Games>()
    {
        new Games {AppId = 0, Name = "Half-life"},
        new Games {AppId = 1, Name = "Half-life: Blue Shift"},
        new Games {AppId = 2, Name = "Half-life: Opposing Force"}
    };

    public UserRepoTests()
    {
        _sut = new UserRepository(_contextMock.Object, _mapperMock.Object, _gamesRepoMock.Object);
    }

    [MemberData(nameof(TestData))]
    [Theory]
    public void GetNewGames_ShouldReturnNewGames(SteamGameDTO[] expected, SteamGameDTO[] gamesToAdd)
    {
        // Act
        var result = this._sut.GetNewGames(gamesToAdd.ToList(), this.DbGames);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetNewGames_ShouldReturnEmptyList_WhenNoGamesSpecified()
    {
        // Act
        var result = this._sut.GetNewGames(new List<SteamGameDTO>(), this.DbGames);
        
        // Assert
        result.Should().BeEmpty();
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[]
        {
            new[]
            {
                new SteamGameDTO {AppId = 3, Name = "Half-life 2"},
                new SteamGameDTO {AppId = 4, Name = "Half-life 2 ep 1"},
                new SteamGameDTO {AppId = 5, Name = "Half-life 2 ep 2"},
            },
            new[]
            {
                new SteamGameDTO {AppId = 0, Name = "Half-life"},
                new SteamGameDTO {AppId = 1, Name = "Half-life: Blue Shift"},
                new SteamGameDTO {AppId = 2, Name = "Half-life: Opposing Force"},
                new SteamGameDTO {AppId = 3, Name = "Half-life 2"},
                new SteamGameDTO {AppId = 4, Name = "Half-life 2 ep 1"},
                new SteamGameDTO {AppId = 5, Name = "Half-life 2 ep 2"},
            }
        };
        yield return new object[]
        {
            new[]
            {
                new SteamGameDTO {AppId = 12, Name = "Wolfenstein: The new order"}
            },
            new[]
            {
                new SteamGameDTO {AppId = 0, Name = "Half-life"},
                new SteamGameDTO {AppId = 1, Name = "Half-life: Blue Shift"},
                new SteamGameDTO {AppId = 12, Name = "Wolfenstein: The new order"},
            }
        };
        yield return new object[]
        {
            Array.Empty<SteamGameDTO>(),
            new[]
            {
                new SteamGameDTO {AppId = 0, Name = "Half-life"},
                new SteamGameDTO {AppId = 1, Name = "Half-life: Blue Shift"},
                new SteamGameDTO {AppId = 2, Name = "Half-life: Opposing Force"},
            }
        };
    }
}