using System;
using Xunit;

namespace Mapper.Tests
{
    [Map(typeof(MapGeneratorTests.User), typeof(MapGeneratorTests.UserDto))]
    public static partial class Mappers
    {

    }
    public class MapGeneratorTests
    {
        public enum TestEnum
        {
            First=1,
            Second=2,
            Third=3
        }
        public enum TestEnumDto
        {
            First = 1,
            Second = 2,
            Third = 3
        }
        public class Score
        {
            public decimal Sc { get; set; }
        }
        public class User
        {
            public string Username { get; set; }
            public int Id { get; set; }
            public TestEnum State { get; set; }
            public Score Score { get; set; }
        }
        public class ScoreDto
        {
            public decimal Sc { get; set; }
        }
        public class UserDto
        {
            public string Username { get; set; }
            public int Id { get; set; }
            public TestEnumDto State { get; set; }
            public ScoreDto Score { get; set; }
        }
        
        [Fact]
        public void SimpleMapCheck()
        {
            
            User user = new User()
            {
                Id = 101,
                Username = "user1",
                State = TestEnum.Second,
                Score = new Score()
                {
                    Sc = 90
                }
            };
            Mappers.Convert(user, out var userDto);
            Assert.NotNull(userDto);
            Assert.Equal(user.Id,userDto.Id);
            Assert.Equal(user.Username, userDto.Username);
            Assert.Equal(TestEnumDto.Second, userDto.State);
            Assert.NotNull(userDto.Score);
            Assert.Equal(user.Score.Sc, userDto.Score.Sc);
        }
    }
}
