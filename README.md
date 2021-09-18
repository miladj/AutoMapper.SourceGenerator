# AutoMapper.SourceGenerator
It is just a simple AutoMapper that uses c# 9 source generator. It is not production ready nor optimized.

***
# How to use
It create method for Mapping two object type,
you have to create a partial class and specify which type you want to map by using `Map` Attribute
```C#
[Map(typeof(MapGeneratorTests.User), typeof(MapGeneratorTests.UserDto))]
public static partial class Mappers
{

}

```
and The generated class will be like

```C#
public static partial class Mappers
{
    public static void Convert(Mapper.Tests.MapGeneratorTests.User inputObj,out Mapper.Tests.MapGeneratorTests.UserDto outputObj)
    {
        outputObj=new Mapper.Tests.MapGeneratorTests.UserDto();
        outputObj.Username=inputObj.Username;
        outputObj.Id=inputObj.Id;
    }
}
```
